using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;

namespace Misp.Initiation.Measure
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer la grille des mesures.
    /// Cette barre contient des bouttons tels que: Indent, Move up, Delete...
    /// </summary>
    public class MeasureGridToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        /// <summary>
        /// 
        /// </summary>
        public MeasureGridToolBar()
        {
            
        }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            return controls;
        }
        

    }
}
