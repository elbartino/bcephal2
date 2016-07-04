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
    public class UserEditorController : EditorController<Domain.User, Misp.Kernel.Domain.Browser.UserBrowserData>
    {
        #region Properties
        
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
       
        #endregion

        public UserEditorController()
        {
            ModuleName = "Administration";//PlugIn.MODULE_NAME;
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
        public UserService GetUserService()
        {
            return (UserService)base.Service;
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
            page.getUserForm().UserService = GetUserService();
            page.Title = user.name;
            page.getUserForm().displayObject();
            getUserEditor().ListChangeHandler.AddNew(user);
            page.getUserForm().userMainPanel.InitProfilComboBox(GetUserService().ProfilService);
            page.getUserForm().userMainPanel.InitRelationPanel(GetUserService());
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.USER;
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
            page.getUserForm().displayObject();
            getEditor().ListChangeHandler.AddNew(user);
            page.getUserForm().userMainPanel.InitProfilComboBox(GetUserService().ProfilService);
            page.getUserForm().userMainPanel.InitRelationPanel(GetUserService());
            return OperationState.CONTINUE;
        }

        /**
         * the new reconciliation
         */
        protected virtual Domain.User GetNewUser()
        {
            Domain.User user = new Domain.User();
            user.name = getNewPageName("User");
            user.visibleInShortcut = true;
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
                DisplayError("Save User", "Unable to save USer.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        private Domain.User GetUser(string name)
        {
            if (!IsNameUsed(name))
            {
                Domain.User us = new Domain.User();
                us.name = name;
                return us;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Domain.User obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another User named: " + name);
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
            UserForm form = ((UserEditorItem)page).getUserForm();
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

        protected override PropertyBar getNewPropertyBar() { return null; }

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

            editorPage.getUserForm().userMainPanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.getUserForm().userMainPanel.nameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getUserForm().userMainPanel.loginTextBox.KeyUp += onLoginTextChange;
            editorPage.getUserForm().userMainPanel.loginTextBox.LostFocus += onLoginTextLostFocus;
            editorPage.getUserForm().userMainPanel.RelationPanel.Changed += OnRelationChange;
            editorPage.getUserForm().userMainPanel.profilcomboBox.SelectionChanged += onProfilFieldChange;
            editorPage.getUserForm().userMainPanel.RelationPanel.ItemChanged += OnRelationItemChange;
            editorPage.getUserForm().userMainPanel.RelationPanel.ItemDeleted += OnRelationItemDelete;
            editorPage.getUserForm().userMainPanel.nameTextBox.LostFocus += onUserNameTextLostFocus;
            editorPage.getUserForm().userMainPanel.nameTextBox.KeyUp += onUserNameTextChange;
        }
             

        private void OnRelationChange()
        {
            OnChange();
        }

        private void OnRelationItemChange(object item)
        {
            if (!(item is Domain.Relation)) return;
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (page == null) return;
            //Domain.Relation relation = (Domain.Relation)item;
            //if (relation.oid.HasValue)
            //{
            //    page.EditedObject.relationsListChangeHandler.AddUpdated((Domain.Relation)item);
            //}
            //else
            //{
            //    page.EditedObject.relationsListChangeHandler.AddNew((Domain.Relation)item);
            //}
            OnChange();
        }

        private void OnRelationItemDelete(object item)
        {
            if (!(item is Domain.Relation)) return;
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (page == null) return;
            page.EditedObject.relationsListChangeHandler.AddDeleted((Domain.Relation)item);
            OnChange();
        }

        

        
        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<Domain.User> users = GetUserService().getAll();
            ((UserSideBar)SideBar).UserGroup.UserTreeview.fillTree(new ObservableCollection<Domain.User>(users));
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((UserSideBar)SideBar).UserGroup.UserTreeview.SelectionChanged += onSelectUserFromSidebar;
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
        protected void onSelectUserFromSidebar(object sender)
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
                page.getUserForm().userMainPanel.nameTextBox.Text = page.Title;
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
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onUserNameTextLostFocus(object sender, RoutedEventArgs args)
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            string newName = page.getUserForm().userMainPanel.nameTextBox.Text;
            Rename(newName);
        }

        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onUserNameTextChange(object sender, KeyEventArgs args)
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                string newName = page.getUserForm().userMainPanel.nameTextBox.Text;
                Rename(newName);
            }
            else if (args.Key == Key.Enter)
            {
                string newName = page.getUserForm().userMainPanel.nameTextBox.Text;
                Rename(newName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onLoginTextLostFocus(object sender, RoutedEventArgs args)
        {
            ValidateEditedLoginName();
        }

        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onLoginTextChange(object sender, KeyEventArgs args)
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getUserForm().userMainPanel.loginTextBox.Text = "";
            }
            else if (args.Key == Key.Tab)
            {
                page.getUserForm().userMainPanel.loginTextBox.Text = page.getUserForm().userMainPanel.loginTextBox.Text.ToString().Trim();
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEditedNewName();
            }
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


        private void onProfilFieldChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {            
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            if (page != null)
            {
                string name = page.getUserForm().userMainPanel.profilcomboBox.SelectedItem.ToString();
                Domain.Profil profil = (Domain.Profil)page.getUserForm().userMainPanel.profilcomboBox.SelectedItem;
                ((UserSideBar)SideBar).UserGroup.UserTreeview.updateUser(name, page.Title, true);
                Domain.User usTemp = page.EditedObject;
                usTemp.profil = profil;
                page.EditedObject = usTemp;
                page.EditedObject.isModified = true;
                OnChange();
            }
        }

        #endregion

        public override bool validateName(EditorItem<Domain.User> page, string name)
        {
            if (!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        public bool validateLogin(EditorItem<Domain.User> page, string name)
        {
            if (!base.validateName(page, name)) return false;
            return ValidateEditedLoginName() == OperationState.CONTINUE;
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
                newName = page.getUserForm().userMainPanel.nameTextBox.Text.Trim();
            }
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The User name can't be mepty!");
                page.getUserForm().userMainPanel.nameTextBox.SelectAll();
                page.getUserForm().userMainPanel.nameTextBox.Focus();
                return OperationState.STOP;
            }

            bool found = false;
            if (GetUserService().getByName(newName) != null) found = true;

            foreach (UserEditorItem unReco in getUserEditor().getPages())
            {
                if ((found && newName != getUserEditor().getActivePage().Title) || (unReco != getUserEditor().getActivePage() && newName == unReco.Title))
                {
                    DisplayError("Duplicate Name", "There is another Target named: " + newName);
                    page.getUserForm().userMainPanel.nameTextBox.Text = page.Title;
                    page.getUserForm().userMainPanel.nameTextBox.SelectAll();
                    page.getUserForm().userMainPanel.nameTextBox.Focus();
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
        /// <returns></returns>
        protected virtual OperationState ValidateEditedLoginName(string loginName = "")
        {
            UserEditorItem page = (UserEditorItem)getUserEditor().getActivePage();
            Domain.User table = page.EditedObject;
            if (string.IsNullOrEmpty(loginName))
            {
                loginName = page.getUserForm().userMainPanel.loginTextBox.Text.Trim();
            }
            if (string.IsNullOrEmpty(loginName))
            {
                DisplayError("Empty Login", "The User login can't be empty!");
                page.getUserForm().userMainPanel.loginTextBox.SelectAll();
                page.getUserForm().userMainPanel.loginTextBox.Focus();
                return OperationState.STOP;
            }
            bool found = false;
            if (GetUserService().getUserByLogin(loginName) != null) found = true;

            foreach (UserEditorItem unReco in getUserEditor().getPages())
            {

                if ((found && loginName != getUserEditor().getActivePage().EditedObject.login) || (unReco != getUserEditor().getActivePage() && loginName == unReco.EditedObject.login))
                {
                    DisplayError("Duplicate Login", "There is another Target named: " + loginName);
                    page.getUserForm().userMainPanel.loginTextBox.Text = getUserEditor().getActivePage().EditedObject.login != null ? getUserEditor().getActivePage().EditedObject.login : getUserEditor().getActivePage().Title;
                    page.getUserForm().userMainPanel.loginTextBox.SelectAll();
                    page.getUserForm().userMainPanel.loginTextBox.Focus();
                    return OperationState.STOP;
                }
            }
            
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
