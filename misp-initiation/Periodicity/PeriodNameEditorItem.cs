using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Initiation.Periodicity
{
    /// <summary>
    /// La vue d'édition de la Periodicité.
    /// </summary>
    public class PeriodNameEditorItem : EditorItem<Misp.Kernel.Domain.PeriodName>
    {
        
        /// <summary>
        /// Contruit une nouvelle instance de PeriodicityEditorItem
        /// </summary>
        public PeriodNameEditorItem() : base()
        {
            this.Title = "Period";
            this.CanClose = false;
            this.CanFloat = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PeriodNameForm getPeriodicityForm()
        {
            return (PeriodNameForm)editorItemForm;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Misp.Kernel.Domain.PeriodName> getNewEditorItemForm()
        {
            return new PeriodNameForm();
        }

        public override void displayObject()
        {
            RemoveChangeHandlers();
            getPeriodicityForm().EditedObject = this.EditedObject;
            getPeriodicityForm().displayObject();
            AddChangeHandlers();
        }

        protected override void AddChangeHandlers()
        {
            base.AddChangeHandlers();
            if (this.ChangeEventHandler != null) getPeriodicityForm().Changed += this.ChangeEventHandler.ChangeEventHandler;
        }

        protected override void RemoveChangeHandlers()
        {
            base.AddChangeHandlers();
            if (this.ChangeEventHandler != null) getPeriodicityForm().Changed -= this.ChangeEventHandler.ChangeEventHandler;
        }


    }
}
