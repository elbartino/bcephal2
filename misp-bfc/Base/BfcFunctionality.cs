using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Base
{
    public class BfcFunctionality : Functionality
    {

        public BfcFunctionality()
        {
            this.Code = BfcFunctionalitiesCode.ADVISEMENT;
            this.Name = "Advisement";
            buildChildren();
        }

        private void buildChildren()
        {
            this.Children.Add(new Functionality(this, BfcFunctionalitiesCode.PREFUNDING_ADVISEMENT, "Prefunding advisement", true, RightType.VIEW, RightType.EDIT, RightType.CREATE));
            this.Children.Add(new Functionality(this, BfcFunctionalitiesCode.MEMBER_ADVISEMENT, "Member advisement", true, RightType.VIEW, RightType.EDIT, RightType.CREATE));
            this.Children.Add(new Functionality(this, BfcFunctionalitiesCode.EXCEPTIONAL_ADVISEMENT, "Exceptional advisement", true, RightType.VIEW, RightType.EDIT, RightType.CREATE));
            this.Children.Add(new Functionality(this, BfcFunctionalitiesCode.SETTLEMENT_ADVISEMENT, "Settlement advisement", true, RightType.VIEW, RightType.EDIT, RightType.CREATE));
        }


    }
}