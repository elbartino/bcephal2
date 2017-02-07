﻿using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.WriteOffConfig.WriteOffElements;
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

namespace Misp.Reconciliation.WriteOffConfig
{
    /// <summary>
    /// Interaction logic for WriteOffFieldPanel.xaml
    /// </summary>
    public partial class WriteOffFieldPanel : Grid
    {

        public event AddEventHandler OnAddFieldValue;
        public event DeleteEventHandler OnDeleteFieldValue;

        public event AddEventHandler OnAddField;
        public event DeleteEventHandler OnDeleteField;

        public WriteOffField writeOffField;

        public WriteOffConfigPanel parent { get; set; }


        public int Index { get; set; }
       
        

        public WriteOffFieldPanel()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public bool showLabel;
        public void showRowLabel(bool show = true)
        {
            showLabel = show;
            InitializeHandlers();
            this.fieldsPanel.showRowLabel(show);
            this.MandatoryValue.showRowLabel(show);
            this.FieldValuePanel.showRowLabel(show);
        }

        public void display()
        {
            this.FieldValuePanel.showLabel = showLabel;
            
            if (parent.nbreLigne == 0)
                this.FieldValuePanel.showLabel = true;
            else this.FieldValuePanel.showLabel = false;

            if (writeOffField == null) writeOffField = new WriteOffField();

            this.fieldsPanel.writeOffField = this.writeOffField;
            this.fieldsPanel.display();

            this.MandatoryValue.mandatoryValue = this.writeOffField.mandatory;
            this.MandatoryValue.display();

            InitializeHandlers();
            this.FieldValuePanel.fieldValueListChangeHandler = writeOffField.valueListChangeHandler;
            this.FieldValuePanel.display();
        }

        private void OnDeleteFields(object item)
        {
            if (OnDeleteField != null) OnDeleteField(this);
        }

        private void OnAddFields(object item)
        {
            if (OnAddField != null)
            {
                OnAddField(item);
            }
        }

        private void OnDeleteFieldsValue(object item)
        {
            if (OnDeleteFieldValue != null) OnDeleteFieldValue(item);
        }

        private void OnAddFieldsValue(object item)
        {
            if (OnAddFieldValue != null) OnAddFieldValue(item);
        }

        public void InitializeHandlers() 
        {
            RemoveHandlers();
            this.fieldsPanel.OnAddField += OnAddFields;
            this.fieldsPanel.OnDeleteField += OnDeleteFields;

            this.FieldValuePanel.OnAddFieldValue += OnAddFieldsValue;
            this.FieldValuePanel.OnDeleteFieldValue += OnDeleteFieldsValue;

            this.FieldValuePanel.writeParent = this;
        }

        private void RemoveHandlers()
        {
            this.fieldsPanel.OnAddField -= OnAddFields;
            this.fieldsPanel.OnDeleteField -= OnDeleteFields;

            this.FieldValuePanel.OnAddFieldValue -= OnAddFieldsValue;
            this.FieldValuePanel.OnDeleteFieldValue -= OnDeleteFieldsValue;
        }

        public WriteOffField FillWriteOffField()
        {
            return new WriteOffField();
        }

    }
}
