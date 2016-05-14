using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class AllocationLogService : Service<Kernel.Domain.AuditInfo, Misp.Kernel.Domain.Browser.BrowserData>
    {


        /// <summary>
        /// retourne la liste des allocations depuis le dernier clear all
        /// </summary>
        /// <returns></returns>
        public List<Kernel.Domain.Browser.AllocationRunBrowserData> GetAllAllocationRun()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/all_run", Method.POST);
                request.RequestFormat = DataFormat.Json;

                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Kernel.Domain.Browser.AllocationRunBrowserData> allocationRunList = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Browser.AllocationRunBrowserData>>(queryResult.Content);
                    return allocationRunList;
                }
                catch (Exception) { return null; }
            }
            catch (Exception e) { throw new BcephalException("Unable to Return allocatuion run list.", e); }
        }

        /// <summary>
        /// retourne la liste des allocations depuis le dernier clear all
        /// </summary>
        /// <returns></returns>
        public AllocationRunBrowserDataPage GetAllocationRunBrowserDataPage(long page, long pageSize)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/run/" + page + "/" + pageSize, Method.POST);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    AllocationRunBrowserDataPage dataPage = RestSharp.SimpleJson.DeserializeObject<AllocationRunBrowserDataPage>(queryResult.Content);
                    return dataPage;
                }
                catch (Exception) { return null; }
            }
            catch (Exception e) { throw new BcephalException("Unable to Return allocatuion run list.", e); }
        }

        /// <summary>
        /// retourne la liste des infos sur une allocation donnée
        /// </summary>
        /// <param name="allocationRunOID"></param>
        /// <returns></returns>
        public AllocationRunInfo GetAllocationRunInfo(long allocationRunOID, long page)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/info/" + page, Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(allocationRunOID);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.AllocationRunInfo auditInfo = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.AllocationRunInfo>(queryResult.Content);
                    return auditInfo;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return auditInfos.", e);
            }
        }


        /// <summary>
        /// retourne la liste des infos sur une allocation donnée
        /// </summary>
        /// <param name="allocationRunOID"></param>
        /// <returns></returns>
        public MetricMeasureAllocation MetricMeasureAllocation(long allocationRunOID)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/metric_measure", Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(allocationRunOID);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.MetricMeasureAllocation metricsInfos = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.MetricMeasureAllocation>(queryResult.Content);
                    return metricsInfos;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return auditInfos.", e);
            }
        }



    }
}
