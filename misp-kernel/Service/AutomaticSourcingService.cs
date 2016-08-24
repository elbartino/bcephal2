using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Misp.Kernel.Service
{
    public class AutomaticSourcingService : Service.Service<Kernel.Domain.AutomaticSourcing, Domain.Browser.BrowserData>
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

        /// <summary>
        /// L'InputTableService
        /// </summary>
        public Service.Service<InputTable,Kernel.Domain.Browser.InputTableBrowserData> InputTableService { get; set; }

        /// <summary>
        /// Le CalculatedMeasureService
        /// </summary>
        public CalculatedMeasureService CalculatedMeasureService { get; set; }


        /// <summary>
        ///  Le TargetService
        /// </summary>
        public TargetService TargetService { get; set; }

        public PeriodNameService PeriodNameService { get; set; }



        //public override List<BrowserData> getBrowserDatas()
        //{
        //    try
        //    {
        //        var request1 = new RestRequest(ResourcePath + "/browserdatas/table", Method.GET);
        //        RestResponse queryResult = (RestResponse)RestClient.Execute(request1);
        //        List<BrowserData> objects = RestSharp.SimpleJson.DeserializeObject<List<BrowserData>>(queryResult.Content);
        //        return objects;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error("Unable to retrieve list of BrowserData.", e);
        //        throw new ServiceExecption("Unable to retrieve list of BrowserData.", e);
        //    }
        //}

        public int runAutomaticSourcing(int oid,String inputTableFilePath)
        {
            
            try
            { 
                var request = new RestRequest(ResourcePath + "/run/"+oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddParameter("application/json", inputTableFilePath, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    int tableOid = RestSharp.SimpleJson.DeserializeObject<int>(queryResult.Content);
                    if (tableOid > 0)
                        return tableOid;
                }
                catch (Exception) 
                {
                    return -1;
                }
           }
            catch (Exception)
            {
                  return -1;
            }
            return -1;
        }

        public List<Object> runAutomaticSourcingCell(int oid, List<Object> test)
        {

            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                
                var request = new RestRequest(ResourcePath + "/runCell/" + oid, Method.POST);
                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(test);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Object> result = RestSharp.SimpleJson.DeserializeObject<List<Object>>(queryResult.Content);
                    if(result == null) return null;
                    return result;

                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public override AutomaticSourcing getByOid(int oid)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var request = new RestRequest(ResourcePath + "/" + oid, Method.GET);
            request.RequestFormat = DataFormat.Json;
            serializer.MaxJsonLength = int.MaxValue;
            var response = RestClient.ExecuteTaskAsync(request);
            RestResponse queryResult = (RestResponse)response.Result;
            bool valid = ValidateResponse(queryResult);
            System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Serializer.MaxJsonLength = int.MaxValue;
            return Serializer.Deserialize<AutomaticSourcing>(queryResult.Content);
 	    }

        public event SaveInfoEventHandler SaveTableHandler;
        public override AutomaticSourcing Save(AutomaticSourcing item)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = buildSocket(SocketResourcePath + "/Save/");
            socket.OnMessage += (sender, e) =>
            {
                Console.Out.WriteLine("Tes t " + e.Data);
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => SaveTableHandler(info, null));
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                AutomaticSourcing automaticSourcing = deserializeAutomaticSourcing(e.Data);
                if (automaticSourcing != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (SaveTableHandler != null) SaveTableHandler(null, automaticSourcing);
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

            //item = base.Save(item);            
            return item;
        }

        private AutomaticSourcing deserializeAutomaticSourcing(string text)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AutomaticSourcing automaticSourcing = Serializer.Deserialize<AutomaticSourcing>(text);
                
                return automaticSourcing;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize issue!", e);
            }
            return null;
        }
 

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

        public List<string> getListColumns(string excelFileName,bool firstRow) 
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            var request = new RestRequest(ResourcePath + "/columns/headers/"+ firstRow, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", excelFileName, ParameterType.RequestBody);
            RestResponse queryResult = (RestResponse)RestClient.Execute(request);
            try
            {
                List<string> result = RestSharp.SimpleJson.DeserializeObject<List<string>>(queryResult.Content);
                if (result == null) return new List<string>(0);
                return result;
            }
            catch (Exception)
            {
                return new List<string>(0);
            }
        }

        private AutomaticSourcingData deserializeAutomaticSourcingData(String text)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AutomaticSourcingData automaticSourcingdata = Serializer.Deserialize<AutomaticSourcingData>(text);
                if (automaticSourcingdata == null || automaticSourcingdata.automaticSourcingOid == 0) return null;
                return automaticSourcingdata;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize issue!", e);
            }
            return null;
        }


        public event OnbuildTableNameEventHandler buildTableNameEventHandler;
        public delegate void OnbuildTableNameEventHandler(Object tableNameIssue);

        public event OnUpdateUniverseEventHandler OnUpdateUniverse;
        public delegate void OnUpdateUniverseEventHandler(Object tableIssue, bool firstTime);

        public  void Run(AutomaticSourcingData  automaticData)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = buildSocket(SocketResourcePath + "/Run/");
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => SaveTableHandler(info, null));
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                AutomaticSourcingData issue = deserializeAutomaticSourcingData(e.Data);
                if (issue != null && !issue.isLast)
                {
                    if (SaveTableHandler != null && issue.tableName != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (buildTableNameEventHandler != null)
                            {
                                buildTableNameEventHandler(issue);
                                string json = serializer.Serialize(issue);
                                socket.Connect();
                                socket.Send(json);
                            }
                        }
                        );
                    }
                }
                

                TableSaveIssue TableIssue = deserializeTableIssue(e.Data);
                if (TableIssue != null)
                {
                    if (SaveTableHandler != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (OnUpdateUniverse != null)
                            {
                                OnUpdateUniverse(TableIssue,true);
                                if (TableIssue != null && TableIssue.targetItem != null) TableIssue.targetItem.attribute = null;
                                string json = serializer.Serialize(TableIssue);
                                socket.Connect();
                                socket.Send(json);
                           }
                        });
                    }
                }

                if (issue is Kernel.Domain.AutomaticSourcingData && issue.isLast)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => SaveTableHandler(null, issue));
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
            string text = serializer.Serialize(automaticData);
            socket.Send(text);
            
            //item = base.Save(item);            
            //return table.tableOid;
        }

              
        public TableSaveIssue deserializeTableIssue(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                TableSaveIssue issue = Serializer.Deserialize<TableSaveIssue>(json);
                if (issue == null || issue.decision == null) return null;
                return issue;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize issue!", e);
            }
            return null;
        }
    }
}
