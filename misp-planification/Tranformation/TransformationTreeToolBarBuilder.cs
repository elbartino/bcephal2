using Misp.Planification.Base;
using Misp.Kernel.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Misp.Kernel.Task;

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeToolBarBuilder: ToolBarHandlerBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public TransformationTreeToolBarBuilder(TransformationTreeEditorController controller) : base(controller) { }

        /// <summary>
        /// Controller
        /// </summary>
        public TransformationTreeEditorController GetTransformationTreeEditorController()
        {
            return (TransformationTreeEditorController)Controller;
        }

        BusyAction action;
        public override void onSaveButtonClic(object sender, RoutedEventArgs e)
        {
            String message = "Saving Tree";
            action = new BusyAction(false)
            {
                DoWork = () =>
                {
                    action.ReportProgress(0, message);
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        action.ReportProgress(10, message);
                        base.onSaveButtonClic(sender, e);
                    }
                    ));
                    action.ReportProgress(99, message);

                    action.ReportProgress(100, message);
                    return Kernel.Application.OperationState.CONTINUE;
                },

                EndWork = () =>
                {
                    
                    return Kernel.Application.OperationState.CONTINUE;
                },
            };
            action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(GetTransformationTreeEditorController().ApplicationManager.MainWindow.OnBusyPropertyChanged);
            action.Run();
            
        }
    }
}
