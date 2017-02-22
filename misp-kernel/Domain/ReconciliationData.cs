using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ReconciliationData
    {
        public List<long> ids { get; set; }
        public Attribute recoType { get; set; }
        public Measure measure { get; set; }
        public decimal writeOffAmount { get; set; }
        public List<WriteOffField> writeOffFields { get; set; }

        public ReconciliationData()
        {
            ids = new List<long>(0);
            writeOffAmount = 0;
        }
    }
}
