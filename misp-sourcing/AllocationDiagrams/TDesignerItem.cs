using DiagramDesigner;
using DiagramDesigner.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Sourcing.AllocationDiagrams
{

    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class TDesignerItem : DesignerItem
    {
        static TDesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TDesignerItem), new FrameworkPropertyMetadata(typeof(TDesignerItem)));
        }

        public TDesignerItem(Guid id) : base(id) { }

        public TDesignerItem() : base() { }

        public override void Edit()
        {
            if (Editor != null && !Editor.IsVisible)
            {
                //Editor.Text = Renderer.Text;
                //Editor.Visibility = System.Windows.Visibility.Visible;
                //Renderer.Visibility = System.Windows.Visibility.Hidden;
                //Editor.SelectAll();
                //Editor.Focus();
            }
        }
    }
}
