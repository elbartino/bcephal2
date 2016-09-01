using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Sourcing.GridViews;
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
    /// Interaction logic for PostingToolBar1.xaml
    /// </summary>
    public partial class PostingToolBar : Grid
    {

        public decimal credit = 0;
        public decimal debit = 0;

        public PostingToolBar()
        {
            InitializeComponent();
        }

        public void displayBalance(System.Collections.IList rows, Kernel.Domain.ReconciliationContext context, Grille grid)
        {
            credit = 0;
            debit = 0;
            if (rows == null || rows.Count == 0)
            {
                displayBalance(0, 0);
                return;
            }
            GrilleColumn amountColumn = grid.GetAmountColumn(context);
            GrilleColumn creditDebitColumn = grid.GetDCColumn(context);
            foreach (object row in rows)
            {
                if (row is GridItem)
                {
                    Object[] datas = ((GridItem)row).Datas;
                    object item = datas[creditDebitColumn.position];
                    Boolean isCredit = item != null && item.ToString().Equals("C", StringComparison.OrdinalIgnoreCase);
                    Boolean isDebit = item != null && item.ToString().Equals("D", StringComparison.OrdinalIgnoreCase);
                    Decimal amount = 0;
                    item = datas[amountColumn.position];
                    try
                    {
                        Decimal.TryParse(item.ToString(), out amount);
                    }
                    catch (Exception) { }
                    if (isCredit) credit += amount;
                    else if (isDebit) debit += amount; 
                }            
            }
            displayBalance(credit, debit);
        }

        public void displayBalance(System.Collections.IList items)
        {
            credit = 0;
            debit = 0;
            if (items == null || items.Count == 0) displayBalance(0, 0);
            foreach (object item in items)
            {
                if (item is PostingBrowserData)
                {
                    PostingBrowserData data = (PostingBrowserData)item;
                    if (String.IsNullOrWhiteSpace(data.dc)) continue;
                    if (data.dc.Equals("D")) debit += data.amount;
                    else credit += data.amount;
                }
            }
            displayBalance(credit, debit);
        }

        /// <summary>
        /// Set the debit value to corresponding label
        /// Add the new posting debit value to the current debit amount
        /// </summary>
        /// <param name="val">New posting debit value</param>
        public void displayBalance(decimal credit, decimal debit) 
        {
            this.credit = credit;
            this.debit = debit;
            decimal balance = getBalance();
            this.label.Content = "Sum crebit: " + credit + " | Sum debit: " + debit + " | Balance: " + balance;
        }

        public decimal getBalance()
        {
            return credit - debit;
        }

        public void HideButtons()
        {
            //this.Children.Remove(reconciliateButton);
            //this.Children.Remove(resetRecoButton);
            //this.Children.Remove(deleteButton);
            //reconciliateButtonCol.Width = new GridLength(0, GridUnitType.Star);
            //resetRecoButtonCol.Width = new GridLength(0, GridUnitType.Star);
            //deleteRecoButtonCol.Width = new GridLength(0, GridUnitType.Star);
            HideDeleteButton();
            HideRecoButton();
            HideResetButton();
        }

        public void HideDeleteButton()
        {            
            deleteRecoButtonCol.Width = new GridLength(0, GridUnitType.Star);
        }

        public void HideRecoButton()
        {
            reconciliateButtonCol.Width = new GridLength(0, GridUnitType.Star);
        }

        public void HideResetButton()
        {
            resetRecoButtonCol.Width = new GridLength(0, GridUnitType.Star);
        }


    }
}
