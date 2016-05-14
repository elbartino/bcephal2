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

namespace Misp.Kernel.Ui.Base.Dimension
{
    /// <summary>
    /// Interaction logic for DimensionItemTextBlock.xaml
    /// </summary>
    public partial class DimensionItemTextBlock : UserControl
    {
        public DimensionItemTextBlock()
        {
            InitializeComponent();
        }

        public void Add(DimensionElement de)
        {
            DimensionItemTB.Inlines.Add(de); 
        }

        public void Clear()
        {
            DimensionItemTB.Inlines.Clear();
        }

        public void Remove(DimensionElement de)
        {
            //DimensionItemTB.Inlines.Remove(de);
        }
    }
}
