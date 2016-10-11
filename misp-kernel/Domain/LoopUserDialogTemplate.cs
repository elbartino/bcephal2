using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class LoopUserDialogTemplate : Persistent
    {
        public string message { get; set; }

        public string help { get; set; }

        public string conditions { get; set; }

        public bool active { get; set; }

        public bool onePossibleChoice { get; set; }

        
        public LoopUserDialogTemplate()
        {
            
        }

        public override string ToString()
        {
            return this.message != null ? this.message : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is LoopUserDialogTemplate)) return 1;
            return this.message.CompareTo(((LoopUserDialogTemplate)obj).message);
        }

    }
}
