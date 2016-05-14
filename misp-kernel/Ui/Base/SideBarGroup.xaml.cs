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
    /// Interaction logic for SideBarGroup.xaml
    /// </summary>
    public partial class SideBarGroup : Expander
    {
        public SideBarGroup()
        {
            InitializeComponent();
        }

        public SideBarGroup(string header) : this()
        {
            this.Header = header;
        }

        public SideBarGroup(string header, bool expanded)
            : this(header)
        {
            this.IsExpanded = expanded;
        }
    }
}
