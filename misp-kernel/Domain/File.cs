using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class File : Persistent
    {

        public string code { get; set; }

        public string name { get; set; }

        public string path { get; set; }

        public string version { get; set; }

        public int allocationStatus { get; set; }

        public string universeGenerated { get; set; }

    }
}
