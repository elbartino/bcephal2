﻿using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Reconciliation.RecoGrid;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.GridViews;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reconciliation.Posting
{
    public class PostingGridEditorItem : InputGridEditorItem
    {

        public PostingToolBar PostingToolBar { get; set; }

        public Kernel.Ui.Base.ChangeEventHandler ReconciliationEndedHandler;

        ReconciliationDialog dialog;

        public PostingGridService PostingGridService { get; set; }

        public PostingGridEditorItem() : base()
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


        public virtual void Search(int currentPage = 0)
        {
            try
            {
                GrilleFilter filter = this.getInputGridForm().GridForm.filterForm.Fill();                
                filter.page = currentPage;
                GrillePage rows = PerformSearch(filter);
                this.getInputGridForm().GridForm.displayPage(rows);
                this.PostingToolBar.displayBalance(0, 0);
            }
            catch (ServiceExecption) { }
        }

        public virtual GrillePage PerformSearch(GrilleFilter filter)
        {
            try
            {
                filter.grid = new Grille();
                filter.grid.code = this.EditedObject.code;
                filter.grid.columnListChangeHandler.originalList = this.EditedObject.columnListChangeHandler.Items.ToList();
                filter.grid.report = this.EditedObject.report;
                filter.grid.oid = this.EditedObject.oid;
                filter.grid.name = this.EditedObject.name;
                filter.grid.reconciliation = true;
                filter.grid.report = this.EditedObject.report;
                filter.pageSize = (int)this.getInputGridForm().GridForm.toolBar.pageSizeComboBox.SelectedItem;
                GrillePage rows = this.PostingGridService.getGridRows(filter);
                return rows;
            }
            catch (ServiceExecption) { }
            return null;
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

            bool result = PostingGridService.PostingService.reconciliate(reco);
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

            getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.CanRemoveColumn += OnTryToRemoveColumns;
        }

        private bool OnTryToRemoveColumns(object columns)
        {
            bool response = true;
            if (columns != null && columns is IList)
            {
                
                int count = 0;
                String values = "";
                String coma = "";
                Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
                foreach (Object item in (IList)columns)
                {
                    if (item is GrilleColumn)
                    {
                        GrilleColumn column = (GrilleColumn)item;
                        if (context.isDefaultColumn(column))
                        {
                            response = false;
                            values += coma + column.name;
                            coma = " ; ";
                            count++;
                        }
                    }
                }
                if (!response)
                {
                    String message = "You are not allowed to remove default column" + (count > 1 ? "s" : "") + " : " + values;
                    MessageDisplayer.DisplayWarning("Unable to remove column", message);
                }
            }
            return response;
        }
        
        private void OnGridSelectionchange()
        {
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            this.PostingToolBar.displayBalance(this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems, context, this.EditedObject);

            int count = this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems.Count;
            this.PostingToolBar.resetRecoButton.IsEnabled = count > 0;
            this.PostingToolBar.reconciliateButton.IsEnabled = count > 0;
            this.PostingToolBar.deleteButton.IsEnabled = count > 0;
        }

        private void OnReconciliate(object sender, RoutedEventArgs e)
        {
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            IList items = this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems;
            int position = this.EditedObject.GetRecoNbrColumn(context).position;
            foreach (object item in items)
            {
                if (item is GridItem)
                {
                    object reco = ((GridItem)item).Datas[position];
                    if (reco != null && !String.IsNullOrWhiteSpace(reco.ToString()))
                    {
                        MessageDisplayer.DisplayWarning("Reconciliation", "Unable to perform reconciliation.\nAn Item in the selection is already reconciliated.");
                        return;
                    }
                }
            }
            dialog = new ReconciliationDialog();
            dialog.PostingGridService = this.PostingGridService;
            dialog.Context = context;
            dialog.display(items, EditedObject);
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
            MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Reset Reconciliation", "You are about to reset reconciliation.\nDo you confirm operation?");
            if (response != MessageBoxResult.Yes) return;
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            IList items = this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems;
            int position = this.EditedObject.GetRecoNbrColumn(context).position;
            List<string> numbers = new List<string>(0);
            foreach (object item in items)
            {
                if (item is GridItem)
                {
                    object reco = ((GridItem)item).Datas[position];
                    if (reco != null && !numbers.Contains(reco.ToString())) numbers.Add(reco.ToString());
                }
            }
            bool result = this.PostingGridService.PostingService.resetReconciliation(numbers);
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
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            IList items = this.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems;
            int position = this.EditedObject.GetRecoNbrColumn(context).position;
            foreach (object item in items)
            {
                if (item is GridItem)
                {
                    object reco = ((GridItem)item).Datas[position];
                    if (reco != null && !String.IsNullOrWhiteSpace(reco.ToString()))
                    {
                        MessageDisplayer.DisplayWarning("Delete Postings", "Unable to delete postings.\nAn Item in the selection is reconciliated.\nReset reconciliation and try again.");
                        return;
                    }
                }
            }            
            List<long> oids = getInputGridForm().GridForm.gridBrowser.GetSelectedOis();
            bool result = PostingGridService.PostingService.deletePosting(oids);
            if (result)
            {
                Search();
                if (ReconciliationEndedHandler != null) ReconciliationEndedHandler();
            }
        }

        

    }
}
