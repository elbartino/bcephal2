﻿using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Misp.Kernel.Administration.User
{
    /// <summary>
    /// Interaction logic for AdministratorPanel.xaml
    /// </summary>
    public partial class AdministratorPanel : Grid
    {
        public static string notEmpty = "cannot be empty!";

        public AdministratorPanel()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            NameTextBox.KeyUp += OnEnter;
            FirstNameTextBox.KeyUp += OnEnter;
            EmailTextBox.KeyUp += OnEnter;
            LoginTextBox.KeyUp += OnEnter;
            PasswordTextBox.KeyUp += OnEnter;
            ConfirmPasswordTextBox.KeyUp += OnEnter;
        }

        private void OnEnter(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter) SaveButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        public bool ValidateEdition()
        {
            String errors = "";
            String line = "";
            bool focusSetted = false;
            if (String.IsNullOrWhiteSpace(NameTextBox.Text) && String.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                errors += line + "Name and first name can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    if (String.IsNullOrWhiteSpace(NameTextBox.Text))
                    {
                        NameTextBox.Focus();
                        NameTextBox.SelectAll();
                    }
                    else
                    {
                        FirstNameTextBox.Focus();
                        FirstNameTextBox.SelectAll();
                    }
                    focusSetted = true;
                }
            }
                        
            if (String.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                //errors += line + "Email can't be empty.";
                //line = "\n";
                //if (!focusSetted)
                //{
                //    EmailTextBox.Focus();
                //    EmailTextBox.SelectAll();
                //    focusSetted = true;
                //}
            }
            else if (!UserUtil.validateEmail(EmailTextBox.Text))
            {
                errors += line + "Wrong email format.";
                line = "\n";
                if (!focusSetted)
                {
                    EmailTextBox.Focus();
                    EmailTextBox.SelectAll();
                    focusSetted = true;
                }
            }

            if (String.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                errors += line + "Login can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    LoginTextBox.Focus();
                    LoginTextBox.SelectAll();
                    focusSetted = true;
                }
            }

            if (String.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                errors += line + "Password can't be empty.";
                line = "\n";
                if (!focusSetted)
                {
                    PasswordTextBox.Focus();
                    PasswordTextBox.SelectAll();
                    focusSetted = true;
                }
            }
            else if (!Util.UserUtil.validatePassword(PasswordTextBox.Password, ConfirmPasswordTextBox.Password))
            {
                errors += line + "Password does not match.";
                line = "\n";
                if (!focusSetted)
                {
                    ConfirmPasswordTextBox.Focus();
                    ConfirmPasswordTextBox.SelectAll();
                    focusSetted = true;
                }
            }
            bool isValid = String.IsNullOrWhiteSpace(errors);
            this.Console.Text = errors;
            this.Console.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            return isValid;
        }


       


        public Domain.User Fill()
        {
            Domain.User user = new Domain.User();
            user.active = true;
            user.login = LoginTextBox.Text.Trim();
            user.email = EmailTextBox.Text.Trim();
            user.name = NameTextBox.Text.Trim();
            user.firstName = FirstNameTextBox.Text.Trim();
            user.password = ConfirmPasswordTextBox.Password;
            user.administrator = true;
            return user;
        }

    }
}
