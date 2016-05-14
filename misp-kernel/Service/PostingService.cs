using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Domain.Browser;
using System.Threading;
using Misp.Kernel.Ui.Base;
using RestSharp;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class PostingService : Service<Misp.Kernel.Domain.Persistent, Misp.Kernel.Domain.Browser.PostingBrowserData>
    {
        #region Properties

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public PeriodNameService periodNameService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public MeasureService measureService { get; set; }

        #endregion
        
        #region methods acions
        
        /// <summary>
        /// postings according to filter
        /// </summary>
        /// <returns>return postings according to filter in rTemplate</returns>
        public List<PostingBrowserData> getPostings(PostingFilter filter)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/postings", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                List<PostingBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<PostingBrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve postings.", e);
                throw new ServiceExecption("Unable to retrieve postings.", e);
            }
        }

        public bool reconciliate(ReconciliationData data) {
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

        public bool resetReconciliation(List<string> numbers)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/reset-reconciliation", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(numbers);
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

        public bool deletePosting(List<long> ids)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/delete-postings", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(ids);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool result = RestSharp.SimpleJson.DeserializeObject<Boolean>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Unable to delete postings.", e);
                throw new ServiceExecption("Unable to delete postings.", e);
            }
        }

        /// <summary>
        /// All account
        /// </summary>
        public List<Account> getAllAccount()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/all-accounts", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);                
                List<Account> objects = RestSharp.SimpleJson.DeserializeObject<List<Account>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve accounts.", e);
                throw new ServiceExecption("Unable to retrieve accounts.", e);
            }
        }
        

        #endregion

    }
}

