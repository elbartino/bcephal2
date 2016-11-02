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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for RPeriodNamePanel.xaml
    /// </summary>
    public partial class RPeriodNamePanel : ScrollViewer
    {

        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement, notament lorsqu'on set la valeur du TargetItem.
        /// </summary>
        public event ChangeEventHandler Changed;

        /// <summary>
        /// 
        /// </summary>
        public event AddEventHandler Added;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateEventHandler Updated;

        /// <summary>
        /// Evenement déclenché lorsqu'on clique sur le boutton pour supprimer le TargetItem.
        /// </summary>
        public event DeleteEventHandler Deleted;

        /// <summary>
        /// 
        /// </summary>
        public event ActivateEventHandler Activated;

        public event ValidateFormulaEventHandler ValidateFormula;

        #endregion

        public int Index 
        {
            set 
            {
                this.ItemPanel1.Index = this.ItemPanel1 != null ? value : 0;
                this.ItemPanel2.Index = this.ItemPanel2 != null ? value : 0;
            }
        }

        public RPeriodNamePanel()
        {
            InitializeComponent();
            UserInit();
            InitializeHandlers();
        }

        public RPeriodNamePanel(String name) : this(name, new List<PeriodItem>(0))
        {
            
        }

        public RPeriodNamePanel(String name, List<PeriodItem> items)
        {
            InitializeComponent();
            UserInit();
            this.SetPeriodName(name);
            if (items.Count > 0) this.ItemPanel1.Display(items[0]);
            if (items.Count > 1) this.ItemPanel2.Display(items[1]);
            if (items.Count == 0) this.ItemPanel1.SignComboBox.SelectedItem = DateOperator.EQUALS.sign;
            OnSign1Changed(null, null);
            //if (items.Count == 1) this.ItemPanel2.Visibility = System.Windows.Visibility.Collapsed;
            InitializeHandlers();
        }

        private void UserInit()
        {
            this.ItemPanel1.throwevent = false;
            this.ItemPanel2.throwevent = false;
            this.ItemPanel1.Button.Visibility = System.Windows.Visibility.Hidden;
            this.ItemPanel2.Label.Content = "";
            this.ItemPanel2.Label.Visibility = System.Windows.Visibility.Hidden;

            this.ItemPanel1.SignComboBox.ItemsSource = new String[] { 
                DateOperator.EQUALS.sign, DateOperator.AFTER.sign, DateOperator.AFTER_OR_EQUALS.sign,DateOperator.BEFORE.sign, DateOperator.BEFORE_OR_EQUALS.sign};
            this.ItemPanel1.SignComboBox.SelectedItem = DateOperator.AFTER_OR_EQUALS.sign;

            this.ItemPanel2.SignComboBox.ItemsSource = new String[] { DateOperator.BEFORE.sign, DateOperator.BEFORE_OR_EQUALS.sign};
            this.ItemPanel2.SignComboBox.SelectedItem = DateOperator.BEFORE_OR_EQUALS.sign;
            this.ItemPanel1.throwevent = true;
            this.ItemPanel2.throwevent = true;
        }

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.ItemPanel1.Deleted += OnDelete;
            this.ItemPanel2.Deleted += OnDelete;

            this.ItemPanel1.Activated += OnActivate;
            this.ItemPanel2.Activated += OnActivate;

            this.ItemPanel1.Added += OnAdd;
            this.ItemPanel2.Added += OnAdd;

            this.ItemPanel1.ValidateFormula += OnValidateFormula;
            this.ItemPanel2.ValidateFormula += OnValidateFormula;

            this.ItemPanel1.Updated += OnUpdate;
            this.ItemPanel2.Updated += OnUpdate;
            
            this.ItemPanel1.SignComboBox.SelectionChanged += OnSign1Changed;
        }

        private void OnUpdate(object item)
        {
            if (Updated != null)
            {
                RPeriodItemPanel panel = (RPeriodItemPanel)item;
                panel.PeriodItem.name = GetPeriodName();
                Updated(item);
            }
        }

        private void OnValidateFormula(object item)
        {
            if (ValidateFormula != null)
            {
                RPeriodItemPanel panel = (RPeriodItemPanel)item;
                panel.PeriodItem.name = GetPeriodName();
                ValidateFormula(item);
            }
        }

        private void OnAdd(object item)
        {
            if (Added != null)
            {
                RPeriodItemPanel panel = (RPeriodItemPanel)item;
                panel.PeriodItem.name = GetPeriodName();
                Added(item);
            }
        }

        private void OnActivate(object item)
        {
            
        }

        private void OnDelete(object item)
        {
            if (Deleted != null)
            {
                RPeriodItemPanel panel = (RPeriodItemPanel)item;
                if (this.ItemPanel2.PeriodItem != null)
                {
                    this.ItemPanel2.PeriodItem.name = GetPeriodName();
                    Deleted(this.ItemPanel2);
                }
                if (this.ItemPanel1.PeriodItem != null)
                {
                    this.ItemPanel1.PeriodItem.name = GetPeriodName();
                    Deleted(this.ItemPanel1);
                }                
            }
        }

        private void OnSign1Changed(object sender, SelectionChangedEventArgs e)
        {
            String sign = this.ItemPanel1.SignComboBox.SelectedItem.ToString();
            if (sign != null && (DateOperator.EQUALS.sign.Equals(sign) || DateOperator.BEFORE.sign.Equals(sign) || DateOperator.BEFORE_OR_EQUALS.sign.Equals(sign)))
            {
                this.ItemPanel2.Visibility = System.Windows.Visibility.Collapsed;
                this.ItemPanel1.Button.Visibility = System.Windows.Visibility.Visible;

                if (this.ItemPanel2.PeriodItem != null)
                {
                    this.ItemPanel2.PeriodItem.name = GetPeriodName();
                    if(Deleted != null) Deleted(this.ItemPanel2);
                    this.ItemPanel2.PeriodItem = null;
                }
            }
            else
            {
                this.ItemPanel2.Visibility = System.Windows.Visibility.Visible;
                this.ItemPanel1.Button.Visibility = System.Windows.Visibility.Hidden;
            }
        }



        public void SetPeriodName(string name)
        {
            this.ItemPanel1.Label.Content = name;
        }

        public string GetPeriodName()
        {
            return this.ItemPanel1.Label.Content.ToString();
        }

        public void UpdateInterval(PeriodInterval interval, int position)
        {
            
            String name = interval.periodName.name;
            bool add = false;
            if (this.ItemPanel1.PeriodItem == null)
            {
                this.ItemPanel1.PeriodItem = new PeriodItem(position++);
                add = true;
            }
            this.ItemPanel1.PeriodItem.name = name;
            this.ItemPanel1.PeriodItem.formula = null;
            this.ItemPanel1.PeriodItem.operatorSign = DateOperator.AFTER_OR_EQUALS.sign;
            this.ItemPanel1.PeriodItem.valueDateTime = interval.periodFromDateTime;
            this.ItemPanel1.Display(this.ItemPanel1.PeriodItem);
            if (add) OnAdd(this.ItemPanel1);
            else OnUpdate(this.ItemPanel1);

            if (this.ItemPanel2.PeriodItem == null) this.ItemPanel2.PeriodItem = new PeriodItem(position++);
            this.ItemPanel2.PeriodItem.name = name;
            this.ItemPanel2.PeriodItem.formula = null;
            this.ItemPanel2.PeriodItem.operatorSign = DateOperator.BEFORE_OR_EQUALS.sign;
            this.ItemPanel2.PeriodItem.valueDateTime = interval.periodToDateTime;
            this.ItemPanel2.Display(this.ItemPanel2.PeriodItem);
            if (add) OnAdd(this.ItemPanel2);
            else OnUpdate(this.ItemPanel2);
        }

    }
}
