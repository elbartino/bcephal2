﻿using Misp.Kernel.Application;
using System;
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

namespace Misp.Kernel.Administration.User
{
    /// <summary>
    /// Interaction logic for ConnectedUserPanel.xaml
    /// </summary>
    public partial class ConnectedUserPanel : Grid
    {
        public ConnectedUserPanel()
        {
            InitializeComponent();
            SingOutLink.RequestNavigate += OnSingOut;
        }
        
        private void OnSingOut(object sender, RequestNavigateEventArgs e)
        {
            HistoryHandler.Instance.tryToSingout();
        }
    }
}