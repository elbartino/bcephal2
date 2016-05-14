using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Controller;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Initiation.Model
{
    public class ModelToolBarHandlerBuilder : Base.InitiationToolBarHandlerBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public ModelToolBarHandlerBuilder(Base.InitiationController initiationController, ModelsEditorController controller)
            : base(initiationController, controller)
        {
            
        }

        /// <summary>
        /// Controller
        /// </summary>
        public ModelsEditorController GetModelsEditorController()
        {
            return (ModelsEditorController)Controller;
        }
        
    }
}
