using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using Misp.Planification.Tranformation.InstructionControls;
using Misp.Planification.Tranformation.LoopCondition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for LoopDialog.xaml
    /// </summary>
    public partial class LoopDialog : Window
    {

        #region Properties

        public TransformationTreeItem Loop { get; set; }

        private Measure ranking;

        public TransformationTreeService TransformationTreeService { get; set; }

        public LoopConditionItemPanel ActiveLoopConditionItemPanel { get; set; }

        public bool trow = false;

        #endregion


        #region Constructor

        public LoopDialog()
        {
            InitializeComponent();
            InitializeHandlers();
            scrollCondition.Visibility = System.Windows.Visibility.Collapsed;
        }
        
        #endregion


        #region Operations

        public void initializeSideBar()
        {
            initializeSideBarData();
            initializeSideBarHandlers();
        }

        public void Dispose()
        {
            //this.ReportPanel.Dispose();
            Close();
        }

        public void SetValue(object value)
        {
            if (TabValues.IsSelected)
            {
                if (value is Measure)
                {
                    this.ranking = (Measure)value;
                    this.RankingTextBox.Text = ranking.name;
                    OnChange();
                    return;
                }
                String valueType = GetLoopType(value);
                if (this.TypeTextBox.Text == null || this.TypeTextBox.Text.Equals(""))
                {
                    this.TypeTextBox.Text = valueType;
                }
                if (!this.TypeTextBox.Text.Equals(valueType))
                {
                    if (this.ValueField.IsEmpty())
                    {
                        this.TypeTextBox.Text = valueType;
                    }else{
                        MessageDisplayer.DisplayWarning("Wrong selection", "The selection is not conform to loop type!");
                        return;
                    }
                }
                this.ValueField.SetValue(value);
            }
            else if (TabConditions.IsSelected)
            {
                if(this.ActiveLoopConditionItemPanel == null)
                {
                    this.ActiveLoopConditionItemPanel =(LoopConditionItemPanel)this.ConditionPanel.Children[0];
                }
                this.ActiveLoopConditionItemPanel.setValue(value);
            }
        }

        private String GetLoopType(object value)
        {
            if (value is Target) return Kernel.Application.ParameterType.SCOPE.ToString();
            else if (value is PeriodName || value is PeriodInterval) return Kernel.Application.ParameterType.PERIOD.ToString();
            else if (value is IList)
            {
                IList list = (IList)value;
                if (list.Count > 0)
                {
                    Object ob = list[0];
                    if (ob is Target) return Kernel.Application.ParameterType.SCOPE.ToString();
                    else if (ob is PeriodName || ob is PeriodInterval) return Kernel.Application.ParameterType.PERIOD.ToString();
                }
            }
            return "";
        }

        public void DisplayItem()
        {
            trow = false;
            //this.ReportPanel.TreeItem = Loop;
            if (Loop == null) { Reset(); trow = true; return; }

            List<TransformationTreeItem> loops = this.Loop.GetAscendentsLoopTree(true).ToList();
            //this.ReportPanel.loops = loops;
            //this.ReportPanel.DisplayItem();

            ObservableCollection<Object>  objects = new ObservableCollection<Object>(loops);
            objects.Insert(0, "Previous Loop");
            this.LoopComboBox.ItemsSource = objects;
            this.LoopComboBox.SelectedIndex = 0;
            TransformationTreeItem item = GetLoopByOid(this.Loop.refreshLoopOid);
            if (item != null) this.LoopComboBox.SelectedItem = item;

           // DisplayCondition();
            DisplayLoopCondition();
            this.ranking = Loop.ranking;
            this.TypeTextBox.Text = Loop.type != null ? Loop.type : "";
            this.NameTextBox.Text = Loop.name;
            this.RankingTextBox.Text = ranking != null ? ranking.name : "";
            this.IncreaseButton.IsChecked = Loop.increase;
            this.DecreaseButton.IsChecked = !Loop.increase;
            this.ValueField.Display(Loop);
            this.SaveButton.IsEnabled = false;

            trow = true;
        }

        private TransformationTreeItem GetLoopByOid(int? oid)
        {
            if (!oid.HasValue) return null;
            //foreach (TransformationTreeItem loop in this.ReportPanel.loops)
            //{
            //    if (!loop.oid.HasValue) continue;
            //    if (loop.oid.Value == oid.Value) return loop;
            //}
            return null;
        }

        protected void DisplayCondition()
        {
            this.ConditionPanel.Children.Clear();
            this.ConditionPanel.Children.Add(ConditionLabel);

            if (this.Loop.Instruction == null && String.IsNullOrWhiteSpace(this.Loop.conditions))
            {
                OnAddCondition(null);
                return;
            }
            if (this.Loop.Instruction == null && !String.IsNullOrWhiteSpace(this.Loop.conditions))
            {
                this.Loop.Instruction = TransformationTreeService.getInstructionObject(this.Loop.conditions);
            }
            foreach (Instruction child in this.Loop.Instruction.getIfInstruction().subInstructions)
            {
                foreach (ConditionItem item in child.conditionItems) this.ConditionPanel.Children.Add(GetNewExpressionPanel(item));                
            }
            if (this.ConditionPanel.Children.Count == 1) OnAddCondition(null);
            else
            {
                ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[1];
                panel.OperatorComboBox.IsEnabled = false;
                panel.OperatorComboBox.SelectedItem = "";
            }
        }

        protected void DisplayLoopCondition()
        {
            this.LoopConditionsPanel.Children.Clear();
            if (this.Loop.Instruction == null && String.IsNullOrWhiteSpace(this.Loop.conditions))
            {
                OnAddConditionItem(null);
                return;
            }
        }

        public void Reset()
        {
            this.TypeTextBox.Text = "";
            this.NameTextBox.Text = "";            
            this.RankingTextBox.Text = "";
            this.IncreaseButton.IsChecked = true;
            this.ValueField.Display(null);

            this.ConditionPanel.Children.Clear();
            this.ConditionPanel.Children.Add(ConditionLabel);

            ranking = null;
        }

        public void FillItem()
        {
            if (Loop == null) Loop = new TransformationTreeItem(true);
            Loop.name = this.NameTextBox.Text.Trim();
            Loop.increase = this.IncreaseButton.IsChecked.Value;
            Loop.ranking = ranking;
            Loop.type = this.TypeTextBox.Text;
            this.ValueField.Fill();

            Loop.Instruction = FillCondition();
            if (Loop.Instruction == null) Loop.conditions = null;
            else Loop.conditions = TransformationTreeService.getInstructionString(Loop.Instruction);

            Loop.refreshLoopOid = null;
            Object item = this.LoopComboBox.SelectedItem;
            if(item != null && item is TransformationTreeItem) Loop.refreshLoopOid = ((TransformationTreeItem)item).oid;
        }

        protected Instruction FillCondition()
        {
            Instruction instruction = new Instruction(Instruction.BLOCK, Instruction.END);

            Instruction ifInstruction = new Instruction(Instruction.IF, Instruction.END);
            ifInstruction.position = 0;
            Instruction condition = new Instruction(Instruction.COND, Instruction.END);
            condition.position = 0;
            condition.args = "";
            ifInstruction.subInstructions.Add(condition);
            foreach (UIElement panel in this.ConditionPanel.Children)
            {
                if (!(panel is ExpressionPanel)) continue;
                condition.args += " " + ((ExpressionPanel)panel).FillAsString();
                ConditionItem item = ((ExpressionPanel)panel).Fill();
                if (!item.isArgsEmpty()) condition.conditionItems.Add(item);
            }

            if (condition.conditionItems.Count <= 0) return null;

            condition.args = condition.args.Trim();
            instruction.subInstructions.Add(ifInstruction);

            Instruction thenInstruction = new Instruction(Instruction.THEN, Instruction.ENDTHEN);
            thenInstruction.position = 1;
            Instruction action = new Instruction(Instruction.CONTINUE, Instruction.END);
            action.position = 0;
            action.args = "";
            thenInstruction.subInstructions.Add(action);
            instruction.subInstructions.Add(thenInstruction);

            Instruction elseInstruction = new Instruction(Instruction.ELSE, Instruction.ENDELSE);
            elseInstruction.position = 2;
            action = new Instruction(Instruction.STOP, Instruction.END);
            action.position = 0;
            action.args = "";
            elseInstruction.subInstructions.Add(action);
            instruction.subInstructions.Add(elseInstruction);

            return instruction;
        }

        public bool SaveLoop()
        {
            return true;
            
            //return this.ReportPanel.SaveReport();
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.DeleteButton.Click += OnDeleteButtonClick;
            this.DeleteAllValuesButton.Click += OnDeleteAllValuesButtonClick;

            this.NameTextBox.TextChanged += onChange;
            this.RankingTextBox.TextChanged += onChange;
            this.IncreaseButton.Click += onChange;
            this.DecreaseButton.Click += onChange;
            this.ValueField.ChangeEventHandler += OnChange;
            //this.ReportPanel.ChangeEventHandler += OnChange;

            this.LoopComboBox.SelectionChanged += OnSelectedLoopChange;
        }
        
        private void OnSelectedLoopChange(object sender, SelectionChangedEventArgs e)
        {
            if (trow) OnChange();
        }

        protected void initHandlers(LoopConditionItemPanel panel) 
        {
            panel.ChangeEventHandler += OnChange;
        }

        protected void initHandlers(ExpressionPanel panel)
        {
            panel.Added += OnAddCondition;
            panel.Deleted += OnDeleteCondition;
            panel.ChangeEventHandler += OnChange;
        }

        #region LoopConditionItemPanel

        public LoopConditionItemPanel GetNewConditionItemPanel(object item)
        {
            LoopConditionItemPanel panel = new LoopConditionItemPanel();
            panel.Margin = new Thickness(0, 0, 0, 10);
            panel.Background = new SolidColorBrush();
            panel.Display(item);
            //panel.Height = 250;
            initLoopConditionHandlers(panel);
            return panel;
        }

        protected void initLoopConditionHandlers(LoopConditionItemPanel loopConditionItemPanel)
        {
            loopConditionItemPanel.Added += OnAddConditionItem;
            loopConditionItemPanel.Deleted += OnDeleteConditionItem;
            loopConditionItemPanel.ChangeEventHandler += OnChange;
            loopConditionItemPanel.Activated += OnActivate;
        }

        private void OnActivate(object item)
        {
            if (item == null) 
            {
                return;
            }

            if (item is LoopConditionItemPanel)
            {
                this.ActiveLoopConditionItemPanel = (LoopConditionItemPanel)item;
            }
        }

        private void OnDeleteConditionItem(object item)
        {
            if (item is UIElement)
            {
                this.LoopConditionsPanel.Children.Remove((UIElement)item);
                if (this.LoopConditionsPanel.Children.Count == 0) OnAddConditionItem(null);
                else
                {
                    LoopConditionItemPanel panel = (LoopConditionItemPanel)this.LoopConditionsPanel.Children[0];
                    panel.OperatorComboBox.IsEnabled = false;
                    panel.OperatorComboBox.SelectedItem = "";
                }
                int index = 1;
                foreach (object pan in this.LoopConditionsPanel.Children)
                {
                    ((LoopConditionItemPanel)pan).Index = index++;
                }
            }
        }

        private void OnAddConditionItem(object item)
        {
            LoopConditionItemPanel panel = GetNewConditionItemPanel(null);
            
            int countContainerChildren = this.LoopConditionsPanel.Children.Count + 1;
            panel.Index = countContainerChildren;

            this.LoopConditionsPanel.Children.Add(panel);
            if (countContainerChildren == 1)
            {
                panel.OperatorComboBox.IsEnabled = false;
                panel.OperatorComboBox.SelectedItem = "";
            }
        }

        #endregion

        public ExpressionPanel GetNewExpressionPanel(ConditionItem item)
        {
            ExpressionPanel panel = new ExpressionPanel();
            panel.Margin = new Thickness(50, 0, 0, 10);
            panel.Background = new SolidColorBrush();
            panel.Display(item);
            panel.Height = 30;
            initHandlers(panel);
            return panel;
        }

        private void OnAddCondition(object item)
        {
            ExpressionPanel panel = GetNewExpressionPanel(null);
            this.ConditionPanel.Children.Add(panel);
            if (this.ConditionPanel.Children.Count == 2)
            {
                panel.OperatorComboBox.IsEnabled = false;
                panel.OperatorComboBox.SelectedItem = "";
            }
        }

        private void OnDeleteCondition(object item)
        {
            if (item is UIElement)
            {
                this.ConditionPanel.Children.Remove((UIElement)item);
                if (this.ConditionPanel.Children.Count == 1) OnAddCondition(null);
                else
                {
                    ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[1];
                    panel.OperatorComboBox.IsEnabled = false;
                    panel.OperatorComboBox.SelectedItem = "";
                }
            }
        }
        
        
        private void OnDeleteAllValuesButtonClick(object sender, RoutedEventArgs e)
        {
            this.ValueField.RemoveAll();
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            this.ranking = null;
            this.RankingTextBox.Text = "";
            this.IncreaseButton.IsChecked = true;
            this.DecreaseButton.IsChecked = false;
        }

        protected void initializeSideBarData()
        {
            SideBar.EntityGroup.ModelService = TransformationTreeService.ModelService;
            SideBar.EntityGroup.InitializeTreeViewDatas();

            Measure rootMeasure = TransformationTreeService.MeasureService.getRootMeasure();
            SideBar.MeasureGroup.MeasureTreeview.DisplayRoot(rootMeasure);

            List<Kernel.Domain.CalculatedMeasure> CalculatedMeasures = TransformationTreeService.CalculatedMeasureService.getAllCalculatedMeasure();
            if (CalculatedMeasures != null)
                SideBar.CalculateMeasureGroup.CalculatedMeasureTreeview.fillTree(new ObservableCollection<CalculatedMeasure>(CalculatedMeasures));

            //Periodicity periodicity = TransformationTreeService.PeriodicityService.getPeriodicity();
            //SideBar.PeriodicityGroup.PeriodicityTreeview.DisplayPeriodicity(periodicity);

            PeriodName RootPeriodName = TransformationTreeService.PeriodNameService.getRootPeriodName();
            SideBar.PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(RootPeriodName);
            
            Target targetAll = TransformationTreeService.ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            SideBar.TargetGroup.TargetTreeview.DisplayTargets(targets);

            List<Target> CustomizedTargets = TransformationTreeService.TargetService.getAll();
            SideBar.CustomizedTargetGroup.TargetTreeview.fillTree(new ObservableCollection<Target>(CustomizedTargets));
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected void initializeSideBarHandlers()
        {            
            SideBar.MeasureGroup.MeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
            SideBar.CalculateMeasureGroup.CalculatedMeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
            SideBar.EntityGroup.EntityTreeview.SelectionChanged += onSelectTargetFromSidebar;
            SideBar.EntityGroup.EntityTreeview.SelectionDoubleClick += onDoubleClickSelectTargetFromSidebar;
            SideBar.CustomizedTargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;
            SideBar.TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;    
            SideBar.PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodNameFromSidebar;
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une mesure sur la sidebar.
        /// Cette opération a pour but d'assigner la mesure sélectionnée 
        /// aux cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La mesure sélectionnée</param>
        protected void onSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && sender is Measure)
            {
                Measure measure = (Measure)sender;
                SetValue(measure);
            }
        }
        
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                object value = null;
                if (sender is Entity) value = sender;
                if (sender is AttributeValue) value = sender;
                if (sender is Kernel.Domain.Attribute)
                {
                    Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                    if (attribute.valueListChangeHandler.Items.Count <= 0) return;
                    List<Kernel.Domain.AttributeValue> values = TransformationTreeService.ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    value = values;
                }
                else value = sender;
                SetValue(value);
            }
        }

        protected void onDoubleClickSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                if (!(sender is AttributeValue))
                {
                    onSelectTargetFromSidebar(sender);
                    return;
                }
                AttributeValue value = (AttributeValue)sender;
                if (value.childrenListChangeHandler.Items.Count <= 0) return;
                SetValue(value.childrenListChangeHandler.Items);
            }
        }


        protected void onSelectPeriodNameFromSidebar(object sender)
        {
            if (sender != null)
            {
                if (sender is PeriodName)
                {
                    PeriodName periodName = (PeriodName)sender;
                    if (periodName.intervalListChangeHandler.Items.Count <= 0) return;
                    object value = periodName.Leafs;                    
                    SetValue(value);
                }
                else if (sender is PeriodInterval)
                {
                    PeriodInterval periodInterval = (PeriodInterval)sender;
                    if (periodInterval.IsLeaf) SetValue(periodInterval);
                    else SetValue(periodInterval.Leafs);
                }
            }
        }


        private void onChange(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        private void onChange(object sender, TextChangedEventArgs e)
        {
            OnChange();
        }

        private void OnChange()
        {
            SaveButton.IsEnabled = true;
        }

        #endregion

    }
}
