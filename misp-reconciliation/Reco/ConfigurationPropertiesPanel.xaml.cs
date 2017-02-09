using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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
        }

        public void displayObject()
        {
            this.NameTextBox.Text = this.EditedObject.name;
            this.groupField.Group = this.EditedObject.group;
            if (this.ReconciliationFilterTemplateService == null) return;
            this.groupField.GroupService = this.ReconciliationFilterTemplateService.GroupService;
            this.groupField.subjectType = SubjectType.RECONCILIATION_FILTER;
            this.groupField.Changed += onGroupFieldChange;
        }

        private void onGroupFieldChange()
        {
            string name =this.groupField.textBox.Text;
            BGroup group = groupField.Group;
            this.EditedObject.isModified = true;
        }

        public  Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
        }
    }
}
