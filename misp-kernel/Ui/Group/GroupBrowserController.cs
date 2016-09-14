using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Misp.Kernel.Ui.Group
{
    public class GroupBrowserController : BrowserController<Misp.Kernel.Domain.BGroup, BrowserData>
    {

        public GroupBrowserController() 
        {
            //ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return FunctionalitiesCode.NEW_GROUP_FUNCTIONALITY; }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() 
        { 
            GroupBrowser browser = new GroupBrowser();
            browser.Grid.BrowserGridContextMenu.Items.Remove(browser.Grid.BrowserGridContextMenu.NewMenuItem);
            browser.Grid.BrowserGridContextMenu.Items.Remove(browser.Grid.BrowserGridContextMenu.SaveAsMenuItem);
            browser.Grid.BrowserGridContextMenu.Items.Remove(browser.Grid.BrowserGridContextMenu.OpenMenuItem);
            return browser;
        }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.GROUP;
        }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Misp.Kernel.Ui.Base.ToolBar getNewToolBar() 
        { 
            BrowserToolBar toolbar = new BrowserToolBar();
            toolbar.NewButton.Visibility = System.Windows.Visibility.Hidden;
            return toolbar;
        }

        protected override void customizeContextMenu()
        {
            this.GetBrowser().Grid.BrowserGridContextMenu.RenameMenuItem.IsEnabled = false;
            this.GetBrowser().Grid.BrowserGridContextMenu.DeleteMenuItem.IsEnabled = false;
            object item = this.GetBrowser().Grid.SelectedItem;
            if (item == null || !(item is BrowserData)) return;
            int count = this.GetBrowser().Grid.SelectedItems.Count;
            BrowserData data = (BrowserData) item;
            bool isDefaultGroup = !String.IsNullOrEmpty(data.name) && data.name.Equals(BGroup.DEFAULT_GROUP_NAME);
            this.GetBrowser().Grid.BrowserGridContextMenu.RenameMenuItem.IsEnabled = count == 1 && !isDefaultGroup;
            this.GetBrowser().Grid.BrowserGridContextMenu.DeleteMenuItem.IsEnabled = count > 1 || !isDefaultGroup;
            
        }

        protected override void initializeSideBarData()
        {
            if (this.SideBar != null && this.Service != null)
            {
                ((BrowserSideBar)SideBar).GroupCatagoryGroup.GroupCatagoryListBox.ItemsSource = SubjectType.GetCategories(); 

                ((BrowserSideBar)SideBar).RemoveGroup(((BrowserSideBar)SideBar).GroupGroup);
                ((BrowserSideBar)SideBar).AddGroup(((BrowserSideBar)SideBar).GroupCatagoryGroup);
            }
        }
        
        protected override void initializeSideBarHandlers()
        {
            if (this.SideBar != null)
            {
                ((BrowserSideBar)SideBar).GroupCatagoryGroup.GroupCatagoryListBox.SelectionChanged += OnCategorySelected;
            }
        }

        private void OnCategorySelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            object item = ((BrowserSideBar)SideBar).GroupCatagoryGroup.GroupCatagoryListBox.SelectedItem;
            if (item == null) return;
            Kernel.Domain.SubjectType category = (Kernel.Domain.SubjectType)item;
            FilterByCategory(category != null && !category.isAll() ? category.label : null);
        }


        /// <summary>
        /// effectue la recherche
        /// </summary>
        /// <returns></returns>
        public virtual OperationState FilterByCategory(String category)
        {
            if (String.IsNullOrEmpty(category)) return Search();
            try
            {                
                BrowserDataFilter filter = GetBrowser().BuildFilter(0);
                BrowserDataPage<BrowserData> page = ((GroupService)this.Service).getBrowserDatasByCategory(filter, category);
                GetBrowser().DisplayPage(page);
                return OperationState.CONTINUE;
            }
            catch (ServiceExecption e)
            {
                DisplayError("error", e.Message);
            }

            return OperationState.STOP;
        }


        public override OperationState RenameItem(string newName)
        {
            Object selection = GetBrowser().Grid.SelectedItem;
            if (selection == null) return OperationState.STOP;
            BrowserData data = (BrowserData)selection;
            BGroup group = ((GroupService)Service).getGroupByNameAndType(newName, data.group);
            if (group != null && group.oid.HasValue && group.oid.Value != data.oid)
            {
                DisplayError("Rename ", "There is another group named : " + newName);
                return OperationState.STOP;
            }
            try
            {
                Service.Rename(data.oid, newName);
                Search();
            }
            catch (Domain.BcephalException)
            {
                DisplayError("Unable to rename group", "Unable to rename group : " + newName);
                return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }


    }
}
