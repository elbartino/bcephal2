using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Application;

namespace Misp.Kernel.Ui.Base.Menu
{
    public class SettingsMenu : ApplicationMenu
    {

        private ApplicationMenu groups;

        public ApplicationMenu Groups { get { return groups; } }

        private ApplicationMenu properties;

        public ApplicationMenu Properties { get { return properties; } }


        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(Groups);
            //menus.Add(Properties);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.SETTINGS_MENU_CODE; 
            this.Header = "Settings";
            groups = BuildMenu(ApplicationMenu.SETTINGS_MENU_CODE, "Groups", NavigationToken.GetSearchViewToken(FunctionalitiesCode.LIST_GROUP_FUNCTIONALITY));
            properties = BuildMenu(ApplicationMenu.SETTINGS_MENU_CODE, "Properties", NavigationToken.GetCreateViewToken(FunctionalitiesCode.PROPERTIES_FUNCTIONALITY));
        }

    }
}
