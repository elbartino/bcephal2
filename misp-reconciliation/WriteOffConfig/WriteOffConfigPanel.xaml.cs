using Misp.Kernel.Domain;
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

        public int nbreLigne = 0;
        
        public Kernel.Domain.WriteOffConfiguration writeOffConfig { get; set; }

        public PersistentListChangeHandler<WriteOffField> fieldListChangeHandler { get; set; } 

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
            //if (OnAddField != null)
            //{
            //    OnAddField(item);
            //    AddAction();
            //    //OnAddField(item);
            //}
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
            wpanel.OnAddFieldValue += OnAddFielddValue;
            wpanel.OnDeleteFieldValue += OnDeleteFieldsValue;

            wpanel.OnAddField += OnAddFields;
            wpanel.OnDeleteField += OnDeleteFields;
            return wpanel;
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
            }
            
        }

        private void AddAction(WriteOffField writeofffield) 
        {
            WriteOffFieldPanel wpanel = getFieldPanel();
            wpanel.parent = this;
            wpanel.Index = nbreLigne;
            wpanel.showRowLabel(nbreLigne == 0);
            wpanel.display();
            nbreLigne++;
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
    }
}
