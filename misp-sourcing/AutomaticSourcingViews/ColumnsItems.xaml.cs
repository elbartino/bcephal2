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
    /// Interaction logic for ColumnsItems.xaml
    /// </summary>
    public partial class ColumnsItems : UserControl
    {
      
        #region Events

        public event OnRemoveColumnItemEventHandler OnRemoveColumnItem;
        public delegate void OnRemoveColumnItemEventHandler(object item);

        public event OnNewColumnItemEventHandler OnNewColumnItem;
        public delegate void OnNewColumnItemEventHandler();

        public event OnSelectionChangeEventHandler OnSelectColumnItem;
        public delegate void OnSelectionChangeEventHandler(object item);

        public event OnSelectionTargetChangeEventHandler OnSelectTarget;
        public delegate void OnSelectionTargetChangeEventHandler(object item, object operatorValue);
        #endregion


        public bool isAutomaticTarget { get; set; }
        #region Constructor

        public ColumnsItems()
        {
            InitializeComponent();
            this.comboBoxOperator.ItemsSource = new Object[] {TargetItem.Operator.AND
                    ,TargetItem.Operator.OR.ToString(),TargetItem.Operator.NOT};
            this.comboBoxOperator.SelectedItem = TargetItem.Operator.AND;
            this.comboBoxOperator.Visibility = Visibility.Collapsed;
        }
   

        #endregion

        #region Properties

        private int index;

        public ColumnTargetItem ColumnTarget { get; set; }

        public int Index 
        { 
            get { return index; } 
            set 
            { 
                index = value; 
                this.Label.Content = "Columns " + index;
            } 
        }

        public Label Label { get {return this.label;} }
        public Button Button { get { return this.button; } }
        public ComboBox ComboBox { get { return this.comboBoxColunm; } }
        public ComboBox ComboBoxColumn { get { return this.ComboBoxColumn; } }
        
        bool update = true;

        #endregion

        #region Operations

        public ColumnsItems(bool isAutomaticTarget) : this()
        {
            this.comboBoxOperator.Visibility = isAutomaticTarget ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Construit une nouvelle instance de TagItemPnel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public ColumnsItems(int index) : this()
        {
            this.Index = index;
            InitializeHandlers();
        }

        public ColumnsItems(int index,bool isAutomaticTarget)
            : this(isAutomaticTarget)
        {
            this.Index = index;
            this.isAutomaticTarget = isAutomaticTarget;
            InitializeHandlers();
        }

        public ColumnsItems(ColumnTargetItem item): this()
        {
            InitializeHandlers();
            Display(item);
        }

        public ColumnsItems(ColumnTargetItem item,bool isAutomaticTarget)
            : this(item)
        {
            this.comboBoxOperator.Visibility = isAutomaticTarget ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Display(ColumnTargetItem item)
        {
            update = false;
            this.ColumnTarget = item;
            if (item != null) this.Index = item.columnIndex + 1;
            update = true;
        }
               
        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.button.Click +=button_Click;
            this.comboBoxColunm.SelectionChanged +=comboBoxColunm_SelectionChanged;
            this.comboBoxOperator.SelectionChanged += comboBoxOperation_SelectionChanged;
            
        }

        public void customizeForAutomaticTarget() 
        {
            this.comboBoxOperator.Visibility = Visibility.Visible;
        }

        private void comboBoxOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnSelectTarget != null && this.ComboBox.SelectedItem != null) OnSelectTarget(this.ComboBox.SelectedItem, ((ComboBox)sender).SelectedItem);
        }

        private void comboBoxColunm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                if (OnSelectTarget != null) OnSelectTarget(((ComboBox)sender).SelectedItem, this.comboBoxOperator.SelectedItem);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (OnRemoveColumnItem != null) OnRemoveColumnItem(this);
        }



        #endregion

        
    }
}
