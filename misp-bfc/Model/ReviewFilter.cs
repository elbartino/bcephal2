using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Bfc.Model
{
    public class ReviewFilter
    {
        public bool details { get; set; }

        public List<int> memberBankIdOids { get; set; }

        public List<int> schemeIdOids { get; set; }

        public String startDate { get; set; }

        public String endDate { get; set; }

        [ScriptIgnore]
        public DateTime? startDateTime
        {
            get { return !string.IsNullOrEmpty(startDate) ? DateTime.Parse(startDate) : new DateTime(); }
            set { this.startDate = value.HasValue ? value.Value.ToShortDateString() : null; }
        }

        [ScriptIgnore]
        public DateTime? endDateTime
        {
            get { return !string.IsNullOrEmpty(endDate) ? DateTime.Parse(endDate) : new DateTime(); }
            set { this.endDate = value.HasValue ? value.Value.ToShortDateString() : null; }
        }

        public ReviewFilter() 
        {
            memberBankIdOids = new List<int>(0);
            schemeIdOids = new List<int>(0);
            details = false;
        }

    }
}
