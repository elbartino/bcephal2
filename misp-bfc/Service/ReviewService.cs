﻿using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Service
{
    public class ReviewService : Misp.Kernel.Service.Service<Object, BrowserData>
    {

        public PrefundingAccountService PrefundingAccountService { get; set; }

    }
}
