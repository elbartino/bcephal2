using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Bfc.Review
{
    public class ReviewBrowser : LayoutDocumentPane, IView
    {

        public ReviewBrowser()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        public void InitializeComponent()
        {
            LayoutDocument page = new LayoutDocument();
            page.CanClose = false;
            page.CanFloat = false;
            page.Title = "Review";
            page.Content = null;
            this.Children.Add(page);
        }

        public void InitializeHandlers()
        {

        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
           
        }

        public void SetReadOnly(bool readOnly)
        {
           
        }

        public void Customize(List<Kernel.Domain.Right> rights, bool readOnly)
        {
            
        }

        public bool IsReadOnly
        {
            get;
            set;
        }
    }
}
