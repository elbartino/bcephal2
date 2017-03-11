using Misp.Bfc.Base;
using Misp.Bfc.Model;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Advisements
{
    public class AdvisementBrowserController : BrowserController<Advisement, AdvisementBrowserData>
    {

        public AdvisementType advisementType { get; set; }
        public AdvisementBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.ADVISEMENT;
        }

        public AdvisementBrowserController(AdvisementType advisementType) :base()
        {
            this.advisementType = advisementType;
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.ADVISEMENT;
        }


        public override string GetEditorFuntionality()
        {
            if (advisementType == AdvisementType.SETTLEMENT) return BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT;
            if (advisementType == AdvisementType.MEMBER) return BfcFunctionalitiesCode.MEMBER_ADVISEMENT;
            if (advisementType == AdvisementType.PREFUNDING) return BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT;
            if (advisementType == AdvisementType.EXCEPTIONAL) return BfcFunctionalitiesCode.EXCEPTIONAL_ADVISEMENT;
            return BfcFunctionalitiesCode.ADVISEMENT;
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.ADVISEMENT;
        }

        protected override Kernel.Ui.Base.IView getNewView()
        {
            return new AdvisementBrowser(this.SubjectType, this.FunctionalityCode,advisementType); 
        }

        protected override void initializeViewData()
        {
        }
    }
}
