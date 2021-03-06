﻿using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Administration.Role
{
   public  class RoleEditorItem : EditorItem <Domain.Role>
    {


       public RoleEditorItem(Domain.SubjectType subjectType) : base(subjectType) 
       {
           this.Title = "Role";
           this.CanFloat = false;
           CanRename = true;
           CanSave = true;
       }

         

       /// <summary>
       /// UNe nouvelle instance de la form.
       /// </summary>
       /// <returns></returns>
       protected override IEditableView<Domain.Role> getNewEditorItemForm()
       {
          return new RoleForm(this.SubjectType);
       }

       public virtual RoleForm getRoleForm()
       {
           return (RoleForm)editorItemForm;
       }

       protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
       {

       }


       public Kernel.Service.GroupService GroupService { get; set; }
    }
}
