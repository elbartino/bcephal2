using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows;

namespace Misp.Reporting.StructuredReport
{
    public class StructuredReportToolBar : Misp.Kernel.Ui.Base.ToolBar
    {

        private Button runButton;

        public System.Windows.Controls.Button RunButton { get { return runButton; } }
        
        /// <summary>
        /// 
        /// </summary>
        public StructuredReportToolBar() { }
        
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
            NewButton.ToolTip = "Add a Structured Report";
            SaveButton.ToolTip = "Save Structured Report";
            CloseButton.ToolTip = "Exit Structured Report";

            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            runButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Run", "Run Structured Report", "GreenButtonStyle", new Thickness(5, 0, 0, 0));
            
        }

    }
}
