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
    /// Interaction logic for WriteOffLine.xaml
    /// </summary>
    public partial class WriteOffLine : Grid
    {
        public Kernel.Domain.WriteOffField writeOffField { get; set; }

        public WriteOffLine()
        {
            InitializeComponent();
            this.valueDatePicker.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
            if (writeOffField == null) return;
            string name = "";
            if (writeOffField.isAttribute())
            {
                name = this.writeOffField.attributeField.name;
                this.valueDatePicker.Visibility = System.Windows.Visibility.Collapsed;
                this.valueCombobox.Visibility = System.Windows.Visibility.Visible;
            }
            else if (writeOffField.isPeriod())
            {
                name = this.writeOffField.periodField.name;
                this.valueDatePicker.Visibility = System.Windows.Visibility.Visible;
                if (this.writeOffField.writeOffFieldValueListChangeHandler != null) 
                {
                    if (this.writeOffField.writeOffFieldValueListChangeHandler.Items.Count > 0)
                    {
                        this.writeOffField.writeOffFieldValueListChangeHandler.Items[0].defaultValueType = WriteOffFieldValueType.TODAY.label;
                        this.valueDatePicker.SelectedDate = DateTime.Now;
                        this.valueDatePicker.IsEnabled = false;
                    }
                }
                this.valueCombobox.Visibility = System.Windows.Visibility.Collapsed;
            }
            bool addStart = this.writeOffField.mandatory;
            this.nameLabel.Content = name + (addStart ? "*" :"") ;
        }
    }
}
