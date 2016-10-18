﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RestSharp;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Base.Menu;

using Misp.Kernel.Plugin;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Administration.Base;

namespace Misp.Kernel.Application
{
    /// <summary>
    /// Cette classe permet de construire les menus de l'application.
    /// </summary>
    public class ApplicationMenusBuilder
    {
                
        /// <summary>
        /// Construit une nouvelle instance de la classe ApplicationMenusBuilder
        /// </summary>
        public ApplicationMenusBuilder(ApplicationManager applicationManager)
        {
            this.ApplicationManager = applicationManager;
        }

        /// <summary>
        /// Gets or sets the ApplicationManager
        /// </summary>
        public ApplicationManager ApplicationManager { get; set; }

        /// <summary>
        /// Build the menu bar
        /// </summary>
        public void build()
        {
            MenuBar menuBar = new MenuBar();

            FileMenu fileMenu = new FileMenu();
            SettingsMenu settingsMenu = new SettingsMenu();
            HelpMenu helpMenu = new HelpMenu();

            if (ApplicationManager.ApplcationConfiguration.IsMultiuser())
            {
                fileMenu.Items.Remove(fileMenu.OpenFile);
            }

            menuBar.ApplicationMenus.Add(fileMenu);
            menuBar.ApplicationMenus.Add(settingsMenu);
            menuBar.ApplicationMenus.Add(helpMenu);

            menuBar.Items.Add(fileMenu);
            buildPluginsMenus(menuBar);
            buildAdministrationMenu(menuBar);
            menuBar.Items.Add(settingsMenu);
            menuBar.Items.Add(helpMenu);

            ApplicationManager.MainWindow.displayMenuBar(menuBar);
            SideBar.StatusNames = buildPluginNames();

            menuBar.customizeForFileClosed();
        }

        /// <summary>
        /// Build the Administration Menu.
        /// </summary>
        /// <param name="menuBar"></param>
        protected void buildAdministrationMenu(MenuBar menuBar)
        {
            if (ApplicationManager.ApplcationConfiguration.IsMultiuser() && ApplicationManager.User.IsAdmin())
            {
                AdministrationMenu menu = new AdministrationMenu();
                menuBar.Items.Add(menu);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuBar"></param>
        protected void buildPluginsMenus(MenuBar menuBar)
        {
            PrivilegeObserver observer = new PrivilegeObserver();
            foreach(IPlugin plugin in ApplicationManager.Plugins){
                foreach (ApplicationMenu menu in plugin.Menus)
                {
                    if (ApplicationManager.ApplcationConfiguration.IsMultiuser())
                    {
                        if (menu.customize(observer) != null) menuBar.DisplayMenu(menu);
                    }
                    else menuBar.DisplayMenu(menu);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuBar"></param>
        protected List<string> buildPluginNames()
        {
            List<string> names = new List<string>(0);
            foreach (IPlugin plugin in ApplicationManager.Plugins)
            {
                names.Add(plugin.Name);
            }
            names.Add("Administration");
            return names;
        }

        
        
        private void onNewModelMenuClick(object sender, RoutedEventArgs e)
        {
            //ApplicationManager.PageManager.performOperation(OperationType.CREATE, ApplicationManager.ControllerFactory.FolderEditorController);
        }

        private void onListModelsMenuClick(object sender, RoutedEventArgs e)
        {
            //ApplicationManager.PageManager.performOperation(OperationType.LIST, ApplicationManager.ControllerFactory.FolderBrowserController);
        }



    }
}
