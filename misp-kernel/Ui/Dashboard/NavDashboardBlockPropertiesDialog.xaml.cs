using DevExpress.Xpf.Core;
using Misp.Kernel.Util;
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
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for NavDashboardBlockPropertiesDialog.xaml
    /// </summary>
    public partial class NavDashboardBlockPropertiesDialog : DXWindow
    {
        
        #region Properties

        public NavBlock Block {get; protected set;}
        
        #endregion


        #region Constructors

        public NavDashboardBlockPropertiesDialog()
        {
            ThemeManager.SetThemeName(this, "Office2016White");
            this.Owner = Application.ApplicationManager.Instance.MainWindow;
            InitializeComponent();
        }

        #endregion


        #region Operations

        public void EditBlock(NavBlock block)
        {
            this.Block = block;
            this.Title = ((TextBlock)block.Content).Text + " - Properties";
            this.ShapePopupColorEdit.Color = block.BackgroundColor;
            this.TextPopupColorEdit.Color = block.ForegroundColor;
            WindowPositioner.ShowCenteredToMouse(this);
            //this.ShowDialog();
        }

        public void Dispode()
        {
            OkButton.Click -= OnOk;
            CancelButton.Click -= OnCancel;
            Close();
        }

        #endregion


        #region Handlers

        private void OnOk(object sender, RoutedEventArgs e)
        {
            this.Block.BackgroundColor = this.ShapePopupColorEdit.Color;
            this.Block.ForegroundColor = this.TextPopupColorEdit.Color;
            Dispode();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Dispode();
        }

        #endregion

        
    }
}
