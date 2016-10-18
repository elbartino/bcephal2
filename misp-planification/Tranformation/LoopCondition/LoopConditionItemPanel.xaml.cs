using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.Table;
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
        public Kernel.Ui.Base.ChangeItemEventHandler Updated;
        public ActivateEventHandler Activated;

        public LoopCalculatedValuePanel ActiveLoopCalucatedValue;

        public Kernel.Domain.LoopCondition LoopCondition { get; set; }

        public bool trow = false;

        private bool isViewDetailsHided { get; set; }
               

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
            this.LoopCalutedValue.filterScopePanel.DisplayScope(null);
            this.LoopCalutedValue.periodPanel.DisplayPeriod(null, true);
            this.LoopCalutedValue.periodPanel.CustomizeForAutomaticSourcing();
          
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
            if(item != null && item is Kernel.Domain.LoopCondition)
            {
                this.LoopCondition = (Kernel.Domain.LoopCondition)item;
                DisplayLoopCondition(this.LoopCondition);

            }
            else
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
            this.LoopCalutedValue.ChangeEventHandler += onChange;
            this.LoopCalutedValue.filterScopePanel.Changed += onChange;
            this.LoopCalutedValue.filterScopePanel.ItemChanged += OnFilterScopeChanged;
            this.LoopCalutedValue.periodPanel.Changed += onChange;
            this.LoopCalutedValue.periodPanel.ItemChanged += OnPeriodChanged;
            this.LoopCalutedValue.periodPanel.ItemDeleted += OnPeriodDeleted;
            this.LoopCalutedValue.filterScopePanel.ItemDeleted += OnFilterScopeDeleted;
            this.LoopCalutedValue.CellMeasurePanel.Added += OnCellMeasureChanged;
            this.LoopCalutedValue.CellMeasurePanel.Updated += OnCellMeasureChanged;
            
            
        }

        private void OnCellMeasureChanged(object item)
        {
            if(item is CellMeasurePanel){
                FillLoopProperties(((CellMeasurePanel)item).CellMeasure);
                if (isViewDetailsHided) HideDetailsView(false);
            }
        }

        public Kernel.Domain.LoopCondition FillConditions(Kernel.Service.TransformationTreeService TransformaionTreeService)
        {
            this.LoopCondition.openBracket = getBrackets()[0];
            this.LoopCondition.closeBracket = getBrackets()[1];
            this.LoopCondition.operatorType = this.OperatorComboBox.SelectedItem != null ? this.OperatorComboBox.SelectedItem.ToString() : Operator.AND.name;
            this.LoopCondition.comment = this.CommentTextBlock.Text.Trim();
            this.LoopCondition.conditions = TransformaionTreeService.getInstructionString(this.LoopCalutedValue.FillCondition());
            return LoopCondition;
        }

        private void OnFilterScopeDeleted(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;

            Target cellScope = this.LoopCalutedValue.filterScopePanel.Scope;
            if (cellScope == null) cellScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            cellScope.SynchronizeDeleteTargetItem(targetItem);
            isDeleted = true;
            FillLoopProperties(cellScope);
            this.LoopCalutedValue.filterScopePanel.DisplayScope(cellScope);
            if (isViewDetailsHided) HideDetailsView(false);
            isDeleted = false;
        }

        private void OnPeriodDeleted(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;

            Period period = this.LoopCalutedValue.periodPanel.Period;
            if (period == null) period = new Period();
            period.SynchronizeDeletePeriodItem(periodItem);
            FillLoopProperties(period);
            this.LoopCalutedValue.periodPanel.DisplayPeriod(period);
            if (isViewDetailsHided) HideDetailsView(false);
        }

        private void DisplayLoopCondition(Kernel.Domain.LoopCondition loopCondition)
        {
            trow = false;
            this.CommentTextBlock.Text = loopCondition.comment;
            this.OperatorComboBox.SelectedItem = loopCondition.operatorType;
            this.OpenBracketComboBox.SelectedItem = loopCondition.openBracket;
            this.CloseBracketComboBox.SelectedItem = loopCondition.closeBracket;
            this.Index = loopCondition.position +1;
            CellProperty cell = loopCondition.cellProperty;
            if (cell == null) cell = new CellProperty();
            this.LoopCalutedValue.ChangeEventHandler += onChange;
            this.LoopCalutedValue.periodPanel.DisplayPeriod(cell.period);
            this.LoopCalutedValue.filterScopePanel.DisplayScope(cell.cellScope);
            this.LoopCalutedValue.CellMeasurePanel.Display(cell.cellMeasure);
            if (!string.IsNullOrEmpty(loopCondition.conditions)) 
            {
                this.LoopCalutedValue.DisplayInstructions(loopCondition.instructions);
            }
            trow = true;
        }


        private void FillLoopProperties(object item) 
        {
            //setCurrentLoopCondition();

            if (this.LoopCondition == null)
            {
                this.LoopCondition = new Kernel.Domain.LoopCondition();
                this.LoopCondition.position = -1;
                this.LoopCondition.operatorType = this.OperatorComboBox.SelectedItem.ToString();
            }

            if (this.LoopCondition.cellProperty == null) 
            {
                this.LoopCondition.cellProperty = new CellProperty();
            }
            if (item is Period) 
            {
                this.LoopCondition.cellProperty.period = (Period)item;
            }
            else if (item is Target) 
            {
                this.LoopCondition.cellProperty.cellScope = (Target)item;
            }
            else if (item is CellMeasure)
            {
                this.LoopCondition.cellProperty.cellMeasure = (CellMeasure)item;
            }
            else if (item is Operator) 
            {
                this.LoopCondition.operatorType = ((Operator)item).name;
            }
            else if (item is String[])
            {
                String[] brackets = (String[])item;
                this.LoopCondition.openBracket = brackets[0];
                this.LoopCondition.closeBracket = brackets[1];
            }
            if (Updated != null && !isDeleted) Updated(this.LoopCondition);
            if (Deleted != null && isDeleted) Deleted(this.LoopCondition);
        }

        private void OnPeriodChanged(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;

            Period period = this.LoopCalutedValue.periodPanel.Period;
            if (period == null) period = new Period();
            PeriodItem itemUpdated = period.SynchronizePeriodItems(periodItem);
            FillLoopProperties(period);
            this.LoopCalutedValue.periodPanel.DisplayPeriod(period);
            if (isViewDetailsHided) HideDetailsView(false);
        }

        private void OnFilterScopeChanged(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            
            Target cellScope = this.LoopCalutedValue.filterScopePanel.Scope;
            if (cellScope == null) cellScope = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            cellScope.SynchronizeTargetItems(targetItem);
            FillLoopProperties(cellScope);
            this.LoopCalutedValue.filterScopePanel.DisplayScope(cellScope);
            if (isViewDetailsHided) HideDetailsView(false);
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
            HideDetailsView();
        }

        private void OnShowDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView(false);
        }

        private void HideDetailsView(bool hideDetails= true) 
        {
            isViewDetailsHided = hideDetails;
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
            if (trow && ChangeEventHandler != null)
            {
                if (!isDeleted)
                {
                    FillLoopProperties(null);
                }
                ChangeEventHandler();
            }
        }

        public void setCurrentLoopCondition() 
        {
            if (this.LoopCondition == null)
            {
                this.LoopCondition = new Kernel.Domain.LoopCondition();
                this.LoopCondition.position = -1;
                this.LoopCondition.operatorType = this.OperatorComboBox.SelectedItem.ToString();
            }
        }

        private void OnComment(object sender, RoutedEventArgs e)
        {
            this.CommentPopup.IsOpen = true;
        }

        private void OnCommentChange(object sender, TextChangedEventArgs e)
        {
            if (trow)
            {
                onChange();
                refreshCommentIcon();
            }
        }
        private bool isDeleted = false;
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null)
            {
                isDeleted = true;
                Deleted(this);
            }
            onChange();
            isDeleted = false;
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
            if (trow)
            {
                FillLoopProperties(getBrackets());
                onChange();
            }
        }

        private String[] getBrackets() 
        {
            string openBracket = this.OpenBracketComboBox.SelectedItem != null ? this.OpenBracketComboBox.SelectedItem.ToString() : "";
            string closeBracket = this.CloseBracketComboBox.SelectedItem != null ? this.CloseBracketComboBox.SelectedItem.ToString() : "";
            return new String[] {openBracket,closeBracket};
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
                this.LoopCalutedValue.filterScopePanel.SetTargetValue((Kernel.Domain.AttributeValue)item);
            }
            else if (item is TransformationTreeItem) 
            {
                TransformationTreeItem loop = (TransformationTreeItem)item;
                if (loop.IsLoop)
                {
                    if (loop.IsScope)
                    {
                        this.LoopCalutedValue.filterScopePanel.SetLoopValue(loop);
                    }
                    else if (loop.IsPeriod)
                    {
                        this.LoopCalutedValue.periodPanel.SetLoopValue(loop);
                    }
                }
            }
            else if (item is PeriodInterval) 
            {
                this.LoopCalutedValue.periodPanel.SetPeriodInterval((PeriodInterval)item);
            }
            else if (item is List<PeriodInterval>)
            {
                List<PeriodInterval> listInterval = ((List<PeriodInterval>)item);
                if (listInterval.Count == 0)  return;
                String periodName = listInterval[0].periodName.name;
                this.LoopCalutedValue.periodPanel.SetPeriodItemName(periodName);
            }
            else if (item is PeriodName) 
            {
                this.LoopCalutedValue.periodPanel.SetPeriodItemName(((PeriodName)item).name);
            }
            onChange();
          
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
