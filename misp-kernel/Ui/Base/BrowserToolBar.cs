﻿using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer un browser.
    /// Cette barre les bouttons : new, open, delete...
    /// </summary>
    public class BrowserToolBar : ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public BrowserToolBar() { }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(NewButton);
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
            SaveButton.Visibility = System.Windows.Visibility.Collapsed;
            CloseButton.IsEnabled = true;
        }

        /// <summary>
        /// Customize toolbar for connected user
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="readOnly"></param>
        public override void Customize(String fuctionality, PrivilegeObserver observer, List<Domain.Right> rights, bool readOnly = false)
        {
            base.Customize(fuctionality, observer, rights, readOnly);
            SaveButton.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
