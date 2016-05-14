using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Cette classe représente une page de l'éditeur d'InputTable.
    /// On peut ouvrir plusieurs pages à la fois; une page correspondant à une seule InputTable.
    /// </summary>
    public class InputTableEditorItem : EditorItem<InputTable>
    {
        public string DEFAULT_NAME;

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<InputTable> getNewEditorItemForm()
        {
            this.CanFloat = false;
          
            return new InputTableForm();
        }

        public virtual InputTableForm getInputTableForm()
        {
            return (InputTableForm)editorItemForm;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
        {

        }

        public GroupProperty groupProperty { get; set; }

        public bool isImported { get; set; }

        public Kernel.Service.GroupService GroupService { get; set; }

    }
}
