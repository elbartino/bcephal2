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

namespace Misp.Reconciliation.WriteOffConfig.WriteOffElements
{
    /// <summary>
    /// Interaction logic for WriteOffFieldValuePanel.xaml
    /// </summary>
    public partial class WriteOffFieldValuePanel : Grid
    {

        public event AddEventHandler OnAddFieldValue;
        public event DeleteEventHandler OnDeleteFieldValue;

        public event AddEventHandler OnAddField;
        public event DeleteEventHandler OnDeleteField;

        public PersistentListChangeHandler<WriteOffFieldValue> fieldValueListChangeHandler { get; set; }

        public WriteOffValueItem ActiveItem { get; set; }

        public WriteOffFieldPanel writeParent { get; set; }

        public WriteOffFieldValuePanel()
        {
            InitializeComponent();
        }

        public void showRowLabel(bool show = false)
        {
            int i = 0;
            foreach (UIElement element in this.FieldValuePanel.Children)
            {
                if (element is WriteOffValueItem)
                {
                    if (show)   show = i == 0;
                    ((WriteOffValueItem)element).Index = i;
                    ((WriteOffValueItem)element).showRowLabel(show);
                    i++;
                }
            }
        }

        public bool showLabel = true;

        public void display()
        {
            FieldValuePanel.Children.Clear();
            int i = 0;
            if (fieldValueListChangeHandler != null)
            {
                foreach (WriteOffFieldValue field in fieldValueListChangeHandler.Items)
                {
                    WriteOffValueItem item =getPanel();
                    item.WriteOffFieldValue = field;
                    item.Index = i;
                    item.display(field);
                    
                    if (!showLabel) item.showRowLabel(false);
                    else
                    {
                        if (i > 0) item.showRowLabel(false);
                    }

                    this.FieldValuePanel.Children.Add(item);
                    i++;
                }
            }
            if (this.FieldValuePanel.Children.Count == 0)
            {
                WriteOffValueItem item = getPanel();
                item.Index = 0;
                if (!showLabel) item.showRowLabel(showLabel);             
                this.FieldValuePanel.Children.Add(item);
            }
        }

        private void OnDeleteValueField(object item)
        {
            if (OnDeleteFieldValue != null)
            {
                if (item is WriteOffValueItem)
                {
                    int lastIndex = ((WriteOffValueItem)item).Index;
                    RemoveValueItem((WriteOffValueItem)item);
                    if(this.FieldValuePanel.Children.Count == 0)
                    {
                        OnAddValueField(writeParent.Index == 0);
                    }
                    else
                    {
                        WriteOffValueItem wp = (WriteOffValueItem)this.FieldValuePanel.Children[0];
                        if (lastIndex == 0 && writeParent.Index == 0)
                        {
                            wp.showRowLabel(true);
                        }
                    }
                }
                //OnDeleteFieldValue(item);
            }
        }

        private void OnAddValueField(object item)
        {
            if (OnAddFieldValue != null)
            {
                WriteOffValueItem witem = getPanel();
                if (item is bool) AddValueItem(witem, (bool)item);
                else AddValueItem(witem);
                OnAddFieldValue(item);    
            }
        }

        private WriteOffValueItem getPanel()
        {
            WriteOffValueItem witem = new WriteOffValueItem();
            witem.OnAddFieldValue += OnAddValueField;
            witem.OnDeleteFieldValue += OnDeleteValueField;
            return witem;
        }

        public void AddValueItem(WriteOffValueItem valueItem, bool isFirst= false)
        {
            valueItem.Index = this.FieldValuePanel.Children.Count - 1;
            if (isFirst)
            {
                valueItem.showRowLabel(true);
            }
            else 
            {
                valueItem.showRowLabel(false);
            }
            this.FieldValuePanel.Children.Add(valueItem);
        }

        public void RemoveValueItem(WriteOffValueItem valueItem)
        {
            this.FieldValuePanel.Children.Remove(valueItem);
            int i = 0;
            foreach (UIElement writeoff in this.FieldValuePanel.Children) 
            {
                if (writeoff is WriteOffValueItem) 
                {
                    ((WriteOffValueItem)writeoff).Index = i;
                    i++;
                }

            }
        }
    }
}
