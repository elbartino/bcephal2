using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Misp.Kernel.Service
{ 
    public class AutomaticTargetService : AutomaticSourcingService
    {


      /*  public override List<BrowserData> getBrowserDatas()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatas/target", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<BrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<BrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }*/

        public int runAutomaticTarget(int oid, String inputTableFilePath)
        {

            try
            {
                var request = new RestRequest(ResourcePath + "/run/target/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json ;
                request.AddParameter("application/json", inputTableFilePath, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    int tableOid = RestSharp.SimpleJson.DeserializeObject<int>(queryResult.Content);
                    if (tableOid > 0)
                        return tableOid;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return -1;
        }

        public List<Object> runAutomaticTargetCell(int oid, List<Object> test)
        {

            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                var request = new RestRequest(ResourcePath + "/runCell/target/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(test);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Object> result = RestSharp.SimpleJson.DeserializeObject<List<Object>>(queryResult.Content);
                    if (result == null) return null;
                    return result;

                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
