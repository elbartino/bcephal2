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

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Grille> getNewEditorItemForm() { return new InputGridForm(); }

        public virtual InputGridForm getInputGridForm()
        {
            return (InputGridForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args) { }

        public Kernel.Service.GroupService GroupService { get; set; }

    }
}
