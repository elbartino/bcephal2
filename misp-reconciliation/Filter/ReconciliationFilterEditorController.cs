using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Posting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Filter
{
    public class ReconciliationFilterEditorController : PostingGridEditorController
    {

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputGrids.
        /// </summary>
        /// <returns>InputGridService</returns>
        public ReconciliationFilterService GetReconciliationFilterService()
        {
            return (ReconciliationFilterService)base.Service;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            ReconciliationFilterEditor editor = new ReconciliationFilterEditor();
            editor.Service = GetReconciliationFilterService();
            return editor;
        }

        protected override Grille GetNewGrid()
        {
            Grille grid = GetReconciliationFilterService().getNewReconciliationGrid("Filter");
            grid.columnListChangeHandler.Items = new ObservableCollection<GrilleColumn>(grid.columnListChangeHandler.newItems); 
            return grid;
        }

    }
}
