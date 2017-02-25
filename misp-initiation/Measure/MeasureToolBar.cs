using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Initiation.Measure
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer les mesures.
    /// Cette barre contient des bouttons tels que: Indent, Move up, Rename, Save, Open, close...
    /// </summary>
    public class MeasureToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public MeasureToolBar()
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
            SaveButton.ToolTip = "Save Initiation";
            CloseButton.ToolTip = "Close Initiation View";
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);

            SaveButton.IsEnabled = false;
            CloseButton.IsEnabled = true;
        }

    }
}
