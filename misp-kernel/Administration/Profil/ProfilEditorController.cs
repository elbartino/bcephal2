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

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilEditorController : EditorController<Domain.Profil, Misp.Kernel.Domain.Browser.BrowserData>
    {
        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
       

        #endregion

        public ProfilEditorController()
        {
            ModuleName = "Administration_Profil"; // PlugIn.MODULE_NAME;
        }
        

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public ProfilEditor getProfilEditor()
        {
            return (ProfilEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux reconciliation.
        /// </summary>
        /// <returns>UserService</returns>
        public ProfilService GetProfilService()
        {
            return (ProfilService)base.Service;
        }

        #endregion

        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle reco.
        /// </summary>
        /// <returns>CONTINUE si la création de la nouvelle reconciliation se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Domain.Profil profil = GetNewProfil();

            ((ProfilSideBar)SideBar).ProfilGroup.profilTreeview.AddProfil(profil);
            ProfilEditorItem page = (ProfilEditorItem)getProfilEditor().addOrSelectPage(profil);
            initializePageHandlers(page);
            page.Title = profil.name;

            getProfilEditor().ListChangeHandler.AddNew(profil);            
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.PROFIL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Domain.Profil profil)
        {
            ProfilEditorItem page = (ProfilEditorItem)getEditor().addOrSelectPage(profil);
            initializePageHandlers(page);
            page.getProfilForm().displayObject();
            getEditor().ListChangeHandler.AddNew(profil);
            return OperationState.CONTINUE;
        }

        /**
         * the new profil
         */
        protected virtual Domain.Profil GetNewProfil()
        {
            Domain.Profil profil = new Domain.Profil();
            profil.name = getNewPageName("Profil");
            profil.visibleInShortcut = true;
            profil.group = GetProfilService().GroupService.getDefaultGroup();
            return profil;
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
                Domain.Profil target = GetObjectByName(name);
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
        ProfilEditorItem currentPage = new ProfilEditorItem();

        public override OperationState Save(EditorItem<Domain.Profil> page)
        {
            try
            {
                currentPage = (ProfilEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Unable to save Profil", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        private Domain.Profil GetProfil(string name)
        {
            if (!IsNameUsed(name))
            {
                Domain.Profil pf = new Domain.Profil();
                pf.name = name;
                pf.group = GetProfilService().GroupService.getDefaultGroup();
                return pf;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Domain.Profil obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another Profil named: " + name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// handler on page selected
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Domain.Profil> page)
        {
            if (page == null)
            {
                return;
            }
            ProfilForm form = ((ProfilEditorItem)page).getProfilForm();
            if (form.profilPropertyPanel != null)
            {
                ((ProfilPropertyBar)this.PropertyBar).ProfilLayoutAnchorable.Content = form.profilPropertyPanel;
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
            if (base.Rename() != OperationState.CONTINUE)
            {
                return OperationState.STOP;
            }

            IsRenameOnDoubleClick = true;
            ProfilEditorItem page = (ProfilEditorItem)getProfilEditor().getActivePage();
            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            ProfilEditorItem page = (ProfilEditorItem)getProfilEditor().getActivePage();
            page.getProfilForm().profilPropertyPanel.nameTextBox.Text = name;
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
        protected override IView getNewView() { return new ProfilEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new ProfilToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new ProfilSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new ProfilPropertyBar(); }

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
        protected override void initializePageHandlers(EditorItem<Domain.Profil> page)
        {
            
            base.initializePageHandlers(page);
            ProfilEditorItem editorPage = (ProfilEditorItem)page;

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
            List<Domain.Profil> profils = GetProfilService().getAll();
            ((ProfilSideBar)SideBar).ProfilGroup.profilTreeview.fillTree(new ObservableCollection<Domain.Profil>(profils));
            
            BGroup group = GetProfilService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((ProfilSideBar)SideBar).ProfilGroup.profilTreeview.SelectionChanged += onSelectProfilFromSidebar;
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
        protected void onSelectProfilFromSidebar(object sender)
        {
            if (sender != null && sender is Domain.Profil)
            {
                Domain.Profil profil = (Domain.Profil)sender;
                EditorItem<Domain.Profil> page = getProfilEditor().getPage(profil.name);
                if (page != null)
                {
                    page.fillObject();
                    getProfilEditor().selectePage(page);

                }
                else if (profil.oid != null && profil.oid.HasValue)
                {

                    this.Open(profil.oid.Value);
                }
                else
                {
                    page = getProfilEditor().addOrSelectPage(profil);
                    initializePageHandlers(page);
                    page.Title = profil.name;

                    getProfilEditor().ListChangeHandler.AddNew(profil);
                }
                ProfilEditorItem pageOpen = (ProfilEditorItem)getProfilEditor().getActivePage();
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
            ProfilEditorItem page = (ProfilEditorItem)getProfilEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getProfilForm().profilPropertyPanel.nameTextBox.Text = page.Title;
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
            ProfilEditorItem page = (ProfilEditorItem)getProfilEditor().getActivePage();
            string name = page.getProfilForm().profilPropertyPanel.groupField.textBox.Text;
            BGroup group = page.getProfilForm().profilPropertyPanel.groupField.Group;
            ((ProfilSideBar)SideBar).ProfilGroup.profilTreeview.updateProfile(name, page.Title, true);
            Domain.Profil pf = page.EditedObject;
            pf.group = group;
            page.EditedObject = pf;
            page.getProfilForm().profilPropertyPanel.displayProfil(pf);            
            page.EditedObject.isModified = true;
            OnChange();
        }

        #endregion

        public override bool validateName(EditorItem<Domain.Profil> page, string name)
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
            ProfilEditorItem page = (ProfilEditorItem)getProfilEditor().getActivePage();
            Domain.Profil table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
            {
                newName = page.getProfilForm().profilPropertyPanel.nameTextBox.Text.Trim();
            }
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Profil name can't be mepty!");
                page.getProfilForm().profilPropertyPanel.nameTextBox.SelectAll();
                page.getProfilForm().profilPropertyPanel.nameTextBox.Focus();
                return OperationState.STOP;
            }


            foreach (ProfilEditorItem unReco in getProfilEditor().getPages())
            {
                if (unReco != getProfilEditor().getActivePage() && newName == unReco.Title)
                {
                    DisplayError("Duplicate Name", "There is another Target named: " + newName);
                    page.getProfilForm().profilPropertyPanel.nameTextBox.Text = page.Title;
                    page.getProfilForm().profilPropertyPanel.nameTextBox.SelectAll();
                    page.getProfilForm().profilPropertyPanel.nameTextBox.Focus();
                    return OperationState.STOP;
                }
            }
            if (!IsRenameOnDoubleClick)
                if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((ProfilSideBar)SideBar).ProfilGroup.profilTreeview.updateProfile(newName, table.name, false);
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
        protected override Domain.Profil GetObjectByName(string name)
        {
            return ((ProfilSideBar)SideBar).ProfilGroup.profilTreeview.getProfilByName(name);
        }

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}
