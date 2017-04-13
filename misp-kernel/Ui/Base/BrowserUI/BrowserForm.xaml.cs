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

namespace Misp.Kernel.Ui.Base.BrowserUI
{
    /// <summary>
    /// Interaction logic for BrowserForm.xaml
    /// </summary>
    public partial class BrowserForm : Grid
    {
        public BrowserForm()
        {
            InitializeComponent();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.Grid.View.AllowEditing = !readOnly;
        }
    }
}
