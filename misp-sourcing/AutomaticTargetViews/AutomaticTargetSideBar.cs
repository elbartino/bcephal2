using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.AutomaticTargetViews
{
    public class AutomaticTargetSideBar : AutomaticSourcingSideBar
    {
        #region Properties
        public AutomaticSourcingGroup AutomaticTargetGroup { get { return AutomaticSourcingGroup; } set { AutomaticSourcingGroup = value; } }
        #endregion

        protected override void InitializeGroups()
        {
            base.InitializeGroups();
            this.RemoveGroup(this.MeasureGroup);
            this.RemoveGroup(this.CaculatedMeasureGroup);
            this.RemoveGroup(this.PeriodNameGroup);
            this.AutomaticSourcingGroup.Header = "Automatic Target";
        }
    }
}
