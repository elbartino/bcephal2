using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;

namespace Misp.Allocation.Base
{
    public class AllocationMenu : ApplicationMenu
    {

        private ApplicationMenu newAllocationRunMenu;
        private ApplicationMenu listAllocationRunMenu;
        private ApplicationMenu runAllAllocationMenu;
        private ApplicationMenu clearAllAllocationMenu;
        private ApplicationMenu auditAllMenu;
        private ApplicationMenu allocationLogMenu;

        public ApplicationMenu NewAllocationRunMenu { get { return newAllocationRunMenu; } }
        public ApplicationMenu ListAllocationRunMenu { get { return listAllocationRunMenu; } }
        public ApplicationMenu RunAllAllocationMenu { get { return runAllAllocationMenu; } }
        public ApplicationMenu ClearAllAllocationMenu { get { return clearAllAllocationMenu; } }
        public ApplicationMenu AuditAllMenu { get { return auditAllMenu; } }
        public ApplicationMenu AllocationLogMenu { get { return allocationLogMenu; } }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            //menus.Add(NewAllocationRunMenu);
            //menus.Add(ListAllocationRunMenu);
            menus.Add(RunAllAllocationMenu);
            menus.Add(ClearAllAllocationMenu);
            //menus.Add(AuditAllMenu);
            menus.Add(AllocationLogMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.SOURCING_MENU_CODE;
            this.Header = FunctionalitiesLabel.ALLOCATION_LABEL;
            newAllocationRunMenu = BuildMenu(ApplicationMenu.ALLOCATION_MENU_CODE, FunctionalitiesLabel.ALLOCATION_RUN_LIST_LABEL, NavigationToken.GetCreateViewToken(AllocationFunctionalitiesCode.NEW_ALLOCATION_RUN_FUNCTIONALITY));
            listAllocationRunMenu = BuildMenu(ApplicationMenu.ALLOCATION_MENU_CODE, FunctionalitiesLabel.ALLOCATION_NEW_LABEL, NavigationToken.GetSearchViewToken(AllocationFunctionalitiesCode.LIST_ALLOCATION_RUN_FUNCTIONALITY));
            runAllAllocationMenu = BuildMenu(ApplicationMenu.ALLOCATION_MENU_CODE, FunctionalitiesLabel.ALLOCATION_RUN_TABLES_LABEL, NavigationToken.GetSearchViewToken(AllocationFunctionalitiesCode.RUN_ALL_ALLOCATION_FUNCTIONALITY));
            clearAllAllocationMenu = BuildMenu(ApplicationMenu.ALLOCATION_MENU_CODE, FunctionalitiesLabel.ALLOCATION_CLEAR_TABLES_LABEL, NavigationToken.GetSearchViewToken(AllocationFunctionalitiesCode.ALLOCATION_CLEAR_ALL));
            allocationLogMenu = BuildMenu(ApplicationMenu.ALLOCATION_MENU_CODE, FunctionalitiesLabel.ALLOCATION_LOG_LABEL, NavigationToken.GetSearchViewToken(AllocationFunctionalitiesCode.ALLOCATION_LOG));
        }

    }
}
