using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Reporting.StructuredReport;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableService : StructuredReportService
    {
        /// <summary>
        /// Retoune la table sur base de son oid.
        /// </summary>
        /// <param name="oid"> L'identifiant de la table à retourner</param>
        /// <returns>La table</returns>
        public virtual Misp.Kernel.Domain.TransformationTable getByName(String name)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/get/" + name, Method.POST);
                request.DateFormat = "dd/MM/yyyy";
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Misp.Kernel.Domain.TransformationTable table = RestSharp.SimpleJson.DeserializeObject<Misp.Kernel.Domain.TransformationTable>(queryResult.Content);
                    return table;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return StructuredReport identified by: " + name, e);
            }
        }


        public override Kernel.Domain.StructuredReport Save(Kernel.Domain.StructuredReport item)
        {
            if (item == null) return item;
            try
            {

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save_transfo", Method.POST);

                request.RequestFormat = RestSharp.DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                Kernel.Domain.TransformationTable table = (Kernel.Domain.TransformationTable)item;
                string json = serializer.Serialize(table);
                request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                item = Serializer.Deserialize<Kernel.Domain.TransformationTable>(queryResult.Content);
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
