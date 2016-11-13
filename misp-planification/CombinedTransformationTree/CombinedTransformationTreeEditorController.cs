using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Util;
using Misp.Planification.Tranformation.Run_all;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeEditorController : EditorController<Kernel.Domain.CombinedTransformationTree, Misp.Kernel.Domain.Browser.BrowserData>
    {


        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }

        bool isClearOption { get; set; }
    
        #endregion

        public CombinedTransformationTreeEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }
        

        #region Editor and Service
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public CombinedTransformationTreeEditor getCombineTransformationTreeEditor()
        {
            return (CombinedTransformationTreeEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux Designs.
        /// </summary>
        /// <returns>DesignService</returns>
        public CombineTransformationTreeService GetCombineTransformationTreeService()
        {
            return (CombineTransformationTreeService)base.Service;
        }

        #endregion


        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Kernel.Domain.CombinedTransformationTree combinedTransformationTree = GetNewCombinedTransformationTree();
            ((CombinedTransformationTreeSideBar)SideBar).CombineTransformationTreeGroup.combinedTransformationTreeTreeview.AddTarget(combinedTransformationTree);
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().addOrSelectPage(combinedTransformationTree);
            initializePageHandlers(page);
            page.Title = combinedTransformationTree.name;

            getCombineTransformationTreeEditor().ListChangeHandler.AddNew(combinedTransformationTree);
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.COMBINED_TRANSFORMATION_TREE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Kernel.Domain.CombinedTransformationTree combinedTransformationTree)
        {
            if (combinedTransformationTree == null)
            {
                MessageDisplayer.DisplayWarning("Combined Transformation Tree", "This Combined Transformation tree is invalid");
                this.ApplicationManager.HistoryHandler.closePage(this);
                return OperationState.STOP;
            }

            int oidG = (int)combinedTransformationTree.group.oid;            
            BGroup gr = (BGroup)GetCombineTransformationTreeService().GroupService.getByOid(oidG);
            combinedTransformationTree.group = gr;

            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getEditor().addOrSelectPage(combinedTransformationTree);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(combinedTransformationTree);
            page.getCombineTransformationTreeForm().GroupService = GetCombineTransformationTreeService().GroupService;
            return OperationState.CONTINUE;
        }

        protected virtual Kernel.Domain.CombinedTransformationTree GetNewCombinedTransformationTree()
        {
            Kernel.Domain.CombinedTransformationTree combinedTransformationTree = new Kernel.Domain.CombinedTransformationTree();
            combinedTransformationTree.name = getNewPageName("CombinedTransformationTree");
            combinedTransformationTree.group = GetCombineTransformationTreeService().GroupService.getDefaultGroup();
            return combinedTransformationTree;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Kernel.Domain.CombinedTransformationTree combinedTransformationTree = GetObjectByName(name);
                if (combinedTransformationTree == null) return name; 
                i++;
            }
            return name;
        }

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        CombinedTransformationTreeEditorItem currentPage = new CombinedTransformationTreeEditorItem();

        public override OperationState Save(EditorItem<Kernel.Domain.CombinedTransformationTree> page)
        {
            try
            {
                currentPage = (CombinedTransformationTreeEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;                
            }
            catch (Exception)
            {
                DisplayError("Unable to save Target", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        private Kernel.Domain.CombinedTransformationTree GetCombinedTransformationTree(string name)
        {
            if (!IsNameUsed(name))
            {
                Kernel.Domain.CombinedTransformationTree combinedTransformationTree = new Kernel.Domain.CombinedTransformationTree();
                combinedTransformationTree.name = name;
                combinedTransformationTree.group = GetCombineTransformationTreeService().GroupService.getDefaultGroup();
                return combinedTransformationTree;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Kernel.Domain.CombinedTransformationTree obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another Target named: " + name);
                return true;
            }
            return false;
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Kernel.Domain.CombinedTransformationTree> page)
        {
            if (page == null) return;
            CombinedTransformationTreeForm form = ((CombinedTransformationTreeEditorItem)page).getCombineTransformationTreeForm();
            ((CombinedTransformationTreePropertyBar)this.PropertyBar).TableLayoutAnchorable.Content = form.CombinedTransformationTreePropertiesPanel;
        }
        
        /// <summary>
        /// Methode à exécuter lorsqu'il y a un changement sur la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState OnChange()
        {
            base.OnChange();
            return OperationState.CONTINUE;
        }
                      

        public override OperationState Rename()
        {
            if (base.Rename() != OperationState.CONTINUE)
                return OperationState.STOP;

            IsRenameOnDoubleClick = true;
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.Text = name;
            page.EditedObject.name = name;
            base.Rename(name);
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        #endregion


        #region Others

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new CombinedTransformationTreeEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new CombineTransformationTreeToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new CombinedTransformationTreeSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new CombinedTransformationTreePropertyBar(); }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }


        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            if (this.ToolBar == null) return;
            ((CombineTransformationTreeToolBar)this.ToolBar).RunButton.Click += OnRun;
            ((CombineTransformationTreeToolBar)this.ToolBar).ClearButton.Click += OnClear;
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            if (page == null) return;
            if (!this.IsModify)
            {
                if (Save(getCombineTransformationTreeEditor().getActivePage()) == OperationState.STOP) return;
            }
            List<int> listTreeToRun =  page.EditedObject.getTransformationTreesOids();
            GetCombineTransformationTreeService().TransformationTreeService.RunHandler += updateRunProgress;
            GetCombineTransformationTreeService().TransformationTreeService.PowerpointHandler += loadPowerpoint;
            GetCombineTransformationTreeService().TransformationTreeService.Run(listTreeToRun, true);
            Mask(true, RunMessageUtil.getMaskStartText(), true);
        }

        public virtual OperationState StopRun()
        {
            OperationState state = OperationState.CONTINUE;
            GetCombineTransformationTreeService().TransformationTreeService.StopRun();
            return state;
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            if (page == null) return;
            List<int> listTreeToClear = page.EditedObject.getTransformationTreesOids();
            TableActionData tableActionData = new TableActionData();
            tableActionData.oids.AddRange(listTreeToClear);
           
            GetCombineTransformationTreeService().TransformationTreeService.ClearTreeHandler += updateClearProgress;
            GetCombineTransformationTreeService().TransformationTreeService.ClearTree(tableActionData);
            Mask(true, RunMessageUtil.getMaskStartText(true));
        }

        private void updateClearProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                GetCombineTransformationTreeService().TransformationTreeService.ClearTreeHandler -= updateClearProgress;
                this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Service.FileService.SaveCurrentFile();
                Mask(false);
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = runInfo.totalCellCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = runInfo.runedCellCount;
                ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";
            }
        }

        private void updateRunProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                GetCombineTransformationTreeService().TransformationTreeService.RunHandler -= updateRunProgress;
                GetCombineTransformationTreeService().TransformationTreeService.PowerpointHandler -= loadPowerpoint;
                Mask(false, RunMessageUtil.getMaskEndedText());

                Service.FileService.SaveCurrentFile();
                ApplicationManager.MainWindow.treeDetails.Visibility = Visibility.Hidden;
                ApplicationManager.MainWindow.ProgressBarTreeContent.Maximum = 0;
                ApplicationManager.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.MainWindow.statusTextBlockTreeContent.Content = "";
            }
            else
            {
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.ProgressBarTree.Maximum = info.totalCount;
                ApplicationManager.MainWindow.ProgressBarTree.Value = info.runedCount;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = "" + rate + " % " + " (" + info.runedCount + " / " + info.totalCount + ")";

                if (info.currentTreeRunInfo != null)
                {
                    rate = info.currentTreeRunInfo.runedCount != 0 ? (Int32)(info.currentTreeRunInfo.runedCount * 100 / info.currentTreeRunInfo.totalCount) : 0;
                    if (rate > 100) rate = 100;

                    if (info.currentTreeRunInfo.runedCount != 0)
                    {
                        ApplicationManager.MainWindow.treeDetails.Visibility = Visibility.Visible;
                        ApplicationManager.MainWindow.ProgressBarTreeContent.Maximum = info.currentTreeRunInfo.totalCount;
                        ApplicationManager.MainWindow.ProgressBarTreeContent.Value = info.currentTreeRunInfo.runedCount;
                        ApplicationManager.MainWindow.statusTextBlockTreeContent.Content = "" + info.currentTreeRunInfo.item + " :  " + rate + " %" + " (" + info.currentTreeRunInfo.runedCount + " / " + info.currentTreeRunInfo.totalCount + ")";
                    }
                }

            }
        }

        private void loadPowerpoint(Kernel.Ui.Office.PowerpointLoadInfo info)
        {
            if (info == null) return;
            PowerpointLoader.FileTransfertService = GetCombineTransformationTreeService().FileService.FileTransferService;
            PowerpointLoader.Load(info);
        }


        protected void Mask(bool mask, string content = "Saving...", bool isRun = false)
        {
            ApplicationManager.MainWindow.OnCancelProgression -= OnCancelProgression;
            ApplicationManager.MainWindow.setCloseButton1Visible(mask && isRun);
            ApplicationManager.MainWindow.BusyBorder2.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask && isRun)
            {
                ApplicationManager.MainWindow.OnCancelProgression += OnCancelProgression;
                ApplicationManager.MainWindow.setCloseButton1ToolTip("Stop run");
            }
        }

        private void OnCancelProgression()
        {
            if (MessageDisplayer.DisplayYesNoQuestion("Stop run", "Do you want to stop the current run ?") == MessageBoxResult.Yes)
            {
                StopRun();
            }   
        }   

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData()
        {
            
        }

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<Kernel.Domain.CombinedTransformationTree> page)
        {
            base.initializePageHandlers(page);
            CombinedTransformationTreeEditorItem editorPage = (CombinedTransformationTreeEditorItem)page;
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.groupField.GroupService = GetCombineTransformationTreeService().GroupService;
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePanel.Changed += OnCombineTransformationTreePanelChanged;
            editorPage.getCombineTransformationTreeForm().CombinedTransformationTreePanel.ItemDeleted += OnCombineTransformationTreePanelDeleted;
       }

        private void OnCombineTransformationTreePanelChanged()
        {
            OnChange();    
        }


        protected void OnCombineTransformationTreePanelDeleted(object item)
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            if (page == null) return;
            page.EditedObject.DeleteTreeItem((CombinedTransformationTreeItem)item);
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
            {
            //List<Kernel.Domain.CombinedTransformationTree> combinedTransformationTrees = GetCombineTransformationTreeService().getAll();
            List<Kernel.Domain.Browser.BrowserData> combinedBrowserData = GetCombineTransformationTreeService().getBrowserDatas();
            ((CombinedTransformationTreeSideBar)SideBar).CombineTransformationTreeGroup.combinedTransformationTreeTreeview.fillTree(new ObservableCollection<Kernel.Domain.Browser.BrowserData>(combinedBrowserData));
            List<Kernel.Domain.Browser.BrowserData> listTrees = GetCombineTransformationTreeService().TransformationTreeService.getBrowserDatas();
            ((CombinedTransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.fillTree(
               new ObservableCollection<Kernel.Domain.Browser.BrowserData>(listTrees));
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((CombinedTransformationTreeSideBar)SideBar).CombineTransformationTreeGroup.combinedTransformationTreeTreeview.SelectionChanged += onSelectCombinedTreeFromSidebar;
            ((CombinedTransformationTreeSideBar)SideBar).TransformationTreeGroup.TransformationTreeTreeview.SelectionChanged += onSelectTransformationTreeFromSidebar;
        }
        
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectTransformationTreeFromSidebar(object sender)
        {
            if (sender != null && sender is Kernel.Domain.TransformationTree)
            {
                Kernel.Domain.TransformationTree tree = (Kernel.Domain.TransformationTree)sender;
                CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
                if (page == null) return;
                page.getCombineTransformationTreeForm().CombinedTransformationTreePanel.setTransformationTree(tree);
            }
        }

        private void onSelectCombinedTreeFromSidebar(object sender)
        {
            if (sender != null && sender is Kernel.Domain.CombinedTransformationTree)
            {
                Kernel.Domain.CombinedTransformationTree ctree = (Kernel.Domain.CombinedTransformationTree)sender;
                EditorItem<Kernel.Domain.CombinedTransformationTree> page = getCombineTransformationTreeEditor().getPage(ctree.name);
                if (page != null)
                {
                    page.fillObject();
                    getCombineTransformationTreeEditor().selectePage(page);
                }
                else if (ctree.oid != null && ctree.oid.HasValue)
                {
                    this.Open(ctree.oid.Value);
                }
                else
                {
                    page = getCombineTransformationTreeEditor().addOrSelectPage(ctree);
                    initializePageHandlers(page);
                    page.Title = ctree.name;
                    getCombineTransformationTreeEditor().ListChangeHandler.AddNew(ctree);
                }
                CombinedTransformationTreeEditorItem pageOpen = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            }
        }

        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEditedNewName();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextLostFocus(object sender, RoutedEventArgs args)
        {
            ValidateEditedNewName();
        }

        protected void onGroupFieldChange()
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            string name = page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.groupField.textBox.Text;
            ((CombinedTransformationTreeSideBar)SideBar).CombineTransformationTreeGroup.combinedTransformationTreeTreeview.updateTarget(name, page.Title, true);
            OnChange();
        }

        protected void onScopePanelChange()
        {
            OnChange();
        }

        protected void onScopeItemDeleted(object item)
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            page.EditedObject.DeleteTreeItem((CombinedTransformationTreeItem)item);
        }
        

        #endregion

        public override bool validateName(EditorItem<Kernel.Domain.CombinedTransformationTree> page, string name)
        {
            if(!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        private bool IsRenameOnDoubleClick = false;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
            CombinedTransformationTreeEditorItem page = (CombinedTransformationTreeEditorItem)getCombineTransformationTreeEditor().getActivePage();
            Kernel.Domain.CombinedTransformationTree combinedTransformationTree = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
                newName = page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Combine Transformation tree name can't be mepty!");
                page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.SelectAll();
                page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.Focus();
                return OperationState.STOP;
            }

            bool found = false;
            if (GetCombineTransformationTreeService().getByName(newName) != null) found = true;

            foreach (CombinedTransformationTreeEditorItem unInputTable in getCombineTransformationTreeEditor().getPages())
            {
                if ((found && newName != getCombineTransformationTreeEditor().getActivePage().Title) || (unInputTable != getCombineTransformationTreeEditor().getActivePage() && newName == unInputTable.Title))
                {
                    DisplayError("Duplicate Name", "There is another Combine Transformation tree named: " + newName);
                    page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.Text = page.Title;
                    page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.SelectAll();
                    page.getCombineTransformationTreeForm().CombinedTransformationTreePropertiesPanel.nameTextBox.Focus();
                    return OperationState.STOP;
                }
                    }
            if(!IsRenameOnDoubleClick)
                if (combinedTransformationTree.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((CombinedTransformationTreeSideBar)SideBar).CombineTransformationTreeGroup.combinedTransformationTreeTreeview.updateTarget(newName, combinedTransformationTree.name, false);
            combinedTransformationTree.name = newName;
            page.Title = newName;
            OnChange();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Kernel.Domain.CombinedTransformationTree GetObjectByName(string name)
        {
            //return ((CombinedTransformationTreeSideBar)SideBar).CombineTransformationTreeGroup.combinedTransformationTreeTreeview.getTargetByName(name);
            return GetCombineTransformationTreeService().getByName(name);
        }


        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}
