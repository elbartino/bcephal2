using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Ui.TreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Base
{
    public class PeriodNameGroup : SidebarGroup
    {

        #region Events
        public delegate void SelectPeriodName(Domain.PeriodName target);
        public event SelectPeriodName OnSelectPeriodName;

        public delegate void SelectPeriodInterval(Domain.PeriodInterval value);
        public event SelectPeriodInterval OnSelectPeriodInterval;

        #endregion

        #region Properties

        public PeriodNameTreeview PeriodNameTreeview { get; set; }

        public Service.PeriodNameService PeriodNameService { get; set; }

        public PeriodName rootPeriodName { get; set; }

        public PeriodName defaultPeriodName { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public PeriodNameGroup() : base() { }

        public PeriodNameGroup(string header) : base(header) { }

        public PeriodNameGroup(string header, bool expanded)
            : base(header, expanded)
        {
            InitializeHandlers();
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.PeriodNameTreeview = new PeriodNameTreeview();
            this.ContentPanel.Children.Add(this.PeriodNameTreeview);
        }

        #endregion

        public void InitializeTreeViewDatas()
        {
            rootPeriodName = PeriodNameService.getRootPeriodName();
            defaultPeriodName = rootPeriodName.getDefaultPeriodName();
            this.PeriodNameTreeview.DisplayPeriods(rootPeriodName);
        }

        private void InitializeHandlers()
        {
            this.PeriodNameTreeview.SelectionChanged += onSelectPeriodNameFromSidebar;
        }

        private void onSelectPeriodNameFromSidebar(object sender)
        {
            if (sender == null) return;
            if (OnSelectPeriodName != null && sender is Domain.PeriodName) OnSelectPeriodName((Domain.PeriodName)sender);
            if (OnSelectPeriodInterval != null && sender is Domain.PeriodInterval) OnSelectPeriodInterval((Domain.PeriodInterval)sender);
        }
        

    }
}
