using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class LinkedAttributeGridService : Service<LinkedAttributeGrid, Misp.Kernel.Domain.Browser.BrowserData>
    {

        public InputGridService InputGridService { get; set; }

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


        public GrilleEditedResult editCell(GrilleEditedElement element)
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
                GrilleEditedResult objects = RestSharp.SimpleJson.DeserializeObject<GrilleEditedResult>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to edit cell.", e);
                throw new ServiceExecption("Unable to edit cell.", e);
            }
        }

        public bool deleteGridRows(List<long> oids)
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


    }
}
