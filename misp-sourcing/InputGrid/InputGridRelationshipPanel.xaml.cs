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

namespace Misp.Sourcing.InputGrid
{
    /// <summary>
    /// Interaction logic for InputGridRelationshipPanel.xaml
    /// </summary>
    public partial class InputGridRelationshipPanel : Grid
    {

        #region Properties

        public event ChangeEventHandler Changed;

        public Grille Grid { get; set; }

        #endregion


        #region Constructors

        public InputGridRelationshipPanel()
        {
            InitializeComponent();
            this.PrimaryRelationPanel.IsPrimary = true;
            InitializeHandlers();
        }

        #endregion


        #region Operations

        public void Display(Grille grid)
        {
            if (grid == null) return;
            this.Grid = grid;
            if (this.Grid.relationship != null)
            {
                this.Grid.relationship.Grid = grid;
                foreach (GrilleRelationshipItem item in this.Grid.relationship.itemListChangeHandler.Items)
                {
                    item.column = this.Grid.GetColumn(item.column);
                }
            }
            this.PrimaryRelationPanel.Display(this.Grid);
            this.RelationshipPanel.Display(this.Grid);
        }

        public void FillGrid(Grille grid)
        {
            if (grid == null) return;
            
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize les handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            this.PrimaryRelationPanel.Changed += OnChanged;
            this.RelationshipPanel.Changed += OnChanged;
        }

        private void OnChanged()
        {
            if (Changed != null) Changed();
        }
        
        #endregion


    }
}
