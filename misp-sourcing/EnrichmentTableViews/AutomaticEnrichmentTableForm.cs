using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.EnrichmentTableViews
{
    public class AutomaticEnrichmentTableForm : AutomaticSourcingForm
    {
        #region Constructors

        public AutomaticEnrichmentTableForm(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            this.AutomaticTablePropertiesPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.AutomaticSourcingPanel.customizeForEnrichmentTable();
        }

        #endregion
    }

}
