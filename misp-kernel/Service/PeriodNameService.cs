using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class PeriodNameService : Service<Misp.Kernel.Domain.PeriodName, Misp.Kernel.Domain.Browser.BrowserData>
    {
        public bool canPeriodicityModify(Domain.PeriodName periodName)
         {
            return true;
        }
        public List<Kernel.Domain.PeriodItem> getAllTableAndCellsPeriodItems() 
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/list/perioditems", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                Console.WriteLine("List = " + queryResult.StatusCode);

                List<Kernel.Domain.PeriodItem> objects = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.PeriodItem>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of objects.", e);
                throw new ServiceExecption("Unable to retrieve list of objects.", e);
            }            
        }

        public Kernel.Domain.PeriodName getRootPeriodName()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Kernel.Domain.PeriodName root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.PeriodName>(queryResult.Content);
                return root;
            }
            catch (Exception e)
            {
               logger.Error("Unable to Return PeriodName.", e);
               throw new ServiceExecption("Unable to Return PeriodName.", e);
            }
        }
    }
}
