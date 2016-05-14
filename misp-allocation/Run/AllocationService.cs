using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Misp.Allocation.Run
{
    public class AllocationService : Service<Misp.Kernel.Domain.Persistent, Misp.Kernel.Domain.Browser.BrowserData>
    {


        /// <summary>
        /// Run allocation
        /// </summary>
        /// <param name="actionData"></param>
        public event RunInfoEventHandler RunAllocationTableHandler;
        public void RunAllocationTable(TableActionData actionData)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(SocketResourcePath + "/run/all/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (RunAllocationTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunAllocationTableHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError += (sender, e) => { logger.Debug("Socket error  : " + e.Message); };
            socket.OnClose += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.Connect();
            string text = serializer.Serialize(actionData);
            socket.Send(text);
        }

        public Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
        }

        #region WebSocket Service Utils
                
        public AllocationRunInfo deserializeRunInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AllocationRunInfo runInfo = Serializer.Deserialize<AllocationRunInfo>(json);
                if (runInfo == null || (runInfo.runedCellCount == 0 && runInfo.currentInfo == null)) return null;
                return runInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize runInfo!", e);
            }
            return null;
        }

        #endregion

               
        /// <summary>
        /// Demande au serveur d'exécuter toutes les allocations.
        /// </summary>
        /// <returns></returns>
        public virtual AllocationRunInfo RunAll()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/run_all", Method.POST);
                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    AllocationRunInfo info = RestSharp.SimpleJson.DeserializeObject<AllocationRunInfo>(response.Result.Content);
                    return info;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to run all allocations", e);
            }
        }

        /// <summary>
        /// Demande au serveur d'exécuter toutes les allocations.
        /// </summary>
        /// <returns></returns>
        public virtual AllocationRunInfo RunTables(System.Collections.IList tables)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/run_tables", Method.POST);

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(tables);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    AllocationRunInfo info = RestSharp.SimpleJson.DeserializeObject<AllocationRunInfo>(response.Result.Content);
                    return info;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to run tables", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public List<InputTableBrowserData> getTableBrowserDatas()
        {
            try
            {
                var request1 = new RestRequest("/sourcing/table/notemplatebrowserdatas", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<InputTableBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<InputTableBrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                //logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public List<InputTableBrowserData> getTableBrowserDatas(int groupOid)
        {
            try
            {
                var request1 = new RestRequest("/sourcing/table/notemplatebrowserdatasbygroup/" + groupOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<InputTableBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<InputTableBrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual AllocationRunInfo GetRunInfo()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/runinfos", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    Misp.Kernel.Domain.AllocationRunInfo info = RestSharp.SimpleJson.DeserializeObject<Misp.Kernel.Domain.AllocationRunInfo>(queryResult.Content);
                    return info;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to Return AllocationRunInfo ", e);
            }
        }

    }
}
