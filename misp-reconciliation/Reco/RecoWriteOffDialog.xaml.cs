using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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
using System.Windows.Shapes;

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for RecoWriteOffDialog.xaml
    /// </summary>
    public partial class RecoWriteOffDialog : Window
    {

        #region Properties

        public ReconciliationFilterTemplate EditedObject { get; set; }

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        #endregion


        #region Constructors

        public RecoWriteOffDialog()
        {
            InitializeComponent();
            ReconciliationGrid.RecoToolBar.Visibility = Visibility.Collapsed;
            this.confirmationMessageLabel.Content = "You are about to create a reconciliation for the selected items.\nDo you confirm the operation?";
            InitHandlers();
        }

        #endregion


        #region Operations

        public void displayObject(IList items)
        {
            this.ReconciliationGrid.GridBrowser.RebuildGrid = true;
            this.ReconciliationGrid.EditedObject = this.EditedObject != null ? this.EditedObject.bottomGrid : null;
            this.ReconciliationGrid.displayObject();
            ((GridTableView)this.ReconciliationGrid.GridBrowser.gridControl.View).ShowCheckBoxSelectorColumn = false;

            this.ReconciliationGrid.GridBrowser.gridControl.ItemsSource = items;
            this.ReconciliationGrid.GridBrowser.gridControl.SelectAll();
        }
        
        #endregion


        #region Handlers

        private void InitHandlers()
        {

            this.ReconciliationGrid.GridBrowser.FormatConditionsEventHandler += BuildFormatConditions;
            
        }


        private List<FormatCondition> BuildFormatConditions()
        {
            List<FormatCondition> conditions = new List<FormatCondition>(0);
            Measure leftMeasure = this.EditedObject.leftMeasure; 
            Measure rightMeasure = this.EditedObject.rightMeasure;
            if (this.EditedObject.useDebitCredit.Value)
            {
                Misp.Kernel.Domain.ReconciliationContext context = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationContextService().getReconciliationContext();
                if (context != null && context.dcNbreAttribute != null)
                {
                    GrilleColumn creditDebitColumn = this.ReconciliationGrid.GridBrowser.Grille.GetColumn(ParameterType.SCOPE.ToString(), context.dcNbreAttribute.oid.Value);
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
                    GrilleColumn amountColumn = this.ReconciliationGrid.GridBrowser.Grille.GetColumn(ParameterType.MEASURE.ToString(), leftMeasure.oid.Value);
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
                    GrilleColumn amountColumn = this.ReconciliationGrid.GridBrowser.Grille.GetColumn(ParameterType.MEASURE.ToString(), rightMeasure.oid.Value);
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

        #endregion


        
    }
}
