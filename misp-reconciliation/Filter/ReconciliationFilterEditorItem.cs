using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Posting;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterEditorItem : PostingGridEditorItem
    {

        public override void Search(int currentPage = 0)
        {
            try
            {
                GrilleFilter filter = getReconciliationFilterForm().ActiveBrowserForm.filterForm.Fill();
                filter.page = currentPage;
                GrillePage rows = PerformSearch(filter);
                getReconciliationFilterForm().ActiveBrowserForm.displayPage(rows);
                getReconciliationFilterForm().ActiveToolBar.displayBalance(0, 0);                
            }
            catch (ServiceExecption) { }
        }

        public void SearchAll(int currentPage = 0)
        {
            GrilleBrowserForm activeFrom = getReconciliationFilterForm().ActiveBrowserForm;
            PostingToolBar activeToolbar = getReconciliationFilterForm().ActiveToolBar;

            getReconciliationFilterForm().ActiveBrowserForm = getReconciliationFilterForm().leftGrilleBrowserForm;
            getReconciliationFilterForm().ActiveToolBar = getReconciliationFilterForm().leftPostingToolBar;
            Search(currentPage);
            getReconciliationFilterForm().ActiveBrowserForm = getReconciliationFilterForm().rigthGrilleBrowserForm;
            getReconciliationFilterForm().ActiveToolBar = getReconciliationFilterForm().rigthPostingToolBar;
            Search(currentPage);
            getReconciliationFilterForm().ActiveBrowserForm = activeFrom;
            getReconciliationFilterForm().ActiveToolBar = activeToolbar;
        }


        protected override void initializePageHandlers()
        {
            PostingToolBar = getReconciliationFilterForm().recoPostingToolBar;
            base.initializePageHandlers();
            getReconciliationFilterForm().leftGrilleBrowserForm.gridBrowser.SelectedItemChangedHandler += OnLeftGridSelectionchange;
            getReconciliationFilterForm().rigthGrilleBrowserForm.gridBrowser.SelectedItemChangedHandler += OnRigthGridSelectionchange;

            getReconciliationFilterForm().leftGrilleBrowserForm.gridBrowser.DeselectedItemChangedHandler += OnLeftGridDeselectionchange;
            getReconciliationFilterForm().rigthGrilleBrowserForm.gridBrowser.DeselectedItemChangedHandler += OnRigthGridDeselectionchange;

            this.ReconciliationEndedHandler += OnReconciliateEnded;
        }

        private void OnReconciliateEnded()
        {
            getReconciliationFilterForm().GridForm.gridBrowser.displayItems(new List<GridItem>(0));
            SearchAll();
        }

        protected void AddToRecoGrid(Object[] objects)
        {
            List<GridItem> items = new List<GridItem>(0);
            List<int> ids = new List<int>(0);
            if (getReconciliationFilterForm().GridForm.gridBrowser.grid.ItemsSource != null)
            {
                foreach (object row in getReconciliationFilterForm().GridForm.gridBrowser.grid.ItemsSource)
                {
                    if (row is GridItem)
                    {
                        items.Add((GridItem)row);
                        if (((GridItem)row).GetOid().HasValue) ids.Add(((GridItem)row).GetOid().Value);
                    }
                }
            }
            foreach (object obj in objects)
            {
                if (obj is GridItem)
                {
                    GridItem data = (GridItem)obj;
                    if (ids.Contains(data.GetOid().Value)) continue;
                    GridItem itemToAdd = new GridItem(data.Datas);
                    itemToAdd.IsSelected = true;
                    items.Add(itemToAdd);
                }
            }            
            getReconciliationFilterForm().GridForm.gridBrowser.displayItems(items);
            getReconciliationFilterForm().GridForm.gridBrowser.grid.SelectAll();
        }

        protected void AddToRecoGrid(GridItem item)
        {
            Object[] items = new Object[] { item };
            AddToRecoGrid(items);
        }

        protected void RemoveFormRecoGrid(Object[] objects)
        {
            List<GridItem> items = new List<GridItem>(0);
            List<int> ids = new List<int>(0);
            foreach (object obj in objects)
            {
                if (obj is GridItem)
                {
                    GridItem data = (GridItem)obj;
                    if (data.GetOid().HasValue) ids.Add(data.GetOid().Value);
                }
            }
            
            if (getReconciliationFilterForm().GridForm.gridBrowser.grid.ItemsSource != null)
            {
                foreach (object row in getReconciliationFilterForm().GridForm.gridBrowser.grid.ItemsSource)
                {
                    if (row is GridItem)
                    {
                        GridItem item = (GridItem)row;
                        if (!item.GetOid().HasValue || !ids.Contains(item.GetOid().Value)) items.Add((GridItem)row);
                    }
                }
            }
            getReconciliationFilterForm().GridForm.gridBrowser.displayItems(items);
            getReconciliationFilterForm().GridForm.gridBrowser.grid.SelectAll();
        }

        protected void RemoveFormRecoGrid(GridItem item)
        {
            Object[] items = new Object[] { item };
            RemoveFormRecoGrid(items);
        }

        private void OnLeftGridSelectionchange(object item)
        {
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            getReconciliationFilterForm().leftPostingToolBar.displayBalance(getReconciliationFilterForm().leftGrilleBrowserForm.gridBrowser.grid.SelectedItems, context, this.EditedObject);
            if (item is GridItem) AddToRecoGrid((GridItem)item);
            else if (item is Object[]) AddToRecoGrid((Object[])item); //item.GetType()//typeof(item);
        }

        private void OnLeftGridDeselectionchange(object item)
        {
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            getReconciliationFilterForm().leftPostingToolBar.displayBalance(getReconciliationFilterForm().leftGrilleBrowserForm.gridBrowser.grid.SelectedItems, context, this.EditedObject);
            if (item is GridItem) RemoveFormRecoGrid((GridItem)item);
            else if (item is Object[]) RemoveFormRecoGrid((Object[])item);
        }

        private void OnRigthGridSelectionchange(object item)
        {
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            getReconciliationFilterForm().rigthPostingToolBar.displayBalance(getReconciliationFilterForm().rigthGrilleBrowserForm.gridBrowser.grid.SelectedItems, context, this.EditedObject);
            if (item is GridItem) AddToRecoGrid((GridItem)item);
            else if (item is Object[]) AddToRecoGrid((Object[])item);
        }

        private void OnRigthGridDeselectionchange(object item)
        {
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            getReconciliationFilterForm().rigthPostingToolBar.displayBalance(getReconciliationFilterForm().rigthGrilleBrowserForm.gridBrowser.grid.SelectedItems, context, this.EditedObject);
            if (item is GridItem) RemoveFormRecoGrid((GridItem)item);
            else if (item is Object[]) RemoveFormRecoGrid((Object[])item);
        }

        protected override void OnGridSelectionchange()
        {
            PostingToolBar = getReconciliationFilterForm().recoPostingToolBar;
            Kernel.Domain.ReconciliationContext context = this.PostingGridService.ReconciliationContextService.getReconciliationContext();
            PostingToolBar.displayBalance(getReconciliationFilterForm().GridForm.gridBrowser.grid.SelectedItems, context, this.EditedObject);

            int count = getReconciliationFilterForm().GridForm.gridBrowser.grid.SelectedItems.Count;
            PostingToolBar.resetRecoButton.IsEnabled = count > 0;
            PostingToolBar.reconciliateButton.IsEnabled = count > 0;
            PostingToolBar.deleteButton.IsEnabled = count > 0;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Grille> getNewEditorItemForm()
        {
            ReconciliationFilterForm form = new ReconciliationFilterForm();
            PostingToolBar = form.recoPostingToolBar;
            return new ReconciliationFilterForm();
        }

        public virtual ReconciliationFilterForm getReconciliationFilterForm()
        {
            return (ReconciliationFilterForm)editorItemForm;
        }

    }
}
