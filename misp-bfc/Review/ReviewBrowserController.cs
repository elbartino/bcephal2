using Misp.Bfc.Base;
using Misp.Bfc.Model;
using Misp.Bfc.Service;
using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Bfc.Review
{
    public class ReviewBrowserController : Controller<PrefundingAccountData, BrowserData>
    {

        ReviewService reviewService;

        public ReviewBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.REVIEW;
        }

        public ReviewService getReviewService()
        {
            if (this.reviewService == null)
            {
                BfcServiceFactory factory = new BfcServiceFactory(ApplicationManager);
                this.reviewService = factory.GetReviewService();
            }
            return this.reviewService;
        }


        public override OperationState Search() 
        {
            int oid = 1;
            if (getReviewBrowser().Form.TabControl.SelectedIndex == 0)
            {
                PrefundingAccountData data = getReviewService().PrefundingAccountService.getPrefundingAccountData(oid);
                getReviewBrowser().Form.Display(data);
            }
            else if (getReviewBrowser().Form.TabControl.SelectedIndex == 1)
            {

            }
            return OperationState.CONTINUE; 
        }

        public override Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Kernel.Domain.SubjectType.REVIEW;
        }

        public override OperationState Save() { return OperationState.CONTINUE; }
        public override OperationState SaveAll() { return OperationState.CONTINUE; }
        public override OperationState Rename() { return OperationState.CONTINUE; }
        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }
        public override OperationState Delete() { return OperationState.CONTINUE; }
        public override OperationState TryToSaveBeforeClose() { return OperationState.CONTINUE; }
        public override OperationState Create() { return OperationState.CONTINUE; }
        public override OperationState Open() { return OperationState.CONTINUE; }
        public override OperationState Open(object oid) { return OperationState.CONTINUE; }        
        public override OperationState Search(object oid) { return OperationState.CONTINUE; }

        protected override IView getNewView()
        {
            return new ReviewBrowser();
        }
                      
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            BrowserToolBar toolBar = new BrowserToolBar();
            toolBar.NewButton.Visibility = System.Windows.Visibility.Collapsed;
            return toolBar;
        }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            BrowserToolBarHandlerBuilder toolBarHandler = new BrowserToolBarHandlerBuilder(this);
            return toolBarHandler;
        }

        protected override Kernel.Ui.Sidebar.SideBar getNewSideBar()
        {
            BrowserSideBar sideBar = new BrowserSideBar();
            sideBar.GroupGroup.Visibility = System.Windows.Visibility.Collapsed;
            return sideBar;
        }

        protected override PropertyBar getNewPropertyBar()
        {
            return null;
        }

        public ReviewBrowser getReviewBrowser()
        {
            return (ReviewBrowser)view;
        }

        protected override void initializeViewHandlers() 
        {
            getReviewBrowser().Form.TabControl.SelectionChanged += OnSelectTabChanged;
        }

        private void OnSelectTabChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            Search();
        }


        protected override void initializeViewData()
        {
            List<BfcItem> items = getReviewService().MemberBankService.getAll();
            getReviewBrowser().Form.MemberBankComboBox.ItemsSource = items;
        }

        protected override void initializeSideBarData() { }
        protected override void initializePropertyBarData() { }        
        protected override void initializeSideBarHandlers() { }
        protected override void initializePropertyBarHandlers() { }

    }
}
