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

namespace Misp.Kernel.Ui.Sidebar
{
    /// <summary>
    /// Interaction logic for StatusSidebarGroupItem.xaml
    /// </summary>
    public partial class StatusSidebarGroupItem : Grid
    {

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public StatusSidebarGroupItem()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void SetSelectedIfEquals(String name)
        {
            this.SetSelected(IsEqualTo(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            if (selected)
            {
                this.Image.Source = new BitmapImage(new Uri("../../Resources/Images/Icons/Check.png", UriKind.Relative)); ;
            }
            else
            {
                this.Image.Source = null;
            }
        }

        public bool IsEqualTo(String name)
        {
            return name != null && this.Label.Content.Equals(name);
        }

        #endregion
        
    }
}
