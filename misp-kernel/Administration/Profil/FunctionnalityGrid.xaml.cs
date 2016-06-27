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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for FunctionnalityGrid.xaml
    /// </summary>
    public partial class FunctionnalityGrid : DataGrid
    {
        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;

        public FunctionnalityGrid()
        {
            InitializeComponent();
            InitializeFunctionGrid();
            this.SelectionChanged += onSelectionchange;
        }

        private void onSelectionchange(object sender, SelectionChangedEventArgs e)
        {
            if (ChangeHandler != null) ChangeHandler();
        }

        private void InitializeFunctionGrid()
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1) });
            gridFactory.AppendChild(checkboxFactory);
            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;
            this.RowHeaderTemplate = template;

            var brushConverter = new System.Windows.Media.BrushConverter();
            System.Windows.Media.Brush bruch = (System.Windows.Media.Brush)brushConverter.ConvertFrom(System.Windows.Media.Brushes.LightBlue.Color.ToString());
            this.AlternatingRowBackground = bruch;
            this.AlternatingRowBackground.Opacity = 0.3;

            for (int i = 0; i < getColumnCount(); i++)
            {
                DataGridColumn column = getColumnAt(i);
                column.Header = getColumnHeaderAt(i);
                column.Width = getColumnWidthAt(i);
                if (column is DataGridBoundColumn)
                {
                    ((DataGridBoundColumn)column).Binding = getBindingAt(i);
                }
                this.Columns.Add(column);
            }

            this.SelectionChanged += onFunctionGridSelectionChanged;
        }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected int getColumnCount()
        {
            return 3;
        }

        protected string getTitle() { return "Right Function"; }

        /// <summary>
        /// Build and returns the column at index position
        /// </summary>
        /// <param name="index">The position of the column</param>
        /// <returns></returns>
        protected DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridCheckBoxColumn();
                case 2: return new DataGridCheckBoxColumn();
                default: return new DataGridTextColumn();
            }
        }


        /// <summary>
        /// Column Label
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Function";
                case 1: return "View";
                case 2: return "Edit";
                default: return "";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return 100;
                case 2: return 100;
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string getBindingNameAt(int index)
        {
            switch (index)
            {
                case 0: return "function";
                case 1: return "view";
                case 2: return "edit";
                default: return "oid";
            }
        }

        protected bool isReadOnly(int index)
        {
            switch (index)
            {
                case 0: return false;
                case 1: return true;
                case 2: return true;
                default: return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected System.Windows.Data.Binding getBindingAt(int index)
        {
            return new System.Windows.Data.Binding(getBindingNameAt(index));
        }

        private void onFunctionGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
