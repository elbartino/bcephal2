using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;

namespace Misp.Kernel.Service
{
    public class InitiationService : Service<Persistent, Domain.Browser.BrowserData>
    {

        public ModelService ModelService { get; set; }
        public MeasureService MeasureService { get; set; }
        public PeriodNameService PeriodicityService { get; set; }
          
    }
}
