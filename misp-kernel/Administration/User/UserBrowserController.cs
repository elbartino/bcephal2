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

namespace Misp.Kernel.Administration.User
{
    public class UserBrowserController : BrowserController<Misp.Kernel.Domain.User, BrowserData>
    {
        public UserBrowserController() 
        {
            ModuleName = "Administration_User"; //PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() 
        { 
            return AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_USER; 
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() 
        {
            return new UserBrowser(); 
        }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.USER;
        }

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public UserService getUserService()
        {
            return (UserService)Service;
        }

        protected override SideBar getNewSideBar() { return new UserBrowserSideBar(); }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            if (this.SideBar != null)
            {
                ((UserBrowserSideBar)this.SideBar).ProfilGroup.profilTreeview.SelectionChanged += OnProfilSelected;
            }
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            if (this.SideBar != null && this.Service != null)
            {
                //Kernel.Domain.BGroup rootGroup = getUserService().GroupService.getRootGroup(SubjectTypeFound());
                //((UserBrowserSideBar)SideBar).ProfilGroup.profilTreeview.DisplayRoot(rootGroup);
            }
        }

        private void OnProfilSelected(object newSelection)
        {
            if (newSelection == null) return;
            Kernel.Domain.Profil profil = (Kernel.Domain.Profil)newSelection;
            if (profil.oid == null || !profil.oid.HasValue) Search();
            else FilterByGroup(profil.oid.Value);
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
