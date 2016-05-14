using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Office;
using RestSharp;

namespace Misp.Kernel.Service
{
    public class AuditService : Service<Kernel.Domain.AuditInfo, Misp.Kernel.Domain.Browser.BrowserData>
    {
        
        public Kernel.Domain.AuditInfo AuditCells(Range range, int oidTable)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/cells/" + oidTable, Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(range);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Kernel.Domain.AuditInfo auditInfo = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.AuditInfo>(queryResult.Content);
                    return auditInfo;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return auditInfo.", e);
            }
        }



        ///// <summary>
        ///// retourne la liste des items  infos pour la selection de  cellule audité 
        ///// </summary>
        ///// <returns>auditRuninfos</returns>
        //public Kernel.Domain.AuditInfo auditLevel2(String tableName, long cellOid)
        //{
        //    return auditLevel2(tableName, cellOid, 1);
        //}

        public Kernel.Domain.AuditInfo auditLevel2(String tableName, String cellName)
        {
            return auditLevel2(tableName, cellName, 1);
        }
                
        public Kernel.Domain.AuditInfo auditLevel2(String tableName, String cellName, int pageNumber)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/level2/" + tableName + "/" + cellName, Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(pageNumber);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {

                    Kernel.Domain.AuditInfo auditInfo = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.AuditInfo>(queryResult.Content);
                    return auditInfo;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return auditInfos.", e);
            }
        }

        /// <summary>
        /// retourne la page courante demandé pour l'auditinfo
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public Kernel.Domain.AuditInfo getPage(int pageNumber)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/page", Method.POST);
                request.RequestFormat = DataFormat.Json;
                string json = serializer.Serialize(pageNumber);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {

                    Kernel.Domain.AuditInfo auditInfo = RestSharp.SimpleJson.DeserializeObject<Kernel.Domain.AuditInfo>(queryResult.Content);
                    return auditInfo;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return page.", e);
            }
        }
        

    }
}

