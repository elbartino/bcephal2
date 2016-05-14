﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using System.Windows;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// La barre d'outils utilisée pour gérer les models.
    /// Cette barre contient des bouttons tels que: Rename, Save, Open, close...
    /// </summary>
    public class InputTableToolBar : Misp.Kernel.Ui.Base.ToolBar
    {
        private Button saveClearRunButton;
        private Button runButton;
        private Button clearButton;
        private Button saveAsButton;
        private CheckBox applyToAllCheckBox;
        private Button auditButton;

        /// <summary>
        /// 
        /// </summary>
        public InputTableToolBar()
        {
            
        }

        public System.Windows.Controls.Button SaveClearRunButton { get { return saveClearRunButton; } }
        public System.Windows.Controls.Button RunButton { get { return runButton; } }
        public System.Windows.Controls.Button SaveAsButton { get { return saveAsButton; } }
        public System.Windows.Controls.Button ClearButton { get { return clearButton; } }
        public System.Windows.Controls.Button AuditButton { get { return auditButton; } }
        public System.Windows.Controls.Button AllFunctonnalityButton { get { return auditButton; } }
        public System.Windows.Controls.CheckBox ApplyToAllCheckBox { get { return applyToAllCheckBox; } }

        protected override List<System.Windows.Controls.Control> getAllControls()
        {
            List<System.Windows.Controls.Control> controls = new List<System.Windows.Controls.Control>(0);
            controls.Add(SaveClearRunButton);
            controls.Add(new Separator());
            
            controls.Add(applyToAllCheckBox);
            controls.Add(RunButton);
            controls.Add(ClearButton);
            controls.Add(SaveAsButton);
            controls.Add(new Separator());
            controls.Add(SaveButton);
            controls.Add(CloseButton);
            
            return controls;
        }
        
        /// <summary>
        /// Cette methode permet de configurer la barre pour spécifier les boutons qui doivent être prrésents.
        /// Pour la gestion d'un fichier, nous avons besoin de:
        /// 
        /// </summary>
        protected override void userConfiguration()
        {
            base.userConfiguration();
            SaveClearRunButton.ToolTip = "Save, clear and load table";
            NewButton.ToolTip = "Add a new Table";
            SaveButton.ToolTip = "Save Table";
            SaveAsButton.ToolTip = "Save Table as";
            CloseButton.ToolTip = "Exit Table Editor";

            SaveButton.IsEnabled = false;
            SaveButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
            SaveAsButton.IsEnabled = true;
            SaveAsButton.Margin = new System.Windows.Thickness(30, 0, 0, 0);
            SaveAsButton.Visibility = Visibility.Collapsed;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            saveClearRunButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "SCL", "Save, clear and load table", "SalmonButtonStyle", new Thickness(0, 0, 50, 0));
            runButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Load", "Run allocation", "GreenButtonStyle", new Thickness(5, 0, 0, 0));
            clearButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton(null, "Clear", "Clear allocation", "WineButtonStyle", new Thickness(30, 0, 0, 0));
            saveAsButton = Misp.Kernel.Ui.Base.ToolBar.BuildButton("save-as-white.png", "Save Table as", "Save Table as", "WineButtonStyle", new Thickness(30, 0, 0, 0));
            saveAsButton.Visibility = Visibility.Collapsed;
            
            applyToAllCheckBox = new CheckBox();
            applyToAllCheckBox.IsChecked = true;
            applyToAllCheckBox.Content = "Apply to all cells";
            applyToAllCheckBox.ToolTip = "Apply to all cells";
            applyToAllCheckBox.FlowDirection = System.Windows.FlowDirection.LeftToRight;
            applyToAllCheckBox.Margin = new Thickness(5, 10, 5, 0);

        }

    }
}
