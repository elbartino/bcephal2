using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class MultiTableLoadData
    {

        public int templateOid { get; set; }

        public int groupOid { get; set; }

        public List<String> excelFiles { get; set; }

    }
}
