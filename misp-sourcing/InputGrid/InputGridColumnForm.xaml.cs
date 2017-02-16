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

namespace Misp.Sourcing.InputGrid
{
    /// <summary>
    /// Interaction logic for InputGridColumnForm.xaml
    /// </summary>
    public partial class InputGridColumnForm : UserControl
    {
        
        #region Events

        public event UpdateEventHandler Changed;

        #endregion

        public Misp.Kernel.Domain.Grille Grid { get; set; }

        public GrilleColumn Column { get; set; }

        private bool ModifyThisColumn { get; set; }

        protected bool throwChange = true;

        public InputGridColumnForm()
        {
            InitializeComponent();
            InitializeHandlers();
        }


        public void Display(GrilleColumn column)
        {
            throwChange = false;
            this.Column = column;
            if (this.Column == null) this.Column = GetNewColumn();
            if (this.Column.oid.HasValue) this.Column.isAdded = true;
            String colName = Kernel.Util.RangeUtil.GetColumnName(this.Column.position);
            ColumnTextBox.Text = colName;
            TypeTextBox.Text = this.Column.type != null ? this.Column.type : "";
            NameTextBox.Text = this.Column.name != null ? this.Column.name : "";
            ShowCheckBox.IsChecked = this.Column.show;
            this.ModifyThisColumn = false;
            throwChange = true;
        }

        public void Fill()
        {
            if (this.Column == null) return;          
            this.Column.name = NameTextBox.Text;
            this.Column.show = ShowCheckBox.IsChecked.Value;
        }
        
        public void SetValue(object value)
        {
            if (this.Column == null || !this.ModifyThisColumn) this.Column = GetNewColumn();
            this.Column.SetValue(value);
            Display(this.Column);
            OnChanged(true);
        }

        public GrilleColumn GetNewColumn()
        {
            GrilleColumn column = new GrilleColumn();
            column.isModified = false;
            column.isAdded = false;
            column.position = this.Grid.columnListChangeHandler.Items.Count;
            return column;
        }

        private void InitializeHandlers()
        {
            ShowCheckBox.Checked += OnShowCheckBoxChecked;
            ShowCheckBox.Unchecked += OnShowCheckBoxChecked;
            NameTextBox.KeyUp += OnNameTextChanged;
        }
                  
        private void OnNameTextChanged(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter){
                if (this.Column == null) return;
                this.Column.name = NameTextBox.Text;
                OnChanged(true);
            }
        }

        private void OnShowCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (this.Column == null) return;
            this.Column.show = ShowCheckBox.IsChecked.Value;
            if(throwChange) OnChanged(true);
        }

        private void OnChanged(object rebuild)
        {
            if (Changed != null) Changed((bool)rebuild);
        }
        
    }
}

