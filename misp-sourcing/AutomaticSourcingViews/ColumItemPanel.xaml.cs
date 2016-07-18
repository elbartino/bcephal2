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

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for ColumItemPanel.xaml
    /// </summary>
    public partial class ColumItemPanel : Grid
    {
        public event AddEventHandler Added;
        public event UpdateEventHandler Updated;
        public event DeleteEventHandler Deleted;
        public event ActivateEventHandler Activated;
        public event ChangeEventHandler Changed;

        public AutomaticSourcingColumnItem Item { get; set; }
        public int Position { get; set; }

        protected bool trowEvent;

        public ColumItemPanel()
        {
            InitializeComponent();
            InitializeHandlers();
            this.Position = 0;
            trowEvent = true;
        }

        public ColumItemPanel(int position)
            : this()
        {
            this.Position = position;
        }

        public ColumItemPanel(AutomaticSourcingColumnItem item) : this()
        {
            Display(item);
        }


        public void Display(AutomaticSourcingColumnItem item)
        {
            trowEvent = false;
            this.Item = item;
            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.value)) this.textBox.Text = item.value;
                this.Position = item.position;
            }
            else this.textBox.Text = "";
            trowEvent = true;
        }

        public AutomaticSourcingColumnItem Fill()
        {
            String value = this.textBox.Text;
            if (this.Item == null) this.Item = new AutomaticSourcingColumnItem();
            this.Item.value = value;
            this.Item.position = Position;
            return this.Item;
        }

        public void SetValue(Object value)
        {
            bool added = false;
            if (this.Item == null)
            {
                this.Item = new AutomaticSourcingColumnItem();
                added = true;
            }
            this.Item.value = value != null ? value.ToString() : null;
            this.textBox.Text = value != null ? value.ToString() : "";
            if (trowEvent && Added != null && added) Added(this);
            if (trowEvent && Updated != null && !added) Updated(this);
        }

        protected void InitializeHandlers()
        {
            this.button.Click += OnDeleteButtonClick;
            this.textBox.TextChanged += OnTextChanged;            
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool added = this.Item == null;
            Fill();
            if (trowEvent && Added != null && added) Added(this);
            if (trowEvent && Updated != null && !added) Updated(this);
            if (trowEvent && Changed != null) Changed();
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            this.textBox.Text = "";
            Fill();
            if (trowEvent && Deleted != null) Deleted(this);
            if (trowEvent && Changed != null) Changed();
        }

    }
}
