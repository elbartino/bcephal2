using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Posting;
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
                //getReconciliationFilterForm().ActiveBrowserForm.PostingToolBar.displayBalance(0, 0);                
            }
            catch (ServiceExecption) { }
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Grille> getNewEditorItemForm()
        {
            PostingToolBar = new PostingToolBar();
            ReconciliationFilterForm form = new ReconciliationFilterForm();
            form.GridForm.filterForm.RecoPanel.Visibility = Visibility.Visible;
            form.GridForm.otherToolBarPanel.Children.Add(PostingToolBar);
            form.GridForm.otherToolBarPanel.Visibility = Visibility.Visible;
            return new ReconciliationFilterForm();
        }

        public virtual ReconciliationFilterForm getReconciliationFilterForm()
        {
            return (ReconciliationFilterForm)editorItemForm;
        }

    }
}
