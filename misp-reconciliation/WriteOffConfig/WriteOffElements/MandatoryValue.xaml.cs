﻿using Misp.Kernel.Ui.Base;
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

namespace Misp.Reconciliation.WriteOffConfig.WriteOffElements
{
    /// <summary>
    /// Interaction logic for MandatoryValue.xaml
    /// </summary>
    public partial class MandatoryValue : StackPanel
    {

        public bool mandatoryValue { get; set; }
        public event ActivateEventHandler ActivateFiedValue;
        public event ChangeItemEventHandler ItemChanged;

        public MandatoryValue()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public void showRowLabel(bool show = false) 
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
            RemoveHandlers();
            this.mandatoryCheckBox.IsChecked = this.mandatoryValue;
            InitializeHandlers();
        }

        public void InitializeHandlers() 
        {
            this.mandatoryCheckBox.GotFocus += OnGotFocus;
            this.mandatoryCheckBox.MouseLeftButtonDown += OnGotFocus;
            this.mandatoryCheckBox.Checked += OnCheckMandatory;
            this.mandatoryCheckBox.Unchecked += OnCheckMandatory;
        }

        public void RemoveHandlers()
        {
            this.mandatoryCheckBox.GotFocus -= OnGotFocus;
            this.mandatoryCheckBox.MouseLeftButtonDown -= OnGotFocus;
            this.mandatoryCheckBox.Checked -= OnCheckMandatory;
            this.mandatoryCheckBox.Unchecked -= OnCheckMandatory;
        }

        private void OnCheckMandatory(object sender, RoutedEventArgs e)
        {
            if (ItemChanged != null) ItemChanged(this.mandatoryCheckBox.IsChecked.Value);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (ActivateFiedValue != null) ActivateFiedValue(null);
        }


    }
}
