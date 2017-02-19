using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class ReconciliationFilterTemplateService : Service.Service<ReconciliationFilterTemplate, BrowserData>
    {
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
        ///  Le TargetService
        /// </summary>
        public TargetService TargetService { get; set; }

        public bool reconciliate(ReconciliationData data)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/reconciliate", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(data);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool result = RestSharp.SimpleJson.DeserializeObject<Boolean>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Unable to reconciliate postings.", e);
                throw new ServiceExecption("Unable to reconciliate postings.", e);
            }
        }

        public bool resetReconciliate(int recoTypeOid, List<long> oids)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/reset-reconciliation/" + recoTypeOid, Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(oids);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool result = RestSharp.SimpleJson.DeserializeObject<Boolean>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Unable to reconciliate postings.", e);
                throw new ServiceExecption("Unable to reconciliate postings.", e);
            }
        }

        public bool ContainsReconciliatedItems(int recoTypeOid, List<long> oids)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/contains-reconciliated-items/" + recoTypeOid, Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(oids);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool result = RestSharp.SimpleJson.DeserializeObject<Boolean>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Unable to reconciliate postings.", e);
                throw new ServiceExecption("Unable to reconciliate postings.", e);
            }
        }

        public GrillePage getGridRows(GrilleFilter filter)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/rows", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                GrillePage objects = RestSharp.SimpleJson.DeserializeObject<GrillePage>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve grid rows.", e);
                throw new ServiceExecption("Unable to retrieve grid rows.", e);
            }
        }

        /// <summary>
        /// save ReconciliationFilterTemplate
        /// </summary>
        /// <param name="profil"></param>
        /// <returns></returns>
        public override ReconciliationFilterTemplate Save(ReconciliationFilterTemplate profil)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(profil);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                ReconciliationFilterTemplate recoFilterTemplate = RestSharp.SimpleJson.DeserializeObject<ReconciliationFilterTemplate>(queryResult.Content);
                return recoFilterTemplate;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Domain.Attribute> getReconciliationTypes()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/reco-types", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.Attribute> types = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Attribute>>(queryResult.Content);
                    return types;
                }
                catch (Exception)
                {
                    return new List<Kernel.Domain.Attribute>(0);
                }
            }
            catch (Exception)
            {
                return new List<Kernel.Domain.Attribute>(0);
            }
        }

    }
}
