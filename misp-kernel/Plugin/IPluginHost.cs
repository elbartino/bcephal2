using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Plugin
{
    public interface IPluginHost
    {

        bool Register(IPlugin plugin);

    }
}
