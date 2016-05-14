using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class SaveInfo
    {

        public int oid { get; set; }

        public String item { get; set; }

        public int stepCount { get; set; }

        public int stepRuned { get; set; }

        public bool isEnd { get; set; }

        public bool isError { get; set; }

        public String errorMessage { get; set; }

        public SaveInfo currentStepInfo { get; set; }

        public String message { get; set; }
        
        public List<SaveInfo> infos { get; set; }

        public SaveInfo()
        {
            infos = new List<SaveInfo>(0);
        }

    }
}
