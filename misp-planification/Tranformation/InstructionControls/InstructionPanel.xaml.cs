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

namespace Misp.Planification.Tranformation.InstructionControls
{
    /// <summary>
    /// Interaction logic for InstructionPanel.xaml
    /// </summary>
    public partial class InstructionPanel : StackPanel
    {
        public ChangeEventHandler ChangeEventHandler;
        public Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Kernel.Ui.Base.ChangeItemEventHandler Added;

        #region Events
        public event OnCloseSlideDialogEventHandler OnCloseSlideDialog;
        public event OnCloseTransformationTableDialogEventHandler OnCloseTransformationTableDialog;


        public delegate void OnCloseSlideDialogEventHandler();
        public delegate void OnCloseTransformationTableDialogEventHandler();
        #endregion  

        public bool isIfBloc { get; set; }
        public bool isThenBloc { get; set; }
        public bool isElseBloc { get; set; }

        public bool trow = false;
        
        public InstructionPanel()
        {
            InitializeComponent();
            initHandlers(this.IfItemPanel);
            initHandlers(this.ThenOrElseItemPanel);
        }

        public Instruction Fill()
        {
            Instruction instruction = null;
            if (isIfBloc)
            {
                instruction = new Instruction(Instruction.IF, Instruction.END);
                Instruction condition = new Instruction(Instruction.COND, Instruction.END);
                condition.position = 0;
                condition.args = "";
                instruction.subInstructions.Add(condition);
                foreach (UIElement panel in this.Children)
                {
                    if (!(panel is ExpressionPanel)) continue;
                    condition.args += " " + ((ExpressionPanel)panel).FillAsString();
                    condition.conditionItems.Add(((ExpressionPanel)panel).Fill());
                }
                condition.args = condition.args.Trim();
            }
            else
            {
                if (isThenBloc) instruction = new Instruction(Instruction.THEN, Instruction.END);
                if (isElseBloc) instruction = new Instruction(Instruction.ELSE, Instruction.END);

                int position = 0;
                foreach (UIElement panel in this.Children)
                {
                    Instruction child = Fill(panel);
                    if (child == null) continue;
                    child.position = position++;
                    instruction.subInstructions.Add(child);
                }
            }

            return instruction;
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.ThenOrElseItemPanel.SetReadOnly(readOnly);
            this.IfItemPanel.SetReadOnly(readOnly);
        }

        private Kernel.Domain.Instruction Fill(UIElement panel)
        {
            if (panel == null) return null;
            if (panel is ExpressionPanel) return null; 
            if (panel is ThenOrElseItemPanel) return ((ThenOrElseItemPanel)panel).Fill();
            if (panel is BlockPanel) return ((BlockPanel)panel).Fill();
            return null;
        }

        public void Display(Instruction instruction)
        {
            trow = false;
            if (instruction == null || instruction.subInstructions.Count == 0)
            {
                Reset();
                return;
            }

            this.Children.Clear();
            foreach (Instruction child in instruction.subInstructions)
            {
                if (this.isIfBloc)
                {
                    foreach (ConditionItem item in child.conditionItems) this.Children.Add(GetNewExpressionPanel(item)); 
                    if (child.conditionItems.Count == 0) this.Children.Add(GetNewExpressionPanel(null));
                    if (this.Children.Count > 0)
                    {
                        ExpressionPanel panel = (ExpressionPanel)this.Children[0];                    
                        panel.OperatorComboBox.IsEnabled = false;
                        panel.OperatorComboBox.SelectedItem = null;
                    }
                }
                else
                {
                    UIElement panel = GetNewSubItemPanel(child);
                    if (panel == null) continue;
                    this.Children.Add(panel);
                }
            }            
            trow = true;
        }

        public ExpressionPanel GetNewExpressionPanel(ConditionItem item)
        {
            ExpressionPanel panel = new ExpressionPanel();
            panel.Display(item);
            panel.Height = 30;
            initHandlers(panel);
            return panel;
        }

        public UIElement GetNewSubItemPanel(Instruction child)
        {
            if (this.isThenBloc || this.isElseBloc)
            {
                if (child != null && child.isBlock())
                {
                    BlockPanel panel = new BlockPanel();
                    panel.Display(child);
                    initHandlers(panel);
                    return panel;
                }
                else
                {
                    ThenOrElseItemPanel panel = new ThenOrElseItemPanel();
                    panel.Display(child);
                    panel.Height = 30;
                    initHandlers(panel);
                    return panel;
                }
            }
            return null;
        }

        public void Reset()
        {
            trow = false;
            this.Children.Clear();
            UIElement panel = null;
            if (this.isIfBloc) panel = GetNewExpressionPanel(null);
            else panel = GetNewSubItemPanel(null);
            if (panel != null) this.Children.Add(panel);
            if (this.isIfBloc && panel is ExpressionPanel)
            {
                ((ExpressionPanel)panel).OperatorComboBox.IsEnabled = false;
                ((ExpressionPanel)panel).OperatorComboBox.SelectedItem = null;
            }
            trow = true;
        }



        public void CustomizeForIf()
        {
            this.isIfBloc = true;
            this.isThenBloc = false;
            this.isElseBloc = false;
            Reset();
        }

        public void CustomizeForThen()
        {
            this.isIfBloc = false;
            this.isThenBloc = true;
            this.isElseBloc = false;
            Reset();
        }

        public void CustomizeForElse()
        {
            this.isIfBloc = false;
            this.isThenBloc = false;
            this.isElseBloc = true;
            Reset();
        }

        protected void initHandlers(ExpressionPanel panel)
        {
            panel.Added += OnAdd;
            panel.Deleted += OnDelete;
            panel.ChangeEventHandler += onChange;
        }

        protected void initHandlers(ThenOrElseItemPanel panel)
        {
            panel.Added += OnAdd;
            panel.Deleted += OnDelete;
            panel.IfActionSelected += OnIfActionSelect;
            panel.ChangeEventHandler += onChange;
        }
                
        protected void initHandlers(BlockPanel panel)
        {
            panel.Added += OnAdd;
            panel.Deleted += OnDelete;
            panel.ChangeEventHandler += onChange;
        }

        private void OnAdd(object item)
        {
            UIElement panel = null;
            if (this.isIfBloc) panel = GetNewExpressionPanel(null);
            else panel = GetNewSubItemPanel(null);
            if (panel == null) return;
            if (item is UIElement)
            {
                int index = this.Children.IndexOf((UIElement)item);
                if (index < 0) this.Children.Add(panel);
                else this.Children.Insert(index + 1, panel);
            }
            else this.Children.Add(panel);
        }

        private void OnDelete(object item)
        {
            if (item is UIElement)
            {
                this.Children.Remove((UIElement)item);
                if (this.Children.Count == 0) Reset();
                else if (this.isIfBloc)
                {
                    UIElement panel = this.Children[0];
                    if (panel != null && panel is ExpressionPanel)
                    {
                        ((ExpressionPanel)panel).OperatorComboBox.IsEnabled = false;
                        ((ExpressionPanel)panel).OperatorComboBox.SelectedItem = "";
                    }
                }
            }
        }

        private void OnIfActionSelect(object item)
        {
            BlockPanel panel = new BlockPanel();
            panel.Display(null);
            //panel.Height = 100;
            initHandlers(panel);
            if (item is UIElement)
            {
                int index = this.Children.IndexOf((UIElement)item);
                if (index < 0 || index >= this.Children.Count) this.Children.Add(panel);
                else this.Children.Insert(index, panel);
                this.Children.Remove((UIElement)item);
            }
            else this.Children.Add(panel);
        }

        public void onChange()
        {
            if (trow && ChangeEventHandler != null) ChangeEventHandler();
        }


    }
}
