using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.UserProfile
{
    public class UserProfileEditorItem : EditorItem<Kernel.Domain.User>
    {
          /// <summary>
        /// Contruit une nouvelle instance de UserEditorItem
        /// </summary>
        public UserProfileEditorItem()
            : base()
        {
            this.Title = "User";
            this.CanFloat = false;
        }

        protected override IEditableView<Domain.User> getNewEditorItemForm()
        {
            return new UserProfileForm();
        }

        /// <summary>
        /// UserForm
        /// </summary>
        public UserProfileForm getUserProfileForm()
        {
            return (UserProfileForm)editorItemForm;
        }
    }
}
