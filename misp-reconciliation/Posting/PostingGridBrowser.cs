﻿using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.Posting
{
    public class PostingGridBrowser : InputGridBrowser
    {

        public PostingGridBrowser(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        protected override string getTitle()
        {
            return "Posting Grids";
        }

    }
}
