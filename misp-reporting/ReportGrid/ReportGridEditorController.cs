using Misp.Kernel.Domain;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reporting.ReportGrid
{
    public class ReportGridEditorController : InputGridEditorController
    {

        public ReportGridEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        protected override Grille GetNewGrid()
        {
            Grille grid = new Grille();
            grid.report = true;
            grid.name = getNewPageName("Report Grid");
            grid.group = GetInputGridService().GroupService.getDefaultGroup();
            grid.visibleInShortcut = true;
            return grid;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() 
        {
            InputGridToolBar toolbar = new InputGridToolBar();
            toolbar.LoadButton.Visibility = System.Windows.Visibility.Collapsed;
            toolbar.ClearButton.Visibility = System.Windows.Visibility.Collapsed;
            return toolbar; 
        }

        protected override void initializeSideBarData()
        {
            base.initializeSideBarData();
            ((InputGridSideBar)SideBar).MeasureGroup.InitializeTreeViewDatas(true);
        }

    }
}
