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
using System.Web.Script.Serialization;

namespace Misp.Initiation.Diagram
{
    public class DiagramCanvas : DiagramDesigner.DesignerCanvas
    {

        public Kernel.Domain.Model Model { get; set; }

        public void RefreshEntity(Entity entity)
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
            if (dItem == null) return;
            object tag = ((DiagramDesigner.DesignerItem)dItem).Tag;

            if(tag != null && (tag is Entity || tag is string))
            {
                JavaScriptSerializer serial = new JavaScriptSerializer();
                Entity entity = (tag is Kernel.Domain.Entity) ? (Entity)tag : serial.Deserialize<Kernel.Domain.Entity>((string)tag);
                entity = entity.GetCopy();
                entity.name = getNewName(entity.name,true);
                item.Tag = entity;
                item.Renderer.Text = entity.name;
            }
            else item.Tag = tag;
        }

        public override void AddNewObject()
        {
            Entity newObject = new Entity();
            newObject.isObject = true;
            newObject.name = getNewName("Object");
            AddNewBlock(new ObjectItem(), newObject);
        }

        public override void AddNewValueChain()
        {
            if (Model != null && Model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                Kernel.Util.MessageDisplayer.DisplayWarning("Add Value Chain", "You're not allowed to add value chain to model: " + Model.name + "\nYou have to clear allocation before add value chain!");
                return;
            }
            Entity newValueChain = new Entity();
            newValueChain.isValueChain = true;
            newValueChain.name = getNewName("ValueChain");
            AddNewBlock(new ValueChainItem(), newValueChain);
        }

        protected override void onEdit(DiagramDesigner.DesignerItem item, string name)
        {
            if (item != null && item.Tag != null && item.Tag is Entity)
            {
                DiagramDesigner.DesignerItem block = GetBlockByName(name);
                Entity entity = (Entity)item.Tag;

                if (string.IsNullOrWhiteSpace(name))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Empty name", "The name can't be empty!");
                    item.Renderer.Text = entity.name;
                    EditCurrentSelection();
                    return;
                }

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
            Entity sourceTag = (Entity)source.Tag;
            Entity cibleTag = (Entity)cible.Tag;
            if (sourceTag.isValueChain)
            {
                message = message + "\n" + sourceName + " is a Value Chain.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
            if (cibleTag.isValueChain)
            {
                message = message + "\n" + cibleName + " is a Value Chain.";
                Kernel.Util.MessageDisplayer.DisplayError(title, message);
                return false;
            }
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
          
            if (Model != null && Model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                if (sourceTag.usedToGenerateUniverse || cibleTag.usedToGenerateUniverse)
                {
                    message = message + "\n" + "You have to clear allocation before add this link.";
                    Kernel.Util.MessageDisplayer.DisplayError(title, message);
                    return false;
                }
            }
            return true;
        }


        protected override void DeleteCurrentSelection()
        {
            if (Model != null && Model.IsUniverseGenerated() && Kernel.Application.ApplicationManager.Instance.AllocationCount > 0)
            {
                bool canDelete = true;
                foreach (Connection connection in SelectionService.CurrentSelection.OfType<Connection>())
                {
                    if (connection.Source != null && connection.Source.ParentDesignerItem != null
                        && connection.Sink != null && connection.Sink.ParentDesignerItem != null)
                    {
                        Entity sourceTag = (Entity)connection.Source.ParentDesignerItem.Tag;
                        Entity cibleTag = (Entity)connection.Sink.ParentDesignerItem.Tag;
                        if ((sourceTag != null && sourceTag.usedToGenerateUniverse) || (cibleTag != null && cibleTag.usedToGenerateUniverse))
                        {
                            canDelete = false;
                            break;
                        }
                    }
                }

                foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
                {
                    Entity sourceTag = (Entity)item.Tag;
                    Entity parentOfSourceTag = sourceTag;

                    while (parentOfSourceTag.parent!=null)
                    { 
                        parentOfSourceTag = parentOfSourceTag.parent;
                       
                    }
                    if (!canDelete || parentOfSourceTag.usedToGenerateUniverse)
                    {
                            canDelete = false;
                            break;
                    }
                }

                if (!canDelete)
                {
                    string message = "You're not allowed to delete this selection" + "\n" + "You have to clear allocation before delete.";
                    Kernel.Util.MessageDisplayer.DisplayError("Delete selection", message);
                    return;
                }
            }

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
                name = prefix + (i > 0 ? ""+i: "");
                valid = IsBlockNameValid(name);
                i++;
            }
            return name;
        }






    }
}
