using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class InputGridService : Service<Grille, Misp.Kernel.Domain.Browser.BrowserData>
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
        /// Le DesignService.
        /// </summary>
        public DesignService DesignService { get; set; }

        /// <summary>
        /// Le AuditService.
        /// </summary>
        public AuditService AuditService { get; set; }
        
        /// <summary>
        /// Le CalculatedMeasureService
        /// </summary>
        public CalculatedMeasureService CalculatedMeasureService { get; set; }


        /// <summary>
        ///  Le TargetService
        /// </summary>
        public TargetService TargetService { get; set; }

        public PeriodNameService PeriodNameService { get; set; }



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


        public bool editCell(GrilleEditedElement element)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/edit", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(element);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool objects = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to edit cell.", e);
                throw new ServiceExecption("Unable to edit cell.", e);
            }
        }

        public bool deleteGridRows(List<int> oids)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/delete-rows", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(oids);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool objects = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to delete rows.", e);
                throw new ServiceExecption("Unable to delete rows.", e);
            }
        }

        public bool duplicateGridRows(List<int> oids)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/duplicate-rows", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(oids);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool objects = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to duplicate rows.", e);
                throw new ServiceExecption("Unable to duplicate rows.", e);
            }
        }


        public bool exportToExcel(GrilleFilter filter)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/export-to-excel", Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(filter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool objects = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to export.", e);
                throw new ServiceExecption("Unable to export.", e);
            }
        }

    }
}

