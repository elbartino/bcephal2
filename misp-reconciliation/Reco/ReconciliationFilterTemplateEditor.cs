using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateEditor : Editor<ReconciliationFilterTemplate>
    {

        public ReconciliationFilterTemplateEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<ReconciliationFilterTemplate> getNewPage()
        {
            ReconciliationFilterTemplateEditorItem item = new ReconciliationFilterTemplateEditorItem(this.SubjectType);
            if (this.Service != null)
            {
                //PeriodName name = this.Service.PeriodNameService.getRootPeriodName();
                //PeriodName defaultName = name.getDefaultPeriodName();
                //item.getForm().GridForm.filterForm.periodFilter.DefaultPeriodName = defaultName;
                //item.getForm().GridForm.filterForm.periodFilter.DisplayPeriod(null);
            }
            //item.getForm().GridForm.gridBrowser.Service = this.Service;
            return item;
        }

        public Kernel.Service.GroupService GroupService { get; set; }

        public ReconciliationFilterTemplateService Service { get; set; }

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 2)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
        }

    }
}
