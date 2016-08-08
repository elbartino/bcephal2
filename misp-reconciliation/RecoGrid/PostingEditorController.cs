using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Reporting.ReportGrid
{
    public class PostingEditorController : ReportGridEditorController
    {

        public PostingEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        public override OperationState Create()
        {
            OperationState state = base.Create();
            Search(1);
            return state;
        }

        public override void Search(int currentPage = 0)
        {
            try
            {
                PostingEditorItem page = (PostingEditorItem)getEditor().getActivePage();
                page.Search(currentPage);
                OnChange();
            }
            catch (ServiceExecption e) { }
        }

        public override OperationState OnChange()
        {
            EditorItem<Grille> page = getEditor().getActivePage();
            if (page != null) page.IsModify = false;
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView()
        {
            PostingEditor editor = new PostingEditor();
            editor.Service = GetReconciliationGridService();            
            return editor;
        }

        protected override Kernel.Ui.Base.ToolBar getNewToolBar() 
        {
            InputGridToolBar toolBar = new InputGridToolBar();
            toolBar.SaveButton.Visibility = Visibility.Collapsed;
            return toolBar;
        }

        protected override Grille GetNewGrid()
        {
            ReconciliationGrid grid = GetReconciliationGridService().getNewReconciliationGrid("Postings");
            grid.name = "Postings";
            return grid;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux InputGrids.
        /// </summary>
        /// <returns>InputGridService</returns>
        public ReconciliationGridService GetReconciliationGridService()
        {
            return (ReconciliationGridService)base.Service;
        }



    }
}
