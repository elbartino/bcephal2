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

        //test envoi de fichier. code a supprimer !!!
        public bool sendExcelFile()
        {
            Kernel.Service.FileDirs fileDirs = this.FileService.GetFileDirs();
            string filePath = fileDirs.InputTableDir + "testTransfert.xlsx";
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/sends", Method.POST);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                /*Re-Ecriture du fichier pour se rassurer que le byte est correct*/
                string filePath2 = fileDirs.InputTableDir + "Copy_testTransfert.xlsx";
                System.IO.File.WriteAllBytes(filePath2, fileBytes);

                /*Envoie du fichier au serveur*/
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(fileBytes);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool resp = serializer.Deserialize<bool>(queryResult.Content);
                return resp;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// get Role by oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public void getExcel()
        {
            try
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/getexcel", Method.GET);
                request.RequestFormat = DataFormat.Json;
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Object name = queryResult.Headers[0].Value;
                String fileName = name.ToString().Split("=".ToCharArray())[1];

                byte[] fileByte = new System.Text.UTF8Encoding(true).GetBytes(queryResult.Content);
                Kernel.Service.FileDirs fileDirs = this.FileService.GetFileDirs();
                string filePath = fileDirs.InputTableDir +fileName + ".xlsx";
                System.IO.File.WriteAllBytes(filePath, fileByte);
                //System.IO.File.WriteAllLines(filePath, fileByte);
            }
            catch (Exception e)
            {
            }
        }
        #endregion

    }
}
