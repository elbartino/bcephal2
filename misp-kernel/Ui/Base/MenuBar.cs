using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Base.Menu;


namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// The application menu bar
    /// </summary>
    public class MenuBar : System.Windows.Controls.Menu
    {
        /// <summary>
        /// Liste des menus principaux
        /// </summary>
        public List<ApplicationMenu> ApplicationMenus { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MenuBar()
        {
            ApplicationMenus = new List<ApplicationMenu>(0);
        }

        /// <summary>
        /// Display menu
        /// </summary>
        /// <param name="menu"></param>
        public void DisplayMenu(ApplicationMenu menu){
            if(string.IsNullOrWhiteSpace(menu.ParentCode)) {
                ApplicationMenus.Add(menu);
                Items.Add(menu);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void customizeForFileClosed()
        {
            foreach (ApplicationMenu menu in ApplicationMenus)
            {
                string code = menu.Code;
                if (!string.IsNullOrWhiteSpace(code) && ApplicationMenu.FILE_MENU_CODE.Equals(code))
                {
                    customizeSavingOptionMenus(menu, false);
                    continue;
                }                
                if (!string.IsNullOrWhiteSpace(code) && ApplicationMenu.HELP_MENU_CODE.Equals(code)) continue;
                EnableChildren(menu, false);
            }
        }

        public void customizeSavingOptionMenus(ApplicationMenu menu, bool isFileOpened)
        {
            foreach (object ob in menu.Items)
            {
                if (ob is ApplicationMenu && ((ApplicationMenu)ob).ParentCode.Equals(ApplicationMenu.FILE_MENU_SAVE_AS_CODE))
                {
                    ((MenuItem)ob).IsEnabled = isFileOpened;
                    return;
                }   
            }
           
        }

        public void customizeForFileOpened()
        {
            foreach (ApplicationMenu menu in ApplicationMenus)
            {
                string code = menu.Code;
                if (!string.IsNullOrWhiteSpace(code) && ApplicationMenu.FILE_MENU_CODE.Equals(code))
                {
                    customizeSavingOptionMenus(menu, true);
                    continue;
                }
                //if (!string.IsNullOrWhiteSpace(code) && ApplicationMenu.SETTINGS_MENU_CODE.Equals(code)) continue;
                if (!string.IsNullOrWhiteSpace(code) && ApplicationMenu.HELP_MENU_CODE.Equals(code)) continue;
                EnableChildren(menu, true);
            }
        }

        public FileMenu GetFileMenu()
        {
            foreach (ApplicationMenu menu in ApplicationMenus)
            {
                if(menu is FileMenu) return (FileMenu)menu;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableAll(bool enable)
        {
            foreach (Object ob in this.Items)
            {
                if (ob is MenuItem)
                {
                    EnableChildren((MenuItem)ob, enable);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="enable"></param>
        protected void EnableChildren(MenuItem menu, bool enable)
        {
            foreach (object ob in menu.Items)
            {
                if (ob is MenuItem)
                {
                    var item = (MenuItem) ob;
                    if (item.Items.Count > 0)
                    {
                        EnableChildren(item, enable);
                    }
                    else
                    {
                        item.IsEnabled = enable;
                    }
                }                
            }
        }

        
        
    }
}
