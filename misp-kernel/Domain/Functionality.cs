using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Kernel.Domain
{
    public class Functionality
    {

        public string code { get; set; }
        public string name { get; set; }
        public string module { get; set; }
        public EditionMode mode { get; set; }

        public Functionality(string code, string name, string module, EditionMode mode)
        {
            this.code = code;
            this.name = name;
            this.module = module;
            this.mode = mode;
        }

        public NavigationToken buildNavigationToken()
        {
            if (mode == null) return NavigationToken.GetSearchViewToken(code);
            else if (mode == EditionMode.CREATE) return NavigationToken.GetCreateViewToken(code);
            else if (mode == EditionMode.MODIFY) return NavigationToken.GetCreateViewToken(code);
            else if (mode == EditionMode.READ_ONLY) return NavigationToken.GetSearchViewToken(code);
            return NavigationToken.GetSearchViewToken(code);
        }

        public ApplicationMenu buildMenus(User user = null)
        {
            if (user != null && !user.hasRight(code, mode)) return null;
            return buildMenu(name, buildNavigationToken());
        }


        /// <summary>
        /// Construit un element de menu
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        protected ApplicationMenu buildMenu(string header, NavigationToken token)
        {
            ApplicationMenu menu = new ApplicationMenu(header, token);
            menu.Click += OnMenuClick;
            return menu;
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is ApplicationMenu)
            {
                ApplicationMenu menu = (ApplicationMenu)sender;
                if (menu.NavigationToken != null)
                {
                    HistoryHandler.Instance.openPage(menu.NavigationToken);
                }
            }
        }


    }
}
