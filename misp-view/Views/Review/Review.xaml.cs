using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace misp_view.Views.Review
{
    /// <summary>
    /// Logique d'interaction pour Review.xaml
    /// </summary>
    public partial class Review : UserControl
    {
        public Review()
        {
            InitializeComponent();
            testFill();
            testFillAg();
            
        }



        private void testFill()
        {
            var data = new Test { Total = "Total", Amount = "To receive" };
            var data2 = new Test { Amount = "Paid" };
            var data3 = new Test { Amount = "Pending balance" };
            var data4 = new Test { Amount = "NRFTX" };
            DataGridTest.Items.Add(data);
            DataGridTest.Items.Add(data2);
            DataGridTest.Items.Add(data3);
            DataGridTest.Items.Add(data4);

        }
        
        private void testFillAg()
        {
            var data = new TestAg { a = 10000, b = 9000, c = 8000, d = 4000, e = 8000, f = 7000, g = 10500 };
            var data2 = new TestAg { a = 0, b = 5000, c = 8000, d = 2000, e = 8000, f = 7000, g = 10500};
            var data3 = new TestAg { a = 10000, b = 4000, c = 0, d = 2000, e = 0, f = 0, g= 0 };
            DataGridAg.Items.Add(data);
            DataGridAg.Items.Add(data2);
            DataGridAg.Items.Add(data3);
        }


    }
    public class TestAg
    {
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        public int e { get; set; }
        public int f { get; set; }
        public int g { get; set; }

    }

    public class Test
    {
        public string Total { get; set; }
        public string Amount { get; set; }
    }
}

