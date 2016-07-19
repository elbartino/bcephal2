using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Misp.Sourcing.CustomizedTarget
{
    public class TargetEditorController : EditorController<Target, Misp.Kernel.Domain.Browser.BrowserData>
    {


        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
        #endregion

        public TargetEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }
        

        #region Editor and Service
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public TargetEditor getTargetEditor()
        {
            return (TargetEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux Designs.
        /// </summary>
        /// <returns>DesignService</returns>
        public TargetService GetTargetService()
        {
            return (TargetService)base.Service;
        }

        #endregion


        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Target target = GetNewTarget();

            ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.AddTarget(target);
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().addOrSelectPage(target);
            initializePageHandlers(page);
            page.Title = target.name;

            getTargetEditor().ListChangeHandler.AddNew(target);
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.TARGET;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Target target)
        {
            EditorItem<Target> page = getEditor().addOrSelectPage(target);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(target);
            return OperationState.CONTINUE;
        }

        protected virtual Target GetNewTarget()
        {
            Target target = new Target(Target.Type.OBJECT_VC, Target.TargetType.CUSTOMIZED);
            target.name = getNewPageName("Target");
            target.group = GetTargetService().GroupService.getDefaultGroup();
            return target;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Target target = GetObjectByName(name);
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
        TargetEditorItem currentPage = new TargetEditorItem();

        public override OperationState Save(EditorItem<Target> page)
        {
            try
            {
                currentPage = (TargetEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;                
            }
            catch (Exception)
            {
                DisplayError("Unable to save Target", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        public OperationState Create(string name, Target targetInEdition)
        {
            Target target = null;// targetInEdition.getCopy(name);
            if (target == null) return OperationState.STOP;

            EditorItem<Target> page = getEditor().addOrSelectPage(target);

            ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.AddTarget(target);
            return Open(target);
        }

        private Target GetTarget(string name)
        {
            if (!IsNameUsed(name))
            {
                Target design = new Target();
                design.name = name;
                design.group = GetTargetService().GroupService.getDefaultGroup();
                return design;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Target obj = GetObjectByName(name);
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
        public override void OnPageSelected(EditorItem<Target> page)
        {
            if (page == null) return;
            TargetForm form = ((TargetEditorItem)page).getTargetForm();
            ((TargetPropertyBar)this.PropertyBar).TableLayoutAnchorable.Content = form.TargetPropertiesPanel;
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
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
            page.getTargetForm().TargetPropertiesPanel.nameTextBox.Text = name;
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
        protected override IView getNewView() { return new TargetEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new TargetToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new TargetSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new TargetPropertyBar(); }

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
        protected override void initializePageHandlers(EditorItem<Target> page)
        {
            base.initializePageHandlers(page);
            TargetEditorItem editorPage = (TargetEditorItem)page;
            editorPage.getTargetForm().TargetPropertiesPanel.groupField.GroupService = GetTargetService().GroupService;
            editorPage.getTargetForm().TargetPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getTargetForm().TargetPropertiesPanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.getTargetForm().TargetPropertiesPanel.nameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getTargetForm().TargetPropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getTargetForm().ScopePanel.Changed += onScopePanelChange;
            editorPage.getTargetForm().ScopePanel.ItemDeleted += onScopeItemDeleted;
       }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<Target> targets = Service.getAll();
            ((TargetSideBar)SideBar).EntityGroup.ModelService = GetTargetService().ModelService;
            ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.fillTree(new ObservableCollection<Target>(targets));

            List<Model> models = GetTargetService().ModelService.getModelsForSideBar();
            ((TargetSideBar)SideBar).EntityGroup.EntityTreeview.DisplayModels(models);

            BGroup group = GetTargetService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;            
            ((TargetSideBar)SideBar).EntityGroup.EntityTreeview.SelectionChanged += onSelectStandardTargetFromSidebar;
            ((TargetSideBar)SideBar).EntityGroup.EntityTreeview.ExpandAttribute += OnExpandAttribute;
            ((TargetSideBar)SideBar).StandardTargetGroup.TargetTreeview.SelectionChanged += onSelectStandardTargetFromSidebar;
        }

        private void OnExpandAttribute(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                if (!attribute.LoadValues)
                {
                    List<Kernel.Domain.AttributeValue> values = GetTargetService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
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
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                Target target = (Target)sender;
                EditorItem<Target> page = getTargetEditor().getPage(target.name);
                if (page != null)
                {
                    page.fillObject();
                    getTargetEditor().selectePage(page);
                    
                }
                else if (target.oid != null && target.oid.HasValue)
                {

                    this.Open(target.oid.Value);
                }
                else
                {
                    page = getTargetEditor().addOrSelectPage(target);
                    initializePageHandlers(page);
                    page.Title = target.name;

                    getTargetEditor().ListChangeHandler.AddNew(target);
                }
                TargetEditorItem pageOpen = (TargetEditorItem)getTargetEditor().getActivePage();
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
            if (sender != null && sender is Target)
            {
                Target value = (Target)sender;
                TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
                if (page == null) return;
                page.getTargetForm().ScopePanel.SetTargetItemValue(value);
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
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getTargetForm().TargetPropertiesPanel.nameTextBox.Text = page.Title;
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
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
            string name = page.getTargetForm().TargetPropertiesPanel.groupField.textBox.Text;
            ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.updateTarget(name, page.Title, true);
            OnChange();
        }

        protected void onScopePanelChange()
        {
            OnChange();
        }

        protected void onScopeItemDeleted(object item)
        {
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
            page.EditedObject.targetItemListChangeHandler.AddDeleted((TargetItem)item);
        }
        

        #endregion

        public override bool validateName(EditorItem<Target> page, string name)
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
            TargetEditorItem page = (TargetEditorItem)getTargetEditor().getActivePage();
            Target table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
                newName = page.getTargetForm().TargetPropertiesPanel.nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Target name can't be mepty!");
                page.getTargetForm().TargetPropertiesPanel.nameTextBox.SelectAll();
                page.getTargetForm().TargetPropertiesPanel.nameTextBox.Focus();
                return OperationState.STOP;
            }

            bool found = false;
            if (GetTargetService().getByName(newName) != null) found = true;

            foreach (TargetEditorItem unInputTable in getTargetEditor().getPages())
            {
                if ((found && newName != getTargetEditor().getActivePage().Title) || (unInputTable != getTargetEditor().getActivePage() && newName == unInputTable.Title))
                {
                    DisplayError("Duplicate Name", "There is another Target named: " + newName);
                    page.getTargetForm().TargetPropertiesPanel.nameTextBox.Text = page.Title;
                    page.getTargetForm().TargetPropertiesPanel.nameTextBox.SelectAll();
                    page.getTargetForm().TargetPropertiesPanel.nameTextBox.Focus();
                    return OperationState.STOP;
                }
                    }
            if(!IsRenameOnDoubleClick)
            if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.updateTarget(newName, table.name, false);
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
        protected override Target GetObjectByName(string name)
        {
            return ((TargetSideBar)SideBar).TargetGroup.TargetTreeview.getTargetByName(name);
        }


        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}
