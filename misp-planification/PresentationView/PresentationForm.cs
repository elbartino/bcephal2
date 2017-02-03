using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office.EDraw;
using System;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Misp.Planification.PresentationView
{
    public class PresentationForm : Grid, IEditableView<Presentation>
    {

        public bool IsReadOnly { get; set; }
        
        public bool IsModify
        {
            get;
            set;
        }

        public Presentation EditedObject
        {
            get;
            set;
        }

        public List<Kernel.Domain.Browser.InputTableBrowserData> listeBrowserData { get; set; }

        public EdrawSlide SlideView { get; set; }

        public PresentationPropertiesPanel PresentationPropertiesPanel { get; set; }
        WindowsFormsHost windowsFormsHost;
        System.Windows.Controls.Image image;
        
        public PresentationForm()
        {
            InitializeComponents();
        }


        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }


        protected virtual void InitializeComponents()
        {
            Uri rd1 = new Uri("../Resources/Styles/TabControl.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(Application.LoadComponent(rd1) as ResourceDictionary);

            this.PresentationPropertiesPanel = new PresentationPropertiesPanel();
            this.listeBrowserData = new List<Kernel.Domain.Browser.InputTableBrowserData>(0);
            
            Grid grid = new Grid();
            try
            {
                windowsFormsHost = new WindowsFormsHost();
                this.SlideView = new EdrawSlide();
                windowsFormsHost.Child = SlideView;

                image = new System.Windows.Controls.Image();
                grid.Children.Add(windowsFormsHost);
                grid.Children.Add(image);
                image.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e);
            }
            this.Children.Add(grid);
        }

        public Presentation getNewObject()
        {
            return new Presentation();
        }

        public bool validateEdition()
        {
            return true;
        }

        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = getNewObject();
            this.SlideView.DocumentUrl = this.EditedObject.slideFileName;
            this.PresentationPropertiesPanel.fillPresentation(this.EditedObject);
        }

        public void displayObject()
        {
            this.PresentationPropertiesPanel.displayPresentation(this.EditedObject);
            //this.PresentationPropertiesPanel.fillReportList(this.listeBrowserData);
        }

        public List<object> getEditableControls()
        {
            return new List<object>(0);
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            
        }

        bool isMasked = false;
        public void Mask(bool mask)
        {
            if (mask)
            {
                if (isMasked) return;
                image.Source = GetScreenInt();
                image.Visibility = System.Windows.Visibility.Visible;
                windowsFormsHost.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                image.Visibility = System.Windows.Visibility.Hidden;
                windowsFormsHost.Visibility = System.Windows.Visibility.Visible;
            }
            isMasked = mask;
        }

        public BitmapSource GetScreenInt()
        {
            Bitmap bm = new Bitmap(SlideView.ClientRectangle.Width, SlideView.ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bm);
            PrintWindow(SlideView.Handle, g.GetHdc(), 0);
            g.ReleaseHdc(); g.Flush();
            BitmapSource src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            src.Freeze();
            bm.Dispose();
            bm = null;
            return src;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
    }
}
