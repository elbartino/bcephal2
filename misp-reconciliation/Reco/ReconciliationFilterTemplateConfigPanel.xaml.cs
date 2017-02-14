﻿using Misp.Kernel.Domain;
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

        public event ChangeItemEventHandler ItemPresent;

        public ReconciliationFilterTemplateConfigPanel()
        {
            InitializeComponent();
            this.ConfigurationPropertiesPanel = new ConfigurationPropertiesPanel();
            InitializeHandlers();
        }

        public void InitializeHandlers() 
        {
            this.AllowWriteOffCheckBox.Checked += OnAllowWriteOff;
            this.AllowWriteOffCheckBox.Unchecked += OnAllowWriteOff;
            this.WriteOffConfigPanel.ItemPresent += OnVerifyIfPresent;
        }

        public void removeHandlers()
        {
            this.AllowWriteOffCheckBox.Checked -= OnAllowWriteOff;
            this.AllowWriteOffCheckBox.Unchecked -= OnAllowWriteOff;
            this.WriteOffConfigPanel.ItemPresent -= OnVerifyIfPresent;
        }


        private void OnAllowWriteOff(object sender, RoutedEventArgs e)
        {
            if (this.EditedObject == null) this.EditedObject = new ReconciliationFilterTemplate();
            this.EditedObject.acceptWriteOff = this.AllowWriteOffCheckBox.IsChecked.Value;
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
            this.ConfigurationPropertiesPanel.EditedObject = this.EditedObject;
            this.ConfigurationPropertiesPanel.displayObject();

            this.WriteOffConfigPanel.EditedObject = this.EditedObject.writeOffConfig;
            this.WriteOffConfigPanel.displayObject();

            InitializeHandlers();
        }



        public void updateObjetct(WriteOffConfiguration writeOffConfiguration)
        {
            this.WriteOffConfigPanel.updateObject(writeOffConfiguration);
        }
    }
}
