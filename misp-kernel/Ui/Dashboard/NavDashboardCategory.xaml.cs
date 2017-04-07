using DevExpress.Xpf.Navigation;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for NavDashboardCategory.xaml
    /// </summary>
    public partial class NavDashboardCategory : TileNavCategory
    {        
        
        #region Properties

        public ChangeItemEventHandler Selection { get; set; }
        public String FunctionalityCode { get; set; }

        public NavDashboardBlock Block { get; set; }

        #endregion


        #region Constructors

        public NavDashboardCategory()
        {
            InitializeComponent();
        }

        public NavDashboardCategory(String title, String functionalityCode = null)
            : this()
        {            
            this.Content = title;
            this.FunctionalityCode = functionalityCode;
        }

        #endregion


        #region Operations

        

        #endregion


        #region Handlers

        private void OnClick(object sender, EventArgs e)
        {
            if (Selection != null) Selection(this);
        }

        #endregion             
        
    }
}
