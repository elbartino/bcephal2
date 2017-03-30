using misp_view.Models;
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
        PrefundingAccountData pf = new PrefundingAccountData();
        SettlementEvolutionData se = new SettlementEvolutionData();
        
        public Review()
        {
            InitializeComponent();
            HideDetailsView();
            HideDetailsFinancialView();
            init();
            testFill();
            testFillAg();

            this.DataContext = pf;
            displayPrefundingAccount(pf);

        }


        protected void init()
        {
            this.ShowDetailsButton.Click += OnShowDetails;
            this.HideDetailsButton.Click += OnHideDetails;
            this.ShowDetailsButtonFinancial.Click += OnShowDetailsFinancial;
            this.HideDetailsButtonFinancial.Click += OnHideDetailsFinancial;


        }
        private void OnHideDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView();
        }

        private void OnShowDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView(false);
        }

        private void OnShowDetailsFinancial(object sender, RoutedEventArgs e)
        {
            HideDetailsFinancialView(false);
        }

        private void OnHideDetailsFinancial(object sender, RoutedEventArgs e)
        {
            HideDetailsFinancialView();
        }


        private void HideDetailsView(bool hideDetails = true)
        {
            pfa.Visibility = hideDetails ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            
            this.HideDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            this.ShowDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void HideDetailsFinancialView(bool hideDetailsFinancial = true)
        {
            fm.Visibility = hideDetailsFinancial ? Visibility.Collapsed : Visibility.Visible;
            this.HideDetailsButtonFinancial.Visibility = hideDetailsFinancial ? Visibility.Hidden : Visibility.Visible;
            this.ShowDetailsButtonFinancial.Visibility = hideDetailsFinancial ? Visibility.Visible : Visibility.Hidden;
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
            var data4 = new TestAg { a = 10500, b = 4200, c = 5000, d = 5000, e = 0, f = 0, g = 0 };
            DataGridAg.Items.Add(data);
            DataGridAg.Items.Add(data2);
            DataGridAg.Items.Add(data3);
            DataGridAg.Items.Add(data4);
        }


        
        public void displayAgeingBalance(AgeingBalanceData ab)
        {

        }
        public void displayCreditRating(creditRatingData cr)
        {

        }
        public void displayGuarantee(GuaranteeData g)
        {

        }
        public void displayListMemberAdvisement(ListMemberAdvisementData lma)
        {

        }
        public void displayListPrefundingAdvisement(ListPrefundingAdvisementData lpfa)
        {

        }
        public void displayListReplenishmentInstruction(ListReplenishmentInstructionData lri)
        {

        }
        public void displayListSettlementAdvisement(ListSettlementAdvisementData lsa)
        {

        }
        public void displayNewMemberAdvisement(NewMemberAdvismentData ma)
        {

        }
        public void displayNewPrefundingAdvisement(NewPrefundingAdvisementData pfa)
        {

        }
        public void displayNewReplenishmentInstruction(NewReplenishmentInstructionData ri)
        {

        }
        public void displayNewSettlementAdvisement(NewSettlementAdvisementData sa)
        {

        }
        public void displayPrefundingAccount(PrefundingAccountData pf)
        {
            pf.sentPrefundingReconcilied = 158;
            pf.ratioPFPeak = 55;
        }
        public void displaySettlementEvolution(SettlementEvolutionData se)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridDetail.Visibility == Visibility.Hidden)
            {
                DataGridDetail.Visibility = Visibility.Visible;
                DataGridAgDetail.Visibility = Visibility.Visible;
            }
            else if(DataGridDetail.Visibility == Visibility.Visible)
            {
                DataGridDetail.Visibility = Visibility.Hidden;
                DataGridAgDetail.Visibility = Visibility.Hidden;
            }
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

