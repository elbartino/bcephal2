using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;
using Misp.Kernel.Domain.Browser;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class MeasureService : Service<Kernel.Domain.Measure, Domain.Browser.BrowserData>
    {

        public List<Kernel.Domain.Measure> getAllLeafts()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/leafts", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.Measure> measures = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Measure>>(queryResult.Content);
                    return measures;
                }
                catch (Exception)
                {
                    return new List<Kernel.Domain.Measure>(0);
                }
            }
            catch (Exception)
            {
                return new List<Kernel.Domain.Measure>(0);
            }
        }

        /// <summary>
        /// Retoune la liste de mesures du fichier ouvert.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>La liste de mesures du fichier ouvert</returns>
        public Kernel.Domain.Measure getRootMeasure(bool showPostingMeasure = true)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root/" + showPostingMeasure, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.Measure root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Measure>(queryResult.Content);
                    return root;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return Measures.", e);
            }
        }

        /// <summary>
        /// Retoune la liste de mesures du fichier ouvert.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>La liste de mesures du fichier ouvert</returns>
        public Kernel.Domain.Measure getRootMeasureForSideBar(bool showPostingMeasure = true)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/root-for-sidebar/" + showPostingMeasure, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.Measure root = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Measure>(queryResult.Content);
                    return root;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return Measures.", e);
            }
        }

        public BrowserDataPage<Kernel.Domain.Measure> getAllDescendents(BrowserDataFilter filter, bool showPostingMeasure = true)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/all-descendents/" + showPostingMeasure, Method.POST);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                BrowserDataPage<Kernel.Domain.Measure> values = RestSharp.SimpleJson.DeserializeObject<BrowserDataPage<Kernel.Domain.Measure>>(queryResult.Content);
                return values;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return measures.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodicity"></param>
        /// <returns></returns>
        public Kernel.Domain.Measure saveMeasures(Kernel.Domain.Measure measure)
        {
            try
            {
                var request = new RestRequest(ResourcePath, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                measure = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.Measure>(queryResult.Content);
                return measure;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to save Periodicity.", e);
            }
        }



        public bool isMeasureUseAllocation(Kernel.Domain.Measure measure)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/useallocation", Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(measure);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);

                try
                {
                    bool isUseallocation = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                    return isUseallocation;
                }
                catch (Exception)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to verify measure in allocations ", e);
            }
        }

    }
}
