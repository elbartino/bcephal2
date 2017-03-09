using Misp.Bfc.Base;
using Misp.Bfc.Model;
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

        public ReviewBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.REVIEW;
        }

        public override OperationState Save()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState SaveAll()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState Rename()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName)
        {
            return OperationState.CONTINUE;
        }

        public override OperationState Delete()
        {
            return OperationState.CONTINUE;
        }

        public override Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Kernel.Domain.SubjectType.REVIEW;
        }

        public override OperationState TryToSaveBeforeClose()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState Create()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState Open()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState Open(object oid)
        {
            throw new NotImplementedException();
        }

        public override OperationState Search()
        {
            return OperationState.CONTINUE;
        }

        public override OperationState Search(object oid)
        {
            return OperationState.CONTINUE;
        }

        protected override IView getNewView()
        {
            return new ReviewBrowser();
        }
                      
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            return null;
        }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder()
        {
            return null;
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

        protected override void initializeViewData()
        {
            
        }

        protected override void initializeSideBarData()
        {
            
        }

        protected override void initializePropertyBarData()
        {
            
        }

        protected override void initializeViewHandlers()
        {
            
        }

        protected override void initializeSideBarHandlers()
        {
            
        }

        protected override void initializePropertyBarHandlers()
        {
            
        }
    }
}
