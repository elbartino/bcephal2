﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.ReconciliationContext
{
    public class ReconciliationContextToolBar : Misp.Kernel.Ui.Base.ToolBar
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
            SaveButton.ToolTip = "Save Reconciliation Context";
            CloseButton.ToolTip = "Exit Reconciliation Context Editor";
            SaveButton.IsEnabled = false;
        }
    }
}
