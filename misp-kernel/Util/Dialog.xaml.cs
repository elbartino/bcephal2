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

namespace Misp.Kernel.Util
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
        }

        public Dialog(string title) : this()
        {
            this.Title = title;
            
        }

        public Dialog(string title, UIElement control) : this(title)
        {
            SetMainControl(control);
        }

        public ScrollViewer MainPanel { get { return mainPanel; } }
        public Button OkButton { get { return okButton; } }
        public Button CancelButton { get { return cancelButton; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contol"></param>
        public void SetMainControl(UIElement contol)
        {
            MainPanel.Content = contol;
        }

        public void OkButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
