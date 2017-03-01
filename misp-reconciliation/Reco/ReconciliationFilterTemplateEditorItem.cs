using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateEditorItem : EditorItem<ReconciliationFilterTemplate>
    {

        public ReconciliationFilterTemplateEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        public virtual void SetTarget(Target target, bool setToFilter = false)
        {
            getForm().SetTarget(target, setToFilter);
        }

        public virtual void SetPeriod(object period, bool setToFilter = false)
        {
            if (period is PeriodInterval)
            {
                PeriodInterval periodInterval = (PeriodInterval)period;
                getForm().SetPeriodInterval(periodInterval, setToFilter);
            }
            else if (period is PeriodName)
            {
                PeriodName name = (PeriodName)period;
                getForm().SetPeriodName(name, setToFilter);
            }
        }

        public virtual void SetMeasure(Measure mesure)
        {
            getForm().SetMeasure(mesure);
        }


        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<ReconciliationFilterTemplate> getNewEditorItemForm() { return new ReconciliationFilterTemplateForm(this.SubjectType); }

        public virtual ReconciliationFilterTemplateForm getForm()
        {
            return (ReconciliationFilterTemplateForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args) { }

        public Kernel.Service.GroupService GroupService { get; set; }


        public void SearchAll(int currentPage = 0)
        {
            /*GrilleBrowserForm activeFrom = getReconciliationFilterForm().ActiveBrowserForm;
            PostingToolBar activeToolbar = getReconciliationFilterForm().ActiveToolBar;

            getReconciliationFilterForm().ActiveBrowserForm = getReconciliationFilterForm().leftGrilleBrowserForm;
            getReconciliationFilterForm().ActiveToolBar = getReconciliationFilterForm().leftPostingToolBar;
            Search(currentPage);
            getReconciliationFilterForm().ActiveBrowserForm = getReconciliationFilterForm().rigthGrilleBrowserForm;
            getReconciliationFilterForm().ActiveToolBar = getReconciliationFilterForm().rigthPostingToolBar;
            Search(currentPage);
            getReconciliationFilterForm().ActiveBrowserForm = activeFrom;
            getReconciliationFilterForm().ActiveToolBar = activeToolbar;*/
        }

    }
}
