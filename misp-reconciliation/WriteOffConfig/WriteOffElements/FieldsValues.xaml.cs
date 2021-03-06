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
    /// Interaction logic for FieldsValues.xaml
    /// </summary>
    public partial class FieldsValues : StackPanel
    {

        public WriteOffField writeOffField { get; set; }

        public event AddEventHandler OnAddField;

        public event DeleteEventHandler OnDeleteField;

        public event ActivateEventHandler ActivateFieldPanel;

        public event ChangeItemEventHandler ItemChanged;

        public FieldsValues()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            this.NewButton.Click += OnHandledButton;
            this.DeleteButton.Click += OnHandledButton;
            this.NewButton.GotFocus += OnGotFocus;
            this.DeleteButton.GotFocus += OnGotFocus;
            this.ValueTypeTextBox.GotFocus += OnGotFocus;
            this.ValueTypeTextBox.MouseLeftButtonDown += OnGotFocus;
            this.DeleteButton.MouseLeftButtonDown += OnGotFocus;
            this.NewButton.MouseLeftButtonDown += OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (ActivateFieldPanel != null) ActivateFieldPanel(null);
        }

        private void OnHandledButton(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ButtonActions((Button)sender);
            }
        }       

        private void ButtonActions(Button button)
        {
            if (button == NewButton)
            {
                if (OnAddField != null) OnAddField(null);
            }
            if (button == DeleteButton)
            {
                if (OnDeleteField != null) OnDeleteField(this.writeOffField);
            }
        }

        public void showRowLabel(bool show = false)
        {
            this.labelRow.Visibility = show ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void display()
        {
             string name = writeOffField == null ? "" : writeOffField.attributeField != null ? writeOffField.attributeField.name :
                 writeOffField.periodField != null ? writeOffField.periodField.name :
                 writeOffField.measureField != null ? writeOffField.measureField.name :"" ;

             this.ValueTypeTextBox.Text = name;       
            
        }

        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.getActiveWriteOffField().setAttribute(attribute);
            if (ItemChanged != null) ItemChanged(this.writeOffField);
            display();
        }
              

        public void setPeriodName(Kernel.Domain.PeriodName PeriodName)
        {
            this.getActiveWriteOffField().setPeriodName(PeriodName);
            if (ItemChanged != null) ItemChanged(this.writeOffField);
            
            display();
        }

        public void setMeasure(Kernel.Domain.Measure measure)
        {
            this.getActiveWriteOffField().setMeasure(measure);
            if (ItemChanged != null) ItemChanged(this.writeOffField);
            display();
        }

        WriteOffField getActiveWriteOffField() 
        {
            if (this.writeOffField == null)
            {
                this.writeOffField = new WriteOffField();
                this.writeOffField.position = -1;
            }            
            return this.writeOffField;
        }


        public void updateObject(WriteOffField writeOffField)
        {
            this.writeOffField = writeOffField;
        }
    }
}
