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

namespace Misp.Planification.Diagram
{
    /// <summary>
    /// Interaction logic for TransformationTreeDiagram.xaml
    /// </summary>
    public partial class TransformationTreeDiagram : Grid
    {

        public TransformationTreeDiagram()
        {
            InitializeComponent();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            designerCanvas.SetReadOnly(readOnly);
        }

        private void onZoom(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = zoomSlide.Value;
            if (designerCanvas != null) designerCanvas.Zoom(value);
        }

        private void onAddNewObject(object sender, MouseButtonEventArgs args)
        {
           // if (designerCanvas != null) designerCanvas.AddNewObject();
        }

        private void onAddNewValueChain(object sender, MouseButtonEventArgs args)
        {
           // if (designerCanvas != null) designerCanvas.AddNewValueChain();
        }


        private void onDesignerCanvasDrop(object sender, DragEventArgs e)
        {

        }
    }
}
