using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Kernel.Ui.File
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer des fichiers.
    /// Cette barre contient des bouttons tels que: Rename, Save, Open, close...
    /// </summary>
    public class FileToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public FileToolBar()
        {
            CollapseAllControls();
        }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            //controls.Add(NewButton);
            //controls.Add(OpenButton);
            //controls.Add(SaveAllButton);
            //controls.Add(RenameButton);
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
            AutomaticButton.ToolTip = "Automatic";
            NewButton.ToolTip = "Create a new file";
            OpenButton.ToolTip = "Open another file";
            SaveAllButton.ToolTip = "Save the file as...";
            RenameButton.ToolTip = "Rename the file";
            CloseButton.ToolTip = "Close the file";
        }

    }
}
