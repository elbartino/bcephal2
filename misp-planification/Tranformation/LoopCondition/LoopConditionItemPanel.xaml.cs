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

namespace Misp.Planification.Tranformation.LoopCondition
{
    /// <summary>
    /// Interaction logic for LoopConditionItemPanel.xaml
    /// </summary>
    public partial class LoopConditionItemPanel : Grid
    {
        public ChangeEventHandler ChangeEventHandler;
        public Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Kernel.Ui.Base.ChangeItemEventHandler Added;
        public ActivateEventHandler Activated;

        public LoopCalculatedValuePanel ActiveLoopCalucatedValue;

        private Target scope;

        public bool trow = false;

        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                this.Arg1Label.Content = "Condition " + index;
            }
        }

        private int index;

        public LoopConditionItemPanel()
        {
            InitializeComponent();
            this.OperatorComboBox.ItemsSource = Instruction.lOGICAL_OPERATORS;
            this.OpenBracketComboBox.ItemsSource = Instruction.OPEN_BRACKETS;
            this.CloseBracketComboBox.ItemsSource = Instruction.CLOSE_BRACKETS;
            this.OperatorComboBox.SelectedItem = "AND";
            HideDetailsView();
            initHandlers();
            scope = null;
        }

         /// <summary>
        /// Construit une nouvelle instance de TargetItemPanel
        /// </summary>
        /// <param name="index">Index du panel</param>
        public LoopConditionItemPanel(int index)
            : this()
        {
            this.Index = index;
        }


        public void Display(object item)
        {
            if (item == null)
            {
                Reset();
                return;
            }
            trow = true;
        }

        public void Reset()
        {
            trow = false;
            this.OperatorComboBox.SelectedItem = "";
            this.OpenBracketComboBox.SelectedItem = "";
            this.CloseBracketComboBox.SelectedItem = "";

            this.CommentTextBlock.Text = "";
            refreshCommentIcon();
            //LoopCalutedValue.filterScopePanel.DisplayScope(null,false);
            
            //LoopCalutedValue.periodPanel.DisplayPeriod(null);

            trow = true;
        }

        protected void initHandlers()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;

            this.OpenBracketComboBox.SelectionChanged += onChange;
            this.CloseBracketComboBox.SelectionChanged += onChange;
            this.OperatorComboBox.SelectionChanged += onChange;
            
            
            this.CommentTextBlock.TextChanged += OnCommentChange;
            this.CommentPopup.Opened += OnCommentPopupOpened;

            this.CommentButton.Checked += OnComment;
            this.NoCommentButton.Checked += OnComment;

            this.ShowDetailsButton.Click += OnShowDetails;
            this.HideDetailsButton.Click += OnHideDetails;

            this.Arg1Label.GotFocus += OnGotFocus;
            this.AddButton.GotFocus += OnGotFocus;
            this.DeleteButton.GotFocus += OnGotFocus;
            this.OpenBracketComboBox.GotFocus += OnGotFocus;
            this.OperatorComboBox.GotFocus += OnGotFocus;
            this.CommentTextBlock.GotFocus += OnGotFocus;
            this.ShowDetailsButton.GotFocus += OnGotFocus;

            this.Arg1Label.MouseDown += OnMouseDown;
            this.AddButton.MouseDown += OnMouseDown;
            this.DeleteButton.MouseDown += OnMouseDown;
            this.OpenBracketComboBox.MouseDown += OnMouseDown;
            this.OperatorComboBox.MouseDown += OnMouseDown;
            this.CommentTextBlock.MouseDown += OnMouseDown;
            this.ShowDetailsButton.MouseDown += OnMouseDown;

            this.LoopCalutedValue.Activated += OnActivate;
            this.LoopCalutedValue.filterScopePanel.ItemChanged += OnFilterScopeChanged;
            this.LoopCalutedValue.periodPanel.ItemChanged += OnPeriodItemChanged;
            this.LoopCalutedValue.filterScopePanel.ItemDeleted += OnFilterScopeDeleted;
            this.LoopCalutedValue.periodPanel.ItemDeleted += OnPeriodItemDeleted;
        }

        private void OnPeriodItemDeleted(object item)
        {
            if (item == null || !(item is PeriodItem))
            {
                return;
            }

            PeriodItem perioditem = (PeriodItem)item;
            Period period = this.LoopCalutedValue.periodPanel.Period;
            if (period == null) period = new Period();
            period.SynchronizeDeletePeriodItem(perioditem);
            this.LoopCalutedValue.periodPanel.DisplayPeriod(period);
        }

        private void OnFilterScopeDeleted(object item)
        {
            if (item == null || !(item is TargetItem))
            {
                return;
            }

            TargetItem targetItem = (TargetItem)item;
            Target scope = this.LoopCalutedValue.filterScopePanel.Scope;
            if (scope == null) scope = GetNewScope();
            scope.SynchronizeDeleteTargetItem(targetItem);
            this.LoopCalutedValue.filterScopePanel.DisplayScope(scope);
        }

        private void OnPeriodItemChanged(object item)
        {
            if(item == null || !(item is PeriodItem))
            {
                return;
            }

            PeriodItem perioditem = (PeriodItem)item;
            Period period = this.LoopCalutedValue.periodPanel.Period;
            if (period == null) period = new Period();
            period.SynchronizePeriodItems(perioditem);
            this.LoopCalutedValue.periodPanel.DisplayPeriod(period);
        }

        private void OnFilterScopeChanged(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            Target scope = this.LoopCalutedValue.filterScopePanel.Scope;
            if (scope == null) scope = GetNewScope();
            scope.SynchronizeTargetItems(targetItem);
            this.LoopCalutedValue.filterScopePanel.DisplayScope(scope);
        }

        private void OnActivate(object item)
        {
            ActivateComponent(item);
        }

       
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ActiveHandler();
        }

        #region Activation Method
            private void ActivateComponent(object item) 
            {
                if (Activated != null)
                {
                    if (item is LoopCalculatedValuePanel)
                    {
                        this.ActiveLoopCalucatedValue = (LoopCalculatedValuePanel)item;
                    }
                    Activated(this);
                }
            }

            private void ActiveHandler() 
            {
                if (Activated != null) Activated(this);
            }
        #endregion

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            ActiveHandler();
        }

        private void OnHideDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView(true);
        }

        private void OnShowDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView(false);
        }

        private void HideDetailsView(bool hideDetails= true) 
        {
            LoopCalutedValue.Visibility = hideDetails ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.HideDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;            
            this.ShowDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void OnCommentPopupOpened(object sender, EventArgs e)
        {
            this.CommentTextBlock.Focus();
        }

        private void refreshCommentIcon()
        {
            bool hasComment = !string.IsNullOrWhiteSpace(this.CommentTextBlock.Text);
            this.CommentButton.Visibility = hasComment ? Visibility.Visible : Visibility.Hidden;
            this.NoCommentButton.Visibility = hasComment ? Visibility.Hidden : Visibility.Visible;
        }

        public void onChange()
        {
            if (trow && ChangeEventHandler != null) ChangeEventHandler();
        }

        private void OnComment(object sender, RoutedEventArgs e)
        {
            this.CommentPopup.IsOpen = true;
        }

        private void OnCommentChange(object sender, TextChangedEventArgs e)
        {
            onChange();
            refreshCommentIcon();
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null) Deleted(this);
            onChange();
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Added != null) Added(this);
        }

        private void onChange(object sender, TextChangedEventArgs e)
        {
            onChange();
        }

        private void onChange(object sender, SelectionChangedEventArgs e)
        {
            onChange();
        }

        public void setValue(object item) 
        {
            if (item is Measure)
            {
                CellMeasure cellMeasure = new CellMeasure();
                cellMeasure.measure = (Measure)item;
                this.LoopCalutedValue.CellMeasurePanel.SetValue(cellMeasure);
            }
            if (item is AttributeValue)
            {

                TargetItem targetitem = new TargetItem((AttributeValue)item, ((AttributeValue)item).attribut, "");
                this.LoopCalutedValue.filterScopePanel.SetTargetValue((AttributeValue)item);
            }
        }

        /// <summary>
        /// New instance of Target
        /// </summary>
        /// <returns></returns>
        public Target GetNewScope()
        {
            Target scope = new Target();
            scope.targetType = Target.TargetType.COMBINED.ToString();
            scope.type = Target.Type.OBJECT_VC.ToString();
            return scope;
        }


    }
}
