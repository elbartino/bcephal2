using DevExpress.Xpf.Grid;
using Misp.Bfc.Model;
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

namespace Misp.Bfc.Review
{
    /// <summary>
    /// Interaction logic for AgeingBalanceForm.xaml
    /// </summary>
    public partial class AgeingBalanceForm : Grid
    {
        
        #region Properties

        public ChangeEventHandler SearchDetail { get; set; }
        
        bool throwHandlers;

        public bool IsDetailGridBussy
        {
            set { this.DetailGridLoadingDecorator.IsSplashScreenShown = value; }
            get { return this.DetailGridLoadingDecorator.IsSplashScreenShown.Value; }
        }

        public bool IsAlreadyLoaded { get; private set; }

        #endregion


        #region Constructors

        public AgeingBalanceForm()
        {
            this.IsAlreadyLoaded = false;
            InitializeComponent();
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void DisplayTotal(List<AgeingBalanceData> datas)
        {
            throwHandlers = false;
            this.TotalGrid.ItemsSource = datas;
            this.DetailExpander.IsExpanded = false;
            throwHandlers = true;
            this.IsAlreadyLoaded = true;
        }

        public void DisplayDetails(List<AgeingBalanceData> datas)
        {
            throwHandlers = false;
            this.DetailGrid.ItemsSource = datas;
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
            this.DetailExpander.Expanded += OnExpended;
        }

        private void OnExpended(object sender, RoutedEventArgs e)
        {
            if (throwHandlers && SearchDetail != null) SearchDetail();
        }

        #endregion

    }
}
