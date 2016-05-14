using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Application
{
    public enum EditionMode
    {
        // read only mode
        READ_ONLY,

        // read and modify
        MODIFY,

        // Create new item
        CREATE
    }
}
