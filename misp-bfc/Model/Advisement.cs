using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Bfc.Model
{
    public class Advisement : Persistent
    {
        public BfcItem memberBank { get; set; }
        public BfcItem scheme { get; set; }
        public BfcItem pml { get; set; }
        public BfcItem platform { get; set; }
        public decimal? alreadyRequestedAmount { get; set; }
        public decimal? amount { get; set; }
        public decimal? balance { get; set; }
        public string valueDate { get; set; }
        public string message { get; set; }
        public string structuredMessage { get; set; }
        public string advisementType { get; set; }
        public string creator { get; set; }
        
        [ScriptIgnore]
        public DateTime? valueDateTime
        {
            get { return !string.IsNullOrEmpty(valueDate) ? DateTime.Parse(valueDate) : new DateTime(); }
            set { this.valueDate = value.HasValue ? value.Value.ToShortDateString() : null; }
        }

    }
}
