﻿using DevExpress.Xpf.LayoutControl;
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

namespace Misp.Kernel.Ui.Dashboard
{
    /// <summary>
    /// Interaction logic for NavDashboardLayout.xaml
    /// </summary>
    public partial class NavDashboardLayout : TileLayoutControl
    {
        
        #region Properties

        public ChangeItemEventHandler Selection { get; set; }
        public ChangeItemEventHandler BlockHide { get; set; }

        #endregion


        #region Constructors

        public NavDashboardLayout()
        {
            InitializeComponent();
        }

        #endregion


        #region Operations

        public void AddBlock(NavBlock block)
        {
            block.Selection -= OnBlockSelected;
            block.Hide -= OnBlockHided;
            block.Edit -= OnBlockEdited;
            block.Selection += OnBlockSelected;
            block.Hide += OnBlockHided;
            block.Edit += OnBlockEdited;
            this.Children.Add(block);
        }

        public void RemoveBlock(NavBlock block)
        {
            block.Selection -= OnBlockSelected;
            block.Hide -= OnBlockHided;
            block.Edit -= OnBlockEdited;
            this.Children.Remove(block);
            block.Dispose();
        }

        public void Clear()
        {
            int count = this.Children.Count;
            while(count > 0)
            {
                UIElement elt = this.Children[0];
                if (elt is NavBlock) this.RemoveBlock((NavBlock)elt);
                else this.Children.Remove(elt);
                count--;
            }
        }

        #endregion>


        #region Handlers
        
        private void OnBlockSelected(object item)
        {
            if (Selection != null) Selection(item);
        }

        private void OnBlockEdited(object item)
        {
            if (item != null && item is NavBlock)
            {
                NavBlock block = (NavBlock)item;
                NavDashboardBlockPropertiesDialog dialog = new NavDashboardBlockPropertiesDialog();
                dialog.EditBlock(block);
            }            
        }

        private void OnBlockHided(object item)
        {
            if (BlockHide != null) BlockHide(item);
        }

        #endregion

    }
}
