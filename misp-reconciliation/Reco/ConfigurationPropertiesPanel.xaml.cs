using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for ConfigurationPropertiesPanel.xaml
    /// </summary>
    public partial class ConfigurationPropertiesPanel : StackPanel
    {

        public ReconciliationFilterTemplateService ReconciliationFilterTemplateService { get; set;}
        /// <summary>
        /// Design en édition
        /// </summary>
        public ReconciliationFilterTemplate EditedObject { get; set; }

        public event ChangeItemEventHandler ItemChanged;

        public ConfigurationPropertiesPanel()
        {
            InitializeComponent();
            this.DCFormulaComboBox.ItemsSource = new String[] 
            {
                DebitCreditFormula.DEBIT_NEGATIVE.label,
                DebitCreditFormula.DEBIT_NOT_NEGATIVE.label
            };
            this.BalanceFormulaComboBox.ItemsSource = new String[]
            {
                BalanceFormula.LEFT_MINUS_RIGHT.label,
                BalanceFormula.LEFT_PLUS_RIGHT.label
            };
            this.visibleInShortcutCheckbox.IsChecked = true;

            this.visibleInShortcutCheckbox.Unchecked += OnChooseVisibility;
            this.visibleInShortcutCheckbox.Checked += OnChooseVisibility;

            this.DCFormulaComboBox.SelectionChanged += OnChooseDCForumula;
            this.BalanceFormulaComboBox.SelectionChanged += OnChooseBalanceFormula;
        }

        private void OnChooseBalanceFormula(object sender, SelectionChangedEventArgs e)
        {
            this.EditedObject.balanceFormulaEnum = BalanceFormula.getByLabel(this.BalanceFormulaComboBox.SelectedItem.ToString());
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void OnChooseDCForumula(object sender, SelectionChangedEventArgs e)
        {
            this.EditedObject.debitCreditFormulaEnum = DebitCreditFormula.getByLabel(this.DCFormulaComboBox.SelectedItem.ToString());
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }


        private void OnChooseVisibility(object sender, RoutedEventArgs e)
        {
            this.EditedObject.visibleInShortcut = this.visibleInShortcutCheckbox.IsChecked.Value;
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        public void displayObject()
        {
            this.NameTextBox.Text = this.EditedObject.name;
            this.groupField.Group = this.EditedObject.group;
            this.BalanceFormulaComboBox.SelectedItem = this.EditedObject.balanceFormulaEnum != null ?  this.EditedObject.balanceFormulaEnum.label : "";
            this.DCFormulaComboBox.SelectedItem = this.EditedObject.debitCreditFormulaEnum != null ? this.EditedObject.debitCreditFormulaEnum.label : "";

            if (this.ReconciliationFilterTemplateService == null) return;
            this.groupField.GroupService = this.ReconciliationFilterTemplateService.GroupService;
            this.groupField.subjectType = SubjectType.RECONCILIATION_FILTER;
            this.groupField.Changed += onGroupFieldChange;
        }



        protected void onGroupFieldChange()
        {
            string name = groupField.textBox.Text;
            BGroup group = groupField.Group;
            this.EditedObject.group = group;
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }      

    }
}
