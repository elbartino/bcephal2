using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Application;
using Misp.Kernel.Service;
using System.Windows.Data;
using Misp.Reporting.Base;
using Misp.Reporting.Report;
using Misp.Kernel.Util;
using System.Windows.Input;
using System.Windows;
using DiagramDesigner;
using Misp.Planification.CombinedTransformationTree;
using Misp.Kernel.Ui.Office;
using Misp.Planification.Tranformation.InstructionControls;
using System.Windows.Controls;
using Misp.Kernel.Ui.Sidebar;


namespace Misp.Planification.Tranformation
{
    public class TransformationTreeEditorController : EditorController<TransformationTree, BrowserData>
    {

        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }

        #endregion



        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TransformationTreeEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
        }

        #endregion


        #region Operations

        /// <summary>
        /// Cette methode permet de créer un nouvel arbre.
        /// </summary>
        /// <returns>
        /// - CONTINUE si la création de l'arbre se termine avec succès. 
        /// - STOP sinon
        /// </returns>
        public override OperationState Create()
        {
            try
            {
                TransformationTree transformationTree = GetNewTransformationTree();
                ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.AddTransformationTree(transformationTree);
                TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().addOrSelectPage(transformationTree);
                page.GetTransformationTreeForm().TransformationTreeService = this.GetTransformationTreeService();
                initializePageHandlers(page);
                page.Title = transformationTree.name;
                page.EditedObject = transformationTree;
                getTransformationTreeEditor().ListChangeHandler.AddNew(transformationTree);
            }
            catch (Exception e) { 
                logger.Error("Unable to create new Transformation tree", e);
            }
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
        }

        /// <summary>
        /// Ouvre l'arbre passé en parametre dans l'éditeur.
        /// </summary>
        /// <param name="table">L'arbre à ouvrir</param>
        /// <returns>
        /// - CONTINUE si l'ouverture de l'arbre se termine avec succès. 
        /// - STOP sinon
        /// </returns>
        public override OperationState Open(TransformationTree tree)
        {
            if (tree == null)
            {
                MessageDisplayer.DisplayInfo("Transformation Tree", "Unable to open the selected tree");
                this.ApplicationManager.HistoryHandler.closePage(this);
                return OperationState.STOP;
            }

            bool isReadonly = false;
            if (tree.oid.HasValue)
            {
                bool isOk = GetTransformationTreeService().locked(ApplicationManager.File.oid.Value, tree.oid.Value);
                if (!isOk)
                {
                    MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Tree Locked", "Tree '" + tree.name + "' is locked by another user!\n"
                        + "You cannot edit the tree until the tree is open by another user.\n"
                        + "Do you want to switch in read only mode ?");
                    if (MessageBoxResult.Yes != response) return OperationState.STOP;
                    else isReadonly = true;
                }
            }
            ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.AddTransformationTreeIfNatExist(tree);
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().addOrSelectPage(tree, isReadonly);
            page.GetTransformationTreeForm().TransformationTreeService = this.GetTransformationTreeService();
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(tree);
            return OperationState.CONTINUE;
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        public virtual OperationState Run()
        {
            OperationState state = OperationState.CONTINUE;
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            if (page == null) return state;
            List<int> stringOid = new List<int>();
            if (!page.EditedObject.oid.HasValue) Save(page);
            if (!page.EditedObject.oid.HasValue) return state;
            stringOid.Add((int)page.EditedObject.oid);
            PowerpointLoader.stop = false;
            GetTransformationTreeService().RunHandler += updateRunProgress;
            GetTransformationTreeService().PowerpointHandler += loadPowerpoint;
            PowerpointLoader.PRESENTATIONS.Clear();
            GetTransformationTreeService().Run(stringOid);
            Mask(true, "Running...", true);
            return state;
        }

        public virtual OperationState StopRun()
        {
            OperationState state = OperationState.CONTINUE;
            PowerpointLoader.Stop();
            GetTransformationTreeService().StopRun();
            return state;
        }

        private void updateRunProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                GetTransformationTreeService().RunHandler -= updateRunProgress;
                GetTransformationTreeService().PowerpointHandler -= loadPowerpoint;
                Mask(false);
            }
            else
            {
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.ProgressBarTree.Maximum = info.totalCount;
                ApplicationManager.MainWindow.ProgressBarTree.Value = info.runedCount;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = "Tree running: " + rate + " %" + " (" + info.runedCount + "/" + info.totalCount + ")";
            }
        }

        private void updatePowerpointLoadProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                PowerpointLoader.RunHandler -= updatePowerpointLoadProgress;
                ApplicationManager.MainWindow.PowerpointProgressBarPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                ApplicationManager.MainWindow.PowerpointProgressBarPanel.Visibility = Visibility.Visible;
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.PowerpointProgressBar.Maximum = info.totalCount;
                ApplicationManager.MainWindow.PowerpointProgressBar.Value = info.runedCount;
                ApplicationManager.MainWindow.PowerpointProgressBarLabel.Content = info.errorMessage + " [" + rate + " %]";
            }
        }

        private void loadPowerpoint(Kernel.Ui.Office.PowerpointLoadInfo info)
        {
            if (info == null) return;
            PowerpointLoader.RunHandler += updatePowerpointLoadProgress;
            PowerpointLoader.FileTransfertService = GetTransformationTreeService().FileService.FileTransferService;
            PowerpointLoader.LoopCount = info.items.Count;
            PowerpointLoader.Load(info);
        }


        /// <summary>
        /// Cette methode permet d'exporter le fichier excel ouvert dans la page active.
        /// On ouvre le dialogue pour permettre à l'utilisateur d'indiquer le répertoire et le nom
        /// sous lequel il faut exporter le fichier.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Clear()
        {
            OperationState state = OperationState.CONTINUE;
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            if (page == null) return state;
            if (page.IsModify) state = Save(page);
            if (state == OperationState.STOP) return state;            
            if (page.EditedObject.oid == null || !page.EditedObject.oid.HasValue) return state;

            TableActionData data = new TableActionData(page.EditedObject.oid.Value, null);

            GetTransformationTreeService().ClearTreeHandler += updateClearProgress;
            GetTransformationTreeService().ClearTree(data);
            Mask(true, "Clearing...");
            return state;
        }

        private void updateClearProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                GetTransformationTreeService().ClearTreeHandler -= updateClearProgress;
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
                Mask(false);
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.ProgressBarTree.Maximum = runInfo.totalCellCount;
                ApplicationManager.MainWindow.ProgressBarTree.Value = runInfo.runedCellCount;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = "" + rate + " %";
            }
        }


        protected void Mask(bool mask, string content = "Saving...", bool isRun = false)
        {
            ApplicationManager.MainWindow.OnCancelProgression -= OnCancelProgression;
            ApplicationManager.MainWindow.setCloseButton1Visible(mask && isRun);
            ApplicationManager.MainWindow.BusyBorder2.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                ApplicationManager.MainWindow.ProgressBarTree.Maximum = 100;
                ApplicationManager.MainWindow.ProgressBarTree.Value = 0;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = content;
                ApplicationManager.MainWindow.ProgressBarTree.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.statusTextBlockTree.Visibility = Visibility.Visible; 
                if (isRun)
                {
                    ApplicationManager.MainWindow.OnCancelProgression += OnCancelProgression;
                    ApplicationManager.MainWindow.setCloseButton1ToolTip("Stop run");
                }
            }
        }

        private void OnCancelProgression()
        {
            if (MessageDisplayer.DisplayYesNoQuestion("Stop run", "Do you want to stop the current run ?") == MessageBoxResult.Yes)
            {
                StopRun();
            }            
        }    

        protected override void Rename(string name)
        {            
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getEditor().getActivePage();
            if (validateName(page, name))
            {
                ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.updateTransformationTree(name, page.Title, false);
                page.GetTransformationTreeForm().TransformationTreePropertiePanel.nameTextBox.Text = name;
                base.Rename(name);
            }
            else
            {
                page.GetTransformationTreeForm().TransformationTreePropertiePanel.nameTextBox.Text = page.Title;
            }
        }

        public override bool validateName(EditorItem<TransformationTree> page, string name)
        {
            if (!base.validateName(page, name)) return false;
            TransformationTree tree = ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.getTransformationTreeByName(name);
            if (tree == null) return true;

            if (page.EditedObject.oid.HasValue && tree.oid.HasValue && page.EditedObject.oid.Value != tree.oid.Value)
            {
                Kernel.Util.MessageDisplayer.DisplayError("Duplicate Name", "Another tree named " + name + " already exists!");
                return false;
            }

            if (page.EditedObject.oid.HasValue != (tree.oid.HasValue && tree.oid.Value > 0))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Duplicate Name", "Another tree named " + name + " already exists!");
                return false;
            }

            return true;
        }
        
        #endregion

        
        #region Editor and Service

        public TransformationTreeEditor getTransformationTreeEditor() { return (TransformationTreeEditor)this.View; }

        protected TransformationTreeService GetTransformationTreeService() { return (TransformationTreeService)Service; }

        #endregion


        #region ToolBar

        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new TransformationTreeToolBar(); }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return new TransformationTreeToolBarBuilder(this);
        }

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            if (this.ToolBar == null) return;
            ((TransformationTreeToolBar)this.ToolBar).RunButton.Click += OnRun;
            ((TransformationTreeToolBar)this.ToolBar).ClearButton.Click += OnClear;
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            OperationState result = Run();
            if (result == OperationState.CONTINUE) this.AfterSave();
        }

        private void OnClear(object sender, RoutedEventArgs e) { Clear(); }

        #endregion


        #region SideBar

        protected override SideBar getNewSideBar() { return new TransformationTreeSideBar(); }

        protected override void initializeSideBarData()
        {
            List<BrowserData> datas = this.GetTransformationTreeService().getBrowserDatas();
            ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.fillTree(new ObservableCollection<BrowserData>(datas));
            BGroup group = GetTransformationTreeService().GroupService.getDefaultGroup();

        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.SelectionChanged += onSelectTransformationTreeFromSidebar;
        }
        
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectTransformationTreeFromSidebar(object sender)
        {
            if (sender != null && sender is TransformationTree)
            {
                TransformationTree tree = (TransformationTree)sender;
                EditorItem<TransformationTree> page = getTransformationTreeEditor().getPage(tree.name);
                if (page != null)
                {
                    page.fillObject();
                    getTransformationTreeEditor().selectePage(page);
                }
                else if (tree.oid != null && tree.oid.HasValue)
                {
                    this.Open(tree.oid.Value);
                }
                else
                {
                    page = getTransformationTreeEditor().addOrSelectPage(tree);
                    initializePageHandlers(page);
                    page.Title = tree.name;

                    getTransformationTreeEditor().ListChangeHandler.AddNew(tree);
                }
                //UpdateStatusBar();
            }
        }

        #endregion


        #region PropertyBar

        protected override PropertyBar getNewPropertyBar() { return new TransformationTreePropertyBar(); }

        #endregion


        #region Page
         /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new TransformationTreeEditor(this.SubjectType); }

        protected override void initializePageHandlers(EditorItem<TransformationTree> page)
        {
            base.initializePageHandlers(page);
            TransformationTreeEditorItem editorPage = (TransformationTreeEditorItem)page;
            editorPage.GetTransformationTreeForm().TransformationTreePropertiePanel.groupField.GroupService = GetTransformationTreeService().GroupService;
            editorPage.GetTransformationTreeForm().TransformationTreePropertiePanel.groupField.subjectType = SubjectTypeFound();
            editorPage.GetTransformationTreeForm().TransformationTreePropertiePanel.groupField.Changed += onGroupFieldChange;
            editorPage.GetTransformationTreeForm().TransformationTreePropertiePanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.GetTransformationTreeForm().TransformationTreePropertiePanel.visibleInShortcutCheckBox.Checked += OnTableVisibleInShortcutOptionChecked;
            editorPage.GetTransformationTreeForm().TransformationTreePropertiePanel.visibleInShortcutCheckBox.Unchecked += OnTableVisibleInShortcutOptionChecked;
            editorPage.GetTransformationTreeForm().TransformationTreeDiagramView.designerCanvas.Editing += OnEditingItem;

            editorPage.GetTransformationTreeForm().SaveEventHandler += OnEditingItemEnded;

            if (editorPage.GetTransformationTreeForm().AdministrationBar != null)
            {
                editorPage.GetTransformationTreeForm().AdministrationBar.Changed += OnChangeEventHandler;
            }
        }

        private void OnChangeEventHandler()
        {
            OnChange();
        }

        private void OnTableVisibleInShortcutOptionChecked(object sender, RoutedEventArgs e)
        {
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            if (page == null) return;
            page.EditedObject.isModified = true;
            page.EditedObject.visibleInShortcut = (sender as CheckBox).IsChecked.Value;
            OnChange();
        }

        private void OnEditingItemEnded(object obj)
        {
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            if (page.GetTransformationTreeForm().EditedDesignerItem == null || page.GetTransformationTreeForm().EditedDesignerItem.Tag == null) return;
            TransformationTreeItem item = (TransformationTreeItem)page.GetTransformationTreeForm().EditedDesignerItem.Tag;
            TransformationTreeItem parent = item.parent;
            item = GetTransformationTreeService().SaveTransformationTreeItem(item);
            AfterSave();
            if (item != null)
            {
                item.parent = parent;
                item.tree = page.EditedObject;

                if (item.parent != null) item.parent.ReplaceChild(item);
                else page.EditedObject.ReplaceItem(item);

                page.GetTransformationTreeForm().refreshItem(item);
                
                page.GetTransformationTreeForm().EditedDesignerItem.Tag = item;
                page.GetTransformationTreeForm().RedisplayItem(item);
            }
            BlockPanel.Loops = new List<TransformationTreeItem>(page.EditedObject.GetAllLoops().ToList());
        }        

        protected void OnEditingItem(DesignerItem item)
        {
            if (item.Tag != null)
            {
                if(!(item.Tag is TransformationTreeItem)){
                    System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    Serializer.MaxJsonLength = 99999999;
                    item.Tag = Serializer.Deserialize<TransformationTreeItem>(item.Tag.ToString());             
                }

                TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
                TransformationTreeItem treeItem = (TransformationTreeItem)item.Tag;
                //if (treeItem.IsAction && page.EditedObject.hasUnsavedLoop())
                if (page.IsModify)
                {
                    Save(page);
                    DesignerItem block = page.GetTransformationTreeForm().TransformationTreeDiagramView.designerCanvas.GetBlockByName(treeItem.name);
                    if (block == null) return;
                    item = block;
                    treeItem = (TransformationTreeItem)item.Tag;
                }
                page.GetTransformationTreeForm().EditedDesignerItem = item;
                this.RemoveCommands();
                page.GetTransformationTreeForm().Edit(treeItem);
                this.initializeCommands();
            }
        }

        protected void onGroupFieldChange()
        {
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            string name = page.GetTransformationTreeForm().TransformationTreePropertiePanel.groupField.textBox.Text;
            BGroup group = page.GetTransformationTreeForm().TransformationTreePropertiePanel.groupField.Group;
            page.EditedObject.group = group;
            ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.updateTransformationTree(name, page.Title, true);
            //page.GetTransformationTreeForm().TransformationTreePropertiePanel.displayTransformationTree(tree);
            page.EditedObject.isModified = true;
        }

        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.GetTransformationTreeForm().TransformationTreePropertiePanel.nameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                String name = page.GetTransformationTreeForm().TransformationTreePropertiePanel.nameTextBox.Text;
                Rename(name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Kernel.Domain.TransformationTree> page)
        {
            if (page == null) return;
            base.OnPageSelected(page);
            TransformationTreeForm form = ((TransformationTreeEditorItem)page).GetTransformationTreeForm();
            if (form.TransformationTreePropertiePanel != null)
                ((TransformationTreePropertyBar)this.PropertyBar).TableLayoutAnchorable.Content = form.TransformationTreePropertiePanel;
            TransformationTreePropertyBar bar = (TransformationTreePropertyBar)this.PropertyBar;
            if (bar.AdministratorLayoutAnchorable != null) bar.AdministratorLayoutAnchorable.Content = form.AdministrationBar;
        }

        protected override void OnPageClosed(object sender, EventArgs args)
        {            
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)sender;
            if (page == null)
            {
                base.OnPageClosed(sender, args);
                return;
            }
            page.GetTransformationTreeForm().Dispose();
            if (!page.EditedObject.oid.HasValue)
            {
                ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.RemoveTransformationTree(page.EditedObject);
            }
            base.OnPageClosed(sender, args);
            GetTransformationTreeService().unlocked(ApplicationManager.Instance.File.oid.Value,page.EditedObject.oid.Value);
        }

        /// <summary>
        /// Close all opened Excel files
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();
            Kernel.Util.ClipbordUtil.ClearClipboard();
            GetTransformationTreeService().unlockedAll(ApplicationManager.Instance.File.oid.Value);
        }
        

        private void UpdateSaveInfo(SaveInfo info, object transformationTree)
        {
            TransformationTreeEditorItem page = (TransformationTreeEditorItem)getTransformationTreeEditor().getActivePage();
            if (page != null && transformationTree != null && transformationTree is TransformationTree)
            {
                Mask(false);
                page.EditedObject = (TransformationTree)transformationTree;
                page.displayObject();
                page.IsModify = false;
                return;
            }

            if (info == null || info.isEnd == true)
            {
                Mask(false);
                GetTransformationTreeService().SaveTransformationTreeHandler -= UpdateSaveInfo;
                Service.FileService.SaveCurrentFile();
            }
            else
            {
                int rate = info.stepCount != 0 ? (Int32)(info.stepRuned * 100 / info.stepCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.stepCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.stepRuned;
                ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";
            }
        }

       
        #endregion


        #region Utils

        protected virtual TransformationTree GetNewTransformationTree()
        {
            TransformationTree tree = new TransformationTree();
            tree.name = getNewPageName("Transformation Tree");
            tree.group = GetTransformationTreeService().GroupService.getDefaultGroup();
            return tree;
        }

        protected override TransformationTree GetObjectByName(string name)
        {
            return ((TransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.getTransformationTreeByName(name);
        }

        #endregion
       

    }
}
