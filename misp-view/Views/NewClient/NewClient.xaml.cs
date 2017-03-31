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


namespace misp_view.Views.NewClient
{
    /// <summary>
    /// Logique d'interaction pour NewClient.xaml
    /// </summary>
    public partial class NewClient : UserControl
    {

        public DetailClient detailC;
        //public ActivateEventHandler Activated;
        public Misp.Kernel.Ui.Base.ChangeItemEventHandler Deleted;
        public Misp.Kernel.Ui.Base.ChangeItemEventHandler Added;
        public bool trow = false;

        public NewClient()
        {
            InitializeComponent();
            HideDetailsView();
            init();
        }

        protected void init()
        {
            this.AddButton.Click += OnAddButtonClick;
            this.DeleteButton.Click += OnDeleteButtonClick;
            this.ShowDetailsButton.Click += OnShowDetails;
            this.HideDetailsButton.Click += OnHideDetails;
            //this.DClient.Activated += OnActivate;
            DClient.btnAdd.Click += updateText;

        }

        private void updateText(object sender, RoutedEventArgs e)
        {
            if (DClient.tbLastName.Text != "" || DClient.tbLastName.Text != "")
            {
                tbNameClient.Text = DClient.tbFirstName.Text;
                tbLastNameClient.Text = DClient.tbLastName.Text;
                cbMainContact.Items.Add(new ComboBoxItem() { Content = DClient.tbFirstName.Text + ", " + DClient.tbLastName.Text });
                DClient.btnAdd.Visibility = Visibility.Hidden;
            }
        }

        public void display()
        {   Grid t = gridNew;
            t.Height = gridNew.Height;
            t.Width = gridNew.Height;
            Button add = this.AddButton;
            Button delete = this.DeleteButton;
            Label lName = this.lNameClient;
            TextBox tbName = tbNameClient;
            Label lLastName = this.lLastNameClient;
            TextBox tbLastName = tbLastNameClient;

            //t.Children.Add(add);
            //t.Children.Add(delete);
            //t.Children.Add(lName);
            //t.Children.Add(lLastName);
            //t.Children.Add(tbName);
            //t.Children.Add(tbLastName);

        }

        public void Reset()
        {
            /*trow = false;
            
            this.
            this.CommentTextBlock.Text = "";
            trow = true;
            */
        }


        private void OnHideDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView();
        }

        private void OnShowDetails(object sender, RoutedEventArgs e)
        {
            HideDetailsView(false);
        }

        private void HideDetailsView(bool hideDetails = true)
        {
            DClient.Visibility = hideDetails ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.HideDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            this.ShowDetailsButton.Visibility = hideDetails ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private bool isDeleted = false;
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (trow && Deleted != null)
            {
                isDeleted = true;
                Deleted(this);
            }
            isDeleted = false;
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {   
            display();

            //if (trow && Added != null) Added(this);
            //MessageBox.Show("CLick");
        }




        //private void OnActivate(object item)
        //{
        //    ActivateComponent(item);
        //    MessageBox.Show("On Activate");
        //}


        //private void OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    ActiveHandler();
        //    MessageBox.Show("Mouse");
        //}

        #region Activation Method
        //private void ActivateComponent(object item)
        //{
        //    if (Activated != null)
        //    {
        //        if (item is DetailClient)
        //        {
        //            this.detailC = (DetailClient)item;
        //        }
        //        Activated(this);
        //        MessageBox.Show("Activation");
        //    }
        //}
        #endregion

        //private void ActiveHandler()
        //{
        //    if (Activated != null) Activated(this);
        //}


    }
}