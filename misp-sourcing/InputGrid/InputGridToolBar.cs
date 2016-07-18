using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
                
        /// <summary>
        /// 
        /// </summary>
        public InputGridToolBar() { }
        
        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            //controls.Add(ExportButton);
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
            ExportButton.ToolTip = "Export to Excel";
            SaveButton.ToolTip = "Save Grid";
            CloseButton.ToolTip = "Exit Grid";

            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }

    }
}
