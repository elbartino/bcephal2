using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridEditorItem : EditorItem<Grille>
    {

        public InputGridEditorItem(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        public virtual void SetTarget(Target target)
        {
            if (getInputGridForm().SelectedIndex == 1 && target is Kernel.Domain.Attribute)
                getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SetValue(target);
            else if (getInputGridForm().SelectedIndex != 1 && !(target is Kernel.Domain.Attribute))
                getInputGridForm().SetTarget((Target)target);
            else
            {
                getInputGridForm().SetTarget((Target)target);
            }
        }

        public virtual void SetPeriod(object period)
        {
            if (getInputGridForm().SelectedIndex == 1)
                getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SetValue(period);
            else
            {
                if (period is PeriodInterval)
                {
                    PeriodInterval periodInterval = (PeriodInterval)period;
                    getInputGridForm().SetPeriodInterval(periodInterval);
                }
                else if (period is PeriodName)
                {
                    getInputGridForm().SetPeriodItemName(((PeriodName)period).name);
                }
            }
        }

        public virtual void SetMeasure(Measure mesure)
        {
            if (getInputGridForm().SelectedIndex == 1)
                getInputGridForm().InputGridSheetForm.InputGridPropertiesPanel.SetValue(mesure);
        }


        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Grille> getNewEditorItemForm() { return new InputGridForm(this.SubjectType); }

        public virtual InputGridForm getInputGridForm()
        {
            return (InputGridForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args) { }

        public Kernel.Service.GroupService GroupService { get; set; }

    }
}
