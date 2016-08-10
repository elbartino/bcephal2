using Misp.Kernel.Domain;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Posting
{
    public class PostingGridEditorController : InputGridEditorController
    {

        protected override Grille GetNewGrid()
        {
            Grille grid = new Grille();
            grid.report = false;
            grid.reconciliation = true;
            grid.name = getNewPageName("Posting Grid");
            grid.group = GetInputGridService().GroupService.getDefaultGroup();
            grid.visibleInShortcut = true;            
            return grid;
        }

    }
}
