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

        public bool IsReadOnly { get; set; }

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
            if (this.LoopUserTemplate != null)
            {
                this.ActiveCheckbox.IsChecked = this.LoopUserTemplate.active;
                this.OnePossibleChoiceCheckbox.IsChecked = this.LoopUserTemplate.onePossibleChoice;
                this.EditMessageTextBox.Text = this.LoopUserTemplate.message;
                this.HelpMessageTextBox.Text = this.LoopUserTemplate.help;
                this.LoopConditionsPanel.LoopUserTemplate = this.LoopUserTemplate;
                this.LoopConditionsPanel.Instruction = this.LoopUserTemplate.Instruction;
                this.LoopConditionsPanel.conditions = this.LoopUserTemplate.conditions;
            }
            else
            {
                resetComponent();
            }
            if (this.IsReadOnly) SetReadOnly(this.IsReadOnly);
            this.LoopConditionsPanel.TransformationTreeService = this.TransformationTreeService;
            this.LoopConditionsPanel.DisplayLoopCondition();
            trow = false;
        }

        public void SetReadOnly(bool readOnly) 
        {
            this.ActiveCheckbox.IsEnabled = !readOnly;
            this.OnePossibleChoiceCheckbox.IsEnabled = !readOnly;
            this.EditMessageTextBox.IsEnabled = !readOnly;
            this.HelpMessageTextBox.IsEnabled = !readOnly;
            this.LoopConditionsPanel.SetReadOnly(readOnly);
        }

        public void Fill()
        {
            if (this.LoopUserTemplate == null) this.LoopUserTemplate = new LoopUserDialogTemplate();
            this.LoopUserTemplate.active = this.ActiveCheckbox.IsChecked.Value;
            this.LoopUserTemplate.onePossibleChoice = this.OnePossibleChoiceCheckbox.IsChecked.Value;
            this.LoopUserTemplate.message = this.EditMessageTextBox.Text;
            this.LoopUserTemplate.help = this.HelpMessageTextBox.Text;
            this.LoopConditionsPanel.LoopUserTemplate = this.LoopUserTemplate;
            this.LoopConditionsPanel.FillLoopCondition();
            this.LoopConditionsPanel.conditions = this.LoopUserTemplate.conditions;
            trow = false;
        }

        public void setValue(object value)
        {
            this.LoopConditionsPanel.setValue(value);
        }

        public void resetComponent()
        {
            this.ActiveCheckbox.IsChecked = true;
            this.OnePossibleChoiceCheckbox.IsChecked = false;
            this.EditMessageTextBox.Clear();
            this.HelpMessageTextBox.Clear();

            this.LoopConditionsPanel.LoopUserTemplate = null;
            this.LoopConditionsPanel.Instruction = null;
            this.LoopConditionsPanel.conditions = null;
        }
    }
}
