using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Planification.PresentationView
{
    public class PresentationToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        private Button runButton;
        private Button clearButton;
        private CheckBox applyToAllCheckBox;
        private Button auditButton;

        /// <summary>
        /// 
        /// </summary>
        public PresentationToolBar()
        {
            
        }

     
        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
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
            NewButton.ToolTip = "Add a new Presentation";
            SaveButton.ToolTip = "Save Presentation";
            CloseButton.ToolTip = "Exit Presentation Editor";

            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }

    }
}
