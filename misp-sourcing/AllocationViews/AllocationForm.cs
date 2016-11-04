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
using System.Windows;
using System.Windows.Controls;

namespace Misp.Sourcing.AllocationViews
{
    public class AllocationForm : ScrollViewer
    {

        #region Properties

        public AllocationDiagram AllocationDiagramView { get; set; }
        public AllocationBoxDialog AllocationBoxDialog { get; set; }
        public DesignerItem EditedDesignerItem { get; set; }
        public TransformationTree EditedObject { get; set; }
        public TransformationTreeService TransformationTreeService { get; set; }
        public bool IsModify { get; set; }

        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        public Kernel.Ui.Base.ChangeEventHandler Change;

        #endregion


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


        #region Handlers

        /// <summary>
        /// Initialisation des handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            AllocationDiagramView.designerCanvas.Editing += OnEditingItem;
            AllocationDiagramView.designerCanvas.SelectionService.SelectionChanged += onSelectBlok;
            AllocationDiagramView.designerCanvas.SelectionService.Clear += onClearSelection;
            AllocationDiagramView.designerCanvas.AddBlock += onAddBlock;
            AllocationDiagramView.designerCanvas.DeleteBlock += onDeleteBlock;
            AllocationDiagramView.designerCanvas.ModifyBlock += onModifyBlock;          
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
                
                this.EditedObject.itemListChangeHandler.AddNew(transformationTreeItem);  
            }
            this.IsModify = true;
            if (Change != null) Change();
            if (ChangeEventHandler != null) ChangeEventHandler.change();
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
                foreach(TransformationTreeItem item in this.EditedObject.itemListChangeHandler.Items){
                    if (item.name.Equals(entity.name))
                    {
                        this.EditedObject.itemListChangeHandler.newItems.Remove(item);
                        this.EditedObject.itemListChangeHandler.deletedItems.Remove(item);
                        this.EditedObject.itemListChangeHandler.updatedItems.Remove(item);
                        this.EditedObject.itemListChangeHandler.Items.Remove(item);
                        break;
                    }
                }
            }
            this.IsModify = true;
            if (Change != null) Change();
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
            }
            this.IsModify = true;
            if (Change != null) Change();
        }


        private void OnLoopDialogCancel(object sender, RoutedEventArgs e)
        {
            this.AllocationBoxDialog.Hide();
            this.EditedDesignerItem = null;
        }

        private void OnLoopDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.AllocationBoxDialog.Hide();
            this.EditedDesignerItem = null;
        }

        private void OnLoopDialogSave(object sender, RoutedEventArgs e)
        {
            if (!ValidateEdition(this.EditedDesignerItem, this.AllocationBoxDialog.NameTextBox.Text.Trim())) return;
            this.AllocationBoxDialog.FillItem();
            if (this.EditedDesignerItem != null)
            {
                this.EditedDesignerItem.Renderer.Text = this.AllocationBoxDialog.Loop.name;
                this.EditedDesignerItem.Tag = this.AllocationBoxDialog.ValueField.Line;
            }
            this.AllocationBoxDialog.SaveButton.IsEnabled = false;
            this.AllocationBoxDialog.ValueField.ValueListChangeHandler = new PersistentListChangeHandler<TransformationTreeLoopValue>();
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
                    TransformationTreeItem tag = (TransformationTreeItem)block.Tag;
                    if (tag != null && !tag.name.Equals(entity.name))
                    {
                        Kernel.Util.MessageDisplayer.DisplayError("Duplicate name", "There is another block named: " + name + ".");
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        
        #region Operations
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Edit(TransformationTreeItem item)
        {
            if (item == null) return;
            item = item.oid != null ? this.TransformationTreeService.getItemByOid(item.oid) : item ;
            if (this.AllocationBoxDialog == null)
            {
                this.AllocationBoxDialog = new AllocationBoxDialog();
                this.AllocationBoxDialog.TransformationTreeService = this.TransformationTreeService;
                this.AllocationBoxDialog.initializeSideBar();
                this.AllocationBoxDialog.SaveButton.Click += OnLoopDialogSave;
                this.AllocationBoxDialog.CancelButton.Click += OnLoopDialogCancel;
                this.AllocationBoxDialog.Closing += OnLoopDialogClosing;
                this.AllocationBoxDialog.Owner = ApplicationManager.Instance.MainWindow;
            }
                
            this.AllocationBoxDialog.Loop = item;
            this.AllocationBoxDialog.DisplayItem();
            if (!this.AllocationBoxDialog.IsVisible) this.AllocationBoxDialog.ShowDialog();
        }

               
        public void RedisplayItem(TransformationTreeItem item)
        {
            if (item == null) return;
            if (this.AllocationBoxDialog == null)
            {
                Edit(item);
                return;
            }
            this.AllocationBoxDialog.Loop = item;
            this.AllocationBoxDialog.SaveButton.IsEnabled = false;
            if (!this.AllocationBoxDialog.IsVisible) this.AllocationBoxDialog.ShowDialog();
        }


        public void Dispose()
        {
            if (this.AllocationBoxDialog != null) this.AllocationBoxDialog.Dispose();
            this.AllocationBoxDialog = null;
        }

        public void SetValue(object value)
        {
            if (this.AllocationBoxDialog != null)
            {
                this.AllocationBoxDialog.SetValue(value);
                return;
            }
        }

        public TransformationTree getNewObject() { return new TransformationTree(); }

        public bool validateEdition() { return true; }

        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.EditedObject.diagramXml = this.AllocationDiagramView.designerCanvas.AsString();
        }

        public void displayObject()
        {            
            if (this.EditedObject == null)
            {
                this.AllocationDiagramView.designerCanvas.Children.Clear();
                return;
            }
            if (!string.IsNullOrEmpty(this.EditedObject.diagramXml)) this.AllocationDiagramView.designerCanvas.Display(this.EditedObject.diagramXml);
            foreach (TransformationTreeItem item in this.EditedObject.itemListChangeHandler.Items)
            {
                refreshItem(item);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void refreshItem(TransformationTreeItem item)
        {
            item.tree = this.EditedObject;
            this.AllocationDiagramView.designerCanvas.RefreshEntity(item);
            foreach (TransformationTreeItem child in item.childrenListChangeHandler.Items)
            {
                child.parent = item;
                refreshItem(child);
            }
        }

        public List<UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            controls.Add(AllocationDiagramView.designerCanvas);
            return controls;
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        #endregion

    }

}
