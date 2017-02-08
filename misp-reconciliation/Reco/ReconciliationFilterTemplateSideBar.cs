using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Reco
{
    public class ReconciliationFilterTemplateSideBar : SideBar
    {

        #region Constructor


        #endregion


        #region Properties

        public ReconciliationFilterTemplateGroup TemplateGroup { get; set; }
        public ModelSidebarGroup EntityGroup { get; set; }
        public MeasureSidebarGroup MeasureGroup { get; set; }
        public TargetGroup TargetGroup { get; set; }
        public PeriodSidebarGroup PeriodGroup { get; set; }

        #endregion


        #region Initialization

        /// <summary>
        /// 
        /// </summary>
        public override void InitializeGroups()
        {
            base.InitializeGroups();
            this.TemplateGroup = new ReconciliationFilterTemplateGroup("Filters", true);
            this.EntityGroup = new ModelSidebarGroup();
            this.TargetGroup = new TargetGroup("Targets", true);
            this.MeasureGroup = new MeasureSidebarGroup();
            this.PeriodGroup = new PeriodSidebarGroup();
            
            this.TemplateGroup.Background = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.Background = System.Windows.Media.Brushes.LightBlue;

            this.TemplateGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            this.TargetGroup.BorderBrush = System.Windows.Media.Brushes.LightBlue;

            this.AddGroup(this.TemplateGroup);
            this.AddGroup(this.EntityGroup);
            this.AddGroup(this.MeasureGroup);
            this.AddGroup(this.PeriodGroup);
        }


        #endregion

    }
}