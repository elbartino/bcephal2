using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Initiation.Model;
using Misp.Initiation.Measure;
using Misp.Initiation.Periodicity;
using RestSharp;

namespace Misp.Initiation.Service
{
    public class InitiationService : Service<Object>
    {

        public Misp.Initiation.Model.ModelService ModelService { get; set; }
        public Misp.Initiation.Measure.MeasureService MeasureService { get; set; }
        public Misp.Initiation.Periodicity.PeriodicityService PeriodicityService { get; set; }
          
    }
}
