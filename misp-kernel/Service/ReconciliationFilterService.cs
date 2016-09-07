using Misp.Kernel.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Service
{
    public class ReconciliationFilterService : PostingGridService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public override Grille getNewReconciliationGrid(String name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/new/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                ReconciliationFilter value = Serializer.Deserialize<ReconciliationFilter>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }

        public override Grille getByOid(int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                Kernel.Domain.ReconciliationFilter value = Serializer.Deserialize<Kernel.Domain.ReconciliationFilter>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve ReconciliationFilter from server.", e);
                throw new ServiceExecption("Unable to retrieve ReconciliationFilter from server.", e);
            }
        }


        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override Grille Save(Grille item)
        {
            if (item == null) return item;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save-filter", Method.POST);

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(item);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                item = Serializer.Deserialize<ReconciliationFilter>(queryResult.Content);
                try
                {
                    if (FileService != null) FileService.SaveCurrentFile();
                }
                catch (Exception) { }
                return item;
            }
            catch (Exception e)
            {
                logger.Error("Unable to save Item.", e);
                throw new BcephalException("Unable to save Item.", e);
            }
        }


    }
}
