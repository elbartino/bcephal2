﻿using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Util;
using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace Misp.Reconciliation.Posting
{
    /// <summary>
    /// Interaction logic for PostingConfirmationDialog.xaml
    /// </summary>
    public partial class PostingConfirmationDialog : Window
    {
        public PostingService PostingService { get; set; }

        public PostingConfirmationDialog()
        {
            InitializeComponent();
            this.confirmationMessageLabel.Content = "You are about to create a reconciliation posting for the selected items.\nDo you confirm the operation?";
            toolbar.HideButtons();
        }

        public void display(IList items)
        {
            grid.ItemsSource = items;
            grid.SelectAll();
            toolbar.displayBalance(items);
            decimal balance = toolbar.getBalance();
            writeOffForm.Visibility = balance == 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            
            if (balance != 0)
            {
                writeOffForm.displayAount(balance);
                List<PostingBrowserData> datas = new List<PostingBrowserData>(0);
                List<String> numbers = new List<String>(0);
                foreach (object item in items)
                {
                    if (item is PostingBrowserData)
                    {
                        PostingBrowserData data = (PostingBrowserData)item;
                        if (!numbers.Contains(data.account))
                        {
                            datas.Add(data);
                            numbers.Add(data.account);
                        }
                    }
                }
                writeOffForm.debitedOrCreditedAccountComboBox.ItemsSource = datas;
                List<Account> accounts = this.PostingService.getAllAccounts();
                writeOffForm.writeOffAccountComboBox.ItemsSource = accounts;
            }
        }

        public bool validateEdition()
        {
            decimal balance = toolbar.getBalance();
            if (balance != 0)
            {
                if (writeOffForm.writeOffAccountComboBox.SelectedItem == null)
                {
                    MessageDisplayer.DisplayWarning("Reconciliation write-off", "The write-off account can't be empty!");
                    return false;
                }
                if (writeOffForm.debitedOrCreditedAccountComboBox.SelectedItem == null)
                {
                    String message = balance < 0 ? "The account to credite can't be empty!" : "The account to debite can't be empty!";
                    MessageDisplayer.DisplayWarning("Reconciliation write-off", message);
                    return false;
                }
            }
            return true;
        }

        public String getWriteOffDC()
        {
            decimal balance = toolbar.getBalance();
            return balance < 0 ? "D" : "C";
        }

        public decimal getWriteOffAmount()
        {
            return Math.Abs(toolbar.getBalance());
        }

        public Account getWriteOffAccount()
        {
            return (Account)writeOffForm.writeOffAccountComboBox.SelectedItem;
        }

        public PostingBrowserData getDebitedOrCreditedAccount()
        {
            return (PostingBrowserData)writeOffForm.debitedOrCreditedAccountComboBox.SelectedItem;
        }

    }
}
