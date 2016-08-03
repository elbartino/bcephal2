using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reporting.ReportGrid
{
    public class PostingEditorController : ReportGridEditorController
    {

        public PostingEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            PostingEditor editor = new PostingEditor();
            editor.Service = GetReconciliationGridService();            
            return editor;
        }

        protected override Grille GetNewGrid()
        {
            ReconciliationGrid grid = GetReconciliationGridService().getNewReconciliationGrid("Postings");
            //grid.report = true;
            //grid.name =  getNewPageName("Postings");
            //grid.group = GetReconciliationGridService().GroupService.getDefaultGroup();
            //grid.visibleInShortcut = true;
            return grid;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputGrids.
        /// </summary>
        /// <returns>InputGridService</returns>
        public ReconciliationGridService GetReconciliationGridService()
        {
            return (ReconciliationGridService)base.Service;
        }

        //protected override void initializePageHandlers(EditorItem<Grille> page)
        //{
        //    base.initializePageHandlers(page);
        //    PostingEditorItem editorPage = (PostingEditorItem)page;
        //    editorPage.getInputGridForm().GridForm.gridBrowser.ChangeHandler += OnGridSelectionchange;

        //    editorPage.PostingToolBar.reconciliateButton.Click += OnReconciliate;
        //    editorPage.PostingToolBar.resetRecoButton.Click += OnResetReconciliation;
        //    editorPage.PostingToolBar.deleteButton.Click += OnDeletePostings;
        //}

        //private void OnGridSelectionchange()
        //{
        //    PostingEditorItem page = (PostingEditorItem) getEditor().getActivePage();
        //    page.PostingToolBar.displayBalance(page.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems);
        //    int count = page.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems.Count;
        //    page.PostingToolBar.resetRecoButton.IsEnabled = count > 0;
        //    page.PostingToolBar.reconciliateButton.IsEnabled = count > 0;
        //    page.PostingToolBar.deleteButton.IsEnabled = count > 0;
        //}

        //private void OnResetReconciliation(object sender, RoutedEventArgs e)
        //{
        //    PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();
        //    MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Reset Reconciliation", "You are about to reset reconciliation for the selected items.\nDo you confirm operation?");
        //    if (response != MessageBoxResult.Yes) return;
        //    List<string> numbers = new List<string>(0);

        //    List<int> oids = page.getInputGridForm().GridForm.gridBrowser.GetSelectedOis();

        //    foreach (object item in page.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems)
        //    {
        //        Object[] objects = (Object[]) item;

        //        if (item is PostingBrowserData)
        //        {
        //            PostingBrowserData data = (PostingBrowserData)item;
        //            if (!String.IsNullOrWhiteSpace(data.reconciliationNumber) && !numbers.Contains(data.reconciliationNumber)) numbers.Add(data.reconciliationNumber);
        //        }
        //    }
        //    bool result = GetReconciliationGridService().PostingService.resetReconciliation(numbers);
        //    if (result)
        //    {
        //        Search();
        //        if (page.ReconciliationEndedHandler != null) page.ReconciliationEndedHandler();
        //    }
        //}

        //private void OnDeletePostings(object sender, RoutedEventArgs e)
        //{
        //    PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();
        //    MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Delete Postings", "You are about to delete selected postings.\nDo you confirm operation?");
        //    if (response != MessageBoxResult.Yes) return;
        //    List<long> ids = new List<long>(0);
        //    List<long> oids = page.getInputGridForm().GridForm.gridBrowser.GetSelectedOis();

        //    //foreach (object item in page.getInputGridForm().GridForm.gridBrowser.grid.SelectedItems)
        //    //{
        //    //    Object[] objects = (Object[]) item;
        //    //    if (item is PostingBrowserData)
        //    //    {
        //    //        PostingBrowserData data = (PostingBrowserData)item;
        //    //        if (!String.IsNullOrWhiteSpace(data.reconciliationNumber))
        //    //        {
        //    //            MessageDisplayer.DisplayWarning("Delete Postings", "Unable to delete postings.\nAn Item in the selection is reconciliated.\nReset reconciliation and try again.");
        //    //            return;
        //    //        }
        //    //        ids.Add(data.id);
        //    //    }
        //    //}
        //    bool result = GetReconciliationGridService().PostingService.deletePosting(oids);
        //    if (result)
        //    {
        //        Search();
        //        if (page.ReconciliationEndedHandler != null) page.ReconciliationEndedHandler();
        //    }
        //}

        //private void OnReconciliate(object sender, RoutedEventArgs e)
        //{
        //    PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();
        //    List<long> oids = page.getInputGridForm().GridForm.gridBrowser.GetSelectedOis();
        //    //foreach (object item in PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();.SelectedItems)
        //    //{
        //    //    if (item is PostingBrowserData)
        //    //    {
        //    //        PostingBrowserData data = (PostingBrowserData)item;
        //    //        if (!String.IsNullOrWhiteSpace(data.reconciliationNumber))
        //    //        {
        //    //            MessageDisplayer.DisplayWarning("Reconciliation", "Unable to perform reconciliation.\nAn Item in the selection is already reconciliated.");
        //    //            return;
        //    //        }
        //    //    }
        //    //}

        //    //dialog = new PostingConfirmationDialog();
        //    //dialog.PostingService = this.PostingService;
        //    //dialog.display(grid.SelectedItems);
        //    //dialog.yesButton.Click += OnConfirmReconciliation;
        //    //dialog.noButton.Click += OnCancelReconciliation;
        //    //dialog.ShowDialog();
        //}


    }
}
