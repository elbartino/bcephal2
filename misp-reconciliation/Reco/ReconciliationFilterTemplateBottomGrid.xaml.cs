﻿using Misp.Kernel.Application;
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
    /// Interaction logic for ReconciliationFilterTemplateBottomGrid.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateBottomGrid : Grid
    {
        
        #region Properties

        protected bool throwHandler = true;

        /// <summary>
        /// Design en édition
        /// </summary>
        public Grille EditedObject { get; set; }

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        public Decimal LeftAmount { get; set; }
        public Decimal RightAmount { get; set; }
        public Decimal BalanceAmount { get; set; }

        #endregion


        #region Constructors

        public ReconciliationFilterTemplateBottomGrid()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        #endregion


        #region Operations

        public void displayObject()
        {
            throwHandler = false;
            if (this.EditedObject == null) return;
            if (this.GridBrowser.RebuildGrid)
            {
                this.GridBrowser.buildColumns(this.EditedObject);
            }
            throwHandler = true;
        }

        public void AddLines(List<long> oids, String side)
        {
            Search(oids, side);
        }

        private void Search(List<long> oids, String side)
        {
            try
            {
                GrilleFilter filter = new GrilleFilter();
                filter.grid = new Grille();
                filter.grid.columnListChangeHandler = this.EditedObject.columnListChangeHandler;
                filter.grid.report = this.EditedObject.report;
                filter.grid.reconciliation = this.EditedObject.reconciliation;
                filter.oids = oids;
                filter.page = 1;
                filter.pageSize = int.MaxValue;
                filter.showAll = true;
                GrillePage rows = this.Service.getGridRows(filter);
                this.GridBrowser.displayPage(rows, true, side);
                this.GridBrowser.gridControl.SelectAll();
            }
            catch (ServiceExecption)
            {
                GrillePage rows = new GrillePage();
                rows.rows = new List<object[]>(0);
                this.GridBrowser.displayPage(rows);
            }
        }

        public void SetBalance(Decimal left, Decimal right, Decimal balance)
        {
            this.LeftAmount = left;
            this.RightAmount = right;
            this.BalanceAmount = balance;
            this.CreditLabel.Content = "Left: " + left;
            this.DebitLabel.Content = "Right: " + right;
            this.BalanceLabel.Content = "Balance: " + balance;
        }

        #endregion


        #region Handlers

        protected void InitializeHandlers()
        {
            
        }
        
        #endregion


        #region Utils
                
        #endregion
        
    }
}
