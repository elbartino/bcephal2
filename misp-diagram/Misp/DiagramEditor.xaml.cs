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

namespace DiagramDesigner.Misp
{
    /// <summary>
    /// Interaction logic for DiagramEditor.xaml
    /// </summary>
    public partial class DiagramEditor : Grid
    {
        public DiagramEditor()
        {
            InitializeComponent();
        }

        private void DesignerCanvas_Drop_1(object sender, DragEventArgs e)
        {

        }
                

        private void onZoom(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = zoomSlide.Value;
            if (DesignerCanvas != null) DesignerCanvas.Zoom(value);
        }
    }
}
