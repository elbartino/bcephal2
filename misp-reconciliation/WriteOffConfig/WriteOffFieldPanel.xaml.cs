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

        public event ActivateEventHandler ActivateFieldPanel;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        public WriteOffValueItem ActiveFieldItemPanel;

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
            this.DefaultValueCombo.showRowLabel(show);
            this.FieldValuePanel.showRowLabel(show);
        }

        public void display()
        {
            this.FieldValuePanel.showLabel = showLabel;
            if (parent.nbreLigne == 0)
                this.FieldValuePanel.showLabel = true;
            else this.FieldValuePanel.showLabel = false;                     

            this.fieldsPanel.writeOffField = this.writeOffField;
            this.fieldsPanel.display();
            bool mandatory = this.writeOffField != null ? this.writeOffField.mandatory ? this.writeOffField.mandatory : this.writeOffField.isIncremental()  : false;
            this.MandatoryValue.mandatoryValue = mandatory;
            this.MandatoryValue.display();
            this.DefaultValueCombo.display(this.writeOffField);
            InitializeHandlers();
            this.FieldValuePanel.fieldValueListChangeHandler = this.writeOffField != null ? writeOffField.writeOffFieldValueListChangeHandler : null;
            this.FieldValuePanel.isDateView = this.writeOffField != null && this.writeOffField.isPeriod();
            this.FieldValuePanel.isIncremental = this.writeOffField != null && this.writeOffField.isIncremental();
            this.FieldValuePanel.display();
        }

        private void OnDeleteFields(object item)
        {
            if (OnDeleteField != null)
            {
                if (item is Kernel.Domain.WriteOffField)
                {
                    this.writeOffField = (Kernel.Domain.WriteOffField)item;
                }
                OnDeleteField(this);
            }
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
            if (OnDeleteFieldValue != null)
            {
                if (item is Kernel.Domain.WriteOffFieldValue)
                {
                    this.writeOffField.SynchronizeDeleteWriteOffFieldValue((Kernel.Domain.WriteOffFieldValue)item);
                    if (ItemChanged != null) ItemChanged(this.writeOffField);
                }
            }
        }

        private void OnAddFieldsValue(object item)
        {
            if (OnAddFieldValue != null) OnAddFieldValue(item);
        }

        public void InitializeHandlers() 
        {
            RemoveHandlers();

            this.fieldsPanel.ActivateFieldPanel += OnActivateFieldsValue;
            this.fieldsPanel.OnAddField += OnAddFields;
            this.fieldsPanel.OnDeleteField += OnDeleteFields;
            this.fieldsPanel.ItemChanged += OnFieldsPanelChanged;
            this.DefaultValueCombo.ItemChanged += OnDefaultValueChanged;
            this.MandatoryValue.ItemChanged += OnActivateMandatory;

            this.FieldValuePanel.OnAddFieldValue += OnAddFieldsValue;
            this.FieldValuePanel.OnDeleteFieldValue += OnDeleteFieldsValue;
            this.FieldValuePanel.ActivateFiedValue += OnActivateFieldsValue;
            this.FieldValuePanel.ItemChanged += OnFieldValueChanged;
            this.FieldValuePanel.getActiveItem();
            this.FieldValuePanel.writeParent = this;
        }

        private void OnActivateMandatory(object item)
        {
            if (ItemChanged != null) 
            {
                if (item is bool) 
                {
                    bool value = (bool)item;
                    this.getActiveWriteOffField().mandatory = value;
                    if (ItemChanged != null) ItemChanged(this.writeOffField);
                }
            }
        }

        private void OnFieldValueChanged(object item)
        {
            if (ItemChanged != null)
            {
                if (item is Kernel.Domain.WriteOffFieldValue)
                {
                    WriteOffFieldValue valueToUpdate =  this.getActiveWriteOffField().SynchronizeWriteOffFieldValue((Kernel.Domain.WriteOffFieldValue)item);
                    this.FieldValuePanel.updateObject(valueToUpdate);
                    if (ItemChanged != null) ItemChanged(this.writeOffField);                    
                }
            }
        }

        private void RemoveHandlers()
        {
            this.fieldsPanel.OnAddField -= OnAddFields;
            this.fieldsPanel.OnDeleteField -= OnDeleteFields;
            this.fieldsPanel.ActivateFieldPanel -= OnActivateFieldsValue;
            this.fieldsPanel.ItemChanged -= OnFieldsPanelChanged;
            this.DefaultValueCombo.ItemChanged -= OnDefaultValueChanged;
            this.MandatoryValue.ItemChanged -= OnActivateMandatory;

            this.FieldValuePanel.ActivateFiedValue -= OnActivateFieldsValue;
            this.FieldValuePanel.ItemChanged -= OnFieldValueChanged;
            this.FieldValuePanel.OnAddFieldValue -= OnAddFieldsValue;
            this.FieldValuePanel.OnDeleteFieldValue -= OnDeleteFieldsValue;
        }

        private void OnDefaultValueChanged(object item)
        {
            if (ItemChanged != null)
            {
                if (item is WriteOffFieldValueType)
                {
                    WriteOffFieldValueType value = (WriteOffFieldValueType)item;
                    this.getActiveWriteOffField().defaultValueTypeEnum = value;
                    if (ItemChanged != null) ItemChanged(this.writeOffField);
                }
            }
        }

        private void OnFieldsPanelChanged(object item)
        {
            if (item is WriteOffField) this.writeOffField = (WriteOffField)item;
            if (ItemChanged != null) ItemChanged(this.writeOffField);
        }

        private void OnActivateFieldsValue(object item)
        {
            if (ActivateFieldPanel != null)
            {
                if (item is WriteOffFieldValuePanel)
                {
                    ActivateFieldPanel(item);
                }
                else
                ActivateFieldPanel(this);
            }
        }

        public WriteOffField FillWriteOffField()
        {
            return new WriteOffField();
        }
        
        public void setAttribute(Kernel.Domain.Attribute attribute)
        {
            this.fieldsPanel.setAttribute(attribute);
            this.MandatoryValue.mandatoryValue = attribute.incremental;
            this.MandatoryValue.display();
            this.DefaultValueCombo.DefaultValuesCombobox.Visibility = attribute.incremental ? Visibility.Hidden : Visibility.Visible;
            this.FieldValuePanel.removeDateView(attribute.incremental);
        }
        
        public void setAttributeValue(Kernel.Domain.AttributeValue attributeValue)
        {
            this.FieldValuePanel.setAttributeValue(attributeValue);
        }

        public void setMeasure(Kernel.Domain.Measure measure)
        {
            this.fieldsPanel.setMeasure(measure);
            this.FieldValuePanel.removeDateView();
        }

        public void setPeriodName(PeriodName periodName)
        {
            this.DefaultValueCombo.setDateView();
            this.fieldsPanel.setPeriodName(periodName);
            this.FieldValuePanel.setPeriodView();
        }

        public void setPeriodInterval(PeriodInterval periodInterval) 
        {
            this.FieldValuePanel.setPeriodInterval(periodInterval);
        }

        public void UpdateObject(WriteOffField writeOffField) 
        {
            this.writeOffField = writeOffField;
            this.fieldsPanel.updateObject(this.writeOffField);
        }


        public void UpdateValues(WriteOffField writeofffield)
        {
            this.FieldValuePanel.updateValue(writeOffField);
        }

        public WriteOffField getActiveWriteOffField()
        {
            if (this.writeOffField == null)
            {
                this.writeOffField = new WriteOffField();
                this.writeOffField.position = -1;
            }
            return this.writeOffField;
        }
    }
}
