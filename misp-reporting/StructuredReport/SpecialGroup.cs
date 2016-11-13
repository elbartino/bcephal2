using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Misp.Reporting.StructuredReport
{
    public class SpecialGroup : SidebarGroup
    {

        #region Properties

        public TreeView SpecialTreeView { get; set; }
        public TreeViewItem IncrementalTreeViewItem { get; set; }
        public TreeViewItem FreeTextTreeViewItem { get; set; }

        public string[] specialItems = {"Incremental N°","Free Text" };

        /// <summary>
        /// Evènement du InputTableTreeview qui renvoit le inputTable selectionné
        /// </summary>
        public event SelectedItemChangedEventHandler SelectionChanged;

        public event SelectedItemChangedEventHandler SelectionSpecialChanged;

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public SpecialGroup() : base() { }

        public SpecialGroup(string header) : base(header) { }

        public SpecialGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.SpecialTreeView = new TreeView();
            this.IncrementalTreeViewItem = new TreeViewItem();
            this.IncrementalTreeViewItem.Header = "Incremental N°";
            this.FreeTextTreeViewItem = new TreeViewItem();
            this.FreeTextTreeViewItem.Header = "Free Text";
            this.SpecialTreeView.Items.Add(IncrementalTreeViewItem);
            this.SpecialTreeView.Items.Add(FreeTextTreeViewItem);
            this.ContentPanel.Children.Add(this.SpecialTreeView);

            this.IncrementalTreeViewItem.MouseLeftButtonUp += OnIncrementalClick;
            this.FreeTextTreeViewItem.MouseLeftButtonUp += OnFreeTextClick;
        }

        private void OnIncrementalClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectionChanged != null) SelectionChanged(Kernel.Domain.StructuredReportColumn.Type.INCREMENTAL);
            if (SelectionSpecialChanged != null) SelectionSpecialChanged(specialItems[0]);
        }

        private void OnFreeTextClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectionChanged != null) SelectionChanged(Kernel.Domain.StructuredReportColumn.Type.FREE);
            if (SelectionSpecialChanged != null) SelectionSpecialChanged(specialItems[1]);
        }

        #endregion
    }

}
