using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;
using WebSocketSharp;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain.Browser;

namespace Misp.Allocation.Clear
{
    public class ClearAllocationService : Service<Misp.Kernel.Domain.Persistent, Misp.Kernel.Domain.Browser.BrowserData>
    {

        /// <summary>
        /// RestClient
        /// </summary>
        public string ClearAllSocketResourcePath { get; set; }


        /// <summary>
        /// Clear allocation
        /// </summary>
        /// <param name="actionData"></param>
        public event ClearInfoEventHandler ClearAllocationTableHandler;
        public void ClearAllocationTable(TableActionData actionData)
        {

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = buildSocket(SocketResourcePath + "/clear/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (ClearAllocationTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => ClearAllocationTableHandler(runInfo));
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


        #region WebSocket Service Utils

        public AllocationRunInfo deserializeRunInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AllocationRunInfo runInfo = Serializer.Deserialize<AllocationRunInfo>(json);
                if (runInfo == null || (runInfo.runedCellCount == 0 && !runInfo.runEnded)) return null;
                return runInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize runInfo!", e);
            }
            return null;
        }

        #endregion



        public void CleaAllAllocation()
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = buildSocket(ClearAllSocketResourcePath);
            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError += (sender, e) => { logger.Debug("Socket error  : " + e.Message); };
            socket.OnClose += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (this.ClearAllocationTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => ClearAllocationTableHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            socket.Send("Clear all allocatins");
        }



        /// <summary>
        /// Demande au serveur d'exécuter le nettoyage de toutes les allocation.
        /// </summary>
        /// <returns></returns>
        public virtual AllocationRunInfo ClearAll()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/clear/clear_all", Method.POST);
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
                throw new BcephalException("Unable to clear all allocations", e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual AllocationRunInfo GetClearInfo()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/clear/clearinfos", Method.GET);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public List<InputTableBrowserData> getRunnedTableBrowserDatas()
        {
            try
            {
                var request1 = new RestRequest("/sourcing/table/run/table", Method.GET);
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
        public List<InputTableBrowserData> getRunedTableBrowserDatas(int groupOid)
        {
            try
            {
                var request1 = new RestRequest("/sourcing/table/run/table/" + groupOid, Method.GET);
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




    }
}
