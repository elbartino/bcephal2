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



namespace Misp.Initiation.Model
{
    public class ModelEditorItem : EditorItem<Misp.Kernel.Domain.Model>
    {
        
        /// <summary>
        /// Contruit une nouvelle instance de PeriodicityEditorItem
        /// </summary>
        public ModelEditorItem(Kernel.Domain.SubjectType subjectType)
            : base(subjectType)
        {
            this.Title = "Model";
            this.CanClose = false;
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Misp.Kernel.Domain.Model> getNewEditorItemForm()
        {
            return new ModelForm(this.SubjectType);
        }

        /// <summary>
        /// ModelForm
        /// </summary>
        public ModelForm GetModelForm()
        {
            return (ModelForm)editorItemForm;
        }
        
        
        
       
    }
}
