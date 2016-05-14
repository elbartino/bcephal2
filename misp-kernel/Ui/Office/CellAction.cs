using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
    public abstract class CellAction
    {

        public abstract void PerformAction(Cell cell, bool firstCell, bool lastCell);

    }
}
