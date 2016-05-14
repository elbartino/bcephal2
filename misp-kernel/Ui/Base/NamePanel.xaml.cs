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

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for NamePanel.xaml
    /// </summary>
    public partial class NamePanel : Grid
    {

        public event OnValidateEventHandler OnValidate;
        public delegate void OnValidateEventHandler(bool isOk,String textValue);

        public NamePanel()
        {
            InitializeComponent();
            this.NameTextBox.KeyUp +=NameTextBox_KeyUp;
        }

        private void NameTextBox_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                if (OnValidate != null) OnValidate(false,"");
            }
            else if (args.Key == Key.Enter)
            {
                if (OnValidate != null) OnValidate(true,this.EditedName);
            }
        }

        public string EditedName
        {
            get { return NameTextBox.Text; }
            set { NameTextBox.Text = value; }
        }
    }
}
