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

        public ApplicationMenu LoadTablesAndGridsMenu { get; set; }
        public ApplicationMenu ClearTablesAndGridsMenu { get; set; }
        public ApplicationMenu LoadLogMenu { get; set; }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(LoadTablesAndGridsMenu);
            menus.Add(ClearTablesAndGridsMenu);
            menus.Add(LoadLogMenu);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = FunctionalitiesCode.LOAD;
            this.Header = FunctionalitiesLabel.ALLOCATION_LABEL;
            LoadTablesAndGridsMenu = BuildMenu(FunctionalitiesCode.LOAD, FunctionalitiesLabel.ALLOCATION_RUN_TABLES_LABEL, NavigationToken.GetSearchViewToken(FunctionalitiesCode.LOAD_TABLES_AND_GRIDS));
            ClearTablesAndGridsMenu = BuildMenu(FunctionalitiesCode.LOAD, FunctionalitiesLabel.ALLOCATION_CLEAR_TABLES_LABEL, NavigationToken.GetSearchViewToken(FunctionalitiesCode.LOAD_CLEAR_TABLES_AND_GRIDS));
            LoadLogMenu = BuildMenu(FunctionalitiesCode.LOAD, FunctionalitiesLabel.ALLOCATION_LOG_LABEL, NavigationToken.GetSearchViewToken(FunctionalitiesCode.LOAD_LOG));
        }

    }
}
