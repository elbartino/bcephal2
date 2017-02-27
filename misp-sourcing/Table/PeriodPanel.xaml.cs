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
    /// Interaction logic for PeriodPanel.xaml
    /// </summary>
    public partial class PeriodPanel : UserControl
    {
                
        #region Events

        /// <summary>
        /// Evenement déclenché lorsqu'il y a un changement sur l'un des ScopeItemPanel.
        /// </summary>
        public event ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        public event OnNewPeriodClickedEventHandler OnNewPeriodClicked;
        public delegate void OnNewPeriodClickedEventHandler();
               
             
        #endregion


        #region Properties

        public Period Period { get; set; }

        public PeriodItemPanel ActiveItemPanel { get; set; }

        public Hyperlink PeriodHyperlink { get; private set; }

        protected bool forReport = false;

        protected bool forAutomaticSourcing = false;

        protected bool isTableView = false;

        public bool IsReadOnly { get; set; }

        #endregion


        #region Constructors

        public PeriodPanel()
        {
            InitializeComponent();
            BuildPeriodHyperlink();

            col.Width = new GridLength(65);

            valueCol.Width = new GridLength(1.3, GridUnitType.Star);
            valueCol.MinWidth = 120;
            FormulaCol.MinWidth = 60;
            FormulaCol.Width = new GridLength(1.3, GridUnitType.Star);
        }

        protected void BuildPeriodHyperlink()
        {
            Run run1 = new Run("New Period Name...");
            PeriodHyperlink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "New Period Name")
            };
            PeriodHyperlink.RequestNavigate += PeriodHyperlink_RequestNavigate;
            NewPeriodTextBlock.Inlines.Add(PeriodHyperlink);
            NewPeriodTextBlock.ToolTip = "Create a new Period name";
        }

        private void PeriodHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (OnNewPeriodClicked != null)
                OnNewPeriodClicked();
        }

        public void CustomizeForReport()
        {
            forReport = true;            
            col.Width = new GridLength(103);
            NewPeriodTextBlockRow.Height = new GridLength(0, GridUnitType.Star);
            valueCol.Width = new GridLength(1.3, GridUnitType.Star);
            valueCol.MinWidth = 100;
            FormulaCol.MinWidth = 60;
            FormulaCol.Width = new GridLength(1.3, GridUnitType.Star);
        }

        public void CustomizeForAutomaticSourcing() 
        {
            forAutomaticSourcing = true;
            tagFormula.Visibility = System.Windows.Visibility.Collapsed;

        }

        #endregion
        

        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void DisplayPeriod(Period period, bool isTableView = false,bool readOnly=false)
        {
            this.Period = period;
            this.panel.Children.Clear();
            if(forAutomaticSourcing) this.tagFormula.Visibility = System.Windows.Visibility.Collapsed;
            if (this.IsReadOnly) this.NewPeriodTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            int index = 1;
            if (period == null || period.itemListChangeHandler.Items.Count == 0)
            {
                if (!this.IsReadOnly)
                {
                    this.ActiveItemPanel = new PeriodItemPanel(index, forReport, forAutomaticSourcing, isTableView);
                    this.ActiveItemPanel.NameTextBox.Text = PeriodName.DEFAULT_DATE_NAME;
                    this.ActiveItemPanel.SetReadOnly(readOnly);
                    AddItemPanel(this.ActiveItemPanel);
                }
                return;
            }
            bool isDefaultDate = false;
            foreach (PeriodItem item in period.itemListChangeHandler.Items)
            {
                isDefaultDate = item.name.Equals(PeriodName.DEFAULT_DATE_NAME, StringComparison.OrdinalIgnoreCase);
                isDefaultDate = isDefaultDate && String.IsNullOrEmpty(item.value);
                PeriodItemPanel itemPanel = new PeriodItemPanel(item, forReport,forAutomaticSourcing,isTableView);
                if(this.IsReadOnly)   itemPanel.SetReadOnly(this.IsReadOnly);
                AddItemPanel(itemPanel);
                index++;
            }
            /*if (!isDefaultDate && this.panel.Children.Count > 1)
            {
                this.ActiveItemPanel = new PeriodItemPanel(index, forReport, forAutomaticSourcing,isTableView);
                AddItemPanel(this.ActiveItemPanel);
            }*/
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetPeriodItemValue(PeriodItem value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (PeriodItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetValue(value);
            this.ActiveItemPanel = null;
        }

       

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetPeriodItemName(string name)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (PeriodItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            if (!forReport)
            {
                if (GetPeriodItemPanel(name) != null)
                {
                    Kernel.Util.MessageDisplayer.DisplayInfo("Duplicate Period name", "Item named: " + name + " already exist!");
                    return;
                }
            }
            PeriodItem currentPeriodItem = this.Period != null ? this.Period.GetPeriodItemByName(name) : null;
            bool duplicatePeriodItem = currentPeriodItem != null && name.ToUpper().Equals(currentPeriodItem.name.ToUpper());
            if (duplicatePeriodItem)
            {
                this.ActiveItemPanel = (PeriodItemPanel)this.panel.Children[currentPeriodItem.position];
            }
            else 
            {
                this.ActiveItemPanel.PeriodItem = null;
            }

            this.ActiveItemPanel.SetPeriodName(name);
        }

        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetPeriodItem(PeriodItem item)
        {
            PeriodItem t = new PeriodItem(item.position, item.name, item.value, item.formula);
            SetPeriodItemValue(t);
            //this.ActiveItemPanel.Display(t);            
            //this.ActiveItemPanel.SetTagName(item.name);
        }


        public void SetPeriodInterval(PeriodInterval interval) 
        {
            if (!forReport)
            {
                if (interval.periodName == null)
                {
                    Kernel.Util.MessageDisplayer.DisplayInfo("Unvalid Period interval", "Period name cannot be null !");
                    return;
                }
                if (GetPeriodItemPanelForInterval(interval.periodName.name) != null)
                {
                    Kernel.Util.MessageDisplayer.DisplayInfo("Duplicate Period name", "Item named: " + interval.periodName.name + " already exist!");
                    return;
                }
            }
            
            PeriodItem item = interval.getFromPeriodItem((forReport ? DateOperator.AFTER_OR_EQUALS.sign : ""));
            SetPeriodItemValue(item);
            if (forReport) 
            {
                item = interval.getToPeriodItem(DateOperator.BEFORE_OR_EQUALS.sign);
                SetPeriodItemValue(item);
            }
        }

        public void SetPeriod(Period period)
        {
            foreach (PeriodItem item in period.itemListChangeHandler.Items)
            {
                SetPeriodItem(item);
            }
        }

        public void SetLoopValue(TransformationTreeItem loop)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (PeriodItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            this.ActiveItemPanel.SetLoop(loop);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PeriodItemPanel GetPeriodItemPanel(string name)
        {
            foreach (object item in this.panel.Children)
            {
                PeriodItemPanel panel = (PeriodItemPanel)item;
                if (panel.NameTextBox.Text != null && panel.NameTextBox.Text.Trim().ToUpper().Equals(name.ToUpper())
                  && !name.ToUpper().Equals(PeriodName.DEFAULT_DATE_NAME.ToUpper())) return panel;
            }
            return null;
        }

        public PeriodItemPanel GetPeriodItemPanelForInterval(string name)
        {
            foreach (object item in this.panel.Children)
            {
                PeriodItemPanel panel = (PeriodItemPanel)item;
                if (panel.NameTextBox.Text != null && panel.NameTextBox.Text.ToUpper().Equals(name.ToUpper())) 
                {
                    if (panel.PeriodItem == null) return null;
                    if (name.ToUpper().Equals(PeriodName.DEFAULT_DATE_NAME.ToUpper()) || String.IsNullOrEmpty(panel.PeriodItem.value)) this.ActiveItemPanel = panel;
                    else return panel;
                }
            }
            return null;
        }

        protected void AddItemPanel(PeriodItemPanel itemPanel)
        {
            //itemPanel.Changed += OnChanged;
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.ValidateFormula += OnValidateFormula;
            this.panel.Children.Add(itemPanel);
        }


        #endregion


        #region Handlers

        private void OnValidateFormula(object item)
        {
            PeriodItemPanel panel = (PeriodItemPanel)item;
            OnChanged(panel.PeriodItem);
        }

        private void OnActivated(object item)
        {
            PeriodItemPanel panel = (PeriodItemPanel)item;
            this.ActiveItemPanel = panel;
        }

        private void OnAdded(object item)
        {
            PeriodItemPanel panel = (PeriodItemPanel)item;
            if (this.Period == null) this.Period = GetNewPeriod();
            this.Period.AddPeriodItem(panel.PeriodItem);
            OnChanged(panel.PeriodItem);
        }

        private void OnUpdated(object item)
        {
            PeriodItemPanel panel = (PeriodItemPanel)item;
            if (this.Period == null) this.Period = GetNewPeriod();
            this.Period.UpdatePeriodItem(panel.PeriodItem);
            OnChanged(panel.PeriodItem);

        }

        private void OnDeleted(object item)
        {
            PeriodItemPanel panel = (PeriodItemPanel)item;
            if (this.Period == null) this.Period = GetNewPeriod();
            if (panel.PeriodItem == null) return;
            panel.PeriodItem.period = this.Period;
            this.panel.Children.Remove(panel);
            if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel && this.panel.Children.Count > 0)
                this.ActiveItemPanel = (PeriodItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            int index = 1;
            foreach (object pan in this.panel.Children)
            {
                ((PeriodItemPanel)pan).Index = index++;
            }
            OnChanged(null);
            if (ItemDeleted != null && panel.PeriodItem != null) ItemDeleted(panel.PeriodItem);
        }

        private void OnChanged(object item)
        {
            if (this.Period == null) this.Period = GetNewPeriod();
            if (this.panel.Children.Count <= this.Period.itemListChangeHandler.Items.Count)
            {
                this.ActiveItemPanel = new PeriodItemPanel(this.Period.itemListChangeHandler.Items.Count + 1, forReport);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
         }


        private Period GetNewPeriod()
        {
            Period period = new Period();
            return period;
        }

        #endregion


        public void CustomizeForLoop()
        {
            
        }

        public void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            this.periodGridHeader.Visibility = !readOnly ? Visibility.Visible : Visibility.Collapsed;
            if (this.panel.Children.Count > 0)
            {
                foreach (UIElement item in this.panel.Children)
                {
                    if (item is PeriodItemPanel)
                    {
                        ((PeriodItemPanel)item).SetReadOnly(this.IsReadOnly);
                    }
                }
            }
        }
    }
}
