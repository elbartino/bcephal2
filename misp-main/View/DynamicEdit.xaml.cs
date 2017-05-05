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
using System.Windows.Shapes;
using DevExpress.Xpf.Core;

namespace Moriset_Main_final.View
{
    /// <summary>
    /// Logique d'interaction pour DynamicEdit.xaml
    /// </summary>
    public partial class DynamicEdit : DXWindow
    {
        public AddTile d = new AddTile();
        public DynamicEdit()
        {
            InitializeComponent();
            init();
        }

        public string[] tabSelected = new string[20];

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTile at = new AddTile();
            spAddTile.Children.Add(at);
        }
        protected void init()
        {
            
            this.btnOk.Click += confirm;
            ConfigurationTile ct = new ConfigurationTile();
            spConfig.Children.Add(ct);

        }

        private void confirm(object sender, RoutedEventArgs e)
        {   
            this.DialogResult = true;
            this.Close();
        }
    }
}
