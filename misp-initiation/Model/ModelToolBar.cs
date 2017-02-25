using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Initiation.Model
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer les models.
    /// Cette barre contient des bouttons tels que: Rename, Save, Open, close...
    /// </summary>
    public class ModelToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public ModelToolBar()
        {
            
        }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            
            controls.Add(NewButton);
            controls.Add(SaveButton);
            //controls.Add(SaveAllButton);
            //controls.Add(RenameButton);
            //controls.Add(DeleteButton);
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
            NewButton.ToolTip = "Add a new Model";
            SaveButton.ToolTip = "Save Initiation";
            CloseButton.ToolTip = "Close Initiation View";

            SaveButton.IsEnabled = false;

            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }

    }
}
