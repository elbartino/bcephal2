using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;

namespace Misp.Initiation.Measure
{
    public class MeasureToolBarHandlerBuilder : Base.InitiationToolBarHandlerBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public MeasureToolBarHandlerBuilder(Base.InitiationController initiationController, MeasureEditorController controller)
            : base(initiationController, controller)
        {
            
        }
    }
}
