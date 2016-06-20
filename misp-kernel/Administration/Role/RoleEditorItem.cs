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
using Misp.Kernel.Administration.Role;

namespace Misp.Kernel.Administration.Role
{
    public class RoleEditorItem : EditorItem<Misp.Kernel.Domain.Role>
    {

        /// <summary>
        /// Contruit une nouvelle instance de RoleEditorItem
        /// </summary>
        public RoleEditorItem() : base()
        {
            this.Title = "Roles";
            this.CanClose = false;
            this.CanFloat = false;
        }

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected override IEditableView<Misp.Kernel.Domain.Role> getNewEditorItemForm()
        {
            return new RoleForm();
        }

        public RoleForm getMeasureForm()
        {
            return (RoleForm)editorItemForm;
        }

        public override void displayObject()
        {
            RemoveChangeHandlers();
            getMeasureForm().RoleTree.DisplayRoot(this.EditedObject);
            AddChangeHandlers();
        }

    }
}
