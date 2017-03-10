using Misp.Bfc.Model;
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

namespace Misp.Bfc.Review
{
    /// <summary>
    /// Interaction logic for AgeingBalanceForm.xaml
    /// </summary>
    public partial class AgeingBalanceForm : ScrollViewer
    {
        
        #region Properties

        
        bool throwHandlers;

        #endregion


        #region Constructors

        public AgeingBalanceForm()
        {
            InitializeComponent();
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(List<AgeingBalanceData> datas)
        {
            throwHandlers = false;
            this.Grid.ItemsSource = datas;            
            throwHandlers = true;
        }

        public void FillFilter(ReviewFilter filter)
        {
            if (filter == null) filter = new ReviewFilter();
            /*if (this.Scheme != null)
            {
                filter.schemeIdOids.Add(this.Scheme.oid.Value);
            }
            filter.startDateTime = this.StartDatePicker.SelectedDate;
            filter.endDateTime = this.EndDatePicker.SelectedDate;*/
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            
        }

        #endregion

    }
}
