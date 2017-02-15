using Misp.Kernel.Domain;
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

        public event ChangeItemEventHandler ItemChanged;

        public event ChangeItemEventHandler ItemPresent;

        public int nbreLigne = 0;
                
        public PersistentListChangeHandler<WriteOffField> fieldListChangeHandler { get; set; }

        public WriteOffConfiguration EditedObject { get; set; }

        public WriteOffFieldPanel ActiveFieldPanel { get; set; }

        public UIElement ActivePanel { get; set; }

        private Dictionary<string, Dictionary<string, int>> itemDictionnary;


        public WriteOffConfigPanel()
        {
            InitializeComponent();
        }

        public void display(Kernel.Domain.WriteOffConfiguration writeOffConfig)
        {
            this.configPanel.Children.Clear();
            bool AddDefault = writeOffConfig == null ? true : writeOffConfig != null ?
                writeOffConfig.fieldListChangeHandler == null ? 
                true : writeOffConfig.fieldListChangeHandler.Items.Count == 0 ? true : false : false;

            if (AddDefault)
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
            WriteOffFieldPanel wPanel = (WriteOffFieldPanel)item;
            DeleteAction(wPanel);
            if (ItemChanged != null && wPanel.writeOffField != null) ItemChanged(null);
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
            wpanel.ItemChanged += OnWriteOffConfigChanged;
            return wpanel;
        }

        private void OnWriteOffConfigChanged(object item)
        {
            if (item is Kernel.Domain.WriteOffField)
            {
                if (this.EditedObject == null) this.EditedObject = new WriteOffConfiguration();
                WriteOffField fieldToUpdate = this.EditedObject.SynchronizeWriteOffField((Kernel.Domain.WriteOffField)item);
                UpdateWriteOffField(fieldToUpdate);
                if (ItemChanged != null) ItemChanged(this.EditedObject);
            }
        }

        private void UpdateWriteOffField(WriteOffField writeOffField)
        {
            UIElement apanel = getActivePanel();
            if (apanel is WriteOffFieldPanel)
            {
                ((WriteOffFieldPanel)apanel).UpdateObject(writeOffField);
            }
        }

        private void OnActivateFields(object item)
        {
            if (item is WriteOffFieldPanel)
            {
                this.setActiveFieldPanel((WriteOffFieldPanel)item);
            }
            else if (item is WriteOffFieldValuePanel)
            {
                this.setActiveFieldPanel(((WriteOffFieldValuePanel)item));
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
                WriteOffFieldPanel panel = (WriteOffFieldPanel)item;

                int lastIndex = panel.Index;
                this.configPanel.Children.Remove(panel);
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

                if (panel.writeOffField != null)
                {
                    this.EditedObject.SynchronizeDeleteWriteOffField(panel.writeOffField);
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
            wpanel.writeOffField = writeofffield;
            wpanel.showRowLabel(nbreLigne == 0);
            wpanel.display();
            nbreLigne++;
            this.setActiveFieldPanel(wpanel);
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

        public UIElement getActivePanel()
        {
            if (this.ActivePanel == null)
            {
                int index =  this.configPanel.Children.Count <=  0 ? 0 : this.configPanel.Children.Count - 1;
                return this.configPanel.Children[index] as WriteOffFieldPanel;
            }
            return this.ActivePanel;
        }

        public void setActiveFieldPanel(UIElement woffieldpanel)
        {
            if (woffieldpanel == null) this.ActivePanel = this.configPanel.Children[0] as WriteOffFieldPanel;
            else this.ActivePanel = woffieldpanel;
        }
        
        public void displayObject()
        {
            display(EditedObject);
        }


        public WriteOffConfiguration fillObject() 
        {
            return this.EditedObject;
        }

        public void setMeasure(Measure measure) 
        {
            UIElement apanel = getActivePanel();
            if (apanel is WriteOffFieldPanel)
            {
                ((WriteOffFieldPanel)apanel).setMeasure(measure);
            }
        }

        public void setAttribute(Kernel.Domain.Attribute attribute) 
        {
            UIElement apanel = getActivePanel();
            if (apanel is WriteOffFieldPanel)
            {
                ((WriteOffFieldPanel)apanel).setAttribute(attribute);
            }
            else if (apanel is WriteOffValueItem)
            {
                ((WriteOffValueItem)apanel).setAttribute(attribute);
            }
        }

        public void setPeriodName(PeriodName periodName)
        {
            UIElement apanel = getActivePanel();
            if (apanel is WriteOffFieldPanel)
            {
                WriteOffFieldPanel fieldPanel = (WriteOffFieldPanel)apanel;
                bool isDuplicate = IsDuplicatiLine(fieldPanel, SubjectType.PERIOD, periodName.name);
                if (isDuplicate && ItemPresent != null) ItemPresent(new object[] { SubjectType.PERIOD, periodName.name });
                else fieldPanel.setPeriodName(periodName);
            }
        }

        private bool IsDuplicatiLine(WriteOffFieldPanel fieldPanel,SubjectType subjectType,string name) 
        {
            if (itemDictionnary == null) itemDictionnary = new Dictionary<string, Dictionary<string, int>>();

                if (!itemDictionnary.ContainsKey(subjectType.label))
                {
                    itemDictionnary.Add(subjectType.label, new Dictionary<string, int>());
                }
                Dictionary<string, int> dicolist = null;
                itemDictionnary.TryGetValue(subjectType.label, out dicolist);
                if (!dicolist.ContainsKey(name))
                {
                   int position = dicolist.Count;
                   dicolist.Add(name, position);
                   return false;
                }
                else
                {
                    int position = -1;
                    dicolist.TryGetValue(name, out position);
                    return true;
                }
        }

        public void setPeriodName(PeriodInterval periodInterval)
        {
            UIElement apanel = getActivePanel();
            if (apanel is WriteOffFieldValuePanel)
            {
                ((WriteOffFieldValuePanel)apanel).setPeriodInterval(periodInterval);
            }
        }        

        public void SetTarget(Target target)
        {
            UIElement apanel = getActivePanel();
            if (apanel is WriteOffFieldPanel)
            {
                if (target is Kernel.Domain.Attribute)
                {
                    Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)target;
                    WriteOffFieldPanel fieldPanel = (WriteOffFieldPanel)apanel;
                    bool isDuplicate = IsDuplicatiLine(fieldPanel, SubjectType.ATTRIBUTE, attribute.name);
                    if (isDuplicate && ItemPresent != null) ItemPresent(new object[] { SubjectType.ATTRIBUTE, attribute.name });
                    else fieldPanel.setAttribute(attribute);
                }
            }
            else if (apanel is WriteOffFieldValuePanel)
            {
                if (target is Kernel.Domain.Attribute)
                    ((WriteOffFieldValuePanel)apanel).setAttribute((Kernel.Domain.Attribute)target);
                else if (target is Kernel.Domain.AttributeValue) ((WriteOffFieldValuePanel)apanel).setAttributeValue((Kernel.Domain.AttributeValue)target);
            }
        }


        public void updateObject(WriteOffConfiguration writeOffConfiguration)
        {
            if (writeOffConfiguration == null || writeOffConfiguration.fieldListChangeHandler == null) return;
            foreach (WriteOffField writeofffield in writeOffConfiguration.fieldListChangeHandler.Items)
            {
                if (writeofffield == null) continue;
                foreach (UIElement writeoff in this.configPanel.Children)
                {
                    if (writeoff is WriteOffFieldPanel)
                    {
                        WriteOffFieldPanel writeoffPanel = (WriteOffFieldPanel)writeoff;
                        if (writeoffPanel.writeOffField == null) continue;
                        if (writeoffPanel.writeOffField.position == writeofffield.position)
                        {
                            writeoffPanel.UpdateObject(writeofffield);
                            writeoffPanel.UpdateValues(writeofffield);
                            break;
                        }
                    }
                }    
            }
            
        }

     
    }
}
