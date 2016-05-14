using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.CustomizedTarget
{
    public class TargetEditorItem : EditorItem<Target>
    {

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Target> getNewEditorItemForm()
        {

            return new TargetForm();
        }

        public virtual TargetForm getTargetForm()
        {
            return (TargetForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {

        }


        public Kernel.Service.GroupService GroupService { get; set; }

    }
}

