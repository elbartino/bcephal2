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
    /// Interaction logic for WriteOffValueItem.xaml
    /// </summary>
    public partial class WriteOffValueItem : Grid
    {
        public event AddEventHandler OnAddFieldValue;
        public event DeleteEventHandler OnDeleteFieldValue;
        public Kernel.Domain.WriteOffFieldValue WriteOffFieldValue { get; set; }

        public int Index { get; set; }

        public WriteOffValueItem()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public WriteOffValueItem(Kernel.Domain.WriteOffFieldValue valueField) : base()
        {
            display(valueField);
        }
              
        public void showRowLabel(bool show = true) 
        {
            this.PossibleValues.showRowLabel(show);
            this.DefaultValues.showRowLabel(show);
        }

        public void display()
        {
            this.PossibleValues.display();
        }

        public void display(Kernel.Domain.WriteOffFieldValue valueField)
        {
            this.PossibleValues.writeOffValueField = valueField;
            this.PossibleValues.display();

            this.DefaultValues.writeOffValueField = valueField;
            this.DefaultValues.display();
        }

        public void InitializeHandlers()
        {
            this.PossibleValues.OnAddFieldValue += OnAddFieldsValue;
            this.PossibleValues.OnDeleteFieldValue += OnDeleteFieldsValue;
        }

        private void OnAddFieldsValue(object item)
        {
            if (OnAddFieldValue != null) OnAddFieldValue(null);
        }

        private void OnDeleteFieldsValue(object item)
        {
            if (OnDeleteFieldValue != null) OnDeleteFieldValue(this);
        }

        public Kernel.Domain.WriteOffFieldValue FillWriteOffField()
        {
            return new Kernel.Domain.WriteOffFieldValue();
        }
    }
}
