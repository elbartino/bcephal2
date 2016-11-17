using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
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

namespace Misp.Sourcing.AllocationViews
{
    /// <summary>
    /// Interaction logic for LoopDialog.xaml
    /// </summary>
    public partial class AllocationBoxDialog : Window
    {

        #region Properties

        public TransformationTreeItem Loop { get; set; }

        private Measure ranking;

        public TransformationTreeService TransformationTreeService { get; set; }

        public ChangeItemEventHandler SaveEndedEventHandler { get; set; }
        
        //public LoopConditionItemPanel ActiveLoopConditionItemPanel { get; set; }

        public bool trow = false;

        #endregion


        #region Constructor

        public AllocationBoxDialog()
        {
            InitializeComponent();
            InitializeHandlers();
            //scrollCondition.Visibility = System.Windows.Visibility.Collapsed;
            //this.TabConditions.Visibility = System.Windows.Visibility.Collapsed;
            //this.TabUserTemplate.Visibility = System.Windows.Visibility.Collapsed;
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
                //if(this.ActiveLoopConditionItemPanel == null)
                //{
                //    this.ActiveLoopConditionItemPanel =(LoopConditionItemPanel)this.ConditionPanel.Children[0];
                //}
                //this.ActiveLoopConditionItemPanel.setValue(value);
            }
            else if (TabUserTemplate.IsSelected)
            {
                //this.UserTemplatePanel.setValue(value);
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
            FillTabs(TabLoop);

            //DisplayLoopCondition();

            //DisplayLoopUserDialog();
            
            this.ranking = Loop.ranking;
            this.TypeTextBox.Text = Loop.type != null ? Loop.type : "";
            this.NameTextBox.Text = Loop.name;

            this.GridRanking.Visibility = System.Windows.Visibility.Collapsed;

            this.ValueField.Display(Loop);
            this.SaveButton.IsEnabled = false;

            trow = true;
        }

        //private void DisplayLoopUserDialog()
        //{
        //    this.UserTemplatePanel.TransformationTreeService = this.TransformationTreeService;
        //    this.UserTemplatePanel.LoopUserTemplate = this.Loop.userDialogTemplate;
        //    this.UserTemplatePanel.Display();
        //}

        private TransformationTreeItem GetLoopByOid(int? oid)
        {
            if (!oid.HasValue) return null;
            return   this.Loop.getLoopByOid(oid.Value);
        }

        //protected void DisplayCondition()
        //{
        //    this.ConditionPanel.Children.Clear();
        //    this.ConditionPanel.Children.Add(ConditionLabel);

        //    if (this.Loop.Instruction == null && String.IsNullOrWhiteSpace(this.Loop.conditions))
        //    {
        //        OnAddCondition(null);
        //        return;
        //    }
        //    if (this.Loop.Instruction == null && !String.IsNullOrWhiteSpace(this.Loop.conditions))
        //    {
        //        this.Loop.Instruction = TransformationTreeService.getInstructionObject(this.Loop.conditions);
        //    }
        //    foreach (Instruction child in this.Loop.Instruction.getIfInstruction().subInstructions)
        //    {
        //        foreach (ConditionItem item in child.conditionItems) this.ConditionPanel.Children.Add(GetNewExpressionPanel(item));                
        //    }
        //    if (this.ConditionPanel.Children.Count == 1) OnAddCondition(null);
        //    else
        //    {
        //        //ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[1];
        //        //panel.OperatorComboBox.IsEnabled = false;
        //        //panel.OperatorComboBox.SelectedItem = "";
        //    }
        //}

        //protected void DisplayLoopCondition()
        //{
        //    this.LoopConditionsPanel.Children.Clear();
        //    if (this.Loop.loopConditionsChangeHandler == null) this.Loop.loopConditionsChangeHandler = new PersistentListChangeHandler<Kernel.Domain.LoopCondition>();
        //    if (this.Loop.loopConditionsChangeHandler.Items.Count == 0)
        //    {
        //        OnAddConditionItem(null);
        //        return;
        //    }
        //    else 
        //    {
        //        foreach (Kernel.Domain.LoopCondition condition in this.Loop.loopConditionsChangeHandler.Items) 
        //        {
        //            if (!string.IsNullOrEmpty(condition.conditions))
        //            {
        //               condition.instructions = TransformationTreeService.getInstructionObject(condition.conditions);
        //            }
        //            //LoopConditionItemPanel panel = GetNewConditionItemPanel(condition);
        //            if (condition.position == 0)
        //            {
        //                panel.OperatorComboBox.IsEnabled = false;
        //                panel.OperatorComboBox.SelectedItem = Operator.AND.ToString();
        //            }

        //            this.LoopConditionsPanel.Children.Add(panel);
        //        }
        //    }
        //}

     


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
            //this.ValueField.Fill();
            foreach (TransformationTreeLoopValue loopValue in this.ValueField.ValueListChangeHandler.deletedItems)
            {
                this.Loop.ForgetValue(loopValue);
                this.Loop.valueListChangeHandler.originalList.Remove(loopValue);
            }
            foreach (TransformationTreeLoopValue loopValue in this.ValueField.ValueListChangeHandler.newItems)
            {
                this.Loop.AddValue(loopValue);
            }
        }


        public bool SaveLoop()
        {
            //if (this.SaveEndedEventHandler != null) SaveEndedEventHandler(this.Loop);
            this.SaveButton.IsEnabled = false;
            return true;
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
            this.TabLoop.SelectionChanged += OnTabSelectionChanged;
            this.LoopComboBox.SelectionChanged += OnSelectedLoopChange;
        }

        private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl) 
            {
                FillTabs(sender);
            }
        }

        private void FillTabs(object sender) 
        {
            TabControl mainTabLoop = (TabControl)sender;
            if (mainTabLoop.SelectedItem == TabConditions || mainTabLoop.SelectedItem == TabUserTemplate)
            {
                List<TransformationTreeItem> loops = this.Loop.GetAscendentsLoopTree(true).ToList();
                this.SideBar.TreeLoopGroup.Visibility = System.Windows.Visibility.Visible;
                this.SideBar.TreeLoopGroup.TransformationTreeLoopTreeview.fillTree(new ObservableCollection<TransformationTreeItem>(loops));
            }
            else if (mainTabLoop.SelectedItem == TabValues)
            {
                this.SideBar.TreeLoopGroup.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void OnSelectedLoopChange(object sender, SelectionChangedEventArgs e)
        {
            if (trow) OnChange();
        }
           
        #endregion
                
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
            if (TransformationTreeService == null) return;
            SideBar.EntityGroup.InitializeData();
            SideBar.PeriodGroup.InitializeData();
            
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
            SideBar.EntityGroup.Tree.Click += onSelectTargetFromSidebar;
            SideBar.EntityGroup.Tree.DoubleClick += onDoubleClickSelectTargetFromSidebar;

            SideBar.CustomizedTargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;
            SideBar.TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;

            SideBar.PeriodGroup.Tree.Click += onSelectPeriodFromSidebar;
            SideBar.PeriodGroup.Tree.DoubleClick += onDoubleClickSelectPeriodFromSidebar;

            SideBar.TreeLoopGroup.TransformationTreeLoopTreeview.SelectionChanged += OnSelecteLoopFromSidebar;
        }

        private void OnSelecteLoopFromSidebar(object item)
        {
            if (item is TransformationTreeItem) 
            {
                TransformationTreeItem value = new TransformationTreeItem();
                value.oid = ((TransformationTreeItem)item).oid;
                value.name = ((TransformationTreeItem)item).name;
                value.loop = ((TransformationTreeItem)item).loop;
                value.type = ((TransformationTreeItem)item).type;
                SetValue(value);
            }
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected void onSelectTagFromSidebar(object sender)
        {
            if (sender != null)
            {
                object tag = null;
                tag = (sender is CollectionViewGroup) && (sender as CollectionViewGroup).ItemCount > 0 ? (sender as CollectionViewGroup).Items : null;
                if (tag == null) return;
                SetValue(tag);
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
               else if (sender is Kernel.Domain.Attribute) value = sender;
               else if (sender is AttributeValue) value = sender;
               SetValue(value);
            }
        }

        protected void onDoubleClickSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                object value = null;
                if (sender is Entity) value = sender;
                else if (sender is Kernel.Domain.Attribute)
                {
                    Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                    if (attribute.valueListChangeHandler.Items.Count <= 0) value = attribute;
                    else value = TransformationTreeService.ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                }
                else if (sender is AttributeValue)
                {
                    AttributeValue attributeValue = (AttributeValue)sender;
                    if (attributeValue.childrenListChangeHandler.Items.Count <= 0) value = attributeValue;
                    else value = attributeValue.childrenListChangeHandler.Items;
                }
                SetValue(value);
            }
        }


        protected void onSelectPeriodFromSidebar(object sender)
        {
            if (sender != null && (sender is PeriodName || sender is PeriodInterval))
            {
                SetValue(sender);               
            }
        }


        protected void onDoubleClickSelectPeriodFromSidebar(object sender)
        {
            if (sender != null)
            {
                 if (sender is PeriodName)
                 {
                     PeriodName periodName = (PeriodName)sender;
                     if (periodName.intervalListChangeHandler.Items.Count <= 0) SetValue(sender);
                     else SetValue(periodName.intervalListChangeHandler.Items);  
                 }
                 else if (sender is PeriodInterval)
                 {
                     PeriodInterval periodInterval = (PeriodInterval)sender;
                     if (periodInterval.IsLeaf) SetValue(sender);
                     else SetValue(periodInterval.childrenListChangeHandler.Items);
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


    }
}
