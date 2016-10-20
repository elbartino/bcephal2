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

namespace Misp.Sourcing.AllocationViews
{
    /// <summary>
    /// Interaction logic for AllocationPropertiesPanel.xaml
    /// </summary>
    public partial class AllocationPropertiesPanel : ScrollViewer
    {
        AllocationForm allocationForm = new AllocationForm();

        public AllocationPropertiesPanel()
        {
            InitializeComponent();
            this.AddChild(allocationForm);
        }
    }
}
