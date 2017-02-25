using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridToolBar : Misp.Kernel.Ui.Base.ToolBar
    {

        #region Properties

        public Button LoadButton { get; protected set; }
        public Button ClearButton { get; protected set; }

        #endregion


        #region Constructors

        public InputGridToolBar() { }

        #endregion


        #region Operations

        public override void Customize(List<Kernel.Domain.Right> rights, bool readOnly = false)
        {
            base.Customize(rights);
            LoadButton.Visibility = RightsUtil.HasRight(RightType.LOAD, rights) && !readOnly ? Visibility.Visible : Visibility.Collapsed;
            ClearButton.Visibility = RightsUtil.HasRight(RightType.CLEAR, rights) && !readOnly ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetLoaded(bool loaded)
        {
            LoadButton.IsEnabled = !loaded;
            ClearButton.IsEnabled = loaded;
        }

        public void SetNew()
        {
            LoadButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
        }

        #endregion


        #region Initiation

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(LoadButton);
            controls.Add(ClearButton);
            controls.Add(SaveButton);
            controls.Add(CloseButton);
            return controls;
        }
        
        /// <summary>
        /// Cette methode permet de configurer la barre pour spécifier les boutons qui doivent être prrésents.
        /// Pour la gestion d'un fichier, nous avons besoin de:
        /// 
        /// </summary>
        protected override void userConfiguration()
        {
            base.userConfiguration();
            
            SaveButton.ToolTip = "Save Grid";
            CloseButton.ToolTip = "Exit Grid";

            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
            LoadButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            LoadButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Load", "Load Grid", "GreenButtonStyle", new Thickness(5, 0, 0, 0));
            ClearButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Clear", "Clear Grid", "WineButtonStyle", new Thickness(30, 0, 0, 0));            
        }

        #endregion

    }
}
