using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Sourcing.GridViews;
using System;
using System.Collections;
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
    /// Interaction logic for ReconciliationFilterTemplateForm.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateForm : TabControl, IEditableView<ReconciliationFilterTemplate>
    {

        #region Properties

        public bool IsReadOnly { get; set; }

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
                

        #endregion


        #region Contollers

        public ReconciliationFilterTemplateForm()
        {
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

                this.LeftGrid.NameTextBox.Visibility = Visibility.Visible;
                this.LeftGrid.CommentButton.Visibility = Visibility.Visible;

                this.RightGrid.NameTextBox.Visibility = Visibility.Visible;
                this.RightGrid.CommentButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.LeftGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.LeftGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
                this.RightGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.RightGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
                this.BottomGridProperties.InputGridPropertiesPanel.GroupPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.BottomGridProperties.InputGridPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion


        #region Operations

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }


        public virtual void SetTarget(Target target)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.SetTarget(target);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(target);
            }
        }

        public virtual void SetPeriodInterval(PeriodInterval interval)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setPeriodName(interval);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
            else if (this.SelectedIndex == 4)
            {
                this.BottomGridProperties.InputGridPropertiesPanel.SetValue(interval);
            }
        }

        public virtual void SetPeriodName(PeriodName name)
        {
            if (this.SelectedIndex == 0) { }
            else if (this.SelectedIndex == 1)
            {
                this.ConfigurationPanel.WriteOffConfigPanel.setPeriodName(name);
            }
            else if (this.SelectedIndex == 2)
            {
                this.LeftGridProperties.InputGridPropertiesPanel.SetValue(name);
            }
            else if (this.SelectedIndex == 3)
            {
                this.RightGridProperties.InputGridPropertiesPanel.SetValue(name);
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            return new List<object>(0);
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

            this.LeftGrid.Changed += OnChange;
            this.RightGrid.Changed += OnChange;

            this.LeftGrid.GrilleBrowserForm.gridBrowser.SelectedItemChangedHandler += OnLeftGridSelectionChange;
            this.LeftGrid.GrilleBrowserForm.gridBrowser.DeselectedItemChangedHandler += OnLeftGridDeselectionChange;

            this.RightGrid.GrilleBrowserForm.gridBrowser.SelectedItemChangedHandler += OnRightGridSelectionChange;
            this.RightGrid.GrilleBrowserForm.gridBrowser.DeselectedItemChangedHandler += OnRightGridDeselectionChange;

            this.BottomGrid.GridBrowser.ChangeHandler += OnBottomGridSelectionChange;

            this.BottomGrid.ReconciliateButton.Click += OnReconciliate;
            this.BottomGrid.ResetButton.Click += OnResetReconciliation;
        
        }

        private void OnReconciliate(object sender, RoutedEventArgs e)
        {
            if (this.EditedObject.reconciliationType == null)
            {
                MessageDisplayer.DisplayWarning("Reconciliation", "The reconciliation type is not specified!");
                return;
            }
            RecoWriteOffDialog dialog = new RecoWriteOffDialog();
            dialog.Owner = ApplicationManager.Instance.MainWindow;
            dialog.EditedObject = this.EditedObject;
            dialog.displayObject((List<GridItem>)this.BottomGrid.GridBrowser.gridControl.SelectedItems);
            dialog.Show();
        }

        private void OnResetReconciliation(object sender, RoutedEventArgs e)
        {
            
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

            Decimal[] balances = BuildBalance(this.BottomGrid.EditedObject, this.BottomGrid.GridBrowser);
            String credit = "Left Amount: ";
            String debit = "Right Amount: ";
            String balance = "Balance: ";
            this.BottomGrid.CreditLabel.Content = credit + balances[0];
            this.BottomGrid.DebitLabel.Content = debit + balances[1];
            Decimal balanceValue = balances[0] - balances[1];
            if (this.EditedObject.balanceFormulaEnum != null && this.EditedObject.balanceFormulaEnum == BalanceFormula.LEFT_PLUS_RIGHT)
            {
                balanceValue = balances[0] + balances[1];
            }
            this.BottomGrid.BalanceLabel.Content = balance + balanceValue;
        }
        
        private void BuildBalance(ReconciliationFilterTemplateGrid grid)
        {
            Decimal[] balances = BuildBalance(grid.EditedObject, grid.GrilleBrowserForm.gridBrowser);
            String credit = this.EditedObject.useDebitCredit == true ? "Credit: " : "Positive Amount: ";
            String debit = this.EditedObject.useDebitCredit == true ? "Debit: " : "Negative Amount: ";
            String balance = "Balance: ";
            grid.CreditLabel.Content = credit + balances[0];
            grid.DebitLabel.Content = debit + balances[1];
            grid.BalanceLabel.Content = balance + (balances[0] - balances[1]);
        }

        private Decimal[] BuildBalance(Grille grid, GridBrowser browser)
        {
            Decimal credit = 0;
            Decimal debit = 0;

            GrilleColumn amountColumn = null;
            GrilleColumn creditDebitColumn = null;

            Misp.Kernel.Domain.ReconciliationContext context = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationContextService().getReconciliationContext();

            Measure measure = this.EditedObject.amountMeasure;
            if (measure == null)
            {
                measure = context != null ? context.amountMeasure : null;
            }
            if (measure != null) amountColumn = grid.GetColumn(ParameterType.MEASURE.ToString(), measure.oid.Value);

            if (amountColumn != null)
            {
                if (this.EditedObject.useDebitCredit == true && context != null && context.dcNbreAttribute != null)
                {
                    creditDebitColumn = grid.GetColumn(ParameterType.SCOPE.ToString(), context.dcNbreAttribute.oid.Value);
                }

                String creditValue = context.creditAttributeValue != null ? context.creditAttributeValue.name : "C";
                String debitValue = context.debitAttributeValue != null ? context.debitAttributeValue.name : "D";

                foreach (object row in browser.gridControl.SelectedItems)
                {
                    if (row is GridItem)
                    {
                        Object[] datas = ((GridItem)row).Datas;                        
                        Decimal amount = 0;
                        object item = datas[amountColumn.position];
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
                        else if(grid == this.EditedObject.bottomGrid)
                        {
                            String side = ((GridItem)row).Side;
                            if (string.IsNullOrWhiteSpace(side)) continue;
                            if (side == GridItem.LEFT_SIDE) credit += amount;
                            else debit += amount;
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
                else if (obj is List<GridItem>)
                {
                    foreach (GridItem item in (List<GridItem>)obj)
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
            if (Changed != null) Changed();
        }

        #endregion


        public void updateObject(ReconciliationFilterTemplate reconciliationFilterTemplate)
        {
            this.ConfigurationPanel.updateObjetct(reconciliationFilterTemplate.writeOffConfig);
        }
    }
}
