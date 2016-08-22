using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Application;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    /// <summary>
    /// Cette classe,inspirée de la classe Service,
    /// permet la communication avec le serveur depuis le Dashboard.
    /// </summary>
    public class DashBoardService : Service<Persistent, BrowserData>
    {

        public static String TABLES = "tables";
        public static String REPORTS = "reports";
        public static String TARGETS = "targets";
        public static String TREES = "transformationtrees";
        public static String COMBINED_TREES = "combinedtrees";
        public static String DESIGNS = "designs";
        public static String AUTOMATIC_UPLOADS = "automaticsourcings";
        public static String CALCULATED_MEASURES = "calculatedmeasures";
        public static String MODELS = "models";
        public static String STRUCTURED_REPORTS = "structuredreports";
        public static String RECONCILIATION_FILTERS = "reconciliationgrids";
        public static String RECONCILIATION_POSTINGS = "bankreconciliationpostings";
        public static String TRANSACTION_FILE_TYPES = "transactionfiletypes";

        public static String AUTOMATIC_TARGET = "automatictargets";
		public static String AUTOMATIC_GRID = "automaticgrids";
        public static String INPUT_GRID = "inputgrids";
        public static String REPORT_GRID = "reportgrids";

        public static String AUTOMATIC_POSTING_GRID = "automaticpostinggrids";
        public static String POSTING_GRID = "postinggrids";

        /// <summary>
        /// 
        /// </summary>
        /// <returns>DashboardData</returns>
        public List<BrowserData> getDashboardDatas(string param,int? userOid=null)
        {
            if (string.IsNullOrWhiteSpace(param)) return new List<BrowserData>(0);
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/" + param, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = userOid != null ? serializer.Serialize(userOid) : null;
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<BrowserData> datas = RestSharp.SimpleJson.DeserializeObject<List<BrowserData>>(queryResult.Content);
                return datas;
            }
            catch (Exception e)
            {
                return new List<BrowserData>(0);
            }
        }

        public List<DashBoardConfiguration> getAllDashboardConfiguration(int? userOid = null) 
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/allconfiguration", Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = userOid != null ? serializer.Serialize(userOid) : null;
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<DashBoardConfiguration> datas = RestSharp.SimpleJson.DeserializeObject<List<DashBoardConfiguration>>(queryResult.Content);
                return datas;
            }
            catch (Exception e)
            {
                return new List<DashBoardConfiguration>(0);
            }
        }

        public DashBoardConfiguration getDashboardConfigurationByName(String name, int? userOid = null) 
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/configurationbyname/" + name, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = userOid != null ? serializer.Serialize(userOid) : null;
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                DashBoardConfiguration data = RestSharp.SimpleJson.DeserializeObject<DashBoardConfiguration>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                return new DashBoardConfiguration();
            }
           
        }

        public DashBoardConfiguration getDashboardConfigurationByPosition(int position,int? userOid = null)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/configurationbyposition/" + position, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = userOid != null ? serializer.Serialize(userOid) : null;
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                DashBoardConfiguration data = RestSharp.SimpleJson.DeserializeObject<DashBoardConfiguration>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                return new DashBoardConfiguration();
            }

        }

        public DashBoardConfiguration saveDashboardConfiguration(DashBoardConfiguration dashboardConfiguration,int? userOid = null)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string resPath = userOid != null ? ResourcePath + "/configuration/save/"+userOid : ResourcePath + "/configuration/save";
                var request = new RestRequest(resPath, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(dashboardConfiguration);
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                DashBoardConfiguration data = RestSharp.SimpleJson.DeserializeObject<DashBoardConfiguration>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<DashBoardConfiguration> saveListDashboardConfiguration(List<DashBoardConfiguration> dashboardConfigurations,int? userOid = null)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string resPath = userOid != null ? ResourcePath + "/configuration/saveall/" + userOid : ResourcePath + "/configuration/saveall";
                var request = new RestRequest(resPath, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(dashboardConfigurations);
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                List<DashBoardConfiguration> data = RestSharp.SimpleJson.DeserializeObject<List<DashBoardConfiguration>>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                return new List<DashBoardConfiguration>(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>DashboardData</returns>
        public Boolean setInvisible(string token, int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/invisible/" + "/"+oid, Method.POST);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Boolean data = RestSharp.SimpleJson.DeserializeObject<Boolean>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                throw new ServiceExecption("Unable to set invisible.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>DashboardData</returns>
        public Boolean setInvisible(string token, List<int> oids)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/hide/" + token, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(oids);
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Boolean data = RestSharp.SimpleJson.DeserializeObject<Boolean>(queryResult.Content);
                return data;
            }
            catch (Exception e)
            {
                throw new ServiceExecption("Unable to set invisible.", e);
            }
        }


       

    }
}
