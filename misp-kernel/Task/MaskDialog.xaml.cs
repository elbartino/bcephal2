using Misp.Kernel.Util;
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

namespace Misp.Kernel.Task
{
    /// <summary>
    /// Interaction logic for MaskDialog.xaml
    /// </summary>
    public partial class MaskDialog : Window
    {

        public static String DEFAULT_MESSAGE = "Busy...Please Wait";
        

        public MaskDialog()
        {
            InitializeComponent();
        }

        public void SetMessage(String message)
        {
            MessageTextBlock.Text = message != null ? message : DEFAULT_MESSAGE;
        }
    }
}
