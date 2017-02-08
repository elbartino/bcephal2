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

        public virtual void SetTarget(Target target)
        {
            getForm().SetTarget(target);
        }

        public virtual void SetPeriod(object period)
        {
            if (period is PeriodInterval)
            {
                PeriodInterval periodInterval = (PeriodInterval)period;
                getForm().SetPeriodInterval(periodInterval);
            }
            else if (period is PeriodName)
            {
                PeriodName name = (PeriodName)period;
                getForm().SetPeriodName(name);
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
        protected override IEditableView<ReconciliationFilterTemplate> getNewEditorItemForm() { return new ReconciliationFilterTemplateForm(); }

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
