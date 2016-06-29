using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Role
{
    public class RoleEditorController :  EditorController<Domain.Role, Misp.Kernel.Domain.Browser.BrowserData>
    {

        public RoleEditorController() 
        {
            ModuleName = "Administration_Role";//PlugIn.MODULE_NAME; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public RoleEditor getRoleEditor()
        {
            return (RoleEditor)this.View;
        }

        /// <summary>
        /// retourne le service associé au calculated measure.
        /// </summary>
        /// <returns>CalculatedMeasureService</returns>
        public Service.RoleService GetRoleService()
        {
            return (Service.RoleService)base.Service;
        }

        public override Application.OperationState Delete()
        {
            return Application.OperationState.CONTINUE;
        }

        public override Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.ROLE;
        }

        protected override void initializePageHandlers(EditorItem<Domain.Role> page)
        {
            base.initializePageHandlers(page);
            RoleEditorItem editorPage = (RoleEditorItem)page;
            editorPage.getRoleForm().RolePanel.Changed += onRolePanelChange;
            editorPage.getRoleForm().RolePanel.ItemChanged += onRolePanelItemChange;
            editorPage.getRoleForm().RolePanel.ItemDeleted += onRolePanelItemDeleted;
        }

        private void onRolePanelItemDeleted(object item)
        {
            if (!(item is Domain.Role)) return;
            RoleEditorItem page = (RoleEditorItem)getRoleEditor().getActivePage();
            if (page == null) return;
            page.EditedObject.RemoveChild((Domain.Role)item);
            OnChange();
        }

        private void onRolePanelItemChange(object item)
        {
            if (!(item is Domain.Role)) return;
            RoleEditorItem page = (RoleEditorItem)getRoleEditor().getActivePage();
            if (page == null) return;
            page.EditedObject.UpdateChild((Domain.Role)item);
            OnChange();
        }

        private void onRolePanelChange()
        {
            OnChange();    
        }

        public override Application.OperationState Create()
        {
            Domain.Role root = GetRoleService().getRootRole();

            RoleEditorItem page = (RoleEditorItem)getRoleEditor().addOrSelectPage(root);
            page.Title = "Roles";
            initializePageHandlers(page);
            getRoleEditor().ListChangeHandler.AddNew(root);
            return OperationState.CONTINUE;
        }

        protected override Ui.Base.IView getNewView()
        {
            return new RoleEditor();
        }


        protected override Ui.Base.ToolBar getNewToolBar()
        {
            return new RoleToolBar();
        }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return new ToolBarHandlerBuilder(this);
        }

        protected override Ui.Base.SideBar getNewSideBar()
        {
            return new RoleSideBar();
        }

        protected override Ui.Base.PropertyBar getNewPropertyBar()
        {
            return new RolePropertyBar();
        }

        protected override void initializeSideBarData()
        {
            
        }

        protected override void initializeSideBarHandlers()
        {
            
        }
      
    }
}
