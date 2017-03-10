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
