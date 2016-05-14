using Misp.Kernel.Domain;
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

namespace Misp.Reconciliation.Posting
{
    /// <summary>
    /// Interaction logic for ReconciliationFliterPanel1.xaml
    /// </summary>
    public partial class PeriodTargetFilterForm : Grid
    {
        public Kernel.Ui.Base.ChangeEventHandler ChangeHandler;

      
        public bool thrawChange = true;

        public PeriodTargetFilterForm()
        {
            InitializeComponent();
            this.periodFilter.CustomizeForReport();
            reset();
            IntializeHandlers();            
        }

       

        private void IntializeHandlers()
        {

            this.targetFilter.Changed += OnChange;
            this.periodFilter.Changed += OnChange;
        }

        
        /// <summary>
        /// display filter scope and period 
        /// 
        /// </summary>
        /// <param name="bankReco"></param>
        public void Display(PostingFilter filter)
        {
            thrawChange = false;
            reset();
            if (filter != null)
            {
                targetFilter.DisplayScope(filter.filterScope);
                periodFilter.DisplayPeriod(filter.filterPeriod);
            }
            thrawChange = true;
        }

        /// <summary>
        /// fill object
        /// </summary>
        /// <param name="bankReco"></param>
        public void Fill(PostingFilter filter)        {
            if (filter != null)
            {
                filter.filterScope = targetFilter.Scope;
                filter.filterPeriod = periodFilter.Period;
            }
        }

        public void reset()
        {
            targetFilter.DisplayScope(null);
            periodFilter.DisplayPeriod(null);
        }
               
        
        private void OnChange()
        {
            if (ChangeHandler != null && thrawChange) ChangeHandler();
        }

    }
}
