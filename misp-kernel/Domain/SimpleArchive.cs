using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class SimpleArchive
    {
        public String name;

        public String repository;

        public String comments;

        [ScriptIgnore]
        public bool isDefaultRepository { get; set; }
    }
}
