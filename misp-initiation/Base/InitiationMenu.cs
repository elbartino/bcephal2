using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;

namespace Misp.Initiation.Base
{
    public class InitiationMenu : ApplicationMenu
    {

        private ApplicationMenu model; 
        
        private ApplicationMenu period;

        private ApplicationMenu backupMenu;

        private ApplicationMenu backupSimpleMenu;

        private ApplicationMenu backupAutomaticMenu;

        public ApplicationMenu Model { get { return model; } }
                
        public ApplicationMenu Period { get { return period; } }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(Model);
            menus.Add(period);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = InitiationFunctionalitiesCode.INITIATION;
            this.Header = FunctionalitiesLabel.INITIATION_LABEL;
            model = BuildMenu(InitiationFunctionalitiesCode.INITIATION, FunctionalitiesLabel.INITIATION_EDIT_MODEL_LABEL, NavigationToken.GetSearchViewToken(InitiationFunctionalitiesCode.INITIATION));
            period = BuildMenu(InitiationFunctionalitiesCode.INITIATION, FunctionalitiesLabel.INITIATION_EDIT_PERIOD_LABEL, NavigationToken.GetSearchViewToken(InitiationFunctionalitiesCode.INITIATION_PERIOD));
        }

        public override ApplicationMenu customize(PrivilegeObserver observer)
        {
            ApplicationMenu menu = base.customize(observer);
            if (menu != null)
            {

            }
            return menu;
        }

    }
}
