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
    /// Interaction logic for LoopConditionPanel.xaml
    /// </summary>
    public partial class LoopConditionPanel : ScrollViewer
    {
        public Instruction Instruction { get; set; }

        public String conditions { get; set; }

        public LoopConditionItemPanel ActiveLoopConditionItemPanel { get; set; }

        public  ChangeEventHandler Changed;

        public LoopConditionPanel()
        {
            InitializeComponent();
        }

        public void DisplayLoopCondition()
        {
            this.panel.Children.Clear();
            if (this.Instruction == null && String.IsNullOrWhiteSpace(this.conditions))
            {
                OnAddConditionItem(null);
                return;
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

            int countContainerChildren = this.panel.Children.Count + 1;
            panel.Index = countContainerChildren;

            this.panel.Children.Add(panel);
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
                this.panel.Children.Remove((UIElement)item);
                if (this.panel.Children.Count == 0) OnAddConditionItem(null);
                else
                {
                    LoopConditionItemPanel panel = (LoopConditionItemPanel)this.panel.Children[0];
                    panel.OperatorComboBox.IsEnabled = false;
                    panel.OperatorComboBox.SelectedItem = "";
                }
                int index = 1;
                foreach (object pan in this.panel.Children)
                {
                    ((LoopConditionItemPanel)pan).Index = index++;
                }
            }
        }

        
    }
}
