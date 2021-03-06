﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain.Browser
{
    public class BrowserDataFilter
    {

        public static int DEFAULT_PAGE_SIZE = 25;

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? groupOid { get; set; }

        public string criteria { get; set; }

        public bool orderAsc { get; set; }

        public List<BrowserDataFilterItem> items { get; set; }

        [ScriptIgnore]
        public int totalPages { get; set; }

        public BrowserDataFilter()
        {
            pageSize = DEFAULT_PAGE_SIZE;
            items = new List<BrowserDataFilterItem>(0);
            orderAsc = true;
        }

    }
}
