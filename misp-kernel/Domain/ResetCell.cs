using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ResetCell:Persistent
    {
        public ResetCell() 
        { }

        public string name{get;set;}
        public string nameRow { get; set; }
        public string nameColumn { get; set; }
        public string sheetName {get;set;}
        public int sheetIndex {get;set;}

    }
}
