using Misp.Kernel.Application;
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

        #region Properties

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        public MeasureService MeasureService { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetMeasureService(); } }

        public ReconciliationFilterTemplateService ReconciliationFilterTemplateService { get; set;}

        public ReconciliationFilterTemplate EditedObject { get; set; }

        public event ChangeItemEventHandler ItemChanged;

        #endregion


        #region Constructors

        public ConfigurationPropertiesPanel()
        {
            InitializeComponent();
            UserInitialization();            
        }
        
        #endregion


        #region Operations

        public void displayObject()
        {
            this.NameTextBox.Text = this.EditedObject.name;
            this.groupField.Group = this.EditedObject.group;
            this.BalanceFormulaComboBox.SelectedItem = this.EditedObject.balanceFormulaEnum != null ? this.EditedObject.balanceFormulaEnum.label : "";
            this.UseDebitCreditCheckBox.IsChecked = this.EditedObject.useDebitCredit.HasValue && this.EditedObject.useDebitCredit.Value;
            this.visibleInShortcutCheckbox.IsChecked = this.EditedObject.visibleInShortcut;
            if (this.ReconciliationFilterTemplateService == null) return;
            this.groupField.GroupService = this.ReconciliationFilterTemplateService.GroupService;
            this.groupField.subjectType = SubjectType.RECONCILIATION_FILTER;
            this.groupField.Changed += onGroupFieldChange;
        }

        #endregion


        #region Initializations

        private void UserInitialization()
        {
            List<Kernel.Domain.Measure> measures = this.MeasureService.getAllLeafts();
            this.MeasureComboBox.ItemsSource = measures;

            this.BalanceFormulaComboBox.ItemsSource = new String[]
            {
                BalanceFormula.LEFT_MINUS_RIGHT.label,
                BalanceFormula.LEFT_PLUS_RIGHT.label
            };
            this.visibleInShortcutCheckbox.IsChecked = true;

            this.visibleInShortcutCheckbox.Unchecked += OnChooseVisibility;
            this.visibleInShortcutCheckbox.Checked += OnChooseVisibility;

            this.UseDebitCreditCheckBox.Checked += OnUseDebitCreditChecked;
            this.UseDebitCreditCheckBox.Unchecked += OnUseDebitCreditChecked;
            this.BalanceFormulaComboBox.SelectionChanged += OnChooseBalanceFormula;
        }

        #endregion


        #region Handlers

        private void OnUseDebitCreditChecked(object sender, RoutedEventArgs e)
        {
            this.EditedObject.useDebitCredit = this.UseDebitCreditCheckBox.IsChecked;
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void OnChooseBalanceFormula(object sender, SelectionChangedEventArgs e)
        {
            this.EditedObject.balanceFormulaEnum = BalanceFormula.getByLabel(this.BalanceFormulaComboBox.SelectedItem.ToString());
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }


        private void OnChooseVisibility(object sender, RoutedEventArgs e)
        {
            this.EditedObject.visibleInShortcut = this.visibleInShortcutCheckbox.IsChecked.Value;
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        protected void onGroupFieldChange()
        {
            string name = groupField.textBox.Text;
            BGroup group = groupField.Group;
            this.EditedObject.group = group;
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }    

        #endregion
        
    }
}
