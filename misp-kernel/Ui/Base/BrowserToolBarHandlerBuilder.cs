using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;
using System.Windows;

namespace Misp.Kernel.Ui.Base
{
    public class BrowserToolBarHandlerBuilder : ToolBarHandlerBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public BrowserToolBarHandlerBuilder(Controllable browserController) : base(browserController)
        { }
        
    }
}
