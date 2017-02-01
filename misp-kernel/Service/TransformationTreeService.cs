using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace Misp.Kernel.Service
{
    public class TransformationTreeService : Service<Kernel.Domain.TransformationTree, Misp.Kernel.Domain.Browser.BrowserData>
    {
        /// <summary>
        /// Le MeasureService.
        /// </summary>
        public MeasureService MeasureService { get; set; }

        /// <summary>
        /// Le ModelService.
        /// </summary>
        public ModelService ModelService { get; set; }

        /// <summary>
        /// Le PeriodicityService.
        /// </summary>
        public PeriodicityService PeriodicityService { get; set; }
        
        /// <summary>
        /// Le DesignService.
        /// </summary>
        public DesignService DesignService { get; set; }

        /// <summary>
        /// Le AuditService.
        /// </summary>
        public AuditService AuditService { get; set; }

        public PeriodNameService PeriodNameService { get; set; }

        /// <summary>
        /// L'InputTableService
        /// </summary>
        public Service.Service<InputTable, Kernel.Domain.Browser.InputTableBrowserData> InputTableService { get; set; }



        /// <summary>
        /// Le CalculatedMeasureService
        /// </summary>
        public CalculatedMeasureService CalculatedMeasureService { get; set; }


        /// <summary>
        ///  Le TargetService
        /// </summary>
        public TargetService TargetService { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public virtual bool locked(int fileOid, int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/locked/" + fileOid + "/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                bool value = Serializer.Deserialize<bool>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public virtual bool unlocked(int fileOid, int oid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/unlocked/" + fileOid + "/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                bool value = Serializer.Deserialize<bool>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid">Oid of the object to return.</param>
        /// <returns>Object such that object.oid == oid.</returns>
        public virtual bool unlockedAll(int fileOid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/unlocked-all/" + fileOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                bool value = Serializer.Deserialize<bool>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }


        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual TransformationTreeItem SaveTransformationTreeItem(TransformationTreeItem item)
        {
            if (item == null) return item;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/save/item", Method.POST);

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(item);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                bool valid = ValidateResponse(queryResult);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                item = Serializer.Deserialize<TransformationTreeItem>(queryResult.Content);
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

        public bool usedByCombinedTree(System.Collections.IList items)
        {
            bool resutl = true;
            foreach (object item in items)
            {
                if (item is Persistent) return usedByCombinedTree(((Persistent)item).oid.Value);
                if (item is BrowserData) return usedByCombinedTree(((BrowserData)item).oid);
            }
            return resutl;
        }

        public bool usedByCombinedTree(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/usedByCombinedTree/" + oid, Method.GET);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                ValidateResponse(queryResult);
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                bool result = Serializer.Deserialize<bool>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Unable to delete object.", e);
                return false;
            }
        }
      

        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public event TransformationTreeRunInfoEventHandler RunHandler;
        public event PowerpointLoadInfoEventHandler PowerpointHandler;
        Socket runSocket;
        public void Run(List<int> stringOids,bool isRunAllMode = false)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string socketHeader = "/run/" + (isRunAllMode ? "all/" : "");
            Socket socket =buildSocket(SocketResourcePath +socketHeader);
            socket.OnMessage += (sender, e) =>
            {
                PowerpointLoadInfo pptLoadInfo = deserializePowerpointLoadInfo(e.Data);
                if (pptLoadInfo != null && pptLoadInfo.action != null)
                {
                    if (PowerpointHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => PowerpointHandler(pptLoadInfo));                    
                    return;
                }


                LoopUserDialogTemplateData LoopTemplate = deserializeLoopTemplateData(e.Data);
                if (LoopTemplate != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessPopup dialog = new ProcessPopup();
                        dialog.Display(LoopTemplate);
                        dialog.ShowDialog();
                        LoopTemplate = dialog.LoopUserTemplateData;
                        string json = serializer.Serialize(LoopTemplate);
                        socket.Send(json);
                    });
                    return;
                }

                TransformationTreeRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (RunHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunHandler(runInfo));
                    if (runInfo.runEnded)
                    {
                        socket.Close(CloseStatusCode.Normal);
                        runSocket = null;
                    }
                    return;
                }

            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError += (sender, e) => { logger.Debug("Socket error  : " + e.Message); };
            socket.OnClose += (sender, e) => { 
                logger.Debug("Socket closed : " + e.Reason);
                runSocket = null;
            };

            socket.Connect();
            runSocket = socket;
            string text = serializer.Serialize(stringOids);
            socket.Send(text);
        }

        public LoopUserDialogTemplateData deserializeLoopTemplateData(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                LoopUserDialogTemplateData template = Serializer.Deserialize<LoopUserDialogTemplateData>(json);
                if (template == null || template.values.Count == 0) return null;
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize issue!", e);
            }
            return null;
        }

        public void StopRun()
        {
            if (runSocket != null) runSocket.Send("STOP");
        }

        public SaveInfoEventHandler SaveTransformationTreeHandler;
       /* public override Misp.Kernel.Domain.TransformationTree Save(Misp.Kernel.Domain.TransformationTree item)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket =buildSocket(SocketResourcePath + "/Save/");
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SaveTransformationTreeHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SaveTransformationTreeHandler != null) SaveTransformationTreeHandler(info, null);
                    }
                        );
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                TransformationTree presentation = deserializeTransformationTree(e.Data);
                if (presentation != null)
                {
                    if (SaveTransformationTreeHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SaveTransformationTreeHandler != null)
                            SaveTransformationTreeHandler(null, presentation);
                    }
                        );
                    return;
                }

                logger.Debug("Recieve text : " + e.Data);
            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened!"); };
            socket.OnError += (sender, e) =>
            {
                logger.Debug("Socket error : " + e.Message);
            };
            socket.OnClose += (sender, e) =>
            {
                logger.Debug("Socket closed : " + e.Reason);
            };

            socket.Connect();
            string text = serializer.Serialize(item);
            socket.Send(text);
            return item;
        }*/


        public SaveInfo deserializeSaveInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                SaveInfo saveInfo = Serializer.Deserialize<SaveInfo>(json);
                if (saveInfo == null || saveInfo.stepCount < 1) return null;
                return saveInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize saveInfo!", e);
            }
            return null;
        }

        public TransformationTree deserializeTransformationTree(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                TransformationTree transformationTree = Serializer.Deserialize<TransformationTree>(json);
                if (transformationTree == null || transformationTree.oid == null || !transformationTree.oid.HasValue) return null;
                return transformationTree;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize InputTable!", e);
            }
            return null;
        }

        public event ClearInfoEventHandler ClearTreeHandler;
        public void ClearTree(TableActionData actionData)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = buildSocket(SocketResourcePath + "/clear/");
            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError += (sender, e) => { logger.Debug("Socket error  : " + e.Message); };
            socket.OnClose += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeAllocationRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (ClearTreeHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => ClearTreeHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            string text = serializer.Serialize(actionData);
            socket.Send(text);
        }


        public TransformationTreeRunInfo deserializeRunInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                TransformationTreeRunInfo runInfo = Serializer.Deserialize<TransformationTreeRunInfo>(json);
                return runInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize runInfo!", e);
            }
            return null;
        }

        public PowerpointLoadInfo deserializePowerpointLoadInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                PowerpointLoadInfo info = Serializer.Deserialize<PowerpointLoadInfo>(json);
                return info;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize PowerpointLoadInfo!", e);
            }
            return null;
        }

        public AllocationRunInfo deserializeAllocationRunInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AllocationRunInfo runInfo = Serializer.Deserialize<AllocationRunInfo>(json);
                //if (runInfo == null || runInfo.runedCellCount == 0) return null;
                return runInfo;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize runInfo!", e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Instructions to fill condition panel</returns>
        public Instruction getInstructionObject(string instructionText)
        {
            try
            {
                 var request = new RestRequest(ResourcePath + "/instruction/object/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", instructionText, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                Instruction Instruction = RestSharp.SimpleJson.DeserializeObject<Instruction>(queryResult.Content);
                return Instruction;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve conditions.", e);
                throw new ServiceExecption("Unable to retrieve conditions.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Instructions to fill condition panel</returns>
        public string getInstructionString(Instruction instructionObject)
        {
            if (instructionObject == null) return null;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/instruction/string/", Method.POST);
                string json = serializer.Serialize(instructionObject);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                string Instruction = RestSharp.SimpleJson.DeserializeObject<string>(queryResult.Content);
                return Instruction;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve conditions.", e);
                throw new ServiceExecption("Unable to retrieve conditions.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of BrowserData</returns>
        public List<TransformationTreeBrowserData> getTransformationTreeBrowserDatas()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatas", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<TransformationTreeBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<TransformationTreeBrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                //logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }


        public List<TransformationTreeBrowserData> getRunnedTransformationTreeBrowserDatas()
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatas/runtree", Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<TransformationTreeBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<TransformationTreeBrowserData>>(queryResult.Content);
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
        public List<TransformationTreeBrowserData> getTransformationTreeBrowserDatas(int groupOid)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/browserdatasbygroup/" + groupOid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                List<TransformationTreeBrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<TransformationTreeBrowserData>>(queryResult.Content);
                return objects;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve list of BrowserData.", e);
                throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
            }
        }


        public TransformationTreeItem copyAction(String newName,int treeOid,TransformationTreeItem actionItem)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/copy/action/" + newName + "/" + treeOid, Method.POST);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string json = serializer.Serialize(actionItem);
                request1.AddParameter("application/json", json, ParameterType.RequestBody);
                
                TransformationTreeItem action = RestSharp.SimpleJson.DeserializeObject<TransformationTreeItem>(queryResult.Content);
                return action;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve action.", e);
                throw new ServiceExecption("Unable to retrieve action.", e);
            }
        }

        public TransformationTreeItem getItemByOid(int? oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/item/" + oid, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);

                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                TransformationTreeItem value = Serializer.Deserialize<TransformationTreeItem>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }   
        }
    }
}
