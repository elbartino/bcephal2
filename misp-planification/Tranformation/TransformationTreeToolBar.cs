using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Planification.Tranformation
{
    public  class TransformationTreeToolBar: Misp.Kernel.Ui.Base.ToolBar
    {
        private Button runButton;
        private Button clearButton;
      
      
        /// <summary>
        /// 
        /// </summary>
        public TransformationTreeToolBar() { }

        public Button RunButton { get { return runButton; } }
        public Button ClearButton { get { return clearButton; } }
        public Button LoadButton { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public override void SetReadOnly(bool readOnly)
        {
            base.SetReadOnly(readOnly);
            RunButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
            ClearButton.Visibility = readOnly ? Visibility.Hidden : Visibility.Visible;
        }

        public override void Customize(String fuctionality, PrivilegeObserver observer, List<Kernel.Domain.Right> rights, bool readOnly = false)
        {
            base.Customize(fuctionality, observer, rights, readOnly);
            RunButton.Visibility = RightsUtil.HasRight(RightType.LOAD, rights) && !readOnly ? Visibility.Visible : Visibility.Collapsed;
            ClearButton.Visibility = RightsUtil.HasRight(RightType.CLEAR, rights) && !readOnly ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(RunButton);
            controls.Add(ClearButton);
            controls.Add(new Separator());
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
            runButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Run", "Run Tree", "GreenButtonStyle", new Thickness(5, 0, 0, 0));
            clearButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Clear", "Clear Tree", "WineButtonStyle", new Thickness(30, 0, 0, 0));
           
            base.userConfiguration();
            NewButton.ToolTip = "Add a Transformation Tree";
            SaveButton.ToolTip = "Save Transformation Tree";
            CloseButton.ToolTip = "Exit Transformation Tree";
            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
        }
    }
}
