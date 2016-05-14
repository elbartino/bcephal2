using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class TargetService : Service<Misp.Kernel.Domain.Target, Misp.Kernel.Domain.Browser.BrowserData>
    {
                
        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }

        public virtual Misp.Kernel.Domain.Target getTargetById(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/get", Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Misp.Kernel.Domain.Target target = RestSharp.SimpleJson.DeserializeObject<Misp.Kernel.Domain.Target>(queryResult.Content);
                    return target;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return target identified by: " + oid, e);
            }
        }



    
    }
}

