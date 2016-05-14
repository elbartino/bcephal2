using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Reconciliation.Posting
{
    /// <summary>
    /// Interaction logic for PostingBrowserForm.xaml
    /// </summary>
    public partial class PostingBrowserForm : Grid
    {
        public event Misp.Kernel.Ui.Base.ActivateEventHandler Activated;
        public PostingService PostingService { get; set; }
        public Kernel.Ui.Base.ChangeEventHandler ReconciliationEndedHandler;

        PostingConfirmationDialog dialog;

        public PeriodName rootPeriodName { get; set; }

        public PostingBrowserForm()
        {
            InitializeComponent();
            InitializeHandlers();
            this.filterForm.reset();
        }

        public void Display(PostingFilter filter)
        {
            filterForm.Display(filter);
        }

        public PostingFilter Fill()
        {
            return filterForm.Fill();
        }

        public void Search()
        {
            try
            {
                if (filterForm != null)
                {
                    PostingFilter filter = this.filterForm.Fill();
                    List<PostingBrowserData> items = this.PostingService.getPostings(filter);
                    grid.ItemsSource = items;
                }
                displayBalance();
            }
            catch (ServiceExecption e) { }
        }

        public void Reconciliate()
        {
            ReconciliationData reco = new ReconciliationData();
            decimal credit = dialog.toolbar.credit;
            decimal debit = dialog.toolbar.debit;
            decimal balance = dialog.toolbar.getBalance();
            foreach (object item in grid.SelectedItems)
            {
                if (item is PostingBrowserData)
                {
                    PostingBrowserData data = (PostingBrowserData)item;
                    reco.ids.Add(data.id);
                }
            }            
            reco.writeOffAmount = dialog.getWriteOffAmount();
            reco.writeOffDC = dialog.getWriteOffDC();
            reco.writeOffAccount = dialog.getWriteOffAccount();
            reco.debitedOrCreditedAccount = dialog.getDebitedOrCreditedAccount();

            bool result = PostingService.reconciliate(reco);
            if (result)
            {
                Search();
                dialog.Close();
                dialog = null;
                if (ReconciliationEndedHandler != null) ReconciliationEndedHandler();
            }
        }

        public void displayBalance()
        {
            toolBar.displayBalance(grid.SelectedItems);
        }

        private void InitializeHandlers()
        {
            toolBar.reconciliateButton.Click += OnReconciliate;
            toolBar.resetRecoButton.Click += OnResetReconciliation;
            toolBar.deleteButton.Click += OnDeletePostings;
            filterForm.searchButton.Click += OnSearchClick;
            filterForm.resetButton.Click += OnResetClick;
            grid.ChangeHandler += OnGridSelectionchange;
            filterForm.ChangeHandler += OnFilterChange;
            
            this.GotFocus += ReconciliationFilterGridPanel_GotFocus;
            this.MouseDown += ReconciliationFilterGridPanel_MouseDown;

            
            filterForm.filterPTForm.targetFilter.ItemChanged += OnTargetFilterChange;
            filterForm.filterPTForm.periodFilter.ItemChanged += OnPeriodFilterChange;
            filterForm.filterPTForm.targetFilter.ItemDeleted += OnTargetFilterDelete;
            filterForm.filterPTForm.periodFilter.ItemDeleted += OnPeriodFilterDelete;
        }

        PeriodName defaultPeriodName { get; set; }
        private void OnPeriodFilterDelete(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            Period filterPeriod = filterForm.filterPTForm.periodFilter.Period;
            if (filterPeriod == null)
                filterPeriod = new Period();
            filterPeriod.SynchronizeDeletePeriodItem(periodItem);
            filterForm.filterPTForm.periodFilter.DisplayPeriod(filterPeriod);
            OnFilterChange();
        }

        private void OnTargetFilterDelete(object item)
        {
            TargetItem targetItem = (TargetItem)item;
            Target filterScope = filterForm.filterPTForm.targetFilter.Scope;
            if (filterScope == null)
                filterScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            filterScope.SynchronizeDeleteTargetItem(targetItem);
            filterForm.filterPTForm.targetFilter.DisplayScope(filterScope);
            OnFilterChange();
        }

        private void OnPeriodFilterChange(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            Period filterPeriod = filterForm.filterPTForm.periodFilter.Period;
            if(filterPeriod == null)
                filterPeriod = new Period();
            filterPeriod.SynchronizePeriodItems(periodItem);
            filterForm.filterPTForm.periodFilter.DisplayPeriod(filterPeriod);
            OnFilterChange();
        }

        private void OnTargetFilterChange(object item)
        {
            if (item == null || !(item is TargetItem)) return;            
            TargetItem targetItem = (TargetItem)item;
            Target filterScope = filterForm.filterPTForm.targetFilter.Scope;
            if(filterScope == null)
                filterScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);            
            filterScope.SynchronizeTargetItems(targetItem);
            filterForm.filterPTForm.targetFilter.DisplayScope(filterScope);
        }


        private void OnFilterChange()
        {
            Search();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            filterForm.reset();
            Search();
        }

        private void OnSearchClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Search();
        }

        private void OnResetReconciliation(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Reset Reconciliation", "You are about to reset reconciliation for the selected items.\nDo you confirm operation?");
            if (response != MessageBoxResult.Yes) return;
            List<string> numbers = new List<string>(0);
            foreach (object item in grid.SelectedItems)
            {
                if (item is PostingBrowserData)
                {
                    PostingBrowserData data = (PostingBrowserData)item;
                    if (!String.IsNullOrWhiteSpace(data.reconciliationNumber) && !numbers.Contains(data.reconciliationNumber)) numbers.Add(data.reconciliationNumber);                    
                }
            }
            bool result = PostingService.resetReconciliation(numbers);
            if (result)
            {
                Search();
                if (ReconciliationEndedHandler != null) ReconciliationEndedHandler();
            }
        }

        private void OnDeletePostings(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Delete Postings", "You are about to delete selected postings.\nDo you confirm operation?");
            if (response != MessageBoxResult.Yes) return;
            List<long> ids = new List<long>(0);
            foreach (object item in grid.SelectedItems)
            {
                if (item is PostingBrowserData)
                {
                    PostingBrowserData data = (PostingBrowserData)item;
                    if (!String.IsNullOrWhiteSpace(data.reconciliationNumber))
                    {
                        MessageDisplayer.DisplayWarning("Delete Postings", "Unable to delete postings.\nAn Item in the selection is reconciliated.\nReset reconciliation and try again.");
                        return;
                    }
                    ids.Add(data.id);
                }
            }
            bool result = PostingService.deletePosting(ids);
            if (result)
            {
                Search();
                if (ReconciliationEndedHandler != null) ReconciliationEndedHandler();
            }
        }

        private void OnReconciliate(object sender, RoutedEventArgs e)
        {
            foreach (object item in grid.SelectedItems)
            {
                if (item is PostingBrowserData)
                {
                    PostingBrowserData data = (PostingBrowserData)item;
                    if (!String.IsNullOrWhiteSpace(data.reconciliationNumber))
                    {
                        MessageDisplayer.DisplayWarning("Reconciliation", "Unable to perform reconciliation.\nAn Item in the selection is already reconciliated.");
                        return;
                    }
                }
            }

            dialog = new PostingConfirmationDialog();
            dialog.PostingService = this.PostingService;
            dialog.display(grid.SelectedItems);
            dialog.yesButton.Click += OnConfirmReconciliation;
            dialog.noButton.Click += OnCancelReconciliation;
            dialog.ShowDialog();
        }

        private void OnCancelReconciliation(object sender, RoutedEventArgs e)
        {
            dialog.Close();
            dialog = null;
        }

        private void OnConfirmReconciliation(object sender, RoutedEventArgs e)
        {
            if(dialog.validateEdition())Reconciliate();
        }

        private void OnGridSelectionchange()
        {
            toolBar.displayBalance(grid.SelectedItems);
            int count = grid.SelectedItems.Count;
            toolBar.resetRecoButton.IsEnabled = count > 0;
            toolBar.reconciliateButton.IsEnabled = count > 0;
            toolBar.deleteButton.IsEnabled = count > 0;
        }

        private void ReconciliationFilterGridPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void ReconciliationFilterGridPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        public void HideFilter()
        {
            this.Children.Remove(filterForm);
            this.Children.Remove(splitter);
            filterRow.Height = new GridLength(0, GridUnitType.Star);
            filterForm = null;
        }

        public void HideToolBarButtons()
        {
            this.toolBar.HideButtons();
        }

        public void HideToolBarDeleteButton()
        {
            this.toolBar.HideDeleteButton();
        }

        public void HideToolBarRecoButton()
        {
            this.toolBar.HideRecoButton();
        }

        public void HideToolBarResetButton()
        {
            this.toolBar.HideResetButton();
        }

        public void onSelectPeriodNameFromSidebar(object sender)
        {
            if (sender == null) return;
            if (sender is PeriodName)
            {
                PeriodName periodName = (PeriodName)sender;
                filterForm.filterPTForm.periodFilter.SetPeriodItemName(periodName.name);
            }
            else if (sender is PeriodInterval)
            {
                PeriodInterval periodInterval = (PeriodInterval)sender;
                filterForm.filterPTForm.periodFilter.SetPeriodInterval((PeriodInterval)sender);
            }
        }

        public void onSelectTargetFromSidebar(object sender)
        {
            if (sender == null || !(sender is Kernel.Domain.AttributeValue)) return;
            Target target = (Target)sender;
            filterForm.filterPTForm.targetFilter.SetTargetValue((Target)sender);
        }


        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(this.filterForm.filterPTForm.periodFilter);
            controls.Add(this.filterForm.filterPTForm.targetFilter);
            controls.Add(this.filterForm.debitCheckBox);
            controls.Add(this.filterForm.creditCheckBox);
            controls.Add(this.filterForm.includeRecoCheckBox);
            return controls;
        }

    }
}
