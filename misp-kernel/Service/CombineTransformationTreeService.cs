using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
   public class CombineTransformationTreeService : Service<Kernel.Domain.CombinedTransformationTree,Kernel.Domain.Browser.BrowserData>
   {
       /// <summary>
       ///  le PeriodNameService
       /// </summary>
       public PeriodNameService PeriodNameService { get; set; }

       /// <summary>
       /// Le MeasureService.
       /// </summary>
       public MeasureService MeasureService { get; set; }

       /// <summary>
       /// Le ModelService.
       /// </summary>
       public ModelService ModelService { get; set; }

       /// <summary>
       /// Le PeriodicityService.
       /// </summary>
       public PeriodicityService PeriodicityService { get; set; }
       
       /// <summary>
       /// Le CalculatedMeasureService
       /// </summary>
       public CalculatedMeasureService CalculatedMeasureService { get; set; }


       /// <summary>
       /// Le TargetService.
       /// </summary>
       public TargetService TargetService { get; set; }

       /// <summary>
       /// Le TransformationTreeService
       /// </summary>
       public TransformationTreeService TransformationTreeService { get; set; }

   }
}
