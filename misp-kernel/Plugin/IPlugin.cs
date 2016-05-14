using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Application;


namespace Misp.Kernel.Plugin
{
    public interface IPlugin : IComparable
    {

        /// <summary>
        /// Plugin Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Plug in host
        /// </summary>
        IPluginHost Host { get; set; }

        /// <summary>
        /// ControllerFactory
        /// </summary>
        ControllerFactory ControllerFactory { get; set; }

        /// <summary>
        /// Menu
        /// </summary>
        List<ApplicationMenu> Menus { get; set; }

        /// <summary>
        /// Side bar RadioButton
        /// </summary>
        RadioButton RadioButton { get; set; }
    }
}
