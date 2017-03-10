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
            for (int i = 0; i < 5; i++)
            {
                SettlementEvolutionChartData s = new SettlementEvolutionChartData();
                s.PlatForm = "PlatForm" + (i + 1);
                s.value = 20000 * (i + 1);
                s.date = DateTime.Now.AddMonths((i + 1));
                chartDatas.Add(s);
            }
            throwHandlers = false;
            XYDiagram2D diagram = new XYDiagram2D();
            for (int i = 0; i < chartDatas.Count; i++) 
            {
                SettlementEvolutionChartData s = new SettlementEvolutionChartData();
                s = chartDatas[i];
                LineSeries2D line = new LineSeries2D();
                LineSeries2D line1 = new LineSeries2D();
                for (int p = 0; p < 5; p++)
                {
                    
                    int j = i + 1;
                    line.Name ="c"+p+j;
                    line.Points.Add(new SeriesPoint(s.date.ToShortDateString(), s.value));
                    line.Points.Add(new SeriesPoint(s.date.ToShortDateString(), s.value));
                    line1.Points.Add(new SeriesPoint(s.date.ToShortDateString(), s.value));
                    line1.Points.Add(new SeriesPoint(s.date.ToShortDateString(), s.value));    
                }
                

                diagram.Series.Add(line);
                diagram.Series.Add(line1);
            }
            
          

            //BarSideBySideSeries2D series = new BarSideBySideSeries2D();


            

            this.Chart.Diagram = diagram;
            //this.Chart.DataSource = chartDatas;

            throwHandlers = true;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.SchemeComboBox.SelectionChanged += OnselectScheme;         
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
