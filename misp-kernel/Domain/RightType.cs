using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public enum RightType
    {
        
        VIEW,
        EDIT,
        CREATE,
        EDIT_CELL,
        EDIT_ALLOCATION,
        DELETE,
        LOAD,
        CLEAR,
        SAVE_AS,
        SAVE,
        EDIT_EXCEL,
        EDIT_WRITE_OFF,
        RESET_RECONCILIATION
    }
}
