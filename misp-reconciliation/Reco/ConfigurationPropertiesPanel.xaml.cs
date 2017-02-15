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

        public ReconciliationContextService ContextService { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationContextService(); } }
        
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
            this.MeasureComboBox.SelectedItem = this.EditedObject.amountMeasure;
            this.RecoTypeComboBox.SelectedItem = this.EditedObject.reconciliationType;
            this.BalanceFormulaComboBox.SelectedItem = this.EditedObject.balanceFormulaEnum != null ? this.EditedObject.balanceFormulaEnum.label : "";
            this.UseDebitCreditCheckBox.IsChecked = this.EditedObject.useDebitCredit.HasValue && this.EditedObject.useDebitCredit.Value;
            this.visibleInShortcutCheckbox.IsChecked = this.EditedObject.visibleInShortcut;
            this.groupField.GroupService = this.Service.GroupService;
            this.groupField.subjectType = SubjectType.RECONCILIATION_FILTER;
            this.groupField.Changed += onGroupFieldChange;
        }

        #endregion


        #region Initializations

        private void UserInitialization()
        {
            Misp.Kernel.Domain.ReconciliationContext context = this.ContextService.getReconciliationContext();

            List<Kernel.Domain.Measure> measures = this.MeasureService.getAllLeafts();
            this.MeasureComboBox.ItemsSource = measures;
            if (context != null && context.amountMeasure != null) this.MeasureComboBox.SelectedItem = context.amountMeasure;

            List<Kernel.Domain.Attribute> types = this.Service.getReconciliationTypes();
            this.RecoTypeComboBox.ItemsSource = types;
            if (context != null && context.defaultRecoTypeAttribute != null) this.RecoTypeComboBox.SelectedItem = context.defaultRecoTypeAttribute;
            

            this.BalanceFormulaComboBox.ItemsSource = new String[]
            {
                BalanceFormula.LEFT_MINUS_RIGHT.label,
                BalanceFormula.LEFT_PLUS_RIGHT.label
            };
            this.visibleInShortcutCheckbox.IsChecked = true;

            this.visibleInShortcutCheckbox.Unchecked += OnHandlingCheckbox;
            this.visibleInShortcutCheckbox.Checked += OnHandlingCheckbox;
            this.UseDebitCreditCheckBox.Checked += OnHandlingCheckbox;
            this.UseDebitCreditCheckBox.Unchecked += OnHandlingCheckbox;


            this.BalanceFormulaComboBox.SelectionChanged += OnHandlingCombobox;
            this.RecoTypeComboBox.SelectionChanged += OnHandlingCombobox;
            this.MeasureComboBox.SelectionChanged += OnHandlingCombobox;
        }

       
        #endregion


        #region Handlers

        private void OnHandlingCheckbox(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox)) return;
            CheckBox checkbox = (CheckBox)sender;
            CheckBoxAction(checkbox);
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void OnHandlingCombobox(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox)) return;
            ComboBox combobox = (ComboBox)sender;
            ComboboxActions(combobox);
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void ComboboxActions(ComboBox combobox)
        {
            if (combobox == MeasureComboBox)
            {
                if (combobox.SelectedItem is Kernel.Domain.Measure)
                {
                    Kernel.Domain.Measure measure = (Kernel.Domain.Measure)combobox.SelectedItem;
                    this.EditedObject.amountMeasure = measure;
                }
            }

            if (combobox == RecoTypeComboBox)
            {
                if (combobox.SelectedItem is Kernel.Domain.Attribute)
                {
                    Kernel.Domain.Attribute attribut = (Kernel.Domain.Attribute)combobox.SelectedItem;
                    this.EditedObject.reconciliationType = attribut;
                }
            }
            if (combobox == BalanceFormulaComboBox) this.EditedObject.balanceFormulaEnum = BalanceFormula.getByLabel(combobox.SelectedItem.ToString());
        }

        private void CheckBoxAction(CheckBox checkBox)
        {
            if (checkBox == this.UseDebitCreditCheckBox)
            {
                this.EditedObject.useDebitCredit = checkBox.IsChecked.Value;
            }
            if (checkBox == this.visibleInShortcutCheckbox)
            {
                this.EditedObject.visibleInShortcut = checkBox.IsChecked.Value;
            }
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
