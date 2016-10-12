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
        public ApplicationMenu ReconciliationContextMenu { get; private set; }
     


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
            menus.Add(ReconciliationContextMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = FunctionalitiesCode.RECONCILIATION;
            this.Header = FunctionalitiesLabel.RECONCILIATION_LABEL;
            ReconciliationFiltersMenu = BuildMenu(FunctionalitiesCode.RECONCILIATION, FunctionalitiesLabel.NEW_RECONCILIATION_FILTER_LABEL, NavigationToken.GetCreateViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_EDIT));
            ReconciliationFiltersListMenu = BuildMenu(FunctionalitiesCode.RECONCILIATION, FunctionalitiesLabel.LIST_RECONCILIATION_FILTERS_LABEL, NavigationToken.GetSearchViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_LIST));
            ReconciliationPostingMenu = BuildMenu(FunctionalitiesCode.RECONCILIATION, FunctionalitiesLabel.RECONCILIATION_POSTING_LABEL, NavigationToken.GetCreateViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_POSTINGS));
            ReconciliationContextMenu = BuildMenu(FunctionalitiesCode.RECONCILIATION, FunctionalitiesLabel.RECONCILIATION_CONFIGURATION_LABEL, NavigationToken.GetCreateViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_CONFIGURATION));        
        }
    }
}
