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
    public class HelpMenu : ApplicationMenu
    {

        private ApplicationMenu help;
        private ApplicationMenu aboutBcephal;

        public ApplicationMenu Help { get { return help; } }
        public ApplicationMenu AboutBcephal { get { return aboutBcephal; } }

        public virtual bool isDefaultMenu() { return true; }

        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected override List<Control> getControls()
        {
            List<Control> menus = new List<Control>(0);
            menus.Add(Help);
            menus.Add(AboutBcephal);
            return menus;
        }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected override void initChildren()
        {
            this.Code = ApplicationMenu.HELP_MENU_CODE;
            this.Header = "Help";
            help = BuildMenu(ApplicationMenu.HELP_MENU_CODE, "Help...", null);
            help.IsEnabled = false;
            aboutBcephal = BuildMenu(ApplicationMenu.HELP_MENU_CODE, "About B-Cephal...", NavigationToken.GetCreateViewToken(FunctionalitiesCode.ABOUT_FUNCTIONALITY));
        }

    }
}
