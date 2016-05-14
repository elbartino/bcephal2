using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Application;

namespace Misp.Kernel.Controller
{
    public abstract class EditorItemController<T> : Controller<T, Misp.Kernel.Domain.Browser.BrowserData>
        where T : Domain.Persistent
    {

        public override OperationState Open()
        {
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public virtual EditorItem<T> getEditorItem()
        {
            return (EditorItem<T>)this.View;
        }

        /// <summary>
        /// Sauve l'objet en cours d'édition sur la page active.
        /// </summary>
        /// <returns></returns>
        public override OperationState Save()
        {
            if (getEditorItem() == null) return OperationState.CONTINUE;
            if (!getEditorItem().validateEdition()) return OperationState.STOP;
            getEditorItem().fillObject();
            T editedObject = getEditorItem().EditedObject;
            try
            {
                editedObject = Service.Save(editedObject);
                getEditorItem().EditedObject = editedObject;
                getEditorItem().IsModify = false;
                getEditorItem().displayObject();
                return OperationState.CONTINUE;
            }
            catch (Domain.BcephalException e)
            {
                Util.MessageDisplayer.DisplayError("Error", e.Message);
            }
            return OperationState.STOP;
        }

        /// <summary>
        /// Sauve les objets en cours d'édition sur toutes les pages ouvertes.
        /// </summary>
        /// <returns></returns>
        public override OperationState SaveAll()
        {
            return Save();
        }

        
    }
}
