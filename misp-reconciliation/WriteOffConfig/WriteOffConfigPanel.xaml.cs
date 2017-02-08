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
    /// Interaction logic for WriteOffConfigMainPanel.xaml
    /// </summary>
    public partial class WriteOffConfigPanel : Grid
    {
        public event AddEventHandler OnAddFieldValue;
        public event DeleteEventHandler OnDeleteFieldValue;

        public event AddEventHandler OnAddField;
        public event DeleteEventHandler OnDeleteField;

        public event ActivateEventHandler ActivateFieldPanel;

        public int nbreLigne = 0;
                
        public PersistentListChangeHandler<WriteOffField> fieldListChangeHandler { get; set; }

        public WriteOffConfiguration EditedObject { get; set; }

        public WriteOffFieldPanel ActiveFieldPanel { get; set; }

        public WriteOffConfigPanel()
        {
            InitializeComponent();
        }

        public void display(Kernel.Domain.WriteOffConfiguration writeOffConfig)
        {
            this.configPanel.Children.Clear();

            if (writeOffConfig == null)
            {
                AddAction(null);
            }
            else
            {
                foreach (WriteOffField writeofffield in writeOffConfig.fieldListChangeHandler.Items)
                {
                    AddAction(writeofffield);
                }
            }
        }

        private void OnAddFields(object item)
        {
            AddAction(null);         
        }

        private void OnDeleteFields(object item)
        {
            if (!(item is WriteOffFieldPanel)) return;
            DeleteAction((WriteOffFieldPanel)item);

            if (OnDeleteField != null)
            {
               
            }
        }

        private void OnDeleteFieldsValue(object item)
        {
            if (OnDeleteFieldValue != null) OnDeleteFieldValue(item);
        }

        private void OnAddFielddValue(object item)
        {
            if (OnAddFieldValue != null) OnAddFieldValue(item);
        }

        private WriteOffFieldPanel getFieldPanel() 
        {
            WriteOffFieldPanel wpanel = new WriteOffFieldPanel();
            wpanel.GotFocus += OnActivate;
            wpanel.ActivateFieldPanel += OnActivateFields;
            wpanel.OnAddFieldValue += OnAddFielddValue;
            wpanel.OnDeleteFieldValue += OnDeleteFieldsValue;
            wpanel.OnAddField += OnAddFields;
            wpanel.OnDeleteField += OnDeleteFields;
            return wpanel;
        }

        private void OnActivateFields(object item)
        {
            if (item is WriteOffFieldPanel)
            {
                this.setActiveFieldPanel((WriteOffFieldPanel)item);
            }
            else if (item is WriteOffFieldValuePanel)
            {
                this.setActiveFieldPanel(((WriteOffFieldValuePanel)item).writeParent);
            }
            if (ActivateFieldPanel != null) ActivateFieldPanel(item);
        }

        private void OnActivate(object sender, RoutedEventArgs e)
        {
            if (sender is WriteOffFieldPanel)
            {
                if (ActivateFieldPanel != null) ActivateFieldPanel((WriteOffFieldPanel)sender);
            }
        }

        private void DeleteAction(WriteOffFieldPanel item) 
        {
            if (item is WriteOffFieldPanel)
            {
                int lastIndex = ((WriteOffFieldPanel)item).Index;
                this.configPanel.Children.Remove((WriteOffFieldPanel)item);
                nbreLigne = 0;
                foreach (UIElement writeoff in this.configPanel.Children)
                {
                    if (writeoff is WriteOffFieldPanel)
                    {
                        ((WriteOffFieldPanel)writeoff).Index = nbreLigne;
                        ((WriteOffFieldPanel)writeoff).showRowLabel(nbreLigne == 0);
                        nbreLigne++;
                    }
                }
                              
                if (this.configPanel.Children.Count == 0)
                {
                    AddAction(null);
                    return;
                }
               
                if (lastIndex == 0)
                {
                   ((WriteOffFieldPanel)this.configPanel.Children[0]).showRowLabel(true);
                }
                this.ActiveFieldPanel = null;
            }
            
        }

        private void AddAction(WriteOffField writeofffield) 
        {
            WriteOffFieldPanel wpanel = getFieldPanel();
            wpanel.parent = this;
            wpanel.Index = nbreLigne;
            wpanel.writeOffField = writeofffield;
            wpanel.showRowLabel(nbreLigne == 0);
            wpanel.display();
            nbreLigne++;
            this.ActiveFieldPanel = wpanel;
            this.configPanel.Children.Add(wpanel);
        }

        public List<object> getEditableControls()
        {
            List<object> list = new List<object>();
            //list.Add(postingAttributePanel);
            //list.Add(reconciliationAttributePanel);
            //list.Add(accountNbrAttributePanel);
            //list.Add(accountNameAttributePanel);
            //list.Add(dcAttributePanel);
            ////list.Add(debitValuePanel);
            ////list.Add(creditValuePanel);
            //list.Add(writeOffAccountPanel);
            //list.Add(amountMeasurePanel);
            return list;
        }

        public WriteOffFieldPanel getActiveFieldPanel()
        {
            if (this.ActiveFieldPanel == null) this.ActiveFieldPanel = this.configPanel.Children[0] as WriteOffFieldPanel;
            return this.ActiveFieldPanel;
        }

        public void setActiveFieldPanel(WriteOffFieldPanel woffieldpanel)
        {
            if (woffieldpanel == null) getActiveFieldPanel();
            else this.ActiveFieldPanel = woffieldpanel;
        }
        
        public void displayObject()
        {
            display(EditedObject);
        }


        public void setMeasure(Measure measure) 
        {
            this.ActiveFieldPanel.setMeasure(measure);
        }

        public void setAttribute(Kernel.Domain.Attribute attribte) 
        {
            this.ActiveFieldPanel.setAttribute(attribte);
        }

        public void setPeriodName(PeriodName periodName)
        {
            this.ActiveFieldPanel.setPeriodName(periodName);
        }

        public void setPeriodName(PeriodInterval periodInterval)
        {
            this.ActiveFieldPanel.setPeriodInterval(periodInterval);
        }

        public WriteOffConfiguration FillObject() 
        {
            WriteOffConfiguration writeoffConfig = new WriteOffConfiguration();
            //writeoffConfig.fieldListChangeHandler = this.

            return writeoffConfig;
        }

        public void SetTarget(Target target)
        {
            if (target is Kernel.Domain.Attribute) this.ActiveFieldPanel.setAttribute((Kernel.Domain.Attribute)target);
            else if (target is Kernel.Domain.AttributeValue) this.ActiveFieldPanel.setAttributeValue((Kernel.Domain.AttributeValue)target);

        }
    }
}
