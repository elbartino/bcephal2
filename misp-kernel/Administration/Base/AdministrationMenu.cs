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
        public ApplicationMenu NewUserMenu { get; private set; }
        public ApplicationMenu ListUserMenu { get; private set; }
        
        public ApplicationMenu ProfilMenu { get; private set; }
        public ApplicationMenu NewProfilMenu { get; private set; }
        public ApplicationMenu ListProfilMenu { get; private set; }
        
        public ApplicationMenu RoleMenu { get; private set; }



        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(UserMenu);
            menus.Add(ProfilMenu);
            menus.Add(RoleMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = FunctionalitiesCode.ADMINISTRATION;
            this.Header = FunctionalitiesLabel.ADMINISTRATION_LABEL;

            UserMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION, FunctionalitiesLabel.ADMINISTRATION_USER_LABEL, FunctionalitiesCode.ADMINISTRATION_USER);
            NewUserMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION_USER, FunctionalitiesLabel.ADMINISTRATION_NEW_USER_LABEL, Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_USER_EDIT));
            ListUserMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION_USER, FunctionalitiesLabel.ADMINISTRATION_LIST_USER_LABEL, Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_USER_LIST));
            UserMenu.Items.Add(NewUserMenu);
            UserMenu.Items.Add(ListUserMenu);

            ProfilMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION, FunctionalitiesLabel.ADMINISTRATION_PROFIL_LABEL, FunctionalitiesCode.ADMINISTRATION_PROFIL);
            NewProfilMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION_PROFIL, FunctionalitiesLabel.ADMINISTRATION_NEW_PROFIL_LABEL, Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_EDIT));
            ListProfilMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION_PROFIL, FunctionalitiesLabel.ADMINISTRATION_LIST_PROFIL_LABEL, Kernel.Application.NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_PROFIL_LIST));
            ProfilMenu.Items.Add(NewProfilMenu);
            ProfilMenu.Items.Add(ListProfilMenu);

            RoleMenu = BuildMenu(FunctionalitiesCode.ADMINISTRATION, FunctionalitiesLabel.ADMINISTRATION_ROLE_LABEL, Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_ROLE));

        }
    }
}
