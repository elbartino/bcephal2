using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeEditorItem : EditorItem<Kernel.Domain.CombinedTransformationTree>
    {

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Kernel.Domain.CombinedTransformationTree> getNewEditorItemForm()
        {
            return new CombinedTransformationTreeForm();
        }

        public virtual CombinedTransformationTreeForm getCombineTransformationTreeForm()
        {
            return (CombinedTransformationTreeForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {

        }


        public Kernel.Service.GroupService GroupService { get; set; }

    }
}

