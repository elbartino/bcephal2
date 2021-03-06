﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class GrilleFilter
    {

        public Grille grid { get; set; }

        public Attribute attribute { get; set; }

        public Target filterScope { get; set; }

        public Period filterPeriod { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public bool showAll { get; set; }

        public GrilleColumn orderByColumn { get; set; }

        public bool orderDes { get; set; }

        public String file { get; set; }

        
        public bool creditChecked { get; set; }
        
        public bool debitChecked { get; set; }
        
        public bool includeRecoChecked { get; set; }

        public Attribute recoType { get; set; }

        public GrilleColumnFilter filter { get; set; }

        public List<long> oids { get; set; }

        public GrilleFilter()
        {
            oids = new List<long>(0);
            creditChecked = false;
            debitChecked = false;
            includeRecoChecked = false;
        }

        public void ClearColumnFilter()
        {
            filter = null;
        }
        
    }
}
