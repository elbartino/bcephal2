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

    }
}
