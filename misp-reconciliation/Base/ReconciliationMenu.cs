using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;

namespace Misp.Reconciliation.Base
{
    public class ReconciliationMenu : ApplicationMenu
    {

        public ApplicationMenu ReconciliationFiltersMenu { get; private set; }
        public ApplicationMenu ReconciliationFiltersListMenu { get; private set; }
        public ApplicationMenu ReconciliationPostingMenu { get; private set; }
        public ApplicationMenu TransactionfileTypesMenu { get; private set; }

        public ApplicationMenu AdministrationUserMenu { get; private set; }
        public ApplicationMenu AdministrationUserListMenu { get; private set; }
        public ApplicationMenu AdministrationProfilMenu { get; private set; }
        public ApplicationMenu AdministrationProfilListMenu { get; private set; }
        
     


        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(ReconciliationPostingMenu);
            menus.Add(new Separator());
            menus.Add(ReconciliationFiltersMenu);
            menus.Add(ReconciliationFiltersListMenu);
            menus.Add(new Separator());

            menus.Add(AdministrationUserMenu);
            menus.Add(AdministrationUserListMenu);
            menus.Add(new Separator());
            menus.Add(AdministrationProfilMenu);
            menus.Add(AdministrationProfilListMenu);
            menus.Add(new Separator());
            menus.Add(TransactionfileTypesMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.RECONCILIATION_MENU_CODE;
            this.Header = "Reconciliation";
            ReconciliationFiltersMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "Reconciliation Filters", NavigationToken.GetCreateViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_FILTERS_FUNCTIONALITY));
            ReconciliationFiltersListMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "List Reconciliation Filters ", NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.LIST_RECONCILIATION_FILTERS_FUNCTIONALITY));
            ReconciliationPostingMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "Postings", NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_POSTING_FUNCTIONALITY));
            
            TransactionfileTypesMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "Administration Role", NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.ADMINISTRATION_ROLE));

            AdministrationUserMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "New User", NavigationToken.GetCreateViewToken(ReconciliationFunctionalitiesCode.ADMINISTRATION_NEW_USER));
            AdministrationUserListMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "List UserA", NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.ADMINISTRATION_LIST_USER));

            AdministrationProfilMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "New Profil", NavigationToken.GetCreateViewToken(ReconciliationFunctionalitiesCode.ADMINISTRATION_NEW_PROFIL));
            AdministrationProfilListMenu = BuildMenu(ApplicationMenu.RECONCILIATION_MENU_CODE, "List Profil", NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.ADMINISTRATION_LIST_PROFIL));
            
           
        }
    }
}
