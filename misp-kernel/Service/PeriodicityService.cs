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
    public class PeriodicityService : Service<Misp.Kernel.Domain.Periodicity, Domain.Browser.BrowserData>
    {

        /// <summary>
        /// Retoune la périodicité du fichier ouvert.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>La périodicité du fichier ouvert</returns>
        public Kernel.Domain.Periodicity getPeriodicity()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/get", Method.GET);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.Periodicity periodicity = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Periodicity>(queryResult.Content);
                    return periodicity;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return Periodicity.", e);
            }
        }


        public bool canPeriodicityModify(PeriodName periodicity)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/modify", Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(periodicity);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);

                try
                {
                    bool canModify = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                    return canModify ;
                }
                catch (Exception e)
                {
                    throw new BcephalException("Unable to verify periodicity change ", e);
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to verify periodicity change ", e);
            }
        }


        


    }
}
