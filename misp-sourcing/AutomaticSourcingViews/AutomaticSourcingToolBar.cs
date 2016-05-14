using Misp.Sourcing.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Misp.Sourcing.AutomaticSourcingViews
{
  public  class AutomaticSourcingToolBar: Misp.Kernel.Ui.Base.ToolBar
    {
      private System.Windows.Controls.Button runButton;
      
        /// <summary>
        /// 
        /// </summary>
      public AutomaticSourcingToolBar() { }

      public System.Windows.Controls.Button RunButton { get { return runButton; } }
      
      
      

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(RunButton);
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
            NewButton.ToolTip = "Add an Automatic Sourcing";
            SaveButton.ToolTip = "Save Automatic Sourcing";
            CloseButton.ToolTip = "Exit Automatic Sourcing";
            RunButton.ToolTip = "Run Automatic Sourcing";
           
            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            //auditButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Audit", "Audit allocation", "SalmonButtonStyle", new Thickness(30, 0, 40, 0));
            runButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Run", "Run Automatic Sourcing", "GreenButtonStyle", new Thickness(30, 0, 40, 0));
        }

        
    }
}
