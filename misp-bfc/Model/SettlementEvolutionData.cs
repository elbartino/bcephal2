using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Model
{
    public class SettlementEvolutionData
    {

        public string platformID { get; set; }
        public string platformName { get; set; }
        public decimal lastYear { get; set; }
        public decimal yearToDate { get; set; }
        public decimal cagr3Years { get; set; }
        public decimal averageDayLast24Month { get; set; }
        public decimal peakDayLast24Month { get; set; }
        public decimal todayMultilot { get; set; }
        
    }
}
