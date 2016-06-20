using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Administration.Role
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer la grille des roles.
    /// Cette barre contient des bouttons tels que: Indent, Move up, Delete...
    /// </summary>
    public class RoleGridToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public RoleGridToolBar()
        {
            
        }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(DeleteButton);
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
            DeleteButton.ToolTip = "Delete selected measures";
            DeleteButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);

            DeleteButton.IsEnabled = false;
        }

    }
}
