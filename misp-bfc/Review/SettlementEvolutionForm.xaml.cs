using DevExpress.Xpf.Charts;
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
    /// Interaction logic for SettlementEvolutionForm.xaml
    /// </summary>
    public partial class SettlementEvolutionForm : ScrollViewer
    {

        #region Properties

        public ChangeEventHandler SchemeChanged { get; set; }

        public ChangeEventHandler PeriodChanged { get; set; }

        public BfcItem Scheme { get; private set; }

        bool throwHandlers;

        #endregion


        #region Constructors

        public SettlementEvolutionForm()
        {
            InitializeComponent();
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(List<SettlementEvolutionData> datas)
        {
            throwHandlers = false;
            this.Grid.ItemsSource = datas;
            DisplayChart(new List<SettlementEvolutionChartData>());
            throwHandlers = true;
        }

        public void DisplayChart(List<SettlementEvolutionChartData> chartDatas) 
        {
            throwHandlers = false;
            List<List<SettlementEvolutionChartData>> datas = new List<List<SettlementEvolutionChartData>>(0);
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                List<SettlementEvolutionChartData> values = new List<SettlementEvolutionChartData>(0);
                datas.Add(values);
                String platform = "Platform " + i;
                for (int j = 0; j < 50; j++)
                {
                    SettlementEvolutionChartData data = new SettlementEvolutionChartData();
                    data.platform = platform;
                    data.value = random.Next(0, 1000);
                    data.date = DateTime.Now.AddDays(j + 1);
                    values.Add(data);
                }
            }

            XYDiagram2D diagram = new XYDiagram2D();
            for (int i = 0; i < datas.Count; i++) 
            {
                List<SettlementEvolutionChartData> values = datas[i];
                LineSeries2D line = new LineSeries2D();
                line.Name = "platform" + i;
                line.DisplayName = values[0].platform;
                for (int j = 0; j < values.Count; j++)
                {
                    SettlementEvolutionChartData data = values[j];
                    line.Points.Add(new SeriesPoint(data.date, data.value));
                }
                diagram.Series.Add(line);
            }
            this.Chart.Diagram = diagram;

            throwHandlers = true;
        }

        public void FillFilter(ReviewFilter filter)
        {
            if(filter == null) filter = new ReviewFilter();
            if (this.Scheme != null)
            {
                filter.schemeIdOids.Add(this.Scheme.oid.Value);
            }
            filter.startDateTime = this.StartDatePicker.SelectedDate;
            filter.endDateTime = this.EndDatePicker.SelectedDate;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.SchemeComboBox.SelectionChanged += OnselectScheme;
            this.StartDatePicker.SelectedDateChanged += OnselectPeriod;
            this.EndDatePicker.SelectedDateChanged += OnselectPeriod;
        }

        private void OnselectPeriod(object sender, SelectionChangedEventArgs e)
        {
            if (throwHandlers && PeriodChanged != null) PeriodChanged();
        }

        private void OnselectScheme(object sender, SelectionChangedEventArgs e)
        {
            Object obj = this.SchemeComboBox.SelectedItem;
            if (obj != null && obj is BfcItem)
            {
                BfcItem item = (BfcItem)obj;
                SchemeTextBox.Text = item.id;
                this.Scheme = item;
            }
            else
            {
                this.Scheme = null;
                SchemeTextBox.Text = "";
            }
            if (throwHandlers && SchemeChanged != null) SchemeChanged();
        }

        #endregion

    }
}
