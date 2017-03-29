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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
            getReviewBrowser().Form.IsBussy = true;
            Kernel.Application.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    try
                    {
                        ReviewFilter filter = getReviewBrowser().Form.GetFilter();
                        if (getReviewBrowser().Form.TabControl.SelectedIndex == 0)
                        {
                            PrefundingAccountData data = getReviewService().getPrefundingAccountData(filter);
                            getReviewBrowser().Form.Display(data);
                        }
                        else if (getReviewBrowser().Form.TabControl.SelectedIndex == 1)
                        {
                            List<SettlementEvolutionData> datas = getReviewService().getSettlementEvolutionDatas(filter);
                            getReviewBrowser().Form.Display(datas);
                            UpdateSettlementEvolutionChart(filter);
                        }
                        else if (getReviewBrowser().Form.TabControl.SelectedIndex == 2)
                        {
                            List<AgeingBalanceData> datas = getReviewService().getAgeingBalanceDatas(filter);
                            getReviewBrowser().Form.DisplayTotal(datas);
                        }
                        getReviewBrowser().Form.IsBussy = false;
                    }
                    catch (Exception)
                    {
                        getReviewBrowser().Form.IsBussy = false;
                    }
                    finally { getReviewBrowser().Form.IsBussy = false; }
                }));
            
            return OperationState.CONTINUE; 
        }

        private void UpdateSettlementEvolutionChart(ReviewFilter filter = null)
        {
            try
            {
                getReviewBrowser().Form.SettlementEvolutionForm.IsChartBussy = true;
                if (filter == null) filter = getReviewBrowser().Form.GetFilter();
                List<SettlementEvolutionChartData> datas = getReviewService().getSettlementEvolutionChartDatas(filter);
                getReviewBrowser().Form.SettlementEvolutionForm.DisplayChart(datas);
                getReviewBrowser().Form.SettlementEvolutionForm.IsChartBussy = false;
            }
            catch(Exception){
                getReviewBrowser().Form.SettlementEvolutionForm.IsChartBussy = false;
            }
        }

        private void AgeingBalanceSearchDetail()
        {
            try
            {
                getReviewBrowser().Form.AgeingBalanceForm.IsDetailGridBussy = true;
                ReviewFilter fil = getReviewBrowser().Form.GetFilter();
                ReviewFilter filter = new ReviewFilter();
                filter.details = true;
                filter.memberBankIdOids = fil.memberBankIdOids;
                filter.schemeIdOids = fil.schemeIdOids;
                List<AgeingBalanceData> datas = getReviewService().getAgeingBalanceDatas(filter);
                getReviewBrowser().Form.DisplayDetails(datas);
                getReviewBrowser().Form.AgeingBalanceForm.IsDetailGridBussy = false;
            }
            catch (Exception)
            {
                getReviewBrowser().Form.AgeingBalanceForm.IsDetailGridBussy = false;
            }
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
            getReviewBrowser().Form.MemberBankChanged += OnMemberBankChanged;
            getReviewBrowser().Form.SettlementEvolutionForm.FilterChanged += OnSettlementEvolutionFilterChanged;
            getReviewBrowser().Form.SettlementEvolutionForm.PeriodChanged += OnSettlementEvolutionPeriodChanged;

            getReviewBrowser().Form.AgeingBalanceForm.SearchDetail += OnAgeingBalanceSearchDetail;
        }

        private void OnAgeingBalanceSearchDetail()
        {
            Kernel.Application.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.AgeingBalanceSearchDetail()));
        }

        private void OnMemberBankChanged()
        {
            this.Search();            
        }

        private void OnSettlementEvolutionFilterChanged()
        {
            this.Search();        
        }

        private void OnSettlementEvolutionPeriodChanged()
        {
            Kernel.Application.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.UpdateSettlementEvolutionChart()));
        }
        
        private void OnSelectTabChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            this.Search();        
        }


        protected override void initializeViewData()
        {
            List<BfcItem> banks = getReviewService().MemberBankService.getAll();
            banks.Add(null);
            getReviewBrowser().Form.MemberBankComboBoxEdit.ItemsSource = banks;

            List<BfcItem> schemes = getReviewService().SchemeService.getAll();
            schemes.Add(null);
            getReviewBrowser().Form.SettlementEvolutionForm.SchemeComboBoxEdit.ItemsSource = schemes;

            List<BfcItem> platforms = getReviewService().PlatformService.getAll();
            schemes.Add(null);
            getReviewBrowser().Form.SettlementEvolutionForm.PlatformComboBoxEdit.ItemsSource = platforms;
        }

        protected override void initializeSideBarData() { }
        protected override void initializePropertyBarData() { }        
        protected override void initializeSideBarHandlers() { }
        protected override void initializePropertyBarHandlers() { }

    }
}
