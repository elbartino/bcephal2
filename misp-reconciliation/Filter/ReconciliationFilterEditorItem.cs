using Misp.Kernel.Domain;
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
            return new ReconciliationFilterForm();
        }

    }
}
