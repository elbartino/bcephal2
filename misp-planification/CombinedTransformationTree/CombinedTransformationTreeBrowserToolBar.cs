using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeBrowserToolBar : BrowserToolBar
    {
        private Button runButton;
        private Button clearButton;

        public Button RunButton { get { return runButton; } }
        public Button ClearButton { get { return clearButton; } }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<Control>();
           // controls.Add(RunButton);
           // controls.Add(ClearButton);
            controls.AddRange(base.getAllControls());
            return controls;
        }

        /// <summary>
        /// Cette methode permet de configurer la barre pour spécifier les boutons qui doivent être prrésents.
        /// Pour la gestion d'un fichier, nous avons besoin de:
        /// 
        /// </summary>
        protected override void userConfiguration()
        {
            runButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Run", "Run Tree", "GreenButtonStyle", new Thickness(5, 0, 0, 0));
            clearButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Clear", "Clear Tree", "WineButtonStyle", new Thickness(30, 0, 0, 0));

            base.userConfiguration();

         
        }
    }
}
