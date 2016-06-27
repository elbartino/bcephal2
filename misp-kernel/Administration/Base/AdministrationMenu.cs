using Misp.Kernel.Ui.Base.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.Base
{
    public class AdministrationMenu : ApplicationMenu
    {
        public ApplicationMenu UserMenu { get; private set; }
        public ApplicationMenu ProfilMenu { get; private set; }
        public ApplicationMenu ProfilListMenu { get; private set; }
        public ApplicationMenu UserListMenu { get; private set; }
        public ApplicationMenu RoleMenu { get; private set; }



        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(UserMenu);
            menus.Add(UserListMenu);
            menus.Add(new Separator());
            menus.Add(ProfilMenu);
            menus.Add(ProfilListMenu);
            menus.Add(new Separator());
            menus.Add(RoleMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.ADMINISTRATION_MENU_CODE;
            this.Header = "Administration";
            
            UserMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "New User", Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER));
            UserListMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "List User", Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_USER));
            
            ProfilMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "New Profil", Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL));
            ProfilListMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "List Profil", Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_PROFIL));

            RoleMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "Manage Role", Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_ROLE));

        }
    }
}
