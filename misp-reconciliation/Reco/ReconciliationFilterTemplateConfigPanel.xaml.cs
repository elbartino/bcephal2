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

namespace Misp.Reconciliation.Reco
{
    /// <summary>
    /// Interaction logic for ReconciliationFilterTemplateConfigPanel.xaml
    /// </summary>
    public partial class ReconciliationFilterTemplateConfigPanel : Grid
    {
        public ConfigurationPropertiesPanel ConfigurationPropertiesPanel { get; set; }

        /// <summary>
        /// Design en édition
        /// </summary>
        public ReconciliationFilterTemplate EditedObject { get; set; }

        public ReconciliationFilterTemplateConfigPanel()
        {
            InitializeComponent();
            this.ConfigurationPropertiesPanel = new ConfigurationPropertiesPanel();
        }

        public void displayObject()
        {
            this.ConfigurationPropertiesPanel.EditedObject = this.EditedObject;
            this.ConfigurationPropertiesPanel.displayObject();

            this.WriteOffConfigPanel.EditedObject = this.EditedObject.writeOffConfig;
            this.WriteOffConfigPanel.displayObject();
        }

        
    }
}
