using DevExpress.Xpf.Core;
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

        private Button automaticButton;

        private Button newButton;
        private Button openButton;
        private Button saveButton;
        private Button saveAllButton;
        private Button renameButton;
        private Button deleteButton;

        private Button importButton;
        private Button exportButton;

        private Button closeButton;

        /// <summary>
        /// Construit une nouvelle instance de la classe ToolBar.
        /// </summary>
        public ToolBar()
        {
            InitializeComponent();
            userConfiguration();            
        }


        public System.Windows.Controls.Button AutomaticButton { get { return automaticButton; } }

        public System.Windows.Controls.Button ImportButton { get { return importButton; } }
        public System.Windows.Controls.Button ExportButton { get { return exportButton; } }

        public System.Windows.Controls.Button NewButton { get { return newButton; } }
        public System.Windows.Controls.Button OpenButton { get { return openButton; } }
        public System.Windows.Controls.Button SaveButton { get { return saveButton; } }
        public System.Windows.Controls.Button SaveAllButton { get { return saveAllButton; } }
        public System.Windows.Controls.Button RenameButton { get { return renameButton; } }
        public System.Windows.Controls.Button DeleteButton { get { return deleteButton; } }
        public System.Windows.Controls.Button CloseButton { get { return closeButton; } }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetReadOnly(bool readOnly)
        {
            SaveButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            SaveAllButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            RenameButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            DeleteButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            ImportButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            ExportButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            AutomaticButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Cette methode permet de configurer la barre pour spécifier les boutons qui doivent être prrésents.
        /// </summary>
        protected virtual void userConfiguration()
        {
            foreach(Control control in getAllControls())
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
            controls.Add(automaticButton);
            
            controls.Add(newButton);
            controls.Add(importButton);
            controls.Add(exportButton);
            controls.Add(openButton);
            controls.Add(saveButton);
            controls.Add(saveAllButton);
            controls.Add(renameButton);
            controls.Add(deleteButton);
            controls.Add(closeButton);

            return controls;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void RemoveAllControls()
        {
            foreach (System.Windows.Controls.Control control in getAllControls())
            {
                this.Children.Remove(control);
            }
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


        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeComponent()
        {
            ThemeManager.SetThemeName(this, "None");

            automaticButton = ToolBar.BuildButton(null, "Auto", "Automatic", "WineButtonStyle", new Thickness(5,0,0,0));

            importButton = ToolBar.BuildButton("indent-down-white.png", "Auto", "Import", "SalmonButtonStyle", new Thickness(30, 0, 0, 0));
            exportButton = ToolBar.BuildButton("indent-up-white.png", "Auto", "Export", "SalmonButtonStyle", new Thickness(15, 0, 0, 0));
            
            newButton    = ToolBar.BuildButton("blank-file-white.png", "Auto", "New", "BlueButtonStyle", new Thickness(30,0,0,0));
            openButton   = ToolBar.BuildButton("folder-white.png", "Auto", "Open", "BlueButtonStyle", new Thickness(15,0,0,0));
            saveButton = ToolBar.BuildButton("save-white.png", "Auto", "save", "SalmonButtonStyle", new Thickness(15, 0, 0, 0));
            saveAllButton = ToolBar.BuildButton("save-as-white.png", "Auto", "Save All", "SalmonButtonStyle", new Thickness(15, 0, 0, 0));
            renameButton   = ToolBar.BuildButton("rename-white.png", "Auto", "Rename", "BlueButtonStyle", new Thickness(15,0,0,0));
            deleteButton   = ToolBar.BuildButton("delete-white.png", "Auto", "Delete", "BlueButtonStyle", new Thickness(15,0,0,0));
            
            closeButton = ToolBar.BuildButton("quit-outline-white.png", "Auto", "Close", "WineButtonStyle", new Thickness(30,0,0,0));

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
            if(image != null)
            {
                var im = new Image();
                im.Source = new BitmapImage(new Uri(@"..\..\Resources\Images\Icons\" + image, UriKind.Relative));
                button.Content = im;
            }else button.Content = text;            
            button.ToolTip = toolTip;
            if (margin != null) button.Margin = margin;
            
            if (styleKey != null) button.Style = System.Windows.Application.Current.FindResource(styleKey) as Style;
            return button;
        }


        public virtual void customize(List<Domain.Right> listeRights)
        {
            SaveButton.Visibility = Util.RightsUtil.HasRight(Domain.RightType.SAVE,listeRights) ? Visibility.Hidden : Visibility.Visible;
            SaveAllButton.Visibility = Util.RightsUtil.HasRight(Domain.RightType.SAVE, listeRights) ? Visibility.Hidden : Visibility.Visible;
            RenameButton.Visibility = Util.RightsUtil.HasRight(Domain.RightType.RENAME, listeRights) ? Visibility.Hidden : Visibility.Visible;
            DeleteButton.Visibility = Util.RightsUtil.HasRight(Domain.RightType.DELETE, listeRights) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
