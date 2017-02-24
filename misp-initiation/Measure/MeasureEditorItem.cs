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

namespace Misp.Initiation.Measure
{
    public class MeasureEditorItem : EditorItem<Misp.Kernel.Domain.Measure>
    {

        /// <summary>
        /// Contruit une nouvelle instance de MeasureEditorItem
        /// </summary>
        public MeasureEditorItem(Kernel.Domain.SubjectType subjectType)
            : base(subjectType)
        {
            this.Title = "Measures";
            this.CanClose = false;
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Misp.Kernel.Domain.Measure> getNewEditorItemForm()
        {
            return new MeasureForm(this.SubjectType);
        }

        public MeasureForm getMeasureForm()
        {
            return (MeasureForm)editorItemForm;
        }

        public override void displayObject()
        {
            RemoveChangeHandlers(); 
            getMeasureForm().MeasureTree.DisplayMeasure(this.EditedObject);
            AddChangeHandlers();
        }

    }
}
