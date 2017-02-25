using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Kernel.Administration.Role
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer les mesures.
    /// Cette barre contient des bouttons tels que: Indent, Move up, Rename, Save, Open, close...
    /// </summary>
    public class RoleToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public RoleToolBar()
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
            SaveButton.ToolTip = "Save Role";
            CloseButton.ToolTip = "Close Role View";
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);

            SaveButton.Visibility = System.Windows.Visibility.Visible;
            SaveButton.IsEnabled = false;
            CloseButton.IsEnabled = true;
        }

    }
}
