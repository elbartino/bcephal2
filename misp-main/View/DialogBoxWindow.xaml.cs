using DevExpress.Xpf.Core;
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

namespace Moriset_Main_final.View
{
    /// <summary>
    /// Logique d'interaction pour DialogBoxWindow.xaml
    /// </summary>
    public partial class DialogBoxWindow : DXWindow
    {
        public DialogBoxWindow()
        {
            InitializeComponent();
        }

        public string FolderTile
        {
            get { return cbTileFolder.Text; }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
