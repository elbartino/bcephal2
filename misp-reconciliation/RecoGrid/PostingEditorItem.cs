using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Reconciliation.Posting;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Sourcing.InputGrid
{
    public class PostingEditorItem : InputGridEditorItem
    {

        public PostingToolBar PostingToolBar { get; set; }

        public Kernel.Ui.Base.ChangeEventHandler ReconciliationEndedHandler;

        PostingConfirmationDialog dialog;

        public ReconciliationGridService ReconciliationGridService { get; set; }

        public PostingEditorItem() : base()
        {
            initializePageHandlers();
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Grille> getNewEditorItemForm() 
        {
            PostingToolBar = new PostingToolBar();
            InputGridForm form = new InputGridForm();
            form.GridForm.filterForm.RecoPanel.Visibility = Visibility.Visible;
            form.GridForm.otherToolBarPanel.Children.Add(PostingToolBar);
            return form;
        }


        public void Search(int currentPage = 0)
        {
            try
            {
                GrilleFilter filter = this.getInputGridForm().GridForm.filterForm.Fill();
                filter.grid = new Grille();
                filter.grid.code = this.EditedObject.code;
                filter.grid.columnListChangeHandler = this.EditedObject.columnListChangeHandler;
                filter.grid.report = this.EditedObject.report;
                filter.grid.oid = this.EditedObject.oid;
                filter.grid.name = this.EditedObject.name;
                filter.page = currentPage;
                filter.pageSize = (int)this.getInputGridForm().GridForm.toolBar.pageSizeComboBox.SelectedItem;
                GrillePage rows = this.ReconciliationGridService.getGridRows(filter);
                this.getInputGridForm().GridForm.displayPage(rows);
            }
            catch (ServiceExecption e) { }
        }

        public void Reconciliate()
        {
            ReconciliationData reco = new ReconciliationData();
            decimal credit = dialog.toolbar.credit;
            decimal debit = dialog.toolbar.debit;
            decimal balance = dialog.toolbar.getBalance();
            
            reco.ids = this.getInputGridForm().GridForm.gridBrowser.GetSelectedOis();
            reco.writeOffAmount = dialog.getWriteOffAmount();
            reco.writeOffDC = dialog.getWriteOffDC();
            reco.writeOffAccount = dialog.getWriteOffAccount();
            reco.debitedOrCreditedAccount = dialog.getDebitedOrCreditedAccount();

            bool result = ReconciliationGridService.PostingService.reconciliate(reco);
            if (result)
            {
                Search();
                dialog.Close();
                dialog = null;
                if (ReconciliationEndedHandler != null) ReconciliationEndedHandler();
            }
        }

        protected virtual void initializePageHandlers()
        {
            getInputGridForm().GridForm.gridBrowser.ChangeHandler += OnGridSelectionchange;

            this.PostingToolBar.reconciliateButton.Click += OnReconciliate;
            this.PostingToolBar.resetRecoButton.Click += OnResetReconciliation;
            this.PostingToolBar.deleteButton.Click += OnDeletePostings;
        }

        private void OnGridSelectionchange()
        {
            this.PostingToolBar.displayBalance(this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems);
            int count = this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems.Count;
            this.PostingToolBar.resetRecoButton.IsEnabled = count > 0;
            this.PostingToolBar.reconciliateButton.IsEnabled = count > 0;
            this.PostingToolBar.deleteButton.IsEnabled = count > 0;
        }

        private void OnReconciliate(object sender, RoutedEventArgs e)
        {
            List<long> oids = this.getInputGridForm().GridForm.gridBrowser.GetSelectedOis();
            //foreach (object item in PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();.SelectedItems)
            //{
            //    if (item is PostingBrowserData)
            //    {
            //        PostingBrowserData data = (PostingBrowserData)item;
            //        if (!String.IsNullOrWhiteSpace(data.reconciliationNumber))
            //        {
            //            MessageDisplayer.DisplayWarning("Reconciliation", "Unable to perform reconciliation.\nAn Item in the selection is already reconciliated.");
            //            return;
            //        }
            //    }
            //}

            dialog = new PostingConfirmationDialog();
            dialog.PostingService = this.ReconciliationGridService.PostingService;
            dialog.display(this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems);
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
            if (dialog.validateEdition()) Reconciliate();
        }

        private void OnResetReconciliation(object sender, RoutedEventArgs e)
        {
            //PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();
            //MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Reset Reconciliation", "You are about to reset reconciliation for the selected items.\nDo you confirm operation?");
            //if (response != MessageBoxResult.Yes) return;
            //List<string> numbers = new List<string>(0);

            //List<int> oids = page.getInputGridForm().GridForm.gridBrowser.GetSelectedOis();

            //foreach (object item in page.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems)
            //{
            //    Object[] objects = (Object[])item;

            //    if (item is PostingBrowserData)
            //    {
            //        PostingBrowserData data = (PostingBrowserData)item;
            //        if (!String.IsNullOrWhiteSpace(data.reconciliationNumber) && !numbers.Contains(data.reconciliationNumber)) numbers.Add(data.reconciliationNumber);
            //    }
            //}
            //bool result = GetReconciliationGridService().PostingService.resetReconciliation(numbers);
            //if (result)
            //{
            //    Search();
            //    if (page.ReconciliationEndedHandler != null) page.ReconciliationEndedHandler();
            //}
        }

        private void OnDeletePostings(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Delete Postings", "You are about to delete selected postings.\nDo you confirm operation?");
            if (response != MessageBoxResult.Yes) return;
            List<long> oids = getInputGridForm().GridForm.gridBrowser.GetSelectedOis();

            //foreach (object item in page.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems)
            //{
            //    Object[] objects = (Object[]) item;
            //    if (item is PostingBrowserData)
            //    {
            //        PostingBrowserData data = (PostingBrowserData)item;
            //        if (!String.IsNullOrWhiteSpace(data.reconciliationNumber))
            //        {
            //            MessageDisplayer.DisplayWarning("Delete Postings", "Unable to delete postings.\nAn Item in the selection is reconciliated.\nReset reconciliation and try again.");
            //            return;
            //        }
            //        ids.Add(data.id);
            //    }
            //}

            bool result = ReconciliationGridService.PostingService.deletePosting(oids);
            if (result)
            {
                Search();
                if (ReconciliationEndedHandler != null) ReconciliationEndedHandler();
            }
        }

        

    }
}
