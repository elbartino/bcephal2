using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Application;


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
                this.GetFileMenu().BackupSimpleMenu.IsEnabled = false;
                this.GetFileMenu().BackupAutomaticMenu.IsEnabled = false;
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


        public void disableMenuItem(MenuItem menuItem,String header="", bool disable=true) 
        {
            foreach(MenuItem menuitem in menuItem.Items)
            {
                if (!string.IsNullOrEmpty(header))
                   if (!menuItem.Header.Equals(header)) continue;
                menuItem.IsEnabled = !disable;
                disableMenuItem(menuitem,header,disable);
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
                if (!string.IsNullOrWhiteSpace(code) && ApplicationMenu.REPORTING_MENU_CODE.Equals(code))
                {
                    List<String> headersToDisable = new List<string>();
                    headersToDisable.Add(FunctionalitiesLabel.NEW_PIVOT_TABLE_LABEL);
                    headersToDisable.Add(FunctionalitiesLabel.LIST_PIVOT_TABLE_LABEL);
                    EnableChildrenAndNotHeaders(menu, true, headersToDisable);
                    continue;
                }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="enable"></param>
        protected void EnableChildrenAndNotHeaders(MenuItem menu, bool enable,List<String> Headers = null)
        {
            foreach (object ob in menu.Items)
            {
                if (ob is MenuItem)
                {
                    var item = (MenuItem)ob;
                    if (item.Items.Count > 0)
                    {
                        EnableChildrenAndNotHeaders(item, enable,Headers);
                    }
                    else
                    {
                        bool dismiss = false;
                        if (Headers != null || Headers.Count > 0)
                        {
                            for (int i = Headers.Count-1; i >= 0; i--)
                            {
                                string head = Headers[i];
                                if (!head.Equals(item.Header.ToString())) continue;
                                dismiss = true;
                                Headers.RemoveAt(i);
                                break;
                            }
                        }
                        if (dismiss) item.IsEnabled = !enable;
                        else item.IsEnabled = enable;
                    }
                }
            }
        }

        
        
    }
}
