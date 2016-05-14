using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interface des vues
    /// </summary>
    public interface IView 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler);
    }
}
