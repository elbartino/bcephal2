using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace Misp.Planification.CombinedTransformationTree
{
    public class CombineTransformationTreeToolBar : Misp.Kernel.Ui.Base.ToolBar
    {

        private Button runButton;
        private Button clearButton;

        public Button RunButton { get { return runButton; } }
        public Button ClearButton { get { return clearButton; } }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(RunButton);
            controls.Add(ClearButton);
            controls.Add(new Separator());
            controls.Add(SaveButton);
            controls.Add(CloseButton);
            return controls;
        }

        public override void Customize(String fuctionality, PrivilegeObserver observer, List<Kernel.Domain.Right> rights, bool readOnly = false)
        {
            base.Customize(fuctionality, observer, rights, readOnly);
            RunButton.Visibility = RightsUtil.HasRight(RightType.LOAD, rights) && !readOnly ? Visibility.Visible : Visibility.Collapsed;
            ClearButton.Visibility = RightsUtil.HasRight(RightType.CLEAR, rights) && !readOnly ? Visibility.Visible : Visibility.Collapsed;
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
            SaveButton.ToolTip = "Save Combined Transformation tree";
            CloseButton.ToolTip = "Exit Combined Transformation tree Editor";
            SaveButton.IsEnabled = false;
        }

    }
}
