using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LoopConditionPanel.xaml
    /// </summary>
    public partial class LoopConditionPanel : ScrollViewer
    {
        public Instruction Instruction { get; set; }

        public String conditions { get; set; }

        public LoopConditionItemPanel ActiveLoopConditionItemPanel { get; set; }

        public TransformationTreeItem Loop { get; set; }

        public LoopUserDialogTemplate LoopUserTemplate { get; set; }

        public Kernel.Service.TransformationTreeService TransformationTreeService { get; set; }

        public  ChangeEventHandler Changed;

        public ObservableCollection<Kernel.Domain.LoopCondition> liste 
        {
            get 
            {
                if(Loop != null) return Loop.loopConditionsChangeHandler.Items;
                if(LoopUserTemplate != null) return LoopUserTemplate.loopConditionsChangeHandler.Items;
                return new ObservableCollection<Kernel.Domain.LoopCondition>();
            }
        }

        public LoopConditionPanel()
        {
            InitializeComponent();
        }

        public void DisplayLoopCondition()
        {
            if (TransformationTreeService == null) return;

            this.LoopConditionsPanel.Children.Clear();
            
            if (liste == null) this.Loop.loopConditionsChangeHandler = new PersistentListChangeHandler<Kernel.Domain.LoopCondition>();
            if (liste.Count == 0)
            {
                OnAddConditionItem(null);
                return;
            }
            else
            {
                foreach (Kernel.Domain.LoopCondition condition in liste)
                {
                    if (!string.IsNullOrEmpty(condition.conditions))
                    {
                        condition.instructions = TransformationTreeService.getInstructionObject(condition.conditions);
                    }
                    LoopConditionItemPanel panel = GetNewConditionItemPanel(condition);
                    if (condition.position == 0)
                    {
                        panel.OperatorComboBox.IsEnabled = false;
                        panel.OperatorComboBox.SelectedItem = Operator.AND.ToString();
                    }

                    this.LoopConditionsPanel.Children.Add(panel);
                }
            }
        }
             

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

        public void initLoopConditionHandlers(LoopConditionItemPanel loopConditionItemPanel)
        {
            loopConditionItemPanel.Added += OnAddConditionItem;
            loopConditionItemPanel.Deleted += OnDeleteConditionItem;
            loopConditionItemPanel.ChangeEventHandler += OnChange;
            loopConditionItemPanel.Activated += OnActivate;
        }

        public void OnChange()
        {
            if (Changed != null) Changed();
        }

        public void OnActivate(object item)
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


        public void OnAddConditionItem(object item)
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

        public void OnDeleteConditionItem(object item)
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

        public void FillLoopCondition()
        {
            foreach (UIElement panel in this.LoopConditionsPanel.Children)
            {
                if (!(panel is LoopConditionItemPanel)) continue;
                if (((LoopConditionItemPanel)panel).LoopCondition == null) continue;
                Kernel.Domain.LoopCondition loopCondition = ((LoopConditionItemPanel)panel).FillConditions(this.TransformationTreeService);
                if (!loopCondition.isConditionsEmpty())
                {
                  if(this.Loop != null)   this.Loop.SynchronizeLoopCondition(loopCondition);
                }
                  if (this.LoopUserTemplate != null) this.LoopUserTemplate.SynchronizeLoopCondition(loopCondition);
             }
        }


        public void setValue(object value)
        {
            if (this.ActiveLoopConditionItemPanel == null)
            {
                this.ActiveLoopConditionItemPanel = (LoopConditionItemPanel)this.LoopConditionsPanel.Children[0];
            }
            this.ActiveLoopConditionItemPanel.setValue(value);
        }
    }
}
