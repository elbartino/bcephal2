using Misp.Kernel.Administration.Base;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Role
{
    public class RoleBrowserController : BrowserController<Misp.Kernel.Domain.Role, BrowserData>
    {
        public RoleBrowserController() 
        {
            ModuleName = "Administration_Role";//PlugIn.MODULE_NAME; 
        }
       

        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public override OperationState Search()
        {
            try
            {
                Kernel.Domain.Role root = getRoleService().getRootRole();
                GetRoleBrowser().form.EditedObject = root;
                GetRoleBrowser().form.ChangeEventHandler = this.ChangeEventHandler;
                GetRoleBrowser().form.displayObject();
                return OperationState.CONTINUE;
            }
            catch (ServiceExecption e)
            {
                DisplayError("error", e.Message);
            }

            return OperationState.STOP;
        }

        

        public RoleBrowser GetRoleBrowser()
        {
            return (RoleBrowser)this.View;
        }

        public RoleService getRoleService()
        {
            return (RoleService)Service;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.ROLE;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality()
        {
            return AdministrationFunctionalitiesCode.ADMINISTRATION_ROLE;
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() 
        {
            return new RoleBrowser(); 
        }

        public override OperationState Save()
        {
            try
            {
                GetRoleBrowser().form.fillObject();
                Kernel.Domain.Role root = GetRoleBrowser().form.EditedObject;
                root = getRoleService().saveRole(root);
                GetRoleBrowser().form.EditedObject = root;
                GetRoleBrowser().form.displayObject();
                //if (base.Save() == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception)
            {
                DisplayError("Save Role", "Unable to save Role.");
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() 
        {
            GetRoleBrowser().form.RoleService = getRoleService();
        }

        

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }


        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar()
        {
            return new RoleSideBar();
        }

        protected override Misp.Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            BrowserToolBar bar = (BrowserToolBar)base.getNewToolBar();
            bar.NewButton.Visibility = System.Windows.Visibility.Hidden;
            bar.SaveButton.Visibility = System.Windows.Visibility.Visible;
            return bar;
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

        

        /// <summary>
        /// Edit property
        /// </summary>
        /// <param name="item"></param>
        /// <param name="header"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override OperationState EditProperty(BrowserData item, String header, Object value)
        {
            if (item == null || String.IsNullOrWhiteSpace(header) || value == null) return OperationState.STOP;
            if (header.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                return RenameItem(item, (String)value);
            }
            if (header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase))
            {
                BrowserData data = new BrowserData(item);
                data.visibleInShortcut = (bool)value;
                try
                {
                    item = Service.SaveBrowserData(data);
                    if (item == null) return OperationState.STOP;
                    item.visibleInShortcut = (bool)value;
                }
                catch (Misp.Kernel.Domain.BcephalException)
                {
                    DisplayError("Unable edit item", "Unable edit : " + item.name);
                    return OperationState.STOP;
                }
            }

            return OperationState.CONTINUE;
        }


    }
}
