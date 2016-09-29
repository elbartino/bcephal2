﻿using System;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.Office.EDraw
{
    /// <summary>
    /// Interaction logic for SyncFunsionComponent.xaml
    /// </summary>
    public partial class SyncFunsionComponent : RibbonWindow
    {
        public SyncFunsionComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Provide support for Excel like closing operation when press the close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.spreadsheetControl.Commands.FileClose.Execute(null);
            if (Application.Current.ShutdownMode != ShutdownMode.OnExplicitShutdown)
                e.Cancel = true;
        }
    }
}
