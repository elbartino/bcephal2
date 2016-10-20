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
            
        }

        private void OnAllocationBoxDialogCancel(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void OnAllocationBoxDialogSave(object sender, System.Windows.RoutedEventArgs e)
        {
         
        }


    }
}
