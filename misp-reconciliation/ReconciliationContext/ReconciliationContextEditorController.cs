using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContextEditorController : EditorController<Kernel.Domain.ReconciliationContext, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
       
        private PeriodName rootPeriodName { get; set; }

        private PeriodName defaultPeriodName { get; set; }
        #endregion

        public ReconciliationContextEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }
        

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public ReconciliationContextEditor getReconciliationContextEditor()
        {
            return (ReconciliationContextEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux reconciliation.
        /// </summary>
        /// <returns>ReconciliationService</returns>
        public ReconciliationContextService GetReconciliationContextService()
        {
            return (ReconciliationContextService)base.Service;
        }

        #endregion

        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle reco.
        /// </summary>
        /// <returns>CONTINUE si la création de la nouvelle reconciliation se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            //Kernel.Domain.ReconciliationContext reco = GetNewReconciliationContext();

            //((ReconciliationSideBar)SideBar).RecoGroup.ReconciliationTreeview.AddReconciliation(reco);
            //ReconciliationEditorItem page = (ReconciliationEditorItem)getReconciliationContextEditor().addOrSelectPage(reco);
            //initializePageHandlers(page);
            //page.Title = reco.name;

            //getReconciliationContextEditor().ListChangeHandler.AddNew(reco);
            //page.getReconciliationForm().reconciliationMainPanel.leftFilterGrid.filterForm.reset();
            //page.getReconciliationForm().reconciliationMainPanel.rigthFilterGrid.filterForm.reset();
            //Open(reco);
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.RECONCILIATION;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Kernel.Domain.ReconciliationContext reco)
        {
            ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getEditor().addOrSelectPage(reco);
            initializePageHandlers(page);
            page.getReconciliationContextForm().displayObject();
            getEditor().ListChangeHandler.AddNew(reco);
            return OperationState.CONTINUE;
        }

        /**
         * the new reconciliation
         */
        protected virtual ReconciliationTemplate GetNewReconciliationTemplate()
        {
            ReconciliationTemplate reco = new ReconciliationTemplate();
            reco.name = getNewPageName("Reconciliation Context");
            reco.visibleInShortcut = true;
            reco.group = GetReconciliationContextService().GroupService.getDefaultGroup();
            return reco;
        }

        /**
         * get a new page name
         */
        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Kernel.Domain.ReconciliationContext target = GetObjectByName(name);
                if (target == null) return name;
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
        ReconciliationContextEditorItem currentPage = new ReconciliationContextEditorItem();

        public override OperationState Save(EditorItem<Kernel.Domain.ReconciliationContext> page)
        {
            try
            {
                currentPage = (ReconciliationContextEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Save Reconciliation", "Unable to save Reconciliation.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        private Kernel.Domain.ReconciliationContext GetReconciliation(string name)
        {
            //if (!IsNameUsed(name))
            //{
            //    ReconciliationTemplate reco = new ReconciliationTemplate();
            //    reco.name = name;
            //    reco.group = GetReconciliationService().GroupService.getDefaultGroup();
            //    return reco;
            //}
            return null;
        }


        private bool IsNameUsed(string name)
        {
            //Kernel.Domain.ReconciliationContext obj = GetObjectByName(name);
            //if (obj != null)
            //{
            //    DisplayError("Duplicate Name", "There is another reconciliation named: " + name);
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// handler on page selected
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Kernel.Domain.ReconciliationContext> page)
        {
            if (page == null)
            {
                return;
            }
            ReconciliationContexForm form = ((ReconciliationContextEditorItem)page).getReconciliationContextForm();
            if (form.ReconciliationContextPanel != null)
            {
                //((ReconciliationPropertyBar)this.PropertyBar).ReconciliationLayoutAnchorable.Content = form.ReconciliationPropertiePanel;
            }
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
            //if (base.Rename() != OperationState.CONTINUE)
            //{
            //    return OperationState.STOP;
            //}

            //IsRenameOnDoubleClick = true;
            //ReconciliationEditorItem page = (ReconciliationEditorItem)getReconciliationEditor().getActivePage();
            //return ValidateEditedNewName(page.EditedObject.name);
            return OperationState.CONTINUE;

        }

        protected override void Rename(string name)
        {
            //ReconciliationEditorItem page = (ReconciliationEditorItem)getReconciliationEditor().getActivePage();
            //page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text = name;
            //page.EditedObject.name = name;
            //base.Rename(name);
            //return OperationState.CONTINUE;
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

         #endregion


        #region Others

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new  ReconciliationContextEditor();  }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new ReconciliationContextToolBar();
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return  new ReconciliationContextSideBar();}

        protected override PropertyBar getNewPropertyBar() { return new ReconciliationContextPropertyBar();}

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

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
        protected override void initializePageHandlers(EditorItem<Kernel.Domain.ReconciliationContext> page)
        {
            
            base.initializePageHandlers(page);
            ReconciliationContextEditorItem editorPage = (ReconciliationContextEditorItem)page;

            //editorPage.getReconciliationContextForm().ReconciliationPropertiePanel.groupField.GroupService = GetReconciliationService().GroupService;
            //editorPage.getReconciliationContextForm().ReconciliationPropertiePanel.groupField.subjectType = SubjectTypeFound();
            //editorPage.getReconciliationContextForm().ReconciliationPropertiePanel.nameTextBox.KeyUp += onNameTextChange;
            //editorPage.getReconciliationContextForm().ReconciliationPropertiePanel.nameTextBox.LostFocus += onNameTextLostFocus;
            //editorPage.getReconciliationContextForm().ReconciliationPropertiePanel.groupField.Changed += onGroupFieldChange;

            //editorPage.getReconciliationContextForm().ReconciliationContextPanel.leftFilterGrid.filterForm.resetButton.Click += onResetClick;
            //editorPage.getReconciliationContextForm().ReconciliationContextPanel.rigthFilterGrid.filterForm.resetButton.Click += onResetClick;
            //editorPage.getReconciliationContextForm().ReconciliationContextPanel.rigthFilterGrid.filterForm.filterPTForm.periodFilter.Changed += onFilterPanelChange;
            //editorPage.getReconciliationContextForm().ReconciliationContextPanel.rigthFilterGrid.filterForm.filterPTForm.targetFilter.Changed += onFilterPanelChange;
            //editorPage.getReconciliationContextForm().ReconciliationContextPanel.leftFilterGrid.filterForm.filterPTForm.targetFilter.Changed += onFilterPanelChange;
            //editorPage.getReconciliationContextForm().ReconciliationContextPanel.leftFilterGrid.filterForm.filterPTForm.periodFilter.Changed += onFilterPanelChange;
        
        }

        
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            //List<ReconciliationTemplate> recos = GetReconciliationService().getAll();
            //((ReconciliationSideBar)SideBar).RecoGroup.ReconciliationTreeview.fillTree(new ObservableCollection<ReconciliationTemplate>(recos));

            ((ReconciliationContextSideBar)SideBar).EntityGroup.ModelService = GetReconciliationContextService().ModelService;
            ((ReconciliationContextSideBar)SideBar).EntityGroup.InitializeTreeViewDatas();
          
            //rootPeriodName = GetReconciliationService().periodNameService.getRootPeriodName();
            //defaultPeriodName = rootPeriodName.getDefaultPeriodName();
            //((ReconciliationContextSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);


            //BGroup group = GetReconciliationService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
           // ((ReconciliationContextSideBar)SideBar).RecoGroup.ReconciliationTreeview.SelectionChanged += onSelectReconciliationFromSidebar;
            ((ReconciliationContextSideBar)SideBar).EntityGroup.OnSelectAttributeValue += onSelectStandardTargetFromSidebar;
            ((ReconciliationContextSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodNameFromSidebar;
            ((ReconciliationContextSideBar)SideBar).StandardTargetGroup.TargetTreeview.SelectionChanged += onSelectStandardTargetFromSidebar;
        }

        /// <summary>
        /// Initialisation des Handlers sur la ToolBar.
        /// </summary>
        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
        }
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectReconciliationFromSidebar(object sender)
        {
            //if (sender != null && sender is ReconciliationTemplate)
            //{
            //    ReconciliationTemplate reco = (ReconciliationTemplate )sender;
            //    EditorItem<ReconciliationTemplate> page = getReconciliationEditor().getPage(reco.name);
            //    if (page != null)
            //    {
            //        page.fillObject();
            //        getReconciliationEditor().selectePage(page);

            //    }
            //    else if (reco.oid != null && reco.oid.HasValue)
            //    {

            //        this.Open(reco.oid.Value);
            //    }
            //    else
            //    {
            //        page = getReconciliationEditor().addOrSelectPage(reco);
            //        initializePageHandlers(page);
            //        page.Title = reco.name;

            //        getReconciliationEditor().ListChangeHandler.AddNew(reco);
            //    }
            //    ReconciliationContextEditorItem pageOpen = (ReconciliationContextEditorItem)getReconciliationEditor().getActivePage();
            //}
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectStandardTargetFromSidebar(object sender)
        {
            //ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationEditor().getActivePage();
            //if (page == null) return;
           // page.getReconciliationForm().reconciliationMainPanel.activeFilterGrid.onSelectTargetFromSidebar(sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnExpandAttribute(object sender)
        {
            //if (sender != null && sender is Kernel.Domain.Attribute)
            //{
            //    Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
            //    if (!attribute.LoadValues)
            //    {
            //        List<Kernel.Domain.AttributeValue> values = GetReconciliationService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
            //        attribute.valueListChangeHandler.Items.Clear();
            //        foreach (Kernel.Domain.AttributeValue value in values)
            //        {
            //            attribute.valueListChangeHandler.Items.Add(value);
            //        }
            //        attribute.LoadValues = true;
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void onSelectPeriodNameFromSidebar(object sender)
        {
            //ReconciliationEditorItem page = (ReconciliationEditorItem)getReconciliationEditor().getActivePage();
            //PostingBrowserForm activeBrowserForm = page.getReconciliationForm().reconciliationMainPanel.activeFilterGrid;
            //activeBrowserForm.onSelectPeriodNameFromSidebar(sender);
        }

        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            //ReconciliationEditorItem page = (ReconciliationEditorItem)getReconciliationEditor().getActivePage();
            //if (args.Key == Key.Escape)
            //{
            //    page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text = page.Title;
            //}
            //else if (args.Key == Key.Enter)
            //{
            //    ValidateEditedNewName();
            //}
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

        /// <summary>
        /// 
        /// </summary>
        private void onFilterPanelChange()
        {
            OnChange();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onResetClick(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        protected void onGroupFieldChange()
        {
            //ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationEditor().getActivePage();
            //string name = page.getReconciliationForm().ReconciliationPropertiePanel.groupField.textBox.Text;
            //BGroup group = page.getReconciliationForm().ReconciliationPropertiePanel.groupField.Group;
            ////((ReconciliationContextSideBar)SideBar).RecoGroup.ReconciliationTreeview.updateReconciliation(name, page.Title, true);
            //ReconciliationTemplate rTemp = page.EditedObject;
            //rTemp.group = group;
            //page.EditedObject = rTemp;
            //page.getReconciliationForm().ReconciliationPropertiePanel.displayReconciliation(rTemp);            
            //page.EditedObject.isModified = true;
            OnChange();
        }

        #endregion

        public override bool validateName(EditorItem<Kernel.Domain.ReconciliationContext> page, string name)
        {
            if (!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        private bool IsRenameOnDoubleClick = false;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
//            ReconciliationContextEditorItem page = (ReconciliationContextEditorItem)getReconciliationEditor().getActivePage();
//            Kernel.Domain.ReconciliationContext table = page.EditedObject;
//            if (string.IsNullOrEmpty(newName)) { }
////                newName = page.getReconciliationContextForm().ReconciliationPropertiePanel.nameTextBox.Text.Trim();
//            if (string.IsNullOrEmpty(newName))
//            {
//                DisplayError("Empty Name", "The Reconciliation name can't be mepty!");
//                //page.getReconciliationContextForm().ReconciliationPropertiePanel.nameTextBox.SelectAll();
//                //page.getReconciliationContextForm().ReconciliationPropertiePanel.nameTextBox.Focus();
//                return OperationState.STOP;
//            }

            bool found = false;
            //if (GetReconciliationService().getByName(newName) != null) found = true;

            //foreach (ReconciliationEditorItem unReco in getReconciliationEditor().getPages())
            //{
            //    if ((found && newName != getReconciliationEditor().getActivePage().Title) || (unReco != getReconciliationEditor().getActivePage() && newName == unReco.Title))
            //    {
            //        DisplayError("Duplicate Name", "There is another Target named: " + newName);
            //        page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text = page.Title;
            //        page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.SelectAll();
            //        page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Focus();
            //        return OperationState.STOP;
            //    }
            //}
            //if (!IsRenameOnDoubleClick)
            //    if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ////((ReconciliationContextSideBar)SideBar).RecoGroup.ReconciliationTreeview.updateReconciliation(newName, table.name, false);
            //table.name = newName;
            //page.Title = newName;
            OnChange();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Kernel.Domain.ReconciliationContext GetObjectByName(string name)
        {
            return new Kernel.Domain.ReconciliationContext();
            //return ((ReconciliationContextSideBar)SideBar).RecoGroup.ReconciliationTreeview.getReconciliationByName(name);
        }

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }

}
