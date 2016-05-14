using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Initiation.Base
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer des fichiers.
    /// Cette barre contient des bouttons tels que: Rename, Save, Open, close...
    /// </summary>
    public class InitiationToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public InitiationToolBar()
        {
            CollapseAllControls();
        }

        public void DisplayButtons()
        {
            AutomaticButton.Visibility = System.Windows.Visibility.Visible;
            NewButton.Visibility = System.Windows.Visibility.Visible;
            OpenButton.Visibility = System.Windows.Visibility.Visible;
            SaveAllButton.Visibility = System.Windows.Visibility.Visible;
            RenameButton.Visibility = System.Windows.Visibility.Visible;
            CloseButton.Visibility = System.Windows.Visibility.Visible;
        }

        public void CollapseButtons()
        {
            AutomaticButton.Visibility = System.Windows.Visibility.Collapsed;
            NewButton.Visibility = System.Windows.Visibility.Collapsed;
            OpenButton.Visibility = System.Windows.Visibility.Collapsed;
            SaveAllButton.Visibility = System.Windows.Visibility.Collapsed;
            RenameButton.Visibility = System.Windows.Visibility.Collapsed;
            CloseButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Cette methode permet de configurer la barre pour spécifier les boutons qui doivent être prrésents.
        /// Pour la gestion d'un fichier, nous avons besoin de:
        /// 
        /// </summary>
        protected override void userConfiguration()
        {            
            CollapseAllControls();           
            
            AutomaticButton.Visibility = System.Windows.Visibility.Visible;
            NewButton.Visibility = System.Windows.Visibility.Visible;
            OpenButton.Visibility = System.Windows.Visibility.Visible;
            SaveAllButton.Visibility = System.Windows.Visibility.Visible;
            RenameButton.Visibility = System.Windows.Visibility.Visible;
            CloseButton.Visibility = System.Windows.Visibility.Visible;

            //NewButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
            AutomaticButton.ToolTip = "Automatic";
            NewButton.ToolTip = "Create a new file";
            OpenButton.ToolTip = "Open another file";
            SaveAllButton.ToolTip = "Save the file as...";
            RenameButton.ToolTip = "Rename the file";
            CloseButton.ToolTip = "Close the file";
        }

    }
}
