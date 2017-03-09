﻿using DevExpress.Xpf.Core;
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
    /// Interaction logic for ReviewForm.xaml
    /// </summary>
    public partial class ReviewForm : UserControl
    {

        #region Properties

        public ChangeEventHandler MemberBankChanged { get; set; }

        public BfcItem MemberBank { get; private set; }
        bool throwHandlers;

        #endregion


        #region Constructors

        public ReviewForm()
        {            
            InitializeComponent();
            ThemeManager.SetThemeName(this, "None");
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(PrefundingAccountData data)
        {
            throwHandlers = false;
            this.PrefundingAccountForm.Display(data);
            throwHandlers = true;
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            this.MemberBankComboBox.SelectionChanged += OnselectMemberBank;

        }

        private void OnselectMemberBank(object sender, SelectionChangedEventArgs e)
        {
            Object obj = this.MemberBankComboBox.SelectedItem;
            if (obj != null && obj is BfcItem)
            {
                BfcItem item = (BfcItem)obj;
                MemberBankTextBox.Text = item.id;
                this.MemberBank = item;
            }
            else
            {
                this.MemberBank = null;
                MemberBankTextBox.Text = "";
            }
            if (throwHandlers && MemberBankChanged != null) MemberBankChanged();
        }

        #endregion

        
    }
}