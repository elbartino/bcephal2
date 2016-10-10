using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;

namespace Misp.Kernel.Ui.Base.Menu
{
    /// <summary>
    /// Menu
    /// </summary>
    public class ApplicationMenu : System.Windows.Controls.MenuItem
    {

        public static string FILE_MENU_CODE             = "File Menu";
        public static string FILE_MENU_SAVE_CODE        = "File Save Menu";
        public static string FILE_MENU_SAVE_AS_CODE     = "File Save As Menu";
        public static string INITIATION_MENU_CODE       = "Initiation Menu";
        public static string SOURCING_MENU_CODE         = "Sourcing Menu";
        public static string PLANIFICATION_MENU_CODE    = "Tranformation Tree Menu";        
        public static string ALLOCATION_MENU_CODE       = "Allocation Menu";
        public static string REPORTING_MENU_CODE        = "Reporting Menu";
        public static string RECONCILIATION_MENU_CODE   = "Reconciliation Menu";
        public static string RECONCILIATION_CONTEXT_MENU_CODE = "Reconciliation Context Menu";
        public static string ADMINISTRATION_MENU_CODE = "Administration Menu";
        public static string SETTINGS_MENU_CODE         = "Settings Menu";
        public static string HELP_MENU_CODE             = "Help Menu";

        

        /// <summary>
        /// Ce code quand il existe identifie le parent de manière unique.
        /// Le code du parent permet d'identifier sous quel menu il faut pluguer ce menu.
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// Le code du menu.
        /// Ce code quand il existe identifie le menu de manière unique.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets NavigationToken. 
        /// </summary>
        public NavigationToken NavigationToken { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationMenu()
        {
            initChildren();
            foreach (Control control in getControls()) 
            { 
                this.Items.Add(control);
            }
        }
                
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="token"></param>
        public ApplicationMenu(string header, NavigationToken token)
        {
            this.Header = header;
            NavigationToken = token;
            initChildren();
            foreach (ApplicationMenu menu in getControls()) { this.Items.Add(menu); }
        }
        
        /// <summary>
        /// Liste des sous menus
        /// </summary>
        /// <returns></returns>
        protected virtual List<Control> getControls() { return new List<Control>(0); }

        /// <summary>
        /// Initialisation des sous menus 
        /// </summary>
        protected virtual void initChildren() { }

        /*/// <summary>
        /// Construit un element de menu
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public ApplicationMenu BuildMenu(ApplicationMenu parent, string header, NavigationToken token)
        {
            ApplicationMenu menu = BuildMenu("", header, token);
            menu.Header = header;
            if (parent != null)
            {
                parent.Items.Add(menu);
            }
            return menu;
        }*/

        /// <summary>
        /// Construit un element de menu
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public ApplicationMenu BuildMenu(string parentCode, string header, NavigationToken token)
        {
            ApplicationMenu menu = new ApplicationMenu(header, token);
            menu.ParentCode = parentCode;
            menu.Click += new RoutedEventHandler (this.onMenuClick);
            return menu;
        }
        
        private void onMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is ApplicationMenu)
            {
                ApplicationMenu menu = (ApplicationMenu)sender;
                if (menu.NavigationToken != null)
                {                    
                    HistoryHandler.Instance.openPage(menu.NavigationToken);
                }
                
            }

            //ApplicationManager.PageManager.performOperation(OperationType.LIST, ApplicationManager.ControllerFactory.FolderBrowserController);
        }

        public virtual ApplicationMenu customize(PrivilegeObserver observer)
        {
            if (observer.user == null || !observer.user.active.Value) return null;
            if (observer.user.IsAdmin()) return this;
            if (observer.user.profil == null || !observer.user.profil.active) return null;
            return this;
        }

    }
}
