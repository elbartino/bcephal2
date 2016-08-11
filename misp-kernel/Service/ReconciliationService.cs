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
    public class ReconciliationService : Service<Misp.Kernel.Domain.ReconciliationTemplate, Misp.Kernel.Domain.Browser.BrowserData>
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

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public PostingService postingService { get; set; }
        #endregion


        #region Contructor

        public ReconciliationService()
        {
            
        }

        #endregion

        #region methods acions

        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override ReconciliationTemplate Save(ReconciliationTemplate rTemplate)
        {
            //getExcel();
            //sendExcelFile();

            if (rTemplate != null)
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var request = new RestRequest(ResourcePath + "/save", Method.POST);

                    request.RequestFormat = DataFormat.Json;
                    serializer.MaxJsonLength = int.MaxValue;
                    string json = serializer.Serialize(rTemplate);
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                    var response = RestClient.ExecuteTaskAsync(request);
                    RestResponse queryResult = (RestResponse)response.Result;
                    bool valid = ValidateResponse(queryResult);

                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    Serializer.MaxJsonLength = int.MaxValue;
                    rTemplate = Serializer.Deserialize<ReconciliationTemplate>(queryResult.Content);
                    try
                    {
                        if (FileService != null) FileService.SaveCurrentFile();
                    }
                    catch (Exception) { }
                    return rTemplate;
                }
                catch (Exception e)
                {
                    logger.Error("Unable to save Item.", e);
                    throw new BcephalException("Unable to save Item.", e);
                }
            }
            else return rTemplate;
        }

        


        
        #endregion

    }
}
