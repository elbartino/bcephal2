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
    public class ReconciliationContextService : Service<ReconciliationContext, BrowserData>
    {
        #region Properties

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }


        /// <summary>
        /// Le ModelService.
        /// </summary>
        public MeasureService MeasureService { get; set; }
      
        #endregion
        
        #region methods acions

        /// <summary>
        /// ReconciliationContext
        /// </summary>
        /// <returns>return ReconciliationContext </returns>
        public ReconciliationContext getReconciliationContext()
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/context", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                ReconciliationContext context = RestSharp.SimpleJson.DeserializeObject<ReconciliationContext>(queryResult.Content);
                return context;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve context.", e);
                throw new ServiceExecption("Unable to retrieve context.", e);
            }
        }

        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override ReconciliationContext Save(ReconciliationContext rContext)
        {
            if (rContext != null)
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var request = new RestRequest(ResourcePath + "/save", Method.POST);

                    request.RequestFormat = DataFormat.Json;
                    serializer.MaxJsonLength = int.MaxValue;
                    string json = serializer.Serialize(rContext);
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                    var response = RestClient.ExecuteTaskAsync(request);
                    RestResponse queryResult = (RestResponse)response.Result;
                    bool valid = ValidateResponse(queryResult);

                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    Serializer.MaxJsonLength = int.MaxValue;
                    rContext = Serializer.Deserialize<ReconciliationContext>(queryResult.Content);
                    try
                    {
                        if (FileService != null) FileService.SaveCurrentFile();
                    }
                    catch (Exception) { }
                    return rContext;
                }
                catch (Exception e)
                {
                    logger.Error("Unable to save Item.", e);
                    throw new BcephalException("Unable to save Item.", e);
                }
            }
            else return rContext;
        }



        #endregion

    }
}

