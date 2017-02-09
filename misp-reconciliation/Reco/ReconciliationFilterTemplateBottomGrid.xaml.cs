using Misp.Kernel.Application;
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

        public ReconciliationFilterService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterService(); } }

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
