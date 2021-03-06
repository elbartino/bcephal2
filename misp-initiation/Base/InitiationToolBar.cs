﻿using System;
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
            NewButton.Visibility = System.Windows.Visibility.Visible;
            CloseButton.Visibility = System.Windows.Visibility.Visible;
        }

        public void CollapseButtons()
        {
            NewButton.Visibility = System.Windows.Visibility.Collapsed;
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
            
            NewButton.Visibility = System.Windows.Visibility.Visible;
            CloseButton.Visibility = System.Windows.Visibility.Visible;

            NewButton.ToolTip = "Create a new file";
            CloseButton.ToolTip = "Close the file";
        }

    }
}
