using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Sourcing.InputGrid
{
    /// <summary>
    /// Interaction logic for InputGridRelationshipPanel.xaml
    /// </summary>
    public partial class InputGridRelationshipPanel : Grid
    {

        #region Properties

        public bool throwEvent = true;

        public Grille Grid { get; set; }

        #endregion


        #region Constructors

        public InputGridRelationshipPanel()
        {
            InitializeComponent();
            this.PrimaryRelationPanel.RelationshipPanel = this.RelationshipPanel;
        }

        #endregion


        #region Operations

        public void Display(Grille grid)
        {
            if (grid == null) return;
            throwEvent = false;
            this.Grid = grid;
            this.PrimaryRelationPanel.Display(this.Grid);    
            throwEvent = true;
        }

        public void FillGrid(Grille grid)
        {
            if (grid == null) return;
            
        }

        #endregion

    }
}
