using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class EnrichmentTableData
    {

        public List<int> oids { get; set; }

        public bool deleteTable { get; set; }

    }
}
