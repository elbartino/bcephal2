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
using System.Windows.Shapes;
using Misp.Kernel.Ui.Base;

namespace Misp.Kernel.Ui.EditableTree
{
    /// <summary>
    /// Interaction logic for AttributeDialog.xaml
    /// </summary>
    public partial class AttributeDialog : Window
    {

        public ChangeEventHandler Changed;
        public bool dirty;

        public Domain.Attribute attribute;
        
        public AttributeDialog()
        {
            this.dirty = false;
            InitializeComponent();
            this.CancelButton.Click += OnCancel;
            this.OKButton.Click += OnOk;
            this.CanModifyValuesCheckBox.Checked += OnCheck;
            this.CanModifyValuesCheckBox.Unchecked += OnCheck;
            this.IncrementalValuesCheckBox.Checked += OnCheck;
            this.IncrementalValuesCheckBox.Unchecked += OnCheck;
        }

        public void Display(Domain.Attribute attribute)
        {
            this.attribute = attribute;
            this.NameTextBox.Text = attribute.name;
            this.CanModifyValuesCheckBox.IsChecked = attribute.canUserModifyValues;
            this.IncrementalValuesCheckBox.IsChecked = attribute.incremental;
            this.dirty = false;
        }

        public void Fill()
        {
            this.attribute.canUserModifyValues = this.CanModifyValuesCheckBox.IsChecked.Value;
            this.attribute.incremental = this.IncrementalValuesCheckBox.IsChecked.Value;
        }

        public void Dispose()
        {
            this.CancelButton.Click -= OnCancel;
            this.OKButton.Click -= OnOk;
            this.CanModifyValuesCheckBox.Checked -= OnCheck;
            this.CanModifyValuesCheckBox.Unchecked -= OnCheck;
            this.IncrementalValuesCheckBox.Checked -= OnCheck;
            this.IncrementalValuesCheckBox.Unchecked -= OnCheck;
            this.Close();
        }

        private void OnCheck(object sender, RoutedEventArgs e)
        {
            this.dirty = true;
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Fill();
            if (this.dirty && Changed != null) Changed();
            Dispose();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

    }
}
