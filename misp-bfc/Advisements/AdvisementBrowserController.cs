﻿using Misp.Bfc.Base;
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

        public AdvisementBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.ADVISEMENT;
        }


        public override string GetEditorFuntionality()
        {
            return BfcFunctionalitiesCode.ADVISEMENT;
        }

        public override SubjectType SubjectTypeFound()
        {
            return SubjectType.ADVISEMENT;
        }

        protected override Kernel.Ui.Base.IView getNewView()
        {
            return new AdvisementBrowser(this.SubjectType, this.FunctionalityCode); 
        }

        protected override void initializeViewData()
        {

        }
    }
}