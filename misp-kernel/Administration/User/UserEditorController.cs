using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Misp.Kernel.Administration.User
{
    public class UserEditorController : EditorController<Domain.User, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
       

        #endregion

        public UserEditorController()
        {
            //ModuleName = PlugIn.MODULE_NAME;
        }
        

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public UserEditor getUserEditor()
        {
            return (UserEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux reconciliation.
        /// </summary>
        /// <returns>UserService</returns>
        public SecurityService GetSecurytiService()
        {
            return (SecurityService)base.Service;
        }

        #endregion

        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle reco.
        /// </summary>
        /// <returns>CONTINUE si la création de la nouvelle reconciliation se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Domain.User user = GetNewUser();

            ((UserSideBar)SideBar).UserGroup.UserTreeview.AddUser(user);
            UserEditorItem page = (UserEditorItem)getUserEditor().addOrSelectPage(user);
            initializePageHandlers(page);
            page.Title = user.name;

            getUserEditor().ListChangeHandler.AddNew(user);
            //page.getReconciliationForm().reconciliationMainPanel.leftFilterGrid.filterForm.reset();
            //page.getReconciliationForm().reconciliationMainPanel.rigthFilterGrid.filterForm.reset();
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
        public override OperationState Open(Domain.User user)
        {
            UserEditorItem page = (UserEditorItem)getEditor().addOrSelectPage(user);
            initializePageHandlers(page);
            page.getReconciliationForm().displayObject();
            getEditor().ListChangeHandler.AddNew(user);
            return OperationState.CONTINUE;
        }

        /**
         * the new reconciliation
         */
        protected virtual Domain.User GetNewUser()
        {
            Domain.User user = new Domain.User();
            user.name = getNewPageName("User");
            //reco.visibleInShortcut = true;
            user.group = GetSecurytiService().GroupService.getDefaultGroup();
            return user;
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
                Domain.User target = GetObjectByName(name);
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
        UserEditorItem currentPage = new UserEditorItem();

        public override OperationState Save(EditorItem<Domain.User> page)
        {
            try
            {
                currentPage = (UserEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Unable to save Reconciliation", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        private Domain.User GetReconciliation(string name)
        {
            if (!IsNameUsed(name))
            {
                Domain.User reco = new Domain.User();
                reco.name = name;
                reco.group = GetSecurytiService().GroupService.getDefaultGroup();
                return reco;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Domain.User obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another reconciliation named: " + name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// handler on page selected
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Domain.User> page)
        {
            if (page == null)
            {
                return;
            }
            UserForm form = ((UserEditorItem)page).getReconciliationForm();
            //if (form.ReconciliationPropertiePanel != null)
            //{
            //    ((ReconciliationPropertyBar)this.PropertyBar).TableLayoutAnchorable.Content = form.ReconciliationPropertiePanel;
            //}
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
            {
                return OperationState.STOP;
            }

            IsRenameOnDoubleClick = true;
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
          //  page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text = name;
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
        protected override IView getNewView() { return new UserEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new UserToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new UserSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new UserPropertyBar(); }

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
        protected override void initializePageHandlers(EditorItem<Domain.User> page)
        {
            
            base.initializePageHandlers(page);
            UserEditorItem editorPage = (UserEditorItem)page;

        //    editorPage.getReconciliationForm().ReconciliationPropertiePanel.groupField.GroupService = GetReconciliationService().GroupService;
        //    editorPage.getReconciliationForm().ReconciliationPropertiePanel.groupField.subjectType = SubjectTypeFound();
        //    editorPage.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.KeyUp += onNameTextChange;
        //    editorPage.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.LostFocus += onNameTextLostFocus;
        //    editorPage.getReconciliationForm().ReconciliationPropertiePanel.groupField.Changed += onGroupFieldChange;

        //    editorPage.getReconciliationForm().reconciliationMainPanel.leftFilterGrid.filterForm.resetButton.Click += onResetClick;
        //    editorPage.getReconciliationForm().reconciliationMainPanel.rigthFilterGrid.filterForm.resetButton.Click += onResetClick;
        //    editorPage.getReconciliationForm().reconciliationMainPanel.rigthFilterGrid.filterForm.filterPTForm.periodFilter.Changed += onFilterPanelChange;
        //    editorPage.getReconciliationForm().reconciliationMainPanel.rigthFilterGrid.filterForm.filterPTForm.targetFilter.Changed += onFilterPanelChange;
        //    editorPage.getReconciliationForm().reconciliationMainPanel.leftFilterGrid.filterForm.filterPTForm.targetFilter.Changed += onFilterPanelChange;
        //    editorPage.getReconciliationForm().reconciliationMainPanel.leftFilterGrid.filterForm.filterPTForm.periodFilter.Changed += onFilterPanelChange;
        }

        
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<Domain.User> recos = GetSecurytiService().getAll();
            ((UserSideBar)SideBar).UserGroup.UserTreeview.fillTree(new ObservableCollection<Domain.User>(recos));

            List<Model> models = GetSecurytiService().ModelService.getModelsForSideBar();
            ((UserSideBar)SideBar).EntityGroup.EntityTreeview.DisplayModels(models);

            //rootPeriodName = GetReconciliationService().periodNameService.getRootPeriodName();
            //defaultPeriodName = rootPeriodName.getDefaultPeriodName();
            //((ReconciliationSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);


            BGroup group = GetSecurytiService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((UserSideBar)SideBar).UserGroup.UserTreeview.SelectionChanged += onSelectReconciliationFromSidebar;
            ((UserSideBar)SideBar).EntityGroup.EntityTreeview.SelectionChanged += onSelectStandardTargetFromSidebar;
            ((UserSideBar)SideBar).EntityGroup.EntityTreeview.ExpandAttribute += OnExpandAttribute;
            ((UserSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodNameFromSidebar;
            ((UserSideBar)SideBar).StandardTargetGroup.TargetTreeview.SelectionChanged += onSelectStandardTargetFromSidebar;
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
            if (sender != null && sender is Domain.User)
            {
                Domain.User user = (Domain.User)sender;
                EditorItem<Domain.User> page = getUserEditor().getPage(user.name);
                if (page != null)
                {
                    page.fillObject();
                    getUserEditor().selectePage(page);

                }
                else if (user.oid != null && user.oid.HasValue)
                {

                    this.Open(user.oid.Value);
                }
                else
                {
                    page = getUserEditor().addOrSelectPage(user);
                    initializePageHandlers(page);
                    page.Title = user.name;

                    getUserEditor().ListChangeHandler.AddNew(user);
                }
                UserEditorItem pageOpen = (UserEditorItem)getUserEditor().getActivePage();
            }
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectStandardTargetFromSidebar(object sender)
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (page == null) return;
        //    page.getReconciliationForm().reconciliationMainPanel.activeFilterGrid.onSelectTargetFromSidebar(sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnExpandAttribute(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                if (!attribute.LoadValues)
                {
                    List<Kernel.Domain.AttributeValue> values = GetSecurytiService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    attribute.valueListChangeHandler.Items.Clear();
                    foreach (Kernel.Domain.AttributeValue value in values)
                    {
                        attribute.valueListChangeHandler.Items.Add(value);
                    }
                    attribute.LoadValues = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void onSelectPeriodNameFromSidebar(object sender)
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
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
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
               // page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text = page.Title;
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
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            String name ="";
            //string name = page.getReconciliationForm().ReconciliationPropertiePanel.groupField.textBox.Text;
            //BGroup group = page.getReconciliationForm().ReconciliationPropertiePanel.groupField.Group;
            ((UserSideBar)SideBar).UserGroup.UserTreeview.updateUser(name, page.Title, true);
            Domain.User rTemp = page.EditedObject;
            //rTemp.group = group;
            page.EditedObject = rTemp;
            //page.getReconciliationForm().ReconciliationPropertiePanel.displayReconciliation(rTemp);            
            page.EditedObject.isModified = true;
            OnChange();
        }

        #endregion

        public override bool validateName(EditorItem<Domain.User> page, string name)
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
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            Domain.User table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
            {
             //   newName = page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text.Trim();
            }
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Reconciliation name can't be mepty!");
                //page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.SelectAll();
                //page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Focus();
                return OperationState.STOP;
            }


            foreach (UserEditorItem unReco in getUserEditor().getPages())
            {
                if (unReco != getUserEditor().getActivePage() && newName == unReco.Title)
                {
                    DisplayError("Duplicate Name", "There is another Target named: " + newName);
                    //page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Text = page.Title;
                    //page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.SelectAll();
                    //page.getReconciliationForm().ReconciliationPropertiePanel.nameTextBox.Focus();
                    return OperationState.STOP;
                }
            }
            if (!IsRenameOnDoubleClick)
                if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((UserSideBar)SideBar).UserGroup.UserTreeview.updateUser(newName, table.name, false);
            table.name = newName;
            page.Title = newName;
            OnChange();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Domain.User GetObjectByName(string name)
        {
            return ((UserSideBar)SideBar).UserGroup.UserTreeview.getUserByName(name);
        }

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}
