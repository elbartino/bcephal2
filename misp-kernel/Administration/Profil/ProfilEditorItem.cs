using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.Profil
{
    public class ProfilEditorItem : EditorItem<Kernel.Domain.Profil>
    {
          /// <summary>
        /// Contruit une nouvelle instance de UserEditorItem
        /// </summary>
        public ProfilEditorItem()
            : base()
        {
            this.Title = "Profil";
            this.CanFloat = false;
        }

        protected override IEditableView<Domain.Profil> getNewEditorItemForm()
        {
            return new ProfilForm();
        }

        /// <summary>
        /// UserForm
        /// </summary>
        public ProfilForm getProfilForm()
        {
            return (ProfilForm)editorItemForm;
        }
    }
}
