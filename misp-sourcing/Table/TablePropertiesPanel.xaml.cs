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
using Misp.Kernel.Domain;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Interaction logic for TablePropertiesPanel.xaml
    /// </summary>
    public partial class TablePropertiesPanel : ScrollViewer
    {

        public RPeriodPanel reportPeriodPanel;
        public bool isReport;
        public bool thowEvent = false;

        public TablePropertiesPanel()
        {
            InitializeComponent();
            isReport = true;
            Expand(true);
            thowEvent = true;
        }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void displayTable(InputTable table, bool isNoAllocation = false,bool readOnly=false)
        {
            if (table == null) return;
            thowEvent = false;
            nameTextBox.Text = table.name;
            groupField.Group = table.group;
            activeCheckBox.IsChecked = table.active;
            templateCheckBox.IsChecked = table.template;
            visibleInShortcutCheckBox.IsChecked = table.visibleInShortcut;

            if (reportPeriodPanel != null) this.reportPeriodPanel.DisplayPeriod(table.period, true,readOnly);
            else periodPanel.DisplayPeriod(table.period, true,readOnly);
            filterScopePanel.DisplayScope(table.correctFilter(),isNoAllocation,readOnly);
            thowEvent = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public void fillTable(InputTable table)
        {
            if (table == null) return;
            table.name = nameTextBox.Text;
            if (groupField.Group == null) groupField.Group = new BGroup();
            groupField.Group.subjectType = Kernel.Domain.SubjectType.INPUT_TABLE.label;
            table.group = groupField.Group;
            table.active = activeCheckBox.IsChecked.Value;
            table.template = templateCheckBox.IsChecked.Value;
            table.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
            table.period = periodPanel.Period;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            //controls.Add(this.nameTextBox);
           // controls.Add(this.activeCheckBox);
            //controls.Add(this.templateCheckBox);
            //controls.Add(this.visibleInShortcutCheckBox);
            //controls.Add(this.groupField);
            //controls.Add(this.filterScopePanel);
            //controls.Add(this.periodPanel);
            return controls;
        }

        public void CustomizeForReport()
        {
            if (reportPeriodPanel == null) reportPeriodPanel = new RPeriodPanel();
            periodGroupBox.Content = reportPeriodPanel;
        }
        
        public void Expand(bool expand)
        {
            filterScopePanel.Expand(expand);
        }
                
        public void SetReadOnly(bool readOnly)
        {
            if (activeCheckBox != null) activeCheckBox.IsEnabled = !readOnly;
            if (templateCheckBox != null) templateCheckBox.IsEnabled = !readOnly;
            if (visibleInShortcutCheckBox != null) visibleInShortcutCheckBox.IsEnabled = !readOnly;
            this.ResetAllCellsButton.Visibility = readOnly ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            if (nameTextBox != null) nameTextBox.IsEnabled = !readOnly;
            if (reportPeriodPanel != null) reportPeriodPanel.SetReadOnly(readOnly);
            if (periodPanel != null) periodPanel.SetReadOnly(readOnly);
            if (filterScopePanel != null) filterScopePanel.SetReadOnly(readOnly);
            if (groupField != null) groupField.SetReadOnly(readOnly);
        }
    }
}
