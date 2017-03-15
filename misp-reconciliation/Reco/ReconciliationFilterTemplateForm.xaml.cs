using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Administration.ObjectAdmin;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.GridViews;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ReconciliationFilterTemplateForm.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateForm : TabControl, IEditableView<ReconciliationFilterTemplate>
    {

        #region Properties

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }


        /// <summary>
        /// Can user write off
        /// </summary>
        public bool CanEditWriteOff { get; set; }

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        public ReconciliationFilterTemplate EditedObject { get; set; }

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        public ChangeEventHandler Changed { get; set; }

        public event ChangeEventHandler FormChanged;

        public RecoWriteOffDialog dialog { get; set; }

        public AdministrationBar AdministrationBar { get; set; }

        #endregion


        #region Contollers

        public ReconciliationFilterTemplateForm(SubjectType subjectType)
        {
            this.CanEditWriteOff = true;
            this.SubjectType = subjectType;
            InitializeComponent();
            UserInit();
            InitHandlers();
        }

        #endregion
        

        #region Initializations

        private void UserInit()
        {
            if (ApplicationManager.Instance.User != null && !ApplicationManager.Instance.User.IsAdmin())
            {
                this.Items.Remove(this.ConfigTabItem);
                this.Items.Remove(this.LeftTabItem);
                this.Items.Remove(this.RightTabItem);
                this.Items.Remove(this.BottomTabItem);

                //this.LeftGrid.NameTextBox.IsReadOnly = true;
                //this.LeftGrid.CommentTextBlock.IsEnabled = false;

                //this.RightGrid.NameTextBox.IsReadOnly = true;
                //this.RightGrid.CommentTextBlock.IsEnabled = false;
            }
            else
            {
                this.LeftGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.LeftGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
                this.RightGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.RightGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
                this.BottomGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.BottomGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;

                this.AdministrationBar = new AdministrationBar(this.SubjectType);
            }
        }

        #endregion


        #region Operations

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        /// <summary>
        /// Customize for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public virtual void Customize(List<Kernel.Domain.Right> rights, bool readOnly = false)
        {
            bool editWriteOff = RightsUtil.HasRight(Kernel.Domain.RightType.EDIT_WRITE_OFF, rights);
            bool resetWriteOff = RightsUtil.HasRight(Kernel.Domain.RightType.RESET_RECONCILIATION, rights);
            this.CanEditWriteOff = editWriteOff;
            this.BottomGrid.ResetButton.Visibility = resetWriteOff ? Visibility.Visible : Visibility.Collapsed;
        }

        public virtual void SetTarget(Target target, bool setToFilter = false)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.SetTarget(target);
            }
            else if (this.SelectedIndex == 2)
            {
                if (setToFilter) this.LeftGrid.GrilleBrowserForm.filterForm.targetFilter.SetTargetValue(target);
                else if(target is Kernel.Domain.Attribute) this.LeftGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
            else if (this.SelectedIndex == 3)
            {
                if (setToFilter) this.RightGrid.GrilleBrowserForm.filterForm.targetFilter.SetTargetValue(target);
                else if (target is Kernel.Domain.Attribute) this.RightGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
        }

        public virtual void SetPeriodInterval(PeriodInterval interval, bool setToFilter = false)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setPeriodName(interval);
            }
            else if (this.SelectedIndex == 2)
            {
                if (setToFilter) this.LeftGrid.GrilleBrowserForm.filterForm.periodFilter.SetPeriodInterval(interval);
                else this.LeftGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
            else if (this.SelectedIndex == 3)
            {
                if (setToFilter) this.RightGrid.GrilleBrowserForm.filterForm.periodFilter.SetPeriodInterval(interval);
                else this.RightGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
        }

        public virtual void SetPeriodName(PeriodName name, bool setToFilter = false)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setPeriodName(name);
            }
            else if (this.SelectedIndex == 2)
            {
                if (setToFilter) this.LeftGrid.GrilleBrowserForm.filterForm.periodFilter.SetPeriodItemName(name.name);
                else this.LeftGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
            else if (this.SelectedIndex == 3)
            {
                if (setToFilter) this.RightGrid.GrilleBrowserForm.filterForm.periodFilter.SetPeriodItemName(name.name);
                else this.RightGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
        }

        public virtual void SetMeasure(Measure measure)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setMeasure(measure);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(measure);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(measure);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(measure);
            }
        }
                
        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public ReconciliationFilterTemplate getNewObject()
        {
            return new ReconciliationFilterTemplate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(Misp.Kernel.Ui.Base.ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public virtual void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = new ReconciliationFilterTemplate();
            if (ApplicationManager.Instance.User != null && ApplicationManager.Instance.User.IsAdmin())
            {
                this.EditedObject.writeOffConfig = ConfigurationPanel.WriteOffConfigPanel.fillObject();
                this.LeftGrid.GrilleBrowserForm.fillObject();
                this.LeftGrid.EditedObject.loadFilters();
                this.RightGrid.GrilleBrowserForm.fillObject();
                this.RightGrid.EditedObject.loadFilters();
            }
        }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            if (this.EditedObject != null)
            {
                if (this.EditedObject.leftGrid.GrilleFilter == null) this.EditedObject.leftGrid.GrilleFilter = new GrilleFilter();
                if (this.EditedObject.rigthGrid.GrilleFilter == null) this.EditedObject.rigthGrid.GrilleFilter = new GrilleFilter();
            }

            this.LeftGrid.GrilleBrowserForm.gridBrowser.FormatConditionsEventHandler += BuildLeftFormatConditions;
            this.RightGrid.GrilleBrowserForm.gridBrowser.FormatConditionsEventHandler += BuildRightFormatConditions;
            this.BottomGrid.GridBrowser.FormatConditionsEventHandler += BuildBottomFormatConditions;
            
            this.LeftGrid.Template = this.EditedObject;
            this.RightGrid.Template = this.EditedObject;

            this.LeftGrid.EditedObject = this.EditedObject != null ? this.EditedObject.leftGrid : null;
            this.RightGrid.EditedObject = this.EditedObject != null ? this.EditedObject.rigthGrid : null;
            this.BottomGrid.EditedObject = this.EditedObject != null ? this.EditedObject.bottomGrid : null;
            this.LeftGrid.displayObject();
            this.RightGrid.displayObject();
            this.BottomGrid.displayObject();

            if (ApplicationManager.Instance.User != null && ApplicationManager.Instance.User.IsAdmin())
            {
                this.ConfigurationPanel.EditedObject = this.EditedObject;
                this.ConfigurationPanel.displayObject();
                this.LeftGridProperties.EditedObject = this.EditedObject.leftGrid;
                this.RightGridProperties.EditedObject = this.EditedObject.rigthGrid;
                this.BottomGridProperties.EditedObject = this.EditedObject.bottomGrid;
                this.LeftGridProperties.displayObject();
                this.RightGridProperties.displayObject();
                this.BottomGridProperties.displayObject();
            }

            if (this.AdministrationBar != null)
            {
                this.AdministrationBar.EditedObject = this.EditedObject;
                this.AdministrationBar.Display();
            }
        }


        private List<FormatCondition> BuildLeftFormatConditions()
        {
            return BuildFormatConditions(this.LeftGrid.GrilleBrowserForm.gridBrowser, this.EditedObject.leftMeasure, null);
        }

        private List<FormatCondition> BuildRightFormatConditions()
        {
            return BuildFormatConditions(this.RightGrid.GrilleBrowserForm.gridBrowser, null, this.EditedObject.rightMeasure);
        }

        private List<FormatCondition> BuildBottomFormatConditions()
        {
            return BuildFormatConditions(this.BottomGrid.GridBrowser, this.EditedObject.leftMeasure, this.EditedObject.rightMeasure);
        }

        private List<FormatCondition> BuildFormatConditions(GridBrowser grid, Measure leftMeasure, Measure rightMeasure)
        {
            List<FormatCondition> conditions = new List<FormatCondition>(0);
            if (this.EditedObject.useDebitCredit.Value)
            {
                Misp.Kernel.Domain.ReconciliationContext context = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationContextService().getReconciliationContext();
                if (context != null && context.dcNbreAttribute != null)
                {
                    GrilleColumn creditDebitColumn = grid.Grille.GetColumn(ParameterType.SCOPE.ToString(), context.dcNbreAttribute.oid.Value);
                    if (creditDebitColumn != null)
                    {
                        String debitValue = context.debitAttributeValue != null ? context.debitAttributeValue.name : "D";
                        conditions.Add(new FormatCondition()
                        {
                            ApplyToRow = true,
                            Expression = "[" + creditDebitColumn.name + "] == '" + debitValue + "'",
                            FieldName = creditDebitColumn.name,
                            Format = new Format() { Foreground = Brushes.Red }
                        });
                    }
                }
            }
            else
            {
                if (leftMeasure != null)
                {
                    GrilleColumn amountColumn = grid.Grille.GetColumn(ParameterType.MEASURE.ToString(), leftMeasure.oid.Value);
                    if (amountColumn != null)
                    {
                        conditions.Add(new FormatCondition()
                        {
                            ApplyToRow = true,
                            Expression = "[" + amountColumn.name + "] < 0.0",
                            FieldName = amountColumn.name,
                            Format = new Format() { Background = Brushes.Red }
                        });
                    }
                }
                if (rightMeasure != null)
                {
                    GrilleColumn amountColumn = grid.Grille.GetColumn(ParameterType.MEASURE.ToString(), rightMeasure.oid.Value);
                    if (amountColumn != null)
                    {
                        conditions.Add(new FormatCondition()
                        {
                            ApplyToRow = true,
                            Expression = "[" + amountColumn.name + "] < 0.0",
                            FieldName = amountColumn.name,
                            Format = new Format() { Background = Brushes.Red }
                        });
                    }
                }
            }
            return conditions;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            return new List<object>(0);
        }

        public void Reconciliate()
        {
            ReconciliationData reco = new ReconciliationData();
            decimal credit = dialog.ReconciliationGrid.LeftAmount;
            decimal debit = dialog.ReconciliationGrid.RightAmount;
            decimal balance = dialog.ReconciliationGrid.BalanceAmount;
            if (balance != 0 && this.EditedObject.acceptWriteOff)
            {
                if (!dialog.WriteOffBlock.Validate()) return;
                reco.writeOffFields = dialog.WriteOffBlock.Fill();
                reco.writeOffAmount = balance;
            }
                        
            reco.ids = dialog.ReconciliationGrid.GridBrowser.GetSelectedOis();
            reco.recoType = this.EditedObject.reconciliationType;
            WriteOffFieldValueType type = this.EditedObject.writeoffDefaultMeasureTypeEnum;
            if (type == WriteOffFieldValueType.CUSTOM) reco.writeOffMeasure = this.EditedObject.writeoffMeasure;
            else if (type == WriteOffFieldValueType.LEFT_SIDE) reco.writeOffMeasure = this.EditedObject.leftMeasure;
            else if (type == WriteOffFieldValueType.RIGHT_SIDE) reco.writeOffMeasure = this.EditedObject.rightMeasure;
            try
            {
                bool result = this.Service.reconciliate(reco);
                if (result)
                {
                    this.LeftGrid.Search(this.LeftGrid.EditedObject.GrilleFilter != null ? this.LeftGrid.EditedObject.GrilleFilter.page : 1);
                    this.RightGrid.Search(this.RightGrid.EditedObject.GrilleFilter != null ? this.RightGrid.EditedObject.GrilleFilter.page : 1);
                    this.BottomGrid.Clear();
                    this.BottomGrid.ReconciliateButton.IsEnabled = false;
                    this.BottomGrid.ResetButton.IsEnabled = false;
                    this.BottomGrid.ClearButton.IsEnabled = false;
                    dialog.ReconciliateButton.Click -= OnDialogReconciliate;
                    dialog.CancelButton.Click -= OnDialogCancel;
                    this.dialog.Close();
                    dialog = null;
                    BuildBalance(this.LeftGrid);
                    BuildBalance(this.RightGrid);
                }
            }
            catch (Exception)
            {
                MessageDisplayer.DisplayWarning("Reconciliation Error", "An error occurred while trying to reconciliate!");
            }
        }

        public void ResetReconciliate()
        {
            bool result = this.Service.resetReconciliate(this.EditedObject.reconciliationType.oid.Value, this.BottomGrid.GridBrowser.GetSelectedOis());
            if (result)
            {
                this.LeftGrid.Search(this.LeftGrid.EditedObject.GrilleFilter != null ? this.LeftGrid.EditedObject.GrilleFilter.page : 1);
                this.RightGrid.Search(this.RightGrid.EditedObject.GrilleFilter != null ? this.RightGrid.EditedObject.GrilleFilter.page : 1);
                this.BottomGrid.Clear();
                this.BottomGrid.ReconciliateButton.IsEnabled = false;
                this.BottomGrid.ResetButton.IsEnabled = false;
                this.BottomGrid.ClearButton.IsEnabled = false;
                BuildBalance(this.LeftGrid);
                BuildBalance(this.RightGrid);
            }
        }

        #endregion


        #region Handlers

        private void InitHandlers()
        {
            if (ApplicationManager.Instance.User != null && ApplicationManager.Instance.User.IsAdmin())
            {
                this.LeftGridProperties.InputGridPropertiesPanel.Changed += OnLeftGridPropertiesChange;
                this.RightGridProperties.InputGridPropertiesPanel.Changed += OnRightGridPropertiesChange;
                this.BottomGridProperties.InputGridPropertiesPanel.Changed += OnBottomGridPropertiesChange;

                this.RightGridProperties.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
                this.LeftGridProperties.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
                this.BottomGridProperties.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;

                this.BottomGridProperties.InputGridPropertiesPanel.CommentTextBlock.KeyUp += onNameTextChange;
                this.LeftGridProperties.InputGridPropertiesPanel.CommentTextBlock.KeyUp += onNameTextChange;
                this.RightGridProperties.InputGridPropertiesPanel.CommentTextBlock.KeyUp += onNameTextChange;
                //this.BottomGridProperties.InputGridPropertiesPanel.

                //this.RightGridProperties.InputGridPropertiesPanel.ColumnForms.KeyUp += onNameTextChange;
                //this.LeftGridProperties.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
                //this.BottomGridProperties.InputGridPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
            }

            //this.LeftGrid.GrilleBrowserForm.gridBrowser..RowCellStyle += gridView_RowCellStyle;

            this.LeftGrid.Changed += OnChange;
            this.RightGrid.Changed += OnChange;

            this.LeftGrid.GrilleBrowserForm.gridBrowser.SelectedItemChangedHandler += OnLeftGridSelectionChange;
            this.LeftGrid.GrilleBrowserForm.gridBrowser.DeselectedItemChangedHandler += OnLeftGridDeselectionChange;

            this.RightGrid.GrilleBrowserForm.gridBrowser.SelectedItemChangedHandler += OnRightGridSelectionChange;
            this.RightGrid.GrilleBrowserForm.gridBrowser.DeselectedItemChangedHandler += OnRightGridDeselectionChange;

            this.BottomGrid.GridBrowser.ChangeHandler += OnBottomGridSelectionChange;

            this.BottomGrid.ReconciliateButton.Click += OnReconciliate;
            this.BottomGrid.ResetButton.Click += OnResetReconciliation;
            this.BottomGrid.ClearButton.Click += OnClearButtonGrid;
        
        }

        private void OnClearButtonGrid(object sender, RoutedEventArgs e)
        {
            this.BottomGrid.Clear();
            this.BottomGrid.ReconciliateButton.IsEnabled = false;
            this.BottomGrid.ResetButton.IsEnabled = false;
            this.BottomGrid.ClearButton.IsEnabled = false;
        }

        private void OnReconciliate(object sender, RoutedEventArgs e)
        {
            if (this.EditedObject.reconciliationType == null)
            {
                MessageDisplayer.DisplayWarning("Reconciliation", "The reconciliation type is not specified!");
                return;
            }
            if (this.EditedObject.leftMeasure == null)
            {
                MessageDisplayer.DisplayWarning("Reconciliation", "The left measure is not specified!");
                return;
            }
            if (this.EditedObject.rightMeasure == null)
            {
                MessageDisplayer.DisplayWarning("Reconciliation", "The right measure is not specified!");
                return;
            }
            if (ContentsReconciliatedItems())
            {
                MessageDisplayer.DisplayWarning("Reconciliation", "You can't create a new reconciliation with this selction.\nThere is at least one reconciliated item in selection!");
                return;
            }

            if (this.BottomGrid.BalanceAmount != 0)
            {
                if (!CanEditWriteOff)
                {
                    MessageDisplayer.DisplayWarning("Reconciliation", "You are not allowed to create a write off!");
                    return;
                }
                if(!this.EditedObject.acceptWriteOff)
                {
                    MessageDisplayer.DisplayWarning("Reconciliation", "You can't create a new reconciliation with this selction.\nWrite off is not allowed!");
                    return;
                }
                WriteOffFieldValueType type = this.EditedObject.writeoffDefaultMeasureTypeEnum;
                if (type == WriteOffFieldValueType.CUSTOM && this.EditedObject.writeoffMeasure == null)
                {
                    MessageDisplayer.DisplayWarning("Reconciliation", "The write off measure is not specified!");
                    return;
                }
            }

            dialog = new RecoWriteOffDialog();
            dialog.Owner = ApplicationManager.Instance.MainWindow;
            dialog.EditedObject = this.EditedObject;
            dialog.displayObject(this.BottomGrid.GridBrowser.gridControl.SelectedItems);
            dialog.ReconciliationGrid.SetBalance(this.BottomGrid.LeftAmount, this.BottomGrid.RightAmount, this.BottomGrid.BalanceAmount);
            if (this.BottomGrid.BalanceAmount != 0)
            {
                dialog.WriteOffBlock.Visibility = Visibility.Visible;
                dialog.WriteOffBlock.WriteOffConfiguration = this.EditedObject.writeOffConfig;
                dialog.WriteOffBlock.display();
            }
            else
            {
                dialog.WriteOffBlock.Visibility = Visibility.Collapsed;
            }
            
            dialog.ReconciliateButton.Click += OnDialogReconciliate;
            dialog.CancelButton.Click += OnDialogCancel;            
            dialog.Show();
        }

        private void OnDialogReconciliate(object sender, RoutedEventArgs e)
        {
            Reconciliate();
        }

        private void OnDialogCancel(object sender, RoutedEventArgs e)
        {
            dialog.ReconciliateButton.Click -= OnDialogReconciliate;
            dialog.CancelButton.Click -= OnDialogCancel;
            this.dialog.Close();
            dialog = null;
        }


        private void OnResetReconciliation(object sender, RoutedEventArgs e)
        {
            if (this.EditedObject.reconciliationType == null)
            {
                MessageDisplayer.DisplayWarning("Reset Reconciliation", "The reconciliation type is not specified!");
                return;
            }
            if (!ContentsReconciliatedItems())
            {
                MessageDisplayer.DisplayWarning("Reset Reconciliation", "There is no reconciliated item in selection!");
                return;
            }

            MessageBoxResult result = MessageDisplayer.DisplayYesNoQuestion("Reset Reconciliation", "You're about to reset reconciliation.\nDou You want to continue?");
            if (result == MessageBoxResult.Yes)
            {
                ResetReconciliate();
            }
        }

        protected bool ContentsReconciliatedItems()
        {
            return this.Service.ContainsReconciliatedItems(this.EditedObject.reconciliationType.oid.Value, this.BottomGrid.GridBrowser.GetSelectedOis());
        }

        private void OnRightGridDeselectionChange(object newSelection)
        {
            BuildBalance(this.RightGrid);
        }

        private void OnLeftGridDeselectionChange(object newSelection)
        {
            BuildBalance(this.LeftGrid);
        }

        private void OnBottomGridSelectionChange()
        {
            bool enable = this.BottomGrid.GridBrowser.gridControl.SelectedItems.Count > 0;
            this.BottomGrid.ReconciliateButton.IsEnabled = enable;
            this.BottomGrid.ResetButton.IsEnabled = enable;
            this.BottomGrid.ClearButton.IsEnabled = this.BottomGrid.GetRowCount() > 0;

            Decimal[] balances = BuildBottomBalance(this.BottomGrid.EditedObject, this.BottomGrid.GridBrowser);
            Decimal balanceValue = balances[0] - balances[1];
            if (this.EditedObject.balanceFormulaEnum != null && this.EditedObject.balanceFormulaEnum == BalanceFormula.LEFT_PLUS_RIGHT)
            {
                balanceValue = balances[0] + balances[1];
            }
            this.BottomGrid.SetBalance(balances[0], balances[1], balanceValue);
        }
        
        private void BuildBalance(ReconciliationFilterTemplateGrid grid)
        {
            Measure measure = grid.EditedObject == this.EditedObject.leftGrid ? this.EditedObject.leftMeasure : this.EditedObject.rightMeasure;
            Decimal[] balances = BuildBalance(grid.EditedObject, grid.GrilleBrowserForm.gridBrowser, measure);
            String credit = this.EditedObject.useDebitCredit == true ? "Credit: " : "Positive Amount: ";
            String debit = this.EditedObject.useDebitCredit == true ? "Debit: " : "Negative Amount: ";
            String balance = "Balance: ";
            grid.CreditLabel.Content = credit + balances[0].ToString("N", CultureInfo.CreateSpecificCulture("de-DE"));
            grid.DebitLabel.Content = debit + balances[1].ToString("N", CultureInfo.CreateSpecificCulture("de-DE"));
            grid.BalanceLabel.Content = balance + (balances[0] - balances[1]).ToString("N", CultureInfo.CreateSpecificCulture("de-DE"));
        }

        private Decimal[] BuildBalance(Grille grid, GridBrowser browser, Measure measure)
        {
            Decimal credit = 0;
            Decimal debit = 0;
            GrilleColumn amountColumn = null;
            GrilleColumn creditDebitColumn = null;
            if (measure != null) amountColumn = grid.GetColumn(ParameterType.MEASURE.ToString(), measure.oid.Value);
            
            if (amountColumn != null)
            {
                String creditValue = "C";
                String debitValue = "D";
                if (this.EditedObject.useDebitCredit == true)
                {
                    Misp.Kernel.Domain.ReconciliationContext context = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationContextService().getReconciliationContext();
                    if (context != null)
                    {
                        creditValue = context.creditAttributeValue != null ? context.creditAttributeValue.name : "C";
                        debitValue = context.debitAttributeValue != null ? context.debitAttributeValue.name : "D";
                        if (context.dcNbreAttribute != null)
                            creditDebitColumn = grid.GetColumn(ParameterType.SCOPE.ToString(), context.dcNbreAttribute.oid.Value);
                    }                    
                }
                foreach (object row in browser.gridControl.SelectedItems)
                {
                    if (row is GridItem)
                    {
                        Object[] datas = ((GridItem)row).Datas;
                        Decimal amount = 0;
                        object item = item = datas[amountColumn.position];
                        try
                        {
                            Decimal.TryParse(item.ToString(), out amount);
                        }
                        catch (Exception) { }
                        if (this.EditedObject.useDebitCredit == true)
                        {
                            if (creditDebitColumn == null) continue;
                            item = datas[creditDebitColumn.position];
                            Boolean isCredit = item != null && item.ToString().Equals(creditValue, StringComparison.OrdinalIgnoreCase);
                            Boolean isDebit = item != null && item.ToString().Equals(debitValue, StringComparison.OrdinalIgnoreCase);
                            if (isCredit) credit += amount;
                            else if (isDebit) debit += amount;
                        }
                        else
                        {
                            if (amount > 0) credit += amount;
                            else debit += amount;
                        }
                    }
                }
            }
            Decimal[] balances = new Decimal[] { credit, debit };
            return balances;
        }

        private Decimal[] BuildBottomBalance(Grille grid, GridBrowser browser)
        {
            Decimal credit = 0;
            Decimal debit = 0;
            GrilleColumn leftAmountColumn = null;
            GrilleColumn rightAmountColumn = null;
            GrilleColumn creditDebitColumn = null;
            Measure leftMeasure = this.EditedObject.leftMeasure;
            Measure rightMeasure = this.EditedObject.rightMeasure;

            if (leftMeasure != null) leftAmountColumn = grid.GetColumn(ParameterType.MEASURE.ToString(), leftMeasure.oid.Value);
            if (rightMeasure != null) rightAmountColumn = grid.GetColumn(ParameterType.MEASURE.ToString(), rightMeasure.oid.Value);
            if (leftAmountColumn != null && rightAmountColumn != null)
            {
                String creditValue = "C";
                String debitValue = "D";
                if (this.EditedObject.useDebitCredit == true)
                {
                    Misp.Kernel.Domain.ReconciliationContext context = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationContextService().getReconciliationContext();
                    if (context != null)
                    {
                        creditValue = context.creditAttributeValue != null ? context.creditAttributeValue.name : "C";
                        debitValue = context.debitAttributeValue != null ? context.debitAttributeValue.name : "D";
                        if (context.dcNbreAttribute != null)
                            creditDebitColumn = grid.GetColumn(ParameterType.SCOPE.ToString(), context.dcNbreAttribute.oid.Value);
                    }
                }
                foreach (object row in browser.gridControl.SelectedItems)
                {
                    if (row is GridItem)
                    {
                        String side = ((GridItem)row).Side;
                        if (string.IsNullOrWhiteSpace(side)) continue;
                        GrilleColumn amountColumn = side == GridItem.LEFT_SIDE ? leftAmountColumn : rightAmountColumn;
                        if (amountColumn == null) continue;
                        Object[] datas = ((GridItem)row).Datas;                        
                        Decimal amount = 0;
                        object item = item = datas[amountColumn.position];
                        try
                        {
                            Decimal.TryParse(item.ToString(), out amount);
                        }
                        catch (Exception) { }

                        if (this.EditedObject.useDebitCredit == true)
                        {
                            if (creditDebitColumn == null) continue;
                            item = datas[creditDebitColumn.position];
                            Boolean isCredit = item != null && item.ToString().Equals(creditValue, StringComparison.OrdinalIgnoreCase);
                            Boolean isDebit = item != null && item.ToString().Equals(debitValue, StringComparison.OrdinalIgnoreCase);

                            if (side == GridItem.LEFT_SIDE)
                            {
                                if (isCredit) credit += amount;
                                else if (isDebit) credit -= amount;
                            }
                            else
                            {
                                if (isCredit) debit += amount;
                                else if (isDebit) debit -= amount;
                            }
                        }
                        else
                        {
                            if (side == GridItem.LEFT_SIDE) credit += amount;
                            else debit += amount;
                        }
                    }
                }
            }
            Decimal[] balances = new Decimal[] { credit, debit };
            return balances;
        }

        
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                changeEditedObjectName(sender, false);
            }
            else if (args.Key == Key.Enter)
            {
                changeEditedObjectName(sender, true);
            }
        }

        private void changeEditedObjectName(object sender, bool change) 
        {
            if(!(sender is TextBox)) return;
            TextBox selectedTextBox = (TextBox)sender;
            if (this.SelectedIndex == 2)
            {
                if (selectedTextBox == this.LeftGridProperties.InputGridPropertiesPanel.CommentTextBlock)
                {
                    if(change) this.LeftGridProperties.EditedObject.comment = this.LeftGridProperties.InputGridPropertiesPanel.CommentTextBlock.Text.Trim();
                    else this.LeftGridProperties.InputGridPropertiesPanel.CommentTextBlock.Text = this.LeftGridProperties.EditedObject.comment;
                }
                else
                {
                    if (change)
                    {
                        string name = this.LeftGridProperties.InputGridPropertiesPanel.NameTextBox.Text.Trim();
                        if(string.IsNullOrEmpty(name))
                        {
                            name ="Left";
                            this.LeftGridProperties.InputGridPropertiesPanel.NameTextBox.Text = name;
                        }
                        this.LeftGridProperties.EditedObject.name = name;
                    }
                    else
                    {
                        this.LeftGridProperties.InputGridPropertiesPanel.NameTextBox.Text = this.LeftGridProperties.EditedObject.name;
                    }
                }
            }
            else if (this.SelectedIndex == 3)
            {
                if (selectedTextBox == this.RightGridProperties.InputGridPropertiesPanel.CommentTextBlock)
                {
                    if (change) this.RightGridProperties.EditedObject.comment = this.RightGridProperties.InputGridPropertiesPanel.CommentTextBlock.Text.Trim();
                    else this.RightGridProperties.InputGridPropertiesPanel.CommentTextBlock.Text = this.RightGridProperties.EditedObject.comment;
                }
                else
                {
                    if (change)
                    {
                        string name = this.RightGridProperties.InputGridPropertiesPanel.NameTextBox.Text.Trim();
                        if(string.IsNullOrEmpty(name))
                        {
                            name ="Right";
                            this.RightGridProperties.InputGridPropertiesPanel.NameTextBox.Text = name;
                        }
                        this.RightGridProperties.EditedObject.name = name;
                    }
                    else this.RightGridProperties.InputGridPropertiesPanel.NameTextBox.Text = this.RightGridProperties.EditedObject.name;
                }
            }
            else if (this.SelectedIndex == 4)
            {
                if (selectedTextBox == this.BottomGridProperties.InputGridPropertiesPanel.CommentTextBlock)
                {
                    if (change) this.BottomGridProperties.EditedObject.comment = this.BottomGridProperties.InputGridPropertiesPanel.CommentTextBlock.Text.Trim();
                    else this.BottomGridProperties.InputGridPropertiesPanel.CommentTextBlock.Text = this.BottomGridProperties.EditedObject.comment;
                }
                else
                {
                    if (change)
                    {
                        string name = this.BottomGridProperties.InputGridPropertiesPanel.NameTextBox.Text.Trim();
                        if (string.IsNullOrEmpty(name))
                        {
                            name = "Bottom";
                            this.BottomGridProperties.InputGridPropertiesPanel.NameTextBox.Text = name;
                        }
                        this.BottomGridProperties.EditedObject.name = name;
                    }
                    else this.BottomGridProperties.InputGridPropertiesPanel.NameTextBox.Text = this.BottomGridProperties.EditedObject.name;
                }
            }
            if (FormChanged != null && change) FormChanged();
        }

        private void OnLeftGridSelectionChange(Object obj)
        {
            BuildBalance(this.LeftGrid);
            if (obj != null)
            {
                List<long> oids = new List<long>(0);
                if (obj is GridItem)
                {
                    oids.Add(((GridItem)obj).GetOid().Value);
                }
                else if (obj is IList)
                {
                    foreach (GridItem item in (IList)obj)
                    {
                        oids.Add(item.GetOid().Value);
                    }
                }
                this.BottomGrid.AddLines(oids, GridItem.LEFT_SIDE);
            }            
        }

        private void OnRightGridSelectionChange(Object obj)
        {
            BuildBalance(this.RightGrid);
            if (obj != null)
            {
                List<long> oids = new List<long>(0);
                if (obj is GridItem)
                {
                    oids.Add(((GridItem)obj).GetOid().Value);
                }
                else if (obj is IList)
                {
                    foreach (GridItem item in (IList)obj)
                    {
                        oids.Add(item.GetOid().Value);
                    }
                }
                this.BottomGrid.AddLines(oids, GridItem.RIGHT_SIDE);
            } 
        }
                        
        private void OnBottomGridPropertiesChange(object item)
        {
            this.BottomGridProperties.BuildColunms();
            this.BottomGrid.GridBrowser.RebuildGrid = true;
            if (FormChanged != null) FormChanged();
        }

        private void OnRightGridPropertiesChange(object item)
        {
            this.RightGridProperties.BuildColunms();
            this.RightGrid.GrilleBrowserForm.gridBrowser.RebuildGrid = true;
            if (FormChanged != null) FormChanged();
        }

        private void OnLeftGridPropertiesChange(object item)
        {
            this.LeftGridProperties.BuildColunms();
            this.LeftGrid.GrilleBrowserForm.gridBrowser.RebuildGrid = true;
            if (FormChanged != null) FormChanged();
        }

        public void OnChange()
        {
            if (FormChanged != null) FormChanged();
        }

        #endregion


        public void updateObject(ReconciliationFilterTemplate reconciliationFilterTemplate)
        {
            this.ConfigurationPanel.updateObjetct(reconciliationFilterTemplate.writeOffConfig);
        }
    }
}
