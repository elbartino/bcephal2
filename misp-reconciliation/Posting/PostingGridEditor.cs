using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class PostingGridEditor : InputGridEditor
    {

        /// <summary>
        /// Retourne une nouvelle page.Posting
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Grille> getNewPage() 
        {
            PostingGridEditorItem item = new PostingGridEditorItem();
            if (this.Service != null)
            {
                PeriodName name = this.Service.PeriodNameService.getRootPeriodName();
                PeriodName defaultName = name.getDefaultPeriodName();
                item.getInputGridForm().GridForm.filterForm.periodFilter.DefaultPeriodName = defaultName;
                item.getInputGridForm().GridForm.filterForm.periodFilter.DisplayPeriod(null);
            }
            item.getInputGridForm().GridForm.gridBrowser.Service = this.Service;
            item.PostingGridService = (PostingGridService)this.Service;
            return item;
        }

        
    }
}
