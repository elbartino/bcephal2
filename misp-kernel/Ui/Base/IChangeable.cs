using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public interface IChangeable
    {

        event ChangeEventHandler Changed;

    }
}
