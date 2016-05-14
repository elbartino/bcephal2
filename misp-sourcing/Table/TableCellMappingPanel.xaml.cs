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

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for TableCellMappingPanel.xaml
    /// </summary>
    public partial class TableCellMappingPanel : Grid
    {
        public TableCellMappingPanel()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            //controls.Add(this.nameTextBox);
            //controls.Add(this.groupTextBox);
            //controls.Add(this.activeCheckBox);
            //controls.Add(this.templateCheckBox);
            //controls.Add(this.filterScopePanel);
            //controls.Add(this.periodPanel);
            //controls.Add(this.tagPanel);
            return controls;
        }

    }
}
