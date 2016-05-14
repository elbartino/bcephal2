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
    /// Interaction logic for DimensionPanel.xaml
    /// </summary>
    public partial class DimensionPanel : UserControl
    {
        public DimensionPanel()
        {
            InitializeComponent();
            DimensionItemTextBlock dtb = new DimensionItemTextBlock();
            //Add(dtb); Add(dtb); Add(dtb);
        }

        public void Add(DimensionItemTextBlock dtb)
        {
            DimPanel.Children.Add(dtb);
        }

        public void Remove(DimensionItemTextBlock dtb)
        {
            DimPanel.Children.Remove(dtb);
        }
        public void Clear()
        {
            DimPanel.Children.Clear();
        }
    }
}
