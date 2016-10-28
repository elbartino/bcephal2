using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using Misp.Kernel.Domain;
using DiagramDesigner;
using System.Windows.Media.Imaging;
using Misp.Sourcing.Table;
using Misp.Sourcing.Base;
using System.Web.Script.Serialization;

namespace Misp.Sourcing.AllocationDiagrams
{
    public class DiagramCanvas : DiagramDesigner.DesignerCanvas
    {
        protected override void initContextMenu()
        {
            base.initContextMenu();
            ((DesignerContextMenu)this.ContextMenu).ObjectMenuItem.Header = "Add Allocation Box";
            ((DesignerContextMenu)this.ContextMenu).ObjectMenuItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("Images/Loop.png", UriKind.Relative))
            };

            ((DesignerContextMenu)this.ContextMenu).ValueChainMenuItem.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(sender, e);            
            ((DesignerContextMenu)this.ContextMenu).CustomizeForAllocation();
        }

        protected override DesignerItem GetNewDesignerItem(Guid id)
        {
            DesignerItem item = new DesignerItem(id);
            item.Editing += OnEditing;
            return item;
        }

        public EditingHandler Editing;
        public delegate void EditingHandler(DesignerItem item);
        private void OnEditing(DesignerItem item, string name)
        {
            if (Editing != null) Editing(item);
        }

        public PastingHandler PastingAction;
        public delegate void PastingHandler(DesignerItem item, TransformationTreeItem currentAction);
        private void OnPasting(DesignerItem item,TransformationTreeItem currentAction)
        {
            if (PastingAction != null) PastingAction(item,currentAction);
        }

        public TransformationTree Tree { get; set; }

        public void RefreshEntity(TransformationTreeItem entity)
        {
            foreach (UIElement item in this.Children)
            {
                if (item is DiagramDesigner.DesignerItem)
                {
                    object tag = ((DiagramDesigner.DesignerItem)item).Tag;
                    if (((DiagramDesigner.DesignerItem)item).Renderer.Text.ToUpper().Equals(entity.name.ToUpper())) ((DiagramDesigner.DesignerItem)item).Tag = entity;
                }
            }
        }

        public DiagramDesigner.DesignerItem GetDesignerItemByName(string name)
        {
            foreach (UIElement item in this.Children)
            {
                if (item is DiagramDesigner.DesignerItem)
                {
                    object tag = ((DiagramDesigner.DesignerItem)item).Tag;
                    if (((DiagramDesigner.DesignerItem)item).Renderer.Text.ToUpper().Equals(name.ToUpper())) return (DiagramDesigner.DesignerItem)item;
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected override void SetCopyOfTag(DiagramDesigner.DesignerItem item)
        {
            string name = item.Renderer.Text;
            DiagramDesigner.DesignerItem dItem = GetDesignerItemByName(name);
            bool isCutMode = dItem == null;
            dItem = dItem != null ? dItem : item;
            object tag = ((DiagramDesigner.DesignerItem)dItem).Tag;
            if (tag != null && (tag is TransformationTreeItem || tag is string))
            {
                JavaScriptSerializer serial = new JavaScriptSerializer();
                TransformationTreeItem transformationTreeItem = tag is TransformationTreeItem ? (TransformationTreeItem)tag :
                    serial.Deserialize<TransformationTreeItem>((string)tag);

                if (transformationTreeItem == null) return;
                TransformationTreeItem currenItem = null;

                if (transformationTreeItem.IsLoop)
                {
                    currenItem = transformationTreeItem.GetCopy(isCutMode);
                }
                else if (transformationTreeItem.IsAction) 
                {
                    currenItem = transformationTreeItem.GetCopy(isCutMode);
                    SourcingServiceFactory tableServiceFactory = new SourcingServiceFactory(Kernel.Application.ApplicationManager.Instance);
                    currenItem = transformationTreeItem.setCopyReport(currenItem, tableServiceFactory.GetInputTableService());
                    Kernel.Service.TransformationTreeService treeService = new Kernel.Service.TransformationTreeService();
                    if (PastingAction != null) PastingAction(item, currenItem);
                }
                if (currenItem != null)
                {
                    currenItem.name = getNewName(currenItem.name, true);
                    item.Tag = currenItem;
                    item.Renderer.Text = currenItem.name;
                    ((DiagramDesigner.DesignerItem)dItem).Tag = transformationTreeItem;
                }
            }
            else item.Tag = tag;
        }

        public override void AddNewObject()
        {
            TransformationTreeItem loop = new TransformationTreeItem(true);
            loop.name = getNewName("Block");
            AddNewBlock(new AllocationBoxItem(), loop);
        }


        public override void AddNewValueChain()
        {
            //TransformationTreeItem action = new TransformationTreeItem(false);
            //action.name = getNewName("Action");
            //AddNewBlock(new ActionItem(), action);
        }

        protected override void onEdit(DiagramDesigner.DesignerItem item, string name)
        {
            if (item != null && item.Tag != null && item.Tag is TransformationTreeItem)
            {
                DiagramDesigner.DesignerItem block = GetBlockByName(name);
                TransformationTreeItem entity = (TransformationTreeItem)item.Tag;
                if (block != null && !block.Equals(item))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Duplicate name", "There is another block named: " + name + ".");
                    item.Renderer.Text = entity.name;
                    EditCurrentSelection();
                    return;
                }                
                entity.name = name;
                this.SelectionService.SelectItem(item);
                notifyModifyBlock(item);                
            }
        }

        protected override bool CanAddLinkBetween(DiagramDesigner.DesignerItem source, DiagramDesigner.DesignerItem cible)
        {
            string sourceName = source.Renderer.Text;
            string cibleName = cible.Renderer.Text;
            string title = "Unable to add link between blocks";
            string message = "Unable to add link between block : " + sourceName + " and block : " + cibleName + ".";
            if (source.Tag == null || cible.Tag == null)
            {
                message = message + "\n" + "Source or Target Entity is null!";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
            TransformationTreeItem sourceTag = (TransformationTreeItem)source.Tag;
            TransformationTreeItem cibleTag = (TransformationTreeItem)cible.Tag;

            if (sourceTag.childrenListChangeHandler.Items.Count > 0)
            {
                message = message + "\n" + sourceName + " already has a child.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
            if (cibleTag.parent != null)
            {
                message = message + "\n" + cibleName + " already has a parent.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
            if ((sourceTag.parent != null && sourceTag.parent == cibleTag) ||
                (cibleTag.childrenListChangeHandler.Items.Contains(sourceTag)))
            {
                message = message + "\n" + "There is already a link between " + sourceName + " and " + cibleName + ".";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }

            if (sourceTag.parent != null && cibleTag.childrenListChangeHandler.Items.Count>0) 
            {
                message = message + "\n" + "Unable to build cyclic model.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
          
            return true;
        }

        public override bool CanMoveLinkSource(DesignerItem source, DesignerItem target, DesignerItem newSource)
        {
            string sourceName = source.Renderer.Text;
            string targetName = target.Renderer.Text;
            string newSourceName = newSource.Renderer.Text;

            if (sourceName.Equals(newSourceName)) return true;

            string title = "Unable to move link";
            string message = "Unable to move link from : " + sourceName + " to : " + newSourceName + ".";
            if (source.Tag == null || target.Tag == null || newSource.Tag == null)
            {
                message = message + "\n" + "Source or Target Entity is null!";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
            TransformationTreeItem sourceTag = (TransformationTreeItem)source.Tag;
            TransformationTreeItem targetTag = (TransformationTreeItem)target.Tag;
            TransformationTreeItem newSourceTag = (TransformationTreeItem)newSource.Tag;

            if (newSourceTag.childrenListChangeHandler.Items.Count > 0)
            {
                message = message + "\n" + newSourceName + " already has a child.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }

            if (targetTag.parent != null)
            {
                foreach (TransformationTreeItem item in newSourceTag.GetAscendentsTree())
                {
                    if (item.name.Equals(targetTag.name))
                    {
                        message = message + "\n" + "Unable to build cyclic model.";
                        Kernel.Util.MessageDisplayer.DisplayError(title, message);
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool CanMoveLinkTarget(DesignerItem source, DesignerItem target, DesignerItem newTarget)
        {            
            string sourceName = source.Renderer.Text;
            string targetName = target.Renderer.Text;
            string newTargetName = newTarget.Renderer.Text;

            if (targetName.Equals(newTargetName)) return true;

            string title = "Unable to move link";
            string message = "Unable to move link from : " + targetName + " to : " + newTargetName + ".";
            if (source.Tag == null || target.Tag == null || newTarget.Tag == null)
            {
                message = message + "\n" + "Source or Target Entity is null!";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
            TransformationTreeItem sourceTag = (TransformationTreeItem)source.Tag;
            TransformationTreeItem targetTag = (TransformationTreeItem)target.Tag;
            TransformationTreeItem newTargetTag = (TransformationTreeItem)newTarget.Tag;

            if (newTargetTag.parent != null)
            {
                message = message + "\n" + newTargetName + " already has a parent.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }

            if (sourceTag.parent != null)
            {
                foreach (TransformationTreeItem item in newTargetTag.GetDescendentsTree())
                {
                    if (item.name.Equals(sourceTag.name))
                    {
                        message = message + "\n" + "Unable to build cyclic model.";
                        Kernel.Util.MessageDisplayer.DisplayError(title, message);
                        return false;
                    }
                }                
            }

            return true;
        }


        protected override void DeleteCurrentSelection()
        {
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete Selection", "Do you want to delete selection?");
            if (result == MessageBoxResult.Yes) base.DeleteCurrentSelection();
        }
        
        private string getNewName(string prefix,bool copyMode = false)
        {
            int i = copyMode ? 0 : 1;
            string name = prefix;
            bool valid = false;
            while (!valid)
            {
                name = prefix + (i > 0 ? ""+i :"");
                valid = IsBlockNameValid(name);
                i++;
            }
            return name;
        }


        public override DiagramDesigner.DesignerItem GetBlockByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            foreach (UIElement item in this.Children)
            {
                if (item is DiagramDesigner.DesignerItem)
                {
                    object tag = ((DiagramDesigner.DesignerItem)item).Tag;
                    
                    if(tag == null) return null;
                    TransformationTreeItem treeItem = null;
                    if(tag is string)
                    {
                        JavaScriptSerializer serial = new JavaScriptSerializer();
                        treeItem = serial.Deserialize<TransformationTreeItem>((string)tag);    
                    }
                    if(tag is TransformationTreeItem)
                    {
                        treeItem = (TransformationTreeItem)tag;    
                    }

                    if (treeItem.ToString().ToUpper().Equals(name.ToUpper())) return (DiagramDesigner.DesignerItem)item;
                }
            }
            return null;
        }

        protected override bool IsBlockNameValid(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            foreach (UIElement item in this.Children)
            {
                if (item is DiagramDesigner.DesignerItem)
                {
                    object tag = ((DiagramDesigner.DesignerItem)item).Tag;
                    
                    if (tag != null && (tag is TransformationTreeItem || tag is string))
                    {
                        JavaScriptSerializer serial = new JavaScriptSerializer();
                        tag = tag is TransformationTreeItem ? (TransformationTreeItem)tag :
                        serial.Deserialize<TransformationTreeItem>((string)tag);
                    }
                    if (tag != null && tag.ToString().ToUpper().Equals(name.ToUpper())) return false;
                }
            }
            return true;
        }




    }
}
