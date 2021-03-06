﻿using Misp.Kernel.Ui.Base;
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
        public UserEditorItem(Domain.SubjectType subjectType)
            : base(subjectType)
        {
            this.Title = "User";
            this.CanFloat = false;
            CanRename = true;
            CanSave = true;
        }

        protected override IEditableView<Domain.User> getNewEditorItemForm()
        {
            return new UserForm(this.SubjectType);
        }

        /// <summary>
        /// UserForm
        /// </summary>
        public UserForm getUserForm()
        {
            return (UserForm)editorItemForm;
        }
    }
}
