using DiagramDesigner;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AllocationDiagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Controls;

namespace Misp.Sourcing.AllocationViews
{
    public class AllocationForm : ScrollViewer
    {
        public AllocationDiagram AllocationDiagramView { get; set; }
        public AllocationBoxDialog AllocationBoxDialog { get; set; }
        public DesignerItem EditedDesignerItem { get; set; }
        public TransformationTree EditedObject { get; set; }
        public TransformationTreeService TransformationTreeService { get; set; }
        public bool IsModify { get; set; }

        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        public Kernel.Ui.Base.ChangeEventHandler Change;

          #region Constructor

        public AllocationForm()
        {
            InitializeComponents();
            InitializeHandlers();
        }

                
        public void InitializeComponents()
        {
            this.AllocationDiagramView = new AllocationDiagram();
            this.Content = this.AllocationDiagramView;
        }

        #endregion

        /// <summary>
        /// Initialisation des handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            AllocationDiagramView.designerCanvas.SelectionService.SelectionChanged += onSelectBlok;
            AllocationDiagramView.designerCanvas.SelectionService.Clear += onClearSelection;

            AllocationDiagramView.designerCanvas.AddBlock += onAddBlock;
            AllocationDiagramView.designerCanvas.DeleteBlock += onDeleteBlock;
            AllocationDiagramView.designerCanvas.ModifyBlock += onModifyBlock;
            AllocationDiagramView.designerCanvas.AddLink += onAddLink;
            AllocationDiagramView.designerCanvas.Editing += OnEditAllocationBloc;
        }

        public void displayObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            if (!string.IsNullOrEmpty(this.EditedObject.diagramXml)) this.AllocationDiagramView.designerCanvas.Display(this.EditedObject.diagramXml);
            foreach (TransformationTreeItem item in this.EditedObject.itemListChangeHandler.Items)
            {
                refreshItem(item);
            }
        }

        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();            
            this.EditedObject.diagramXml = this.AllocationDiagramView.designerCanvas.AsString();
        }

        public TransformationTree getNewObject() { return new TransformationTree(); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void refreshItem(TransformationTreeItem item)
        {
            item.tree = this.EditedObject;
            //item.RefreshAttributeEntity();
            this.AllocationDiagramView.designerCanvas.RefreshEntity(item);
            foreach (TransformationTreeItem child in item.childrenListChangeHandler.Items)
            {
                child.parent = item;
                refreshItem(child);
            }
        }

        /// <summary>
        /// Cette methode est appelée lorsau'on a ajouté un lien entre deaux blocks
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        protected void onAddLink(DesignerItem parent, DesignerItem child)
        {
            if (parent.Tag == null || child.Tag == null) return;
            TransformationTreeItem parentTag = (TransformationTreeItem)parent.Tag;
            TransformationTreeItem childTag = (TransformationTreeItem)child.Tag;
            this.EditedObject.ForgetItem(childTag);
            parentTag.AddChild(childTag);
            this.IsModify = true;
        }

        protected void OnEditingItem(DesignerItem item)
        {
            if (item.Tag != null && item.Tag is TransformationTreeItem)
            {
                this.EditedDesignerItem = item;
                Edit((TransformationTreeItem)item.Tag);
            }
        }

        /// <summary>
        /// Cette méthode est exécuté lorqu'un objet ou une Vc est sélectionné dans le diagram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onSelectBlok(object sender)
        {
            if (sender is DesignerItem)
            {
                object tag = ((DesignerItem)sender).Tag;
                if (tag != null && tag is Kernel.Domain.Entity)
                {
                    //DisplayEntity((Kernel.Domain.Entity)tag);
                }
            }
        }


        protected void OnEditAllocationBloc(DesignerItem item)
        {
            if (item.Tag != null)
            {
                if (!(item.Tag is TransformationTreeItem))
                {
                    System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    Serializer.MaxJsonLength = 99999999;
                    item.Tag = Serializer.Deserialize<TransformationTreeItem>(item.Tag.ToString());
                }

                TransformationTreeItem treeItem = (TransformationTreeItem)item.Tag;
                if (this.IsModify)
                {
                    //Save(page);
                    DesignerItem block = this.AllocationDiagramView.designerCanvas.GetBlockByName(treeItem.name);
                    if (block == null) return;
                    item = block;
                    treeItem = (TransformationTreeItem)item.Tag;
                }
                this.EditedDesignerItem = item;
                this.Edit(treeItem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void onClearSelection()
        {
            //DisplayEntity(null);
            //attributeTree.selectedAttributes.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onAddBlock(DesignerItem sender)
        {
            if (this.EditedObject != null && sender.Tag != null)
            {
                JavaScriptSerializer serial = new JavaScriptSerializer();

                TransformationTreeItem transformationTreeItem = null;
                if (sender.Tag is TransformationTreeItem) transformationTreeItem = (TransformationTreeItem)sender.Tag;
                else if (sender.Tag is string) transformationTreeItem = serial.Deserialize<TransformationTreeItem>((string)sender.Tag);
                else return;

                this.EditedObject.AddItem(transformationTreeItem);
                this.IsModify = true;
                if (ChangeEventHandler != null) ChangeEventHandler.change();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onDeleteBlock(DesignerItem sender)
        {
            if (this.EditedObject != null && sender.Tag != null)
            {
                TransformationTreeItem entity = (TransformationTreeItem)sender.Tag;
                if (entity.parent != null)
                {
                    entity.parent.RemoveChild(entity);
                }
                else this.EditedObject.DeleteItem(entity);
                this.IsModify = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onModifyBlock(DesignerItem sender)
        {
            if (this.EditedObject != null && sender.Tag != null)
            {
                TransformationTreeItem entity = (TransformationTreeItem)sender.Tag;
                if (entity.parent != null) entity.parent.UpdateChild(entity);
                else this.EditedObject.UpdateItem(entity);
                this.IsModify = true;
            }
        }

        BusyAction action;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Edit(TransformationTreeItem item)
        {
            if (item == null) return;
            if (item.IsLoop)
            {
                if (this.AllocationBoxDialog == null)
                {
                    this.AllocationBoxDialog = new AllocationBoxDialog();
                    this.AllocationBoxDialog.TransformationTreeService = this.TransformationTreeService;
                    this.AllocationBoxDialog.initializeSideBar();
                    this.AllocationBoxDialog.SaveButton.Click += OnAllocationBoxDialogSave;
                    this.AllocationBoxDialog.CancelButton.Click += OnAllocationBoxDialogCancel;
                    this.AllocationBoxDialog.Closing += OnAllocationBoxDialogClosing;
                    this.AllocationBoxDialog.Owner = ApplicationManager.Instance.MainWindow;
                }

                this.AllocationBoxDialog.Loop = item;
                this.AllocationBoxDialog.DisplayItem();
                if (!this.AllocationBoxDialog.IsVisible) this.AllocationBoxDialog.ShowDialog();
            }
        }

        private void OnAllocationBoxDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.AllocationBoxDialog.Hide();
            e.Cancel = true;
        }

        private void OnAllocationBoxDialogCancel(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AllocationBoxDialog.Hide();
        }

        private void OnAllocationBoxDialogSave(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!ValidateEdition(this.EditedDesignerItem, this.AllocationBoxDialog.NameTextBox.Text.Trim())) return;
            this.AllocationBoxDialog.FillItem();
            //if (this.LoopDialog.Loop.parent != null) this.LoopDialog.Loop.parent.UpdateChild(this.LoopDialog.Loop);
            //else this.EditedObject.UpdateItem(this.LoopDialog.Loop);
            if (this.EditedDesignerItem != null) this.EditedDesignerItem.Renderer.Text = this.AllocationBoxDialog.Loop.name;
            this.AllocationDiagramView.designerCanvas.OnChange();
            if (Change != null) Change();            
        }

        protected bool ValidateEdition(DiagramDesigner.DesignerItem item, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty name", "The name can't be empty!");
                return false;
            }
            if (item != null && item.Tag != null && item.Tag is TransformationTreeItem)
            {
                DiagramDesigner.DesignerItem block = this.AllocationDiagramView.designerCanvas.GetBlockByName(name);
                TransformationTreeItem entity = (TransformationTreeItem)item.Tag;
                if (block != null && !block.Equals(item))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Duplicate name", "There is another block named: " + name + ".");
                    return false;
                }
            }
            return true;
        }

        public List<System.Windows.UIElement> getEditableControls()
        {
            List<System.Windows.UIElement> controls = new List<System.Windows.UIElement>(0);
            controls.Add(AllocationDiagramView.designerCanvas);            
            return controls;
        }


        public void Dispose()
        {
            if (this.AllocationBoxDialog != null) this.AllocationBoxDialog.Dispose();
            this.AllocationBoxDialog = null;            
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

    }
}
