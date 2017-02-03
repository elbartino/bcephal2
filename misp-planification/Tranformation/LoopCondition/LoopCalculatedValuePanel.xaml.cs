using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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

        public TransformationTreeService TransformationTreeService;

        public ChangeEventHandler ChangeEventHandler;

        public bool IsReadOnly { get; set; }

        private bool trow;

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
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Activated != null) Activated(this);
        }

        public void DisplayCondition()
        {
            if (Loop == null) { Reset(); trow = true; return; }
            
            this.ConditionPanel.Children.Clear();
            //this.ConditionPanel.Children.Add(ConditionLabel);

            if (this.Loop.Instruction == null && String.IsNullOrWhiteSpace(this.Loop.conditions))
            {
                OnAddCondition(null);
                return;
            }
            if (Loop.loopConditionsChangeHandler.Items.Count == 0) { }
            if (this.Loop.Instruction == null && !String.IsNullOrWhiteSpace(this.Loop.conditions))
            {
                this.Loop.Instruction = TransformationTreeService.getInstructionObject(this.Loop.conditions);
            }
            foreach (Instruction child in this.Loop.Instruction.getIfInstruction().subInstructions)
            {
                foreach (ConditionItem item in child.conditionItems) this.ConditionPanel.Children.Add(GetNewExpressionPanel(item));
            }
            //if (this.ConditionPanel.Children.Count == 1) OnAddCondition(null);
            //else
            //{
            //    ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[1];
            //    panel.OperatorComboBox.IsEnabled = false;
            //    panel.OperatorComboBox.SelectedItem = "";
            //}
        }

        public void DisplayInstructions(Instruction instruction)
        {
            this.ConditionPanel.Children.Clear();

            if (instruction == null)
            {
                OnAddCondition(null);
                return;
            }

            foreach (Instruction child in instruction.getIfInstruction().subInstructions)
            {
                foreach (ConditionItem item in child.conditionItems) this.ConditionPanel.Children.Add(GetNewExpressionPanel(item));
            }
            if (this.ConditionPanel.Children.Count == 0) OnAddCondition(null);
            else
            {
                ExpressionPanel panel = (ExpressionPanel)this.ConditionPanel.Children[0];
                panel.OperatorComboBox.IsEnabled = false;
                panel.OperatorComboBox.SelectedItem = "";
            }
        }

        public void Reset()
        {
            this.CellMeasurePanel.Display(null);
            this.filterScopePanel.DisplayScope(null);
            this.periodPanel.DisplayPeriod(null);
            this.ConditionPanel.Children.Clear();
        }

        private void OnAddCondition(object item)
        {
            ExpressionPanel panel = GetNewExpressionPanel(null);
            panel.Arg1TextBox.IsEnabled = false;
            panel.Arg1TextBox.Text = "Result";
            this.ConditionPanel.Children.Add(panel);
            if (this.ConditionPanel.Children.Count == 1)
            {
                panel.OperatorComboBox.IsEnabled = false;
                panel.OperatorComboBox.SelectedItem = "";
            }
            if (this.IsReadOnly) panel.SetReadOnly(this.IsReadOnly);
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

        public CellProperty FillCellProperty()
        {
            CellProperty cellProperty = new CellProperty();
            cellProperty.cellMeasure = this.CellMeasurePanel.CellMeasure;
            cellProperty.cellScope = this.filterScopePanel.Scope;
            cellProperty.period = this.periodPanel.Period;
            return cellProperty;
        }

        public String FillInstructions()
        {
            return "";
        }

        public Instruction FillCondition()
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


        private void OnChange()
        {
            if (ChangeEventHandler != null) ChangeEventHandler();
        }

        public ExpressionPanel GetNewExpressionPanel(ConditionItem item)
        {
            ExpressionPanel panel = new ExpressionPanel();
            panel.Margin = new Thickness(50, 0, 0, 10);
            panel.Background = new SolidColorBrush();
            panel.DisplayFromLoop(item);
            panel.Height = 30;
            panel.Arg1TextBox.IsEnabled = false;
            initHandlers(panel);
            return panel;
        }

        public void SetReadOnly(bool readOnly) 
        {
            this.IsReadOnly = readOnly;
            if (this.filterScopePanel != null) this.filterScopePanel.DisplayScope(null, false, readOnly);
            if (this.periodPanel != null) this.periodPanel.DisplayPeriod(null, false, readOnly);
            if (this.CellMeasurePanel != null) this.CellMeasurePanel.Display(null, readOnly);
            foreach (UIElement child in this.ConditionPanel.Children)
            {
                if (child is ExpressionPanel) ((ExpressionPanel)child).SetReadOnly(readOnly);
            }
        }
    }
}
