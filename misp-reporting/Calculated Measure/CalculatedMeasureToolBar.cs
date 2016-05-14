using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Reporting.Calculated_Measure
{
    public class CalculatedMeasureToolBar : Misp.Kernel.Ui.Base.ToolBar
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
        /// Pour la création d'une mesure calculé, nous avons besoin de:
        /// 
        /// </summary>
        protected override void userConfiguration()
        {
            base.userConfiguration();
            SaveButton.ToolTip = "Save Operations for calculated Measure";
            CloseButton.ToolTip = "Exit Calculated Measure Editor";
            SaveButton.IsEnabled = false;
        }
    }
}
