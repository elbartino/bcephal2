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

        public TransformationTreeItem Loop { get; set; }

        public Kernel.Service.TransformationTreeService TransformationTreeService { get; set; }

        public LoopUserDialogTemplate LoopUserTemplate { get; set; }
        
        public bool trow;
               

        public LoopUserDialogTemplatePanel()
        {
            InitializeComponent();
            InitializeHandlers();           
            trow = false;
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
            OnChange();
        }

        private void OnEditMessageChanged(object sender, TextChangedEventArgs e)
        {
            OnChange();
        }

        private void OnActiveChecked(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        private void OnePossibleChoiceChecked(object sender, RoutedEventArgs e)
        {
            OnChange();
        }

        private void OnChange()
        {
            if (!trow) trow = true;
            else return;
            if (trow && Changed != null) Changed();
        }


        public void Display() 
        {
            if (this.Loop != null)
            {
                this.ActiveCheckbox.IsChecked = this.Loop.userDialogTemplate.active;
                this.OnePossibleChoiceCheckbox.IsChecked = this.Loop.userDialogTemplate.onePossibleChoice;
                this.EditMessageTextBox.Text = this.Loop.userDialogTemplate.message;
                this.HelpMessageTextBox.Text = this.Loop.userDialogTemplate.help;

                this.LoopConditionsPanel.Instruction = this.Loop.userDialogTemplate.Instruction;
                this.LoopConditionsPanel.conditions = this.Loop.userDialogTemplate.conditions;
            }
            this.LoopConditionsPanel.TransformationTreeService = this.TransformationTreeService;
            this.LoopConditionsPanel.DisplayLoopCondition();
            trow = false;
        }



        public void Fill()
        {
            if (this.LoopUserTemplate == null) this.LoopUserTemplate = new LoopUserDialogTemplate();
            this.LoopUserTemplate.active = this.ActiveCheckbox.IsChecked.Value;
            this.LoopUserTemplate.onePossibleChoice = this.OnePossibleChoiceCheckbox.IsChecked.Value;
            this.LoopUserTemplate.message = this.EditMessageTextBox.Text;
            this.LoopUserTemplate.help = this.HelpMessageTextBox.Text;

            this.LoopConditionsPanel.FillLoopCondition();
            this.LoopConditionsPanel.conditions = this.LoopUserTemplate.conditions;
            trow = false;
        }

        public void setValue(object value)
        {
            this.LoopConditionsPanel.setValue(value);
        }
    }
}
