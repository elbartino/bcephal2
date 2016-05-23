using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.User
{
    public class UserEditorItem : EditorItem<Kernel.Domain.User>
    {
          /// <summary>
        /// Contruit une nouvelle instance de UserEditorItem
        /// </summary>
        public UserEditorItem()
            : base()
        {
            this.Title = "User";
            this.CanFloat = false;
        }

        protected override IEditableView<Domain.User> getNewEditorItemForm()
        {
            return new UserForm();
        }

        /// <summary>
        /// UserForm
        /// </summary>
        public UserForm getReconciliationForm()
        {
            return (UserForm)editorItemForm;
        }
    }
}
