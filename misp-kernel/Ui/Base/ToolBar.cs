using DevExpress.Xpf.Core;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Misp.Kernel.Ui.Base
{
    public class ToolBar : WrapPanel
    {

        #region Attributes

        private Button newButton;
        private Button saveButton;
        private Button closeButton;

        #endregion


        #region Properties

        public System.Windows.Controls.Button NewButton { get { return newButton; } }
        public System.Windows.Controls.Button SaveButton { get { return saveButton; } }
        public System.Windows.Controls.Button CloseButton { get { return closeButton; } }

        #endregion


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de la classe ToolBar.
        /// </summary>
        public ToolBar()
        {
            InitializeComponent();
            userConfiguration();            
        }

        #endregion


        #region Operations
        
        /// <summary>
        /// Customize toolbar for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public virtual void Customize(List<Domain.Right> rights, bool readOnly = false)
        {
            bool edit = RightsUtil.HasRight(Domain.RightType.EDIT, rights);
            SaveButton.Visibility = edit && !readOnly ? Visibility.Visible : Visibility.Collapsed;
            //NewButton.Visibility = edit && !readOnly ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetReadOnly(bool readOnly)
        {
            SaveButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void CollapseAllControls()
        {
            foreach (System.Windows.Controls.Control control in getAllControls())
            {
                control.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public virtual void DisplayAllControls()
        {
            foreach (System.Windows.Controls.Control control in getAllControls())
            {
                control.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion


        #region Initiation

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeComponent()
        {
            ThemeManager.SetThemeName(this, "None");
            newButton = ToolBar.BuildButton("blank-file-white.png", "Auto", "New", "BlueButtonStyle", new Thickness(30, 0, 0, 0));
            saveButton = ToolBar.BuildButton("save-white.png", "Auto", "save", "SalmonButtonStyle", new Thickness(15, 0, 0, 0));            
            closeButton = ToolBar.BuildButton("quit-outline-white.png", "Auto", "Close", "WineButtonStyle", new Thickness(30, 0, 0, 0));
        }

        /// <summary>
        /// Cette methode permet de configurer la barre pour spécifier les boutons qui doivent être prrésents.
        /// </summary>
        protected virtual void userConfiguration()
        {
            foreach (Control control in getAllControls())
            {
                this.Children.Add(control);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(newButton);
            controls.Add(saveButton);
            controls.Add(closeButton);
            return controls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="toolTip"></param>
        /// <param name="styleKey"></param>
        /// <param name="margin"></param>
        public static Button BuildButton(string image, string text, string toolTip, string styleKey, Thickness margin)
        {
            Button button = new Button();
            if (image != null)
            {
                var im = new Image();
                im.Source = new BitmapImage(new Uri(@"..\..\Resources\Images\Icons\" + image, UriKind.Relative));
                button.Content = im;
            }
            else button.Content = text;
            button.ToolTip = toolTip;
            if (margin != null) button.Margin = margin;

            if (styleKey != null) button.Style = System.Windows.Application.Current.FindResource(styleKey) as Style;
            return button;
        }

        #endregion

        

        

        


        
        


        


        
    }
}
