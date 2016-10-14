﻿using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Base
{
    public class ReconciliationFunctionality : Functionality
    {

        public ReconciliationFunctionality()
        {
            this.Code = FunctionalitiesCode.RECONCILIATION;
            this.Name = "Reconciliation";
            buildChildren();
        }

        private void buildChildren()
        {
            this.Children.Add(new Functionality(this, FunctionalitiesCode.RECONCILIATION_POSTINGS, "Reconciliation Postings List", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.RECONCILIATION_FILTER_LIST, "Reconciliation Filter List", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.RECONCILIATION_FILTER_VIEW, "Reconciliation Filter View", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.RECONCILIATION_FILTER_EDIT, "Reconciliation Filter Edit", true));
            this.Children.Add(new Functionality(this, FunctionalitiesCode.RECONCILIATION_CONFIGURATION, "Reconciliation Configuration", true));
            
        }


    }
}