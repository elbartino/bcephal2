﻿using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

        public event ActivateEventHandler ActivateFiedValue;

        public event ChangeItemEventHandler ItemChanged;

        public DefaultValues()
        {
            InitializeComponent();
            setItemsSource();
            InitializeHandlers();
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display(WriteOffField field)
        {
            RemoveHandlers();
            if (field == null) return;
            this.DefaultValuesCombobox.SelectedItem = field.defaultValueTypeEnum != null ?
                field.defaultValueTypeEnum.ToString() : WriteOffFieldValueType.CUSTOM.label;
            InitializeHandlers();
        }

        public void InitializeHandlers()
        {
            this.DefaultValuesCombobox.SelectionChanged += OnChooseDefaultValues;
            this.DefaultValuesCombobox.GotFocus += OnGotFocus;
            this.DefaultValuesCombobox.MouseLeftButtonDown += OnGotFocus;
        }

        public void RemoveHandlers()
        {
            this.DefaultValuesCombobox.SelectionChanged -= OnChooseDefaultValues;
            this.DefaultValuesCombobox.GotFocus -= OnGotFocus;
            this.DefaultValuesCombobox.MouseLeftButtonDown -= OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (ActivateFiedValue != null) ActivateFiedValue(null);
        }

        private void OnChooseDefaultValues(object sender, SelectionChangedEventArgs e)
        {
            if (ItemChanged != null) ItemChanged(DefaultValuesCombobox.SelectedItem);
        }

        public void setDateView()
        {
            RemoveHandlers();
            setItemsSource(true);
            InitializeHandlers();
        }

        public void removeDateView()
        {
            RemoveHandlers();
            setItemsSource();
            InitializeHandlers();
        }
        
        private void setItemsSource(bool isDate=false) 
        {
            if (isDate)
            {
                this.DefaultValuesCombobox.ItemsSource = new String[] { WriteOffFieldValueType.CUSTOM_DATE.label,
                  WriteOffFieldValueType.TODAY.label};
                this.DefaultValuesCombobox.SelectedItem = WriteOffFieldValueType.TODAY.label;
            }
            else 
            {
                this.DefaultValuesCombobox.ItemsSource = new String[] 
                {
                      WriteOffFieldValueType.LEFT_SIDE.label,
                      WriteOffFieldValueType.RIGHT_SIDE.label,
                      WriteOffFieldValueType.CUSTOM.label
                };
                this.DefaultValuesCombobox.SelectedItem = WriteOffFieldValueType.CUSTOM.label;
            }
        }
    }
}
