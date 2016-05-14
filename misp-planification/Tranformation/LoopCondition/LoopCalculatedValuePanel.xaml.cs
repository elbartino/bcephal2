using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Planification.Tranformation.InstructionControls;
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
    /// Interaction logic for LoopCalculatedValuePanel.xaml
    /// </summary>
    public partial class LoopCalculatedValuePanel : Grid
    {
        public Kernel.Domain.TransformationTreeItem Loop;

        public ActivateEventHandler Activated;

        public LoopCalculatedValuePanel()
        {
            InitializeComponent();
            CellMeasurePanel.CustomizeForLoopCondition();
            periodPanel.CustomizeForLoop();
            Loop = new Kernel.Domain.TransformationTreeItem();
            Loop.IsLoop = true;
            DisplayCondition();
        }

        protected void initHandlers(ExpressionPanel panel)
        {
            panel.Added += OnAddCondition;
            panel.Deleted += OnDeleteCondition;
            panel.ChangeEventHandler += OnChange;
            panel.GotFocus += OnGotFocus;
            panel.MouseDown += OnMouseDown;
            periodPanel.DisplayPeriod(null, true);
            filterScopePanel.DisplayScope(null);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        protected void DisplayCondition()
        {
            //if (Loop == null) { Reset(); trow = true; return; }
            
            this.ConditionPanel.Children.Clear();
            //this.ConditionPanel.Children.Add(ConditionLabel);

            if (this.Loop.Instruction == null && String.IsNullOrWhiteSpace(this.Loop.conditions))
            {
                OnAddCondition(null);
                return;
            }
            //if (this.Loop.Instruction == null && !String.IsNullOrWhiteSpace(this.Loop.conditions))
            //{
            //    this.Loop.Instruction = TransformationTreeService.getInstructionObject(this.Loop.conditions);
            //}
            //foreach (Instruction child in this.Loop.Instruction.getIfInstruction().subInstructions)
            //{
            //    foreach (ConditionItem item in child.conditionItems) this.ConditionPanel.Children.Add(GetNewExpressionPanel(item));
            //}
            //if (this.ConditionPanel.Children.Count == 1) OnAddCondition(null);
            //else
            //{
            //    ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[1];
            //    panel.OperatorComboBox.IsEnabled = false;
            //    panel.OperatorComboBox.SelectedItem = "";
            //}
        }

        public void Reset()
        {
            //this.TypeTextBox.Text = "";
            //this.NameTextBox.Text = "";
            //this.RankingTextBox.Text = "";
            //this.IncreaseButton.IsChecked = true;
            //this.ValueField.Display(null);

            this.ConditionPanel.Children.Clear();
            //this.ConditionPanel.Children.Add(ConditionLabel);

            //ranking = null;
        }

        private void OnAddCondition(object item)
        {
            ExpressionPanel panel = GetNewExpressionPanel(null);
            this.ConditionPanel.Children.Add(panel);
            if (this.ConditionPanel.Children.Count == 1)
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
                if (this.ConditionPanel.Children.Count == 0) OnAddCondition(null);
                else
                {
                    ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[0];
                    panel.OperatorComboBox.IsEnabled = false;
                    panel.OperatorComboBox.SelectedItem = "";
                }
            }
        }

        private void OnChange()
        {
         
        }

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
    }
}
