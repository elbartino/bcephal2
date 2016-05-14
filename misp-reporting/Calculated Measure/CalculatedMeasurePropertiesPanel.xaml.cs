using Misp.Kernel.Ui.Base;
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

namespace Misp.Reporting.Calculated_Measure
{
    /// <summary>
    /// Interaction logic for CalculatedMeasurePropertiesPanel.xaml
    /// </summary>
    public partial class CalculatedMeasurePropertiesPanel : ScrollViewer
    {
        /// <summary>
        /// construit une nouvelle instance de calculatedMeasurePropertiesPanel
        /// </summary>
        public CalculatedMeasurePropertiesPanel()
        {
            InitializeComponent();
            InitializeHandler();
            this.IgnorePropertiesGrid.setVisible(true);
        }

      
        // handler qui ecoute les modifications dans le ignorepropertiesgrid
        public event ChangeEventHandler IgnorePropertiesGridChanged;
        private Kernel.Domain.CalculatedMeasure calculatedMeasure;
        /// <summary>
        /// initialise le(s) handler(s)
        /// </summary>
        public void InitializeHandler()
        {
            this.IgnorePropertiesGrid.Changed += IgnorePropertiesGrid_Changed;
        }

        void IgnorePropertiesGrid_Changed()
        {
            if (IgnorePropertiesGridChanged != null) IgnorePropertiesGridChanged();
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
        /// affiche les valeurs dans le panel properties 
        /// </summary>
        /// <param name="calculatedMeasure"></param>
        public void displayCalculatedMeasureProperties(Kernel.Domain.CalculatedMeasure calculatedMeasure)
        {
            if (calculatedMeasure == null) return;
            if (calculatedMeasure.calculatedMeasureItemListChangeHandler == null) return;
            nameTextBox.Text = calculatedMeasure.name;
            groupField.Group = calculatedMeasure.group;
            visibleInShortcutCheckBox.IsChecked = calculatedMeasure.visibleInShortcut;

            if (calculatedMeasure.calculatedMeasureItemListChangeHandler != null && calculatedMeasure.calculatedMeasureItemListChangeHandler.Items.Count > 0)
            {
                Misp.Kernel.Domain.CalculatedMeasureItem item = calculatedMeasure.calculatedMeasureItemListChangeHandler.Items.Last();
                if (item.sign != null && item.sign.Equals("="))
                {
                    item = calculatedMeasure.GetItemByPosition(calculatedMeasure.calculatedMeasureItemListChangeHandler.Items.Count - 2);
                }
                //displayCalculatedMeasureItemIgnoreProperties(item);
            }
        }
        public void displayCalculatedMeasureItemIgnoreProperties(Kernel.Domain.CalculatedMeasureItem item)
        {
            IgnorePropertiesGrid.isDisplayChecked = false;
            IgnorePropertiesGrid.ignoreAll.IsChecked = item.ignoreAll;
            IgnorePropertiesGrid.ignoreCellObject.IsChecked = item.ignoreCellObject;
            IgnorePropertiesGrid.ignoreCellPeriod.IsChecked = item.ignoreCellPeriod;
            IgnorePropertiesGrid.ignoreCellVC.IsChecked = item.ignoreCellVc;
            IgnorePropertiesGrid.ignoreTableObject.IsChecked = item.ignoreTableObject;
            IgnorePropertiesGrid.ignoreTablePeriod.IsChecked = item.ignoreTablePeriod;
            IgnorePropertiesGrid.ignoreTableVC.IsChecked = item.ignoreTableVc;
            IgnorePropertiesGrid.isDisplayChecked = true;
        }
        /// <summary>
        /// recupere les valeurs edités sur le nom et group , et affecte au calculatedMeasure
        /// </summary>
        /// <param name="calculatedMeasure"></param>
        public void fillCalculatedMeasure(Kernel.Domain.CalculatedMeasure calculatedMeasure)
        {
            if (calculatedMeasure == null) return;
            calculatedMeasure.name = nameTextBox.Text;
            groupField.Group.subjectType = Kernel.Domain.SubjectType.CALCULATED_MEASURE.label;
            calculatedMeasure.group = groupField.Group;
            calculatedMeasure.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;

        }

        public void fillCalculatedMeasureItemIgnoreProperties(Kernel.Domain.CalculatedMeasureItem calculatedMeasureItem)
        {
            if (calculatedMeasureItem == null) return;
            if (calculatedMeasureItem.measure != null)
            {
                calculatedMeasureItem.ignoreAll = (bool)IgnorePropertiesGrid.ignoreAll.IsChecked;
                calculatedMeasureItem.ignoreCellObject = (bool)IgnorePropertiesGrid.ignoreCellObject.IsChecked;
                calculatedMeasureItem.ignoreTableObject = (bool)IgnorePropertiesGrid.ignoreTableObject.IsChecked;
                calculatedMeasureItem.ignoreCellVc = (bool)IgnorePropertiesGrid.ignoreCellVC.IsChecked;
                calculatedMeasureItem.ignoreTableVc = (bool)IgnorePropertiesGrid.ignoreTableVC.IsChecked;
                calculatedMeasureItem.ignoreTablePeriod = (bool)IgnorePropertiesGrid.ignoreTablePeriod.IsChecked;
                calculatedMeasureItem.ignoreCellPeriod = (bool)IgnorePropertiesGrid.ignoreCellPeriod.IsChecked;
            }
        }

        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            controls.Add(this.nameTextBox);
            controls.Add(this.groupField);
            controls.Add(this.visibleInShortcutCheckBox);
            return controls;
        }

        public void setIgnorePropertiesGridVisibility(bool visible)
        {
            this.IgnorePropertiesGrid.setVisible(visible);
        }
    }
}
