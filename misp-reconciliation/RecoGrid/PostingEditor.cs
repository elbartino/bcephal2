using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.InputGrid
{
    public class PostingEditor : InputGridEditor
    {

        /// <summary>
        /// Retourne une nouvelle page.Posting
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Grille> getNewPage() 
        {
            PostingEditorItem item = new PostingEditorItem();
            item.getInputGridForm().GridForm.gridBrowser.Service = this.Service;
            item.ReconciliationGridService = (ReconciliationGridService)this.Service;
            return item;
        }

    }
}
