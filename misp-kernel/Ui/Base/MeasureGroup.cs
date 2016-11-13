using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Kernel.Ui.Base
{
    public class MeasureGroup : SidebarGroup
    {

        #region Properties

        public delegate void SelectMeasureEventHandler(Domain.Target target);
        public event SelectMeasureEventHandler SelectMeasureHandler;

        public MeasureTreeview MeasureTreeview { get; set; }

        public Service.MeasureService MeasureService { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public MeasureGroup() : base() { }

        public MeasureGroup(string header) : base(header) { }

        public MeasureGroup(string header, bool expanded)
            : base(header, expanded) { }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.MeasureTreeview = new MeasureTreeview();
            this.ContentPanel.Children.Add(this.MeasureTreeview);
        }

        #endregion

        public void InitializeTreeViewDatas(bool showPostingMeasure)
        {
            Domain.Measure rootMeasure = this.MeasureService.getRootMeasure(showPostingMeasure);
            this.MeasureTreeview.DisplayRoot(rootMeasure);
        }

        public void InitializeTreeViewDatas(bool showPostingMeasure, List<Domain.CalculatedMeasure> CalculatedMeasures)
        {
            Domain.Measure rootMeasure = this.MeasureService.getRootMeasure(showPostingMeasure);
            this.MeasureTreeview.DisplayRoot(rootMeasure, CalculatedMeasures);
        }

    }
}
