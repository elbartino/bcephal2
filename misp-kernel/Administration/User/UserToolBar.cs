using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.User
{
    public class UserToolBar : ToolBar
    {
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
            SaveButton.ToolTip = "Save User";
            CloseButton.ToolTip = "Exit User Editor";
            SaveButton.IsEnabled = false;
        }
    }
}
