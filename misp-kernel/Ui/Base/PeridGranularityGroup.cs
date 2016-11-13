using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Base
{
    public class PeridGranularityGroup : SidebarGroup
    {
        
        #region Properties

        public event Base.SelectedItemChangedEventHandler SelectionChanged;

        public ListBox PeridGranularityListBox { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public PeridGranularityGroup() : base() { }

        public PeridGranularityGroup(string header) : base(header) { }

        public PeridGranularityGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.PeridGranularityListBox = new ListBox();
            List<String> granularities = new List<string>(0);
            granularities.Add(Granularity.YEAR.name);
            granularities.Add(Granularity.SEMESTER.name);
            granularities.Add(Granularity.QUATER.name);
            granularities.Add(Granularity.MONTH.name);
            granularities.Add(Granularity.DAY.name);
            this.PeridGranularityListBox.ItemsSource = granularities;

            this.PeridGranularityListBox.MouseUp += OnGranularitySelected;

            this.ContentPanel.Children.Add(this.PeridGranularityListBox);
        }

        private void OnGranularitySelected(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Object selection = PeridGranularityListBox.SelectedItem;
            if (selection != null && selection is String && SelectionChanged != null)
            {
                SelectionChanged(selection as String);
            }
        }

        #endregion

    }
}
