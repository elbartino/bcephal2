using Misp.Bfc.Model;
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

        public ReviewForm Form { get; private set; }
        public int DefaultActiveTab { get; set; }

        public ReviewBrowser(int defaultTab = 0)
        {
            this.DefaultActiveTab = defaultTab;
            InitializeComponent();
            InitializeHandlers();
        }

        public void Display(PrefundingAccountData data)
        {
            this.Form.Display(data);
        }

        public void InitializeComponent()
        {
            this.Form = new ReviewForm();
            this.Form.TabControl.SelectedIndex = this.DefaultActiveTab;
            LayoutDocument page = new LayoutDocument();
            page.CanClose = false;
            page.CanFloat = false;
            page.Title = "Review";
            page.Content = this.Form;
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
