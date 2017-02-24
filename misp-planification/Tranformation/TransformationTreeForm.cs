using Misp.Planification.Base;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Sourcing.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Domain;
using Misp.Planification.Diagram;
using DiagramDesigner;
using Misp.Kernel.Service;
using Misp.Kernel.Application;
using System.Collections.ObjectModel;
using Misp.Kernel.Task;
using System.Web.Script.Serialization;
using Misp.Kernel.Administration.ObjectAdmin;

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeForm : Grid, IEditableView<TransformationTree>
    {

        #region Properties

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }
        
        public TransformationTree EditedObject { get; set; }

        public TransformationTreeService TransformationTreeService { get; set; }

        public bool IsModify { get; set; }

        public TransformationTreePropertiePanel TransformationTreePropertiePanel { get; set; }

        public TransformationTreeDiagram TransformationTreeDiagramView { get; set; }

        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public ChangeItemEventHandler SaveEventHandler { get; set; }

        public LoopDialog LoopDialog { get; set; }

        public TreeActionDialog ActionDialog { get; set; }

        public DesignerItem EditedDesignerItem { get; set; }

        public AdministrationBar AdministrationBar { get; set; }

        public static ObservableCollection<Kernel.Domain.Browser.InputTableBrowserData> globalListReport;
        #endregion


        #region Constructor

        public TransformationTreeForm(SubjectType subjectType)
        {
            this.SubjectType = subjectType;
            InitializeComponents();
            InitializeHandlers();
        }

                
        public void InitializeComponents()
        {
            this.TransformationTreePropertiePanel = new TransformationTreePropertiePanel();
            this.TransformationTreeDiagramView = new TransformationTreeDiagram();
            this.Children.Add(this.TransformationTreeDiagramView);
            if (ApplicationManager.Instance.User.IsAdmin())
            {
                this.AdministrationBar = new AdministrationBar(this.SubjectType);
            }
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialisation des handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            //TransformationTreeDiagramView.designerCanvas.Editing += OnEditingItem;
            TransformationTreeDiagramView.designerCanvas.SelectionService.SelectionChanged += onSelectBlok;
            TransformationTreeDiagramView.designerCanvas.SelectionService.Clear += onClearSelection;
            TransformationTreeDiagramView.designerCanvas.AddBlock += onAddBlock;
            TransformationTreeDiagramView.designerCanvas.DeleteBlock += onDeleteBlock;
            TransformationTreeDiagramView.designerCanvas.ModifyBlock += onModifyBlock;
            TransformationTreeDiagramView.designerCanvas.AddLink += onAddLink;
            TransformationTreeDiagramView.designerCanvas.DeleteLink += onDeleteLink;
            TransformationTreeDiagramView.designerCanvas.MoveLinkSource += onMoveLinkSource;
            TransformationTreeDiagramView.designerCanvas.MoveLinkTarget += onMoveLinkTarget;            
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
                    if (entity.childrenListChangeHandler.Items.Count > 0)
                    {
                        TransformationTreeItem entityChild = entity.childrenListChangeHandler.Items[0];
                        entity.ForgetChild(entityChild);
                        entityChild.parent = null;
                        this.EditedObject.AddItem(entityChild);
                    }

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

        /// <summary>
        /// Cette methode est appelée lorsau'on a supprimé un lien entre deaux blocks
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        protected void onDeleteLink(DesignerItem parent, DesignerItem child)
        {
            if (parent.Tag == null || child.Tag == null) return;
            TransformationTreeItem parentTag = parent.Tag is TransformationTreeItem ? (TransformationTreeItem)parent.Tag : null;
            TransformationTreeItem childTag = (TransformationTreeItem)child.Tag;
            if(parentTag != null)
            { 
                parentTag.ForgetChild(childTag);
                childTag.parent = null;
                this.EditedObject.AddItem(childTag);
                this.IsModify = true;
            }
        }

        private void onMoveLinkSource(DesignerItem oldParent, DesignerItem child, DesignerItem newParent)
        {
            if (oldParent.Tag == null || child.Tag == null || newParent.Tag == null) return;
            TransformationTreeItem oldParentTag = (TransformationTreeItem)oldParent.Tag;
            TransformationTreeItem newParentTag = (TransformationTreeItem)newParent.Tag;
            TransformationTreeItem childTag = (TransformationTreeItem)child.Tag;
            oldParentTag.ForgetChild(childTag);
            childTag.parent = null;
            newParentTag.AddChild(childTag);
            this.IsModify = true;
        }

        private void onMoveLinkTarget(DesignerItem parent, DesignerItem oldChild, DesignerItem newChild)
        {
            if (parent.Tag == null || oldChild.Tag == null || newChild.Tag == null) return;
            TransformationTreeItem parentTag = (TransformationTreeItem)parent.Tag;
            TransformationTreeItem oldChildTag = (TransformationTreeItem)oldChild.Tag;
            TransformationTreeItem newChildTag = (TransformationTreeItem)newChild.Tag;
            parentTag.ForgetChild(oldChildTag);
            oldChildTag.parent = null;
            this.EditedObject.AddItem(oldChildTag);
            parentTag.AddChild(newChildTag);
            this.IsModify = true;
        }

        private void OnLoopDialogCancel(object sender, RoutedEventArgs e)
        {
           // this.LoopDialog.ReportPanel.CloseReportWithoutSave();
            this.LoopDialog.Hide();
            this.EditedDesignerItem = null;
        }

        private void OnLoopDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
           // this.LoopDialog.ReportPanel.CloseReportWithoutSave();
            this.LoopDialog.Hide();
            this.EditedDesignerItem = null;
        }

        private void OnLoopDialogSave(object sender, RoutedEventArgs e)
        {
            if (!ValidateEdition(this.EditedDesignerItem, this.LoopDialog.NameTextBox.Text.Trim())) return;
            this.LoopDialog.FillItem();
            //if (this.LoopDialog.Loop.parent != null) this.LoopDialog.Loop.parent.UpdateChild(this.LoopDialog.Loop);
            //else this.EditedObject.UpdateItem(this.LoopDialog.Loop);
            if (this.EditedDesignerItem != null)
            {
                this.EditedDesignerItem.Renderer.Text = this.LoopDialog.Loop.name;
                this.EditedDesignerItem.Tag = this.LoopDialog.ValueField.Line;
            }
            this.TransformationTreeDiagramView.designerCanvas.OnChange();
            if (SaveEventHandler != null) SaveEventHandler(this.EditedDesignerItem);
           // this.LoopDialog.SaveLoop();
        }

        private void OnSaveLoopReportEnded(object item)
        {
            if (SaveEventHandler != null) SaveEventHandler(item);
        }

        private void OnActionDialogCancel(object sender, RoutedEventArgs e)
        {
            this.ActionDialog.CloseReportWithoutSave();
            this.ActionDialog.Hide();
            this.EditedDesignerItem = null;
        }

        private void OnActionDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.ActionDialog.CloseReportWithoutSave();
            this.ActionDialog.Hide();
            this.EditedDesignerItem = null;
        }

        private void OnActionDialogSave(object sender, RoutedEventArgs e)
        {
            if (!ValidateEdition(this.EditedDesignerItem, this.ActionDialog.NameTextBox.Text.Trim())) return;
            this.ActionDialog.FillItem();
            if (this.EditedDesignerItem != null)
            {
                this.EditedDesignerItem.Renderer.Text = this.ActionDialog.Action.name;
                this.EditedDesignerItem.Tag = this.ActionDialog.Action;
            }

            //if (this.ActionDialog.Action.parent != null) this.ActionDialog.Action.parent.UpdateChild(this.ActionDialog.Action);
            //else this.EditedObject.UpdateItem(this.ActionDialog.Action);
            //if (this.EditedDesignerItem != null) this.EditedDesignerItem.Renderer.Text = this.ActionDialog.Action.name;
            this.TransformationTreeDiagramView.designerCanvas.OnChange();
            this.ActionDialog.SaveAction();
            //this.ActionDialog.CloseReportWithSave();
           //this.ActionDialog.Hide();
        }

        private void OnSaveActionReportEnded(object item)
        {
            if (SaveEventHandler != null) SaveEventHandler(item);
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
                DiagramDesigner.DesignerItem block = this.TransformationTreeDiagramView.designerCanvas.GetBlockByName(name);
                TransformationTreeItem entity = (TransformationTreeItem)item.Tag;
                if (block != null && !block.Equals(item))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Duplicate name", "There is another TreeItem named: " + name + ".");
                    return false;
                }
            }
            return true;
        }

        #endregion


        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            if(this.TransformationTreePropertiePanel != null) this.TransformationTreePropertiePanel.SetReadOnly(readOnly);
            if (this.TransformationTreeDiagramView != null) this.TransformationTreeDiagramView.SetReadOnly(readOnly);
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
            TransformationTreeItem parent = item.parent;
            item = item.oid != null ? this.TransformationTreeService.getItemByOid(item.oid) : item ;
            item.parent = parent;
            item.tree = EditedObject;
            if (item.IsLoop)
            {
                if (this.LoopDialog == null)
                {
                    this.LoopDialog = new LoopDialog();
                    this.LoopDialog.TransformationTreeService = this.TransformationTreeService;
                    if (!this.IsReadOnly) this.LoopDialog.initializeSideBar();
                    if (!this.IsReadOnly) this.LoopDialog.SaveButton.Click += OnLoopDialogSave;
                    this.LoopDialog.CancelButton.Click += OnLoopDialogCancel;
                    this.LoopDialog.Closing += OnLoopDialogClosing;
                    this.LoopDialog.Owner = ApplicationManager.Instance.MainWindow;
                }
                
                this.LoopDialog.Loop = item;
                this.LoopDialog.SetReadOnly(this.IsReadOnly);
                this.LoopDialog.DisplayItem();               
                if (!this.LoopDialog.IsVisible) this.LoopDialog.ShowDialog();
            }
            else 
            {
                if (this.ActionDialog == null)
                {
                    this.ActionDialog = new TreeActionDialog();
                    this.ActionDialog.TransformationTreeService = this.TransformationTreeService;
                    if (!this.IsReadOnly) this.ActionDialog.initializeSideBarHandlers();
                    if (!this.IsReadOnly) this.ActionDialog.SaveButton.Click += OnActionDialogSave;
                    this.ActionDialog.CancelButton.Click += OnActionDialogCancel;
                    this.ActionDialog.Closing += OnActionDialogClosing;
                    this.ActionDialog.OnCloseSlideDialog += OnCloseSlideView;
                    this.ActionDialog.OnCloseTransformationTableDialog += OnCloseTransformationTableView;
                    this.ActionDialog.SaveEndedEventHandler += OnSaveActionReportEnded;
                    this.ActionDialog.Owner = ApplicationManager.Instance.MainWindow;
                }
                this.ActionDialog.IsReadOnly = this.IsReadOnly;
                if (this.ActionDialog.ReportEditorController == null) this.ActionDialog.initializeReport();
                action = new BusyAction(false)
                {
                    DoWork = () =>
                    {
                        try
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {                                
                                List<TransformationTreeItem> loops = this.EditedObject.GetAllLoops().ToList();
                                this.ActionDialog.initializeSideBarData(new ObservableCollection<TransformationTreeItem>(loops));
                                this.ActionDialog.Action = item;
                                this.ActionDialog.loops = loops;
                                this.ActionDialog.DisplayItem();
                                //this.ActionDialog.initializeSideBarData(new ObservableCollection<TransformationTreeItem>(loops));
                            }));
                        }
                        catch (Kernel.Service.ServiceExecption) { action = null; }
                        return OperationState.CONTINUE;
                    },

                    EndWork = () =>
                    {
                        try{ } catch (Exception){ }
                        finally{ action = null; }
                        return OperationState.CONTINUE;
                    }
                };
                                
                action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.ActionDialog.OnBusyPropertyChanged);
                this.ActionDialog.IsReadOnly = this.IsReadOnly;
                action.Run();
                this.ActionDialog.SetReadOnly(this.IsReadOnly);
                if(!this.ActionDialog.IsVisible) this.ActionDialog.ShowDialog();
            }
        }

               
        public void RedisplayItem(TransformationTreeItem item)
        {
            if (item == null) return;
            if (item.IsLoop)
            {
                if (this.LoopDialog == null)
                {
                    Edit(item);
                    return;
                }
                this.LoopDialog.Loop = item;
               // this.LoopDialog.ReportPanel.TreeItem = item;
                this.LoopDialog.SaveButton.IsEnabled = false;
                if (!this.LoopDialog.IsVisible) this.LoopDialog.ShowDialog();
            }
            else
            {
                if (this.ActionDialog == null)
                {
                    Edit(item);
                    return;
                }
                this.ActionDialog.Action = item;
                this.ActionDialog.RedisplayItem();
                if (!this.ActionDialog.IsVisible) this.ActionDialog.ShowDialog();
            }
        }

        private void OnCloseTransformationTableView()
        {
            
        }

        private void OnCloseSlideView()
        {
            
        }

        public void Dispose()
        {
            if (this.LoopDialog != null) this.LoopDialog.Dispose();
            if (this.ActionDialog != null) this.ActionDialog.Dispose();
            this.LoopDialog = null;
            this.ActionDialog = null;
        }

        public void SetValue(object value)
        {
            if (this.LoopDialog != null)
            {
                this.LoopDialog.SetValue(value);
                return;
            }
        }

        public TransformationTree getNewObject() { return new TransformationTree(); }

        public bool validateEdition() { return true; }

        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.TransformationTreePropertiePanel.fillTransformationTree(this.EditedObject);
            this.EditedObject.diagramXml = this.TransformationTreeDiagramView.designerCanvas.AsString();
        }

        public void displayObject()
        {
            this.TransformationTreePropertiePanel.displayTransformationTree(this.EditedObject);
            if (!string.IsNullOrEmpty(this.EditedObject.diagramXml)) this.TransformationTreeDiagramView.designerCanvas.Display(this.EditedObject.diagramXml);
            foreach (TransformationTreeItem item in this.EditedObject.itemListChangeHandler.Items)
            {
                refreshItem(item);
            }
            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void refreshItem(TransformationTreeItem item)
        {
            item.tree = this.EditedObject;
            //item.RefreshAttributeEntity();
            this.TransformationTreeDiagramView.designerCanvas.RefreshEntity(item);
            foreach (TransformationTreeItem child in item.childrenListChangeHandler.Items)
            {
                child.parent = item;
                refreshItem(child);
            }
        }

        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(TransformationTreeDiagramView.designerCanvas);
            controls.AddRange(this.TransformationTreePropertiePanel.getEditableControls());
            return controls;
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

    }
}
