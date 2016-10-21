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

namespace DiagramDesigner
{
    /// <summary>
    /// Interaction logic for DesignerContextMenu.xaml
    /// </summary>
    public partial class DesignerContextMenu : ContextMenu
    {
        public DesignerContextMenu()
        {
            InitializeComponent();
        }

        public void CustomizeForDiagram()
        {
            ObjectMenuItem.Visibility = System.Windows.Visibility.Visible;
            ValueChainMenuItem.Visibility = System.Windows.Visibility.Visible;
            EditMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            CutMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            CopyMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            PasteMenuItem.Visibility = System.Windows.Visibility.Visible;
            DeleteMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            HorizontalAlignmentMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            VerticalAlignmentMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            OrderMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            GroupingMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            SelectAllMenuItem.Visibility = System.Windows.Visibility.Visible;
        }

        public void CustomizeForAllocation() 
        {
            ValueChainMenuItem.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void CustomizeForItem()
        {
            ObjectMenuItem.Visibility = System.Windows.Visibility.Visible;
            ValueChainMenuItem.Visibility = System.Windows.Visibility.Visible;
            EditMenuItem.Visibility = System.Windows.Visibility.Visible;
            CutMenuItem.Visibility = System.Windows.Visibility.Visible;
            CopyMenuItem.Visibility = System.Windows.Visibility.Visible;
            PasteMenuItem.Visibility = System.Windows.Visibility.Visible;
            DeleteMenuItem.Visibility = System.Windows.Visibility.Visible;
            HorizontalAlignmentMenuItem.Visibility = System.Windows.Visibility.Visible;
            VerticalAlignmentMenuItem.Visibility = System.Windows.Visibility.Visible;
            OrderMenuItem.Visibility = System.Windows.Visibility.Visible;
            GroupingMenuItem.Visibility = System.Windows.Visibility.Visible;
            SelectAllMenuItem.Visibility = System.Windows.Visibility.Visible;
        }

        public void CustomizeForConnector()
        {
            ObjectMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            ValueChainMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            EditMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            CutMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            CopyMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            PasteMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            DeleteMenuItem.Visibility = System.Windows.Visibility.Visible;
            HorizontalAlignmentMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            VerticalAlignmentMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            OrderMenuItem.Visibility = System.Windows.Visibility.Visible;
            GroupingMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            SelectAllMenuItem.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
