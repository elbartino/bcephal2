using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        
        /// <summary>
        /// 
        /// </summary>
        public ReconciliationFilterTemplateToolBar() { }
        
        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            if (ApplicationManager.Instance.User != null && ApplicationManager.Instance.User.IsAdmin())
            {
                controls.Add(SaveButton);
            }
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
            
            SaveButton.ToolTip = "Save Filter";
            CloseButton.ToolTip = "Exit Filter";

            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
        }


    }
}