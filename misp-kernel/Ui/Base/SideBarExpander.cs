using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Misp.Kernel.Ui.Base
{
    public class SideBarExpander : Expander
    {

        /// <summary>
        /// 
        /// </summary>
        public SideBarExpander()
        {
            InitComponents();
        }

        public SideBarExpander(string header) : this()
        {
            this.Header = header;
        }

        public SideBarExpander(string header, bool expanded)
            : this(header)
        {
            this.IsExpanded = expanded;
        }

        public StackPanel ContentPanel { get; set; }

        protected virtual void InitComponents()
        {
            this.ContentPanel = new StackPanel();
            this.ContentPanel.Background = Brushes.White;
            this.ContentPanel.Orientation = Orientation.Vertical;
            this.Content = this.ContentPanel;
            this.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            this.FontFamily = new System.Windows.Media.FontFamily("Arial");
            this.BorderBrush = Brushes.LightBlue; 
            this.Background = Brushes.LightBlue;
        }

    }
}
