﻿using Misp.Kernel.Domain;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DefaultValues : Grid
    {
        public Kernel.Domain.WriteOffFieldValue writeOffValueField { get; set; }

        public DefaultValues()
        {
            InitializeComponent();
            this.DefaultValuesCombobox.ItemsSource = new String[] 
            {
                  WriteOffFieldValueType.CUSTOM.ToString(),
                  WriteOffFieldValueType.LEFT_SIDE.ToString(),
                  WriteOffFieldValueType.RIGHT_SIDE.ToString(),
                  ""
            };
            InitializeHandlers();
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
            if (this.writeOffValueField == null) return;
            this.DefaultValuesCombobox.SelectedItem = writeOffValueField.defaultValueType.ToString();
        }

        public void InitializeHandlers()
        {
            this.DefaultValuesCombobox.SelectionChanged += OnChooseDefaultValues;
        }

        private void OnChooseDefaultValues(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}