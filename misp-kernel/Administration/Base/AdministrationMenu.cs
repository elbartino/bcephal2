using Misp.Kernel.Application;
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
            this.Header = FunctionalitiesLabel.ADMINISTRATION_LABEL;
            
            UserMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE,FunctionalitiesLabel.ADMINISTRATION_NEW_USER_LABEL, Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_USER_EDIT));
            UserListMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE,FunctionalitiesLabel.ADMINISTRATION_LIST_USER_LABEL, Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_USER_LIST));
            
            ProfilMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE,FunctionalitiesLabel.ADMINISTRATION_NEW_PROFIL_LABEL, Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_EDIT));
            ProfilListMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE,FunctionalitiesLabel.ADMINISTRATION_LIST_PROFIL_LABEL, Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_LIST));

            RoleMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, FunctionalitiesLabel.ADMINISTRATION_ROLE_LABEL, Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_ROLE));

        }
    }
}
