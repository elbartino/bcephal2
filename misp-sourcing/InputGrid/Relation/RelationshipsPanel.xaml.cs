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

namespace Misp.Sourcing.InputGrid.Relation
{
    /// <summary>
    /// Interaction logic for RelationshipsPanel.xaml
    /// </summary>
    public partial class RelationshipsPanel : Grid
    {
        
        #region Properties

        public bool throwEvent = true;

        public GrilleRelationships Relationships { get; set; }

        #endregion


        #region Constructors

        public RelationshipsPanel()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        public void Display(GrilleRelationships Relationships)
        {
            if (Relationships == null) return;
            throwEvent = false;
            this.Relationships = Relationships;            
            //this.PrimaryRelationshipPanel.Display(this.Grid != null ? this.Grid.relationships : null);
            //this.RelationshipPanel.Display(this.Grid != null ? this.Grid.relationships : null);            
            throwEvent = true;
        }

        public void FillGrid(Grille grid)
        {
            if (grid == null) return;
            
        }

        #endregion

    }
}
