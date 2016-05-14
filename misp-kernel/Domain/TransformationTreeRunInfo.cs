using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class TransformationTreeRunInfo
    {
        public bool isError { get; set; }
        public String errorMessage { get; set; }
        public bool runEnded { get; set; }
        public int totalCount { get; set; }
        public int runedCount { get; set; }
        public string item { get; set; }
        public TransformationTreeRunInfo currentTreeRunInfo { get; set; }
    }
}
