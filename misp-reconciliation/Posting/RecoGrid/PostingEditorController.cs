using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Reconciliation.Posting;
using Misp.Reporting.ReportGrid;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reconciliation.RecoGrid
{
    public class PostingEditorController : PostingGridEditorController
    {
        
        public override OperationState Create()
        {
            OperationState state = base.Create();
            Search(1);
            return state;
        }
        
        public override OperationState OnChange()
        {
            base.OnChange();
            EditorItem<Grille> page = getEditor().getActivePage();
            if (page != null) page.IsModify = false;
            this.IsModify = false;
            return OperationState.CONTINUE;
        }
        
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() 
        {
            InputGridToolBar toolBar = new InputGridToolBar();
            toolBar.SaveButton.Visibility = Visibility.Collapsed;
            toolBar.LoadButton.Visibility = Visibility.Collapsed;
            toolBar.ClearButton.Visibility = Visibility.Collapsed;
            return toolBar;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            PostingEditor editor = new PostingEditor();
            editor.Service = GetPostingGridService();
            return editor;
        }

        protected override Grille GetNewGrid()
        {
            Grille grid = GetPostingGridService().getNewReconciliationGrid("Postings");
            grid.name = "Postings";
            grid.columnListChangeHandler.Items = new ObservableCollection<GrilleColumn>(grid.columnListChangeHandler.newItems); 
            return grid;
        }
        
    }
}
