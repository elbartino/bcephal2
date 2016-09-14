using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Posting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterEditor : PostingGridEditor
    {

        /// <summary>
        /// Retourne une nouvelle page.Posting
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Grille> getNewPage()
        {                       
            ReconciliationFilterEditorItem item = new ReconciliationFilterEditorItem();
            if (this.Service != null)
            {
                PeriodName name = this.Service.PeriodNameService.getRootPeriodName();
                PeriodName defaultName = name.getDefaultPeriodName();
                item.getReconciliationFilterForm().GridForm.filterForm.periodFilter.DefaultPeriodName = defaultName;
                item.getReconciliationFilterForm().GridForm.filterForm.periodFilter.DisplayPeriod(null);
                item.getReconciliationFilterForm().leftGrilleBrowserForm.filterForm.periodFilter.DefaultPeriodName = defaultName;
                item.getReconciliationFilterForm().leftGrilleBrowserForm.filterForm.periodFilter.DisplayPeriod(null);
                item.getReconciliationFilterForm().rigthGrilleBrowserForm.filterForm.periodFilter.DefaultPeriodName = defaultName;
                item.getReconciliationFilterForm().rigthGrilleBrowserForm.filterForm.periodFilter.DisplayPeriod(null);
            }
            item.getInputGridForm().GridForm.gridBrowser.Service = this.Service;
            item.PostingGridService = (PostingGridService)this.Service;
            return item;
        }

    }
}
