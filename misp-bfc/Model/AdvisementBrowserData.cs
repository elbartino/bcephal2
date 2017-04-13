using Misp.Kernel.Domain.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Bfc.Model
{
    public class AdvisementBrowserData : BrowserData
    {

        public String code { get; set; }

        public String memberBank { get; set; }
	
	    public String scheme { get; set; }

        public String pml { get; set; }

        public String platform { get; set; }
	
	    public decimal amount{ get; set; }

        public String dc { get; set; }

        public string valueDate { get; set; }

        [ScriptIgnore]
        public String pdf { get { return "pdf" + code; }}

        public String creator { get; set; }

        [ScriptIgnore]
        public DateTime? valueDateTime
        {
            get { return !string.IsNullOrEmpty(valueDate) ? DateTime.Parse(valueDate) : new DateTime(); }
            set { this.valueDate = value.HasValue ? value.Value.ToShortDateString() : null; }
        }

    }
}
