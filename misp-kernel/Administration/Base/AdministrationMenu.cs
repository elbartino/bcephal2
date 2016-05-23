﻿using Misp.Kernel.Ui.Base.Menu;
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
        public ApplicationMenu AdministrationUserMenu { get; private set; }
        public ApplicationMenu AdministrationProfilMenu { get; private set; }
        public ApplicationMenu AdministrationProfilListMenu { get; private set; }
        public ApplicationMenu AdministrationUserListMenu { get; private set; }
        public ApplicationMenu AdministrationRoleMenu { get; private set; }



        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(AdministrationUserMenu);
            menus.Add(AdministrationUserListMenu);
            menus.Add(new Separator());
            menus.Add(AdministrationProfilMenu);
            menus.Add(AdministrationUserListMenu);
            menus.Add(new Separator());
            menus.Add(AdministrationRoleMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.RECONCILIATION_MENU_CODE;
            this.Header = "Administration";
            AdministrationUserMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE,"New User",Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER));
            AdministrationUserListMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "List User", Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER));
            
            AdministrationProfilMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "New User", Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL));
            AdministrationProfilListMenu = BuildMenu(ApplicationMenu.ADMINISTRATION_MENU_CODE, "New User", Kernel.Application.NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_LIST_PROFIL)); 
            

            //AdministrationUserMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "New User", NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.ADMINISTRATION_NEW_USER));
            //AdministrationUserListMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "List User ", NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.LIST_RECONCILIATION_FILTERS_FUNCTIONALITY));
            //AdministrationProfileMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "Postings", NavigationToken.GetSearchViewToken(AdministrationFunctionalitiesCode.RECONCILIATION_POSTING_FUNCTIONALITY));
            //AdministrationRoleMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "Transaction File Types", NavigationToken.GetCreateViewToken(AdministrationFunctionalitiesCode.TRANSACTION_FILE_TYPES_FUNCTIONALITY));

        }
    }
}