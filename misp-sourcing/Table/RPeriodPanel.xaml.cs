using Misp.Kernel.Application;
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
    /// Interaction logic for RPeriodPanel.xaml
    /// </summary>
    public partial class RPeriodPanel : UserControl
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

        public bool IsReadOnly { get; set; }

        public Period Period { get; set; }

        public RPeriodNamePanel ActiveNamePanel { get; set; }

        public PeriodName DefaultPeriodName { get; set; }

        public Hyperlink PeriodHyperlink { get; private set; }

        protected bool forReport = false;

        protected bool forAutomaticSourcing = false;

        #endregion


        #region Constructors

        public RPeriodPanel()
        {
            InitializeComponent();
            BuildPeriodHyperlink();
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


        #endregion
        

        #region Operations

        public void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
            if (NewPeriodTextBlockRow != null) NewPeriodTextBlock.Visibility = readOnly ? Visibility.Collapsed : Visibility.Visible;
            if (this.panel.Children.Count > 0)
            {
                foreach (UIElement element in this.panel.Children)
                {
                    if (element is RPeriodItemPanel) 
                    {
                        ((RPeriodItemPanel)element).SetReadOnly(this.IsReadOnly);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void DisplayPeriod(Period period, bool isTableView = false,bool readOnly = false)
        {
            if (DefaultPeriodName == null)
            {
                PeriodName name = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetPeriodNameService().getRootPeriodName();
                DefaultPeriodName = name.getDefaultPeriodName();
            }

            this.Period = period;
            this.panel.Children.Clear();
            if(forAutomaticSourcing) this.tagFormula.Visibility = System.Windows.Visibility.Collapsed;
            if (readOnly) this.NewPeriodTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            if ((period == null || period.itemListChangeHandler.Items.Count <= 0) && !this.IsReadOnly)
            {
                if (DefaultPeriodName != null) this.ActiveNamePanel = new RPeriodNamePanel(DefaultPeriodName.name);
                else this.ActiveNamePanel = new RPeriodNamePanel();
                AddNamePanel(this.ActiveNamePanel);
                return;
            }

            Dictionary<String, List<PeriodItem>> dictionary = period != null ? period.AsDictionary() : new Dictionary<String, List<PeriodItem>>(0);
            int index = 0;
            foreach (String name in dictionary.Keys)
            {
                List<PeriodItem> items;
                dictionary.TryGetValue(name, out items);
                RPeriodNamePanel itemPanel = new RPeriodNamePanel(name, items);
                if(this.IsReadOnly) itemPanel.SetReadOnly(this.IsReadOnly);
                itemPanel.Index = index;
                AddNamePanel(itemPanel);
                index++;
            }
            if (this.panel.Children.Count == 0 && !this.IsReadOnly)
            {
                this.ActiveNamePanel = new RPeriodNamePanel();
                this.ActiveNamePanel.SetReadOnly(readOnly);
                AddNamePanel(this.ActiveNamePanel);
            }
        }

        
        
        /// <summary>
        /// Définit la valeur du TargetItem en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du TargetItem en cour d'édition</param>
        public void SetPeriodItemName(string name)
        {
            RPeriodNamePanel panel = GetPeriodNamePanel(name);
            if (panel != null) panel.SetPeriodName(name);
            else
            {
                panel = new RPeriodNamePanel(name, new List<PeriodItem>(0));
                panel.Index = this.panel.Children.Count;
                AddNamePanel(panel);
            }
        }

        public void SetPeriodInterval(PeriodInterval interval)
        {
            if (interval.periodName == null)
            {
                Kernel.Util.MessageDisplayer.DisplayInfo("Unvalid Period interval", "Period name cannot be null !");
                return;
            }

            String name = interval.periodName.name;
            RPeriodNamePanel panel = GetPeriodNamePanel(name);
            if (panel != null)
            {
                int position = Period != null ? Period.itemListChangeHandler.Items.Count : 0;
                panel.UpdateInterval(interval, position);
            }
            else
            {
                int position = Period != null ? Period.itemListChangeHandler.Items.Count : 0;
                List<PeriodItem> items = new List<PeriodItem>(0);
                PeriodItem item = new PeriodItem();
                item.name = name;
                item.position = position++;
                item.value = interval.periodFromDateTime.ToShortDateString();
                item.operatorSign = DateOperator.AFTER_OR_EQUALS.sign;
                items.Add(item);

                item = new PeriodItem();
                item.name = name;
                item.position = position++;
                item.value = interval.periodToDateTime.ToShortDateString();
                item.operatorSign = DateOperator.BEFORE_OR_EQUALS.sign;
                items.Add(item);

                panel = new RPeriodNamePanel(name, items);
                AddNamePanel(panel);
                OnAdded(panel.ItemPanel1);
                OnAdded(panel.ItemPanel2);
            }
            
        }

                
        public void SetLoopValue(TransformationTreeItem loop)
        {
            String name = loop.name;
            RPeriodNamePanel panel = GetPeriodNamePanel(loop);
            
            if (panel != null)
            {
                int position = Period != null ? Period.itemListChangeHandler.Items.Count : 0;
                //panel.UpdateInterval(interval, position);
            }
            else
            {
                int position = Period != null ? Period.itemListChangeHandler.Items.Count : 0;
                List<PeriodItem> items = new List<PeriodItem>(0);
                PeriodItem item = new PeriodItem();
                item.name = name;
                item.loop = loop;
                item.position = position++;
                item.operatorSign = DateOperator.EQUALS.sign;
                items.Add(item);
                
                panel = new RPeriodNamePanel(name, items);
                if (loop.IsPeriod)
                {
                    panel.ItemPanel1.OperatorCol.Width = new GridLength(200, GridUnitType.Auto);
                    panel.ItemPanel2.OperatorCol.Width = new GridLength(200, GridUnitType.Auto);
                    panel.ItemPanel1.SignComboBox.Width = 100;
                }
                AddNamePanel(panel);
                OnAdded(panel.ItemPanel1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RPeriodNamePanel GetPeriodNamePanel(string name)
        {
            foreach (object item in this.panel.Children)
            {
                RPeriodNamePanel panel = (RPeriodNamePanel)item;
                String text = panel.ItemPanel1.Label.Content.ToString();
                if (text.ToUpper().Equals(name.ToUpper())) return panel;
            }
            return null;
        }

        public RPeriodNamePanel GetPeriodNamePanel(TransformationTreeItem loop)
        {
            foreach (object item in this.panel.Children)
            {
                RPeriodNamePanel panel = (RPeriodNamePanel)item;
                String text = panel.ItemPanel1.Label.Content.ToString();
                if (loop.IsPeriod)
                {
                    setLoopOperatorSize(panel);
                }
                if (panel.ItemPanel1.PeriodItem != null
                        && panel.ItemPanel1.PeriodItem.loop != null
                        && (panel.ItemPanel1.PeriodItem.loop.Equals(loop) || panel.ItemPanel1.PeriodItem.loop.name.ToUpper().Equals(loop.name.ToUpper()))) return panel;
            }
            return null;
        }

        private void setLoopOperatorSize(RPeriodNamePanel panel) 
        {
            panel.ItemPanel1.OperatorCol.Width = new GridLength(200, GridUnitType.Auto);
            panel.ItemPanel2.OperatorCol.Width = new GridLength(200, GridUnitType.Auto);
            panel.ItemPanel1.SignComboBox.Width = 100;
        }
        
        protected void AddNamePanel(RPeriodNamePanel namePanel)
        {
            namePanel.Added += OnAdded;
            namePanel.Updated += OnUpdated;
            namePanel.Deleted += OnDeleted;
            namePanel.Activated += OnActivated;
            namePanel.ValidateFormula += OnValidateFormula;
            bool isLoop =namePanel.ItemPanel1.PeriodItem != null && namePanel.ItemPanel1.PeriodItem.loop != null && namePanel.ItemPanel1.PeriodItem.loop.IsPeriod;
            if (isLoop) 
            {
                setLoopOperatorSize(namePanel);
            }
            this.panel.Children.Add(namePanel);
        }


        #endregion


        #region Handlers

        private void OnValidateFormula(object item)
        {
            OnUpdated(item);
        }

        private void OnActivated(object item) { }

        private void OnAdded(object item)
        {
            RPeriodItemPanel panel = (RPeriodItemPanel)item;
            if (this.Period == null) this.Period = GetNewPeriod();
            if (panel.PeriodItem != null)
            {
                this.Period.AddPeriodItem(panel.PeriodItem);
                OnChanged(panel.PeriodItem);
            }
        }

        private void OnUpdated(object item)
        {
            RPeriodItemPanel panel = (RPeriodItemPanel)item;
            if (this.Period == null) this.Period = GetNewPeriod();
            if (this.Period.itemListChangeHandler.Items.Contains(panel.PeriodItem))
                this.Period.UpdatePeriodItem(panel.PeriodItem);
            else this.Period.AddPeriodItem(panel.PeriodItem);
            OnChanged(panel.PeriodItem);
        }

        private void OnDeleted(object item)
        {
            RPeriodItemPanel panel = (RPeriodItemPanel)item;
            if (panel.PeriodItem == null) return;
            if (this.Period == null) this.Period = GetNewPeriod();
            if (panel.PeriodItem.isInValidDefault()) 
            {
                if (this.panel.Children.Count > 1 && panel.Index >= 0)
                    this.panel.Children.RemoveAt(panel.Index);
                return;
            }

            if (this.Period.Contains(panel.PeriodItem))
            {
                if (ItemDeleted != null && panel.PeriodItem != null) ItemDeleted(panel.PeriodItem);
            }
            else
            {
                
            }
        }


        private void OnChanged(object item)
        {
            if (this.Period == null) this.Period = GetNewPeriod();
            if (ItemChanged != null && item != null) ItemChanged(item);
         }


        private Period GetNewPeriod()
        {
            Period period = new Period();
            return period;
        }

        #endregion


    }
}
