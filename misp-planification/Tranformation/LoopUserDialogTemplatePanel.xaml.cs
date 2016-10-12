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

namespace Misp.Planification.Tranformation
{
    /// <summary>
    /// Interaction logic for LoopUserDialogTemplatePanel.xaml
    /// </summary>
    public partial class LoopUserDialogTemplatePanel : Grid
    {
        public Instruction Instruction { get; set; }

        public String conditions { get; set; }

        public ChangeEventHandler Changed;

        public bool trow;

        public bool trowHelpMessage;

        public LoopUserDialogTemplatePanel()
        {
            InitializeComponent();
            InitializeHandlers(); 
            Display();
        }

        private void InitializeHandlers() 
        {
            this.LoopConditionsPanel.Changed += OnChange;
            
            this.ActiveCheckbox.Checked += OnActiveChecked;
            this.ActiveCheckbox.Unchecked += OnActiveChecked;
            
            this.OnePossibleChoiceCheckbox.Checked += OnePossibleChoiceChecked;
            this.OnePossibleChoiceCheckbox.Unchecked += OnePossibleChoiceChecked;
            
            this.EditMessageTextBox.TextChanged += OnEditMessageChanged;
            this.HelpMessageTextBox.TextChanged += OnHelpMessageChanged;
        }

        private void OnHelpMessageChanged(object sender, TextChangedEventArgs e)
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }

        private void OnEditMessageChanged(object sender, TextChangedEventArgs e)
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }

        private void OnActiveChecked(object sender, RoutedEventArgs e)
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }

        private void OnePossibleChoiceChecked(object sender, RoutedEventArgs e)
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }

        private void OnChange()
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }


        public void Display() 
        {
            this.LoopConditionsPanel.Instruction = this.Instruction;
            this.LoopConditionsPanel.conditions =  this.conditions;
            this.LoopConditionsPanel.DisplayLoopCondition();
        }

      
    }
}
