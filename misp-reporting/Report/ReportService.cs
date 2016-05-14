using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Sourcing.Table;
using RestSharp;
using System.Web.Script.Serialization;

namespace Misp.Reporting.Report
{
    public class ReportService : InputTableService
    {

        /// <summary>
        /// Le CalculatedMeasureService.
        /// </summary>
        public CalculatedMeasureService calculatedMeasureService { get; set; }


        public override InputTable getByOid(int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                Kernel.Domain.Report value = Serializer.Deserialize<Kernel.Domain.Report>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve Report from server.", e);
                throw new ServiceExecption("Unable to retrieve Report from server.", e);
            }   
        }

        public Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.REPORT;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public void exportBudget(string filepath)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/export_budget", Method.POST);
                request.AddParameter("application/json", filepath, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                
            }
            catch (Exception e)
            {
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }

        public List<Kernel.Domain.Browser.InputTableBrowserData> getAllBrowserDatas()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatas/all", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<Kernel.Domain.Browser.InputTableBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<Kernel.Domain.Browser.InputTableBrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

    }
}
