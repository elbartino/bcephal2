using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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


        public event ChangeItemEventHandler ItemChanged;


        public ReconciliationFilterTemplateConfigPanel()
        {
            InitializeComponent();
            UserInit();
            this.ConfigurationPropertiesPanel = new ConfigurationPropertiesPanel();
            InitializeHandlers();
        }

        private void UserInit()
        {
            this.TypeCombobox.SelectionChanged += OnTypeSelected;
            this.TypeCombobox.ItemsSource = new WriteOffFieldValueType[]  {
                      WriteOffFieldValueType.LEFT_SIDE, WriteOffFieldValueType.RIGHT_SIDE, WriteOffFieldValueType.CUSTOM };
            this.TypeCombobox.SelectedItem = WriteOffFieldValueType.LEFT_SIDE;            
            MeasureService service = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetMeasureService();
            this.MeasureCombobox.ItemsSource = service.getAllLeafts();
        }

        public void InitializeHandlers() 
        {
            this.AllowWriteOffCheckBox.Checked += OnAllowWriteOff;
            this.AllowWriteOffCheckBox.Unchecked += OnAllowWriteOff;
            this.WriteOffConfigPanel.ItemPresent += OnVerifyIfPresent;
            this.TypeCombobox.SelectionChanged += OnTypeComboboxSelectionChanged;
            this.MeasureCombobox.SelectionChanged += OnMeasureComboboxSelectionChanged;
        }
        
        public void removeHandlers()
        {
            this.AllowWriteOffCheckBox.Checked -= OnAllowWriteOff;
            this.AllowWriteOffCheckBox.Unchecked -= OnAllowWriteOff;
            this.WriteOffConfigPanel.ItemPresent -= OnVerifyIfPresent;
            this.TypeCombobox.SelectionChanged -= OnTypeComboboxSelectionChanged;
            this.MeasureCombobox.SelectionChanged -= OnMeasureComboboxSelectionChanged;
        }

        private void OnTypeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (this.TypeCombobox.SelectedItem != null && this.TypeCombobox.SelectedItem is WriteOffFieldValueType)
            {
                WriteOffFieldValueType type = (WriteOffFieldValueType)this.TypeCombobox.SelectedItem;
                this.MeasureCombobox.Visibility = type == WriteOffFieldValueType.CUSTOM ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnMeasureComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillObject();
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void OnTypeComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillObject();
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void OnAllowWriteOff(object sender, RoutedEventArgs e)
        {
            fillObject();
            if (ItemChanged != null) ItemChanged(this.EditedObject);
        }

        private void OnVerifyIfPresent(object item)
        {
            if (item is Array) 
            {
                object[] tab = (object[])item;
                string objectType = ((SubjectType)tab[0]).label;
                string objectName = (string)tab[1];
                Kernel.Util.MessageDisplayer.DisplayInfo("Write Off Configuration", objectType+ " named " + objectName + " is already present");                
            }
        }

        public void displayObject()
        {
            removeHandlers();
            this.AllowWriteOffCheckBox.IsChecked = this.EditedObject != null ? this.EditedObject.acceptWriteOff : false;
            if (this.EditedObject.writeoffDefaultMeasureTypeEnum != null) this.TypeCombobox.SelectedItem = this.EditedObject.writeoffDefaultMeasureTypeEnum;
            else
            {
                this.EditedObject.writeoffDefaultMeasureTypeEnum = WriteOffFieldValueType.LEFT_SIDE;
                this.TypeCombobox.SelectedItem = WriteOffFieldValueType.LEFT_SIDE;
            }
            this.MeasureCombobox.SelectedItem = this.EditedObject.writeoffMeasure;
            
            this.ConfigurationPropertiesPanel.EditedObject = this.EditedObject;
            this.ConfigurationPropertiesPanel.displayObject();

            this.WriteOffConfigPanel.EditedObject = this.EditedObject.writeOffConfig;
            this.WriteOffConfigPanel.displayObject();

            InitializeHandlers();
        }

        public void fillObject()
        {
            if (this.EditedObject == null) this.EditedObject = new ReconciliationFilterTemplate();
            this.EditedObject.acceptWriteOff = this.AllowWriteOffCheckBox.IsChecked.Value;
            WriteOffFieldValueType type = (WriteOffFieldValueType)this.TypeCombobox.SelectedItem;
            this.EditedObject.writeoffDefaultMeasureTypeEnum = type;
            if (type == WriteOffFieldValueType.CUSTOM) this.EditedObject.writeoffMeasure = (Measure)this.MeasureCombobox.SelectedItem;
            else this.EditedObject.writeoffMeasure = null;
        }


        public void updateObjetct(WriteOffConfiguration writeOffConfiguration)
        {
            this.WriteOffConfigPanel.updateObject(writeOffConfiguration);
        }
    }
}
