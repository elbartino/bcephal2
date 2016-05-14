using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;
using Misp.Kernel.Util;
using System.Windows.Controls;

namespace Misp.Kernel.Task
{
    public class Worker
    {
        protected String message;
        protected Window owner;
        protected MaskDialog maskDialog;

        public delegate void Work();
        public event Work OnWork;

        public delegate void WorkWithParameter(object parameters);
        public event WorkWithParameter OnWorkWithParameter;

        double h = ((Grid)Application.ApplicationManager.Instance.MainWindow.Content).RenderSize.Height;
        double w = ((Grid)Application.ApplicationManager.Instance.MainWindow.Content).RenderSize.Width;
     

        public Worker(String message)
        {
            this.message = message;
            this.owner = Application.ApplicationManager.Instance.MainWindow;
        }

        public Worker(String message, Window owner) 
        {
            this.message = message;
            this.owner = owner;
        }

        public void StartWork(object parameters)
        {
            //mask();
            if (this.OnWork != null) OnWork();
            if (this.OnWorkWithParameter != null) OnWorkWithParameter(parameters);
            //unMask();
        }

        protected void mask()
        {
            Action maskAction = new Action(
                delegate()
                {
                    maskDialog = new MaskDialog();
                    //maskDialog.Owner = owner;
                    maskDialog.SetMessage(message);
                    maskDialog.ShowInTaskbar = false;
                    maskDialog.ShowDialog();
                }
            );
            Thread maskThread = new Thread(new ThreadStart(maskAction));
            maskThread.SetApartmentState(ApartmentState.STA);
            maskThread.Start();
        }
      
        protected void unMask()
        {
            if (maskDialog == null) return;
            maskDialog.Dispatcher.Invoke(DispatcherPriority.Background,
                 new Action(
                     delegate() { 
                         maskDialog.Close(); 
                     }
                 )
            );          
        }

    }
}
