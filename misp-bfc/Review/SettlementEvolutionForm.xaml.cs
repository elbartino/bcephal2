using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;
using Misp.Bfc.Model;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SettlementEvolutionForm.xaml
    /// </summary>
    public partial class SettlementEvolutionForm : ScrollViewer
    {

        #region Properties

        public ChangeEventHandler FilterChanged { get; set; }

        public ChangeEventHandler PeriodChanged { get; set; }

        public List<BfcItem> Schemes { get; private set; }

        public List<BfcItem> Platforms { get; private set; }

        bool throwHandlers;

        public bool IsChartBussy
        {
            set { this.ChartLoadingDecorator.IsSplashScreenShown = value; }
            get { return this.ChartLoadingDecorator.IsSplashScreenShown.Value; }
        }

        #endregion


        #region Constructors

        public SettlementEvolutionForm()
        {
            this.Schemes = new List<BfcItem>(0);
            this.Platforms = new List<BfcItem>(0);
            InitializeComponent();
            InitializeHandlers();

            this.EndDatePicker.SelectedDate = DateTime.Now;
            this.StartDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);

            ((TableView)this.Grid.View).BestFitColumns();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(List<SettlementEvolutionData> datas)
        {
            throwHandlers = false;
            this.Grid.ItemsSource = datas;
            throwHandlers = true;
        }

        public void DisplayChart(List<SettlementEvolutionChartData> chartDatas) 
        {
            throwHandlers = false;
            Dictionary<String, LineSeries2D> lines = new Dictionary<string, LineSeries2D>(0);
            XYDiagram2D diagram = new XYDiagram2D();            
            int i = 0;
            foreach (SettlementEvolutionChartData data in chartDatas)
            {
                LineSeries2D line = null;
                if (!lines.ContainsKey(data.platform))
                {
                    line = new LineSeries2D();
                    line.Name = "Platform" + ++i;
                    line.DisplayName = data.platform;
                    lines.Add(data.platform, line);
                    diagram.Series.Add(line);
                }
                if (lines.TryGetValue(data.platform, out line))
                {
                    line.Points.Add(new SeriesPoint(data.date, data.value));
                }
            }
            diagram.ActualAxisX.Title = new AxisTitle() { Content = "Value Date"};
            
            diagram.ActualAxisY.Title = new AxisTitle() { Content = "Amount" };
            diagram.ActualAxisY.Label = new AxisLabel() { TextPattern = "{V:C1}" };
            this.Chart.Diagram = diagram;
            
            throwHandlers = true;
        }

        public void FillFilter(ReviewFilter filter)
        {
            if(filter == null) filter = new ReviewFilter();
            foreach (BfcItem scheme in this.Schemes)
            {
                filter.schemeIdOids.Add(scheme.oid.Value);
            }
            foreach (BfcItem platform in this.Platforms)
            {
                filter.platformIdOids.Add(platform.oid.Value);
            }
            filter.startDateTime = this.StartDatePicker.SelectedDate;
            filter.endDateTime = this.EndDatePicker.SelectedDate;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.StartDatePicker.SelectedDateChanged += OnselectPeriod;
            this.EndDatePicker.SelectedDateChanged += OnselectPeriod;
            this.SchemeComboBoxEdit.PopupClosed += OnSchemePopupClosed;
            this.PlatformComboBoxEdit.PopupClosed += OnPlatformPopupClosed;
        }

        private void OnSchemePopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Normal)
            {
                this.Schemes = new List<BfcItem>(0);
                SchemeTextBox.Text = "";
                ObservableCollection<object> SelectedItems = this.SchemeComboBoxEdit.SelectedItems;
                if (SelectedItems != null && SelectedItems.Count > 0)
                {
                    String coma = "";
                    foreach (object obj in SelectedItems)
                    {
                        if (obj is BfcItem)
                        {
                            BfcItem item = (BfcItem)obj;
                            this.Schemes.Add(item);
                            SchemeTextBox.Text += coma + item.id;
                            coma = ";";
                        }
                    }
                }
                if (throwHandlers && FilterChanged != null) FilterChanged();
            }
        }

        private void OnPlatformPopupClosed(object sender, DevExpress.Xpf.Editors.ClosePopupEventArgs e)
        {
            if (e.CloseMode == DevExpress.Xpf.Editors.PopupCloseMode.Normal)
            {
                this.Platforms = new List<BfcItem>(0);
                PlatformTextBox.Text = "";
                ObservableCollection<object> SelectedItems = this.PlatformComboBoxEdit.SelectedItems;
                if (SelectedItems != null && SelectedItems.Count > 0)
                {
                    String coma = "";
                    foreach (object obj in SelectedItems)
                    {
                        if (obj is BfcItem)
                        {
                            BfcItem item = (BfcItem)obj;
                            this.Platforms.Add(item);
                            PlatformTextBox.Text += coma + item.id;
                            coma = ";";
                        }
                    }
                }
                if (throwHandlers && FilterChanged != null) FilterChanged();
            }
        }

        private void OnselectPeriod(object sender, SelectionChangedEventArgs e)
        {
            if (throwHandlers && PeriodChanged != null) PeriodChanged();
        }
        
        #endregion

    }
}
