using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using RestSharp;
using Misp.Sourcing.Table;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Domain.Browser;
using System.Threading;
using Misp.Kernel.Ui.Base;
using WebSocketSharp;

namespace Misp.Sourcing.Table
{
    public class InputTableService : Service<Misp.Kernel.Domain.InputTable, Misp.Kernel.Domain.Browser.InputTableBrowserData>
    {

        public PeriodNameService PeriodNameService { get; set; }

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
        /// Le TargetService.
        /// </summary>
        public TargetService TargetService { get; set; }


        /// <summary>
        /// Save the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public event SaveInfoEventHandler SaveTableHandler;
        public override Misp.Kernel.Domain.InputTable Save(Misp.Kernel.Domain.InputTable item)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = buildSocket(SocketResourcePath + "/Save/"); 
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                         { 
                             if (SaveTableHandler != null) SaveTableHandler(info, null); 
                         }
                        );
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                TableSaveIssue issue = deserializeIssue(e.Data);
                if (issue != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        ModelModificationDialog dialog = new ModelModificationDialog();
                        dialog.Display(this, issue);
                        dialog.ShowDialog();
                        issue = dialog.tableSaveIssue;
                        if (issue != null && issue.targetItem != null) issue.targetItem.attribute = null;
                        string json = serializer.Serialize(issue);
                        socket.Send(json);
                    });                    
                    return;
                }
                
                InputTable table = deserializeInputTable(e.Data);
                if (table != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (SaveTableHandler != null)
                                SaveTableHandler(null, table);
                        }
                        );
                    return;
                }

                logger.Debug("Recieve text : " + e.Data);
            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened!"); };
            socket.OnError += (sender, e) => { 
                logger.Debug("Socket error : " + e.Message); 
            };
            socket.OnClose += (sender, e) => { 
                logger.Debug("Socket closed : " + e.Reason); 
            };

            socket.Connect();
            socket.Send(item.name);

            //item = base.Save(item);            
            return item;
        }

        public override Misp.Kernel.Domain.InputTable SaveAs(string currentName, string newName)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = buildSocket(SocketResourcePath + "/SaveAs/");
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SaveTableHandler != null) SaveTableHandler(info, null);
                    }
                        );
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                TableSaveIssue issue = deserializeIssue(e.Data);
                if (issue != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        ModelModificationDialog dialog = new ModelModificationDialog();
                        dialog.Display(this, issue);
                        dialog.ShowDialog();
                        issue = dialog.tableSaveIssue;
                        string json = serializer.Serialize(issue);
                        socket.Send(json);
                    });
                    return;
                }

                InputTable table = deserializeInputTable(e.Data);
                if (table != null)
                {
                    if (SaveTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SaveTableHandler != null)
                            SaveTableHandler(null, table);
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
            string text = serializer.Serialize(new String[] { currentName, newName });
            socket.Send(text);         
            return null;
        }

        public Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.INPUT_TABLE;

        }

        public event SaveInfoEventHandler LoadMultipleTableHandler;
        public void LoadMultipleTable(MultiTableLoadData data)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = buildSocket(SocketResourcePath + "/multiple_load/");            
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (LoadMultipleTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => LoadMultipleTableHandler(info, null));
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return; 
                }

                TableSaveIssue issue = deserializeIssue(e.Data);
                if (issue != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        ModelModificationDialog dialog = new ModelModificationDialog();
                        dialog.Display(this, issue);
                        dialog.ShowDialog();
                        issue = dialog.tableSaveIssue;
                        string json = serializer.Serialize(issue);
                        socket.Send(json);
                    });
                    return;
                }
                logger.Debug("Recieve unreconized message : " + e.Data);
            };

            socket.OnOpen   += (sender, e) => { logger.Debug("Socket opened!"); };
            socket.OnError  += (sender, e) => { 
                logger.Debug("Socket error : " + e.Message); 
            };
            socket.OnClose  += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public TableSaveIssue deserializeIssue(String json)
        {
            try {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                TableSaveIssue issue = Serializer.Deserialize<TableSaveIssue>(json);
                if (issue == null || issue.decision == null) return null;
			    return issue;
		    } catch (Exception e) {
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

        public InputTable deserializeInputTable(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                InputTable table = Serializer.Deserialize<InputTable>(json);
                if (table == null || table.oid == null || !table.oid.HasValue) return null;
                return table;
            }
            catch (Exception e)
            {
                logger.Debug("Fail to deserialize InputTable!", e);
            }
            return null;
        }

        public AllocationRunInfo deserializeRunInfo(String json)
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
        /// Demande au serveur d'exécuter l'allocation de toutes les cellules de la table..
        /// </summary>
        /// <param name="oid"> L'identifiant de la table</param>
        /// <returns>La table</returns>
        public virtual AllocationRunInfo RunAll(int oid)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/runall/" + oid, Method.POST);
                request.DateFormat = "dd/MM/yyyy";
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
                throw new BcephalException("Unable to Return Input table identified by: " + oid, e);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual AllocationRunInfo GetRunInfo(long page)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/runinfos/" + page, Method.GET);
                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
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
        
        public void createTable(Kernel.Domain.InputTable table)
        {
            try
            {
                if (table == null) return;
                var request = new RestRequest(ResourcePath + "/create/table", Method.POST);
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;

                string json = serializer.Serialize(table);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;

                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                Serializer.MaxJsonLength = int.MaxValue;
                bool result = Serializer.Deserialize<bool>(queryResult.Content);
                return;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to parametrize cell ", e);
            }
        }
        
        /// <summary>
        /// For table parametrization
        /// </summary>
        /// <param name="tableName">Table that contains the cellproperty</param>
        /// <param name="range">the cellproperties range</param>
        /// <param name="cellparams">the cell params {Measure, Target,Tag,Period}</param>
        /// <returns>true=> cells well parametrized ; false => no parametrization done.</returns>
        public InputTable parametrizeTable(Kernel.Domain.Parameter parameter)
        {
            try
            {
                if (parameter == null) return null;
                var request = new RestRequest(ResourcePath + "/parametrize/table/", Method.POST);
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                request.RequestFormat = DataFormat.Json;
                serializer.MaxJsonLength = int.MaxValue;

                string json = serializer.Serialize(parameter);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;

                bool valid = ValidateResponse(queryResult);
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                Serializer.MaxJsonLength = int.MaxValue;
                InputTable result = Serializer.Deserialize<InputTable>(queryResult.Content);
                return result;
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to parametrize cell ", e);
            }
        }
        
        public virtual bool closeTable(String name)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/close/table/" + name, Method.POST);
                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    bool result = RestSharp.SimpleJson.DeserializeObject<bool>(response.Result.Content);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to close table : " + name, e);
            }
        }

        public virtual bool closeAllTables()
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/closeall", Method.POST);
                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    bool result = RestSharp.SimpleJson.DeserializeObject<bool>(response.Result.Content);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to close all tables", e);
            }
        }

        public virtual bool renameTable(String newName,InputTable table)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/rename/table/" + newName, Method.POST);

                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;

                string json = serializer.Serialize(table);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    bool result = RestSharp.SimpleJson.DeserializeObject<bool>(response.Result.Content);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to close table : " + newName, e);
            }
        }

        public virtual bool buildCellProperty(String tableName, GroupProperty groupProperty)
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/build/cell/" + tableName, Method.POST);

                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;

                string json = serializer.Serialize(groupProperty);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    bool result = RestSharp.SimpleJson.DeserializeObject<bool>(response.Result.Content);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to close table : " + tableName, e);
            }
        }

        public CellProperty getActiveCell(String tableName, GroupProperty group, int row, int col, String sheetName) 
        {
            try
            {
                var request = new RestRequest(ResourcePath + "/cell/active/" + tableName + "/" + row + "/" + col + "/" + sheetName, Method.POST);
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(group);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    CellProperty result = RestSharp.SimpleJson.DeserializeObject<CellProperty>(response.Result.Content);
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to close table : " + tableName, e);
            }
        }

        public override List<Misp.Kernel.Domain.CellProperty> getCellsValues(String tableName)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/getcellvalues/" + tableName, Method.POST);
                //request.RequestFormat = DataFormat.Json;
               // request.AddParameter("application/json", inputTableFilePath, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    List<Misp.Kernel.Domain.CellProperty> result = RestSharp.SimpleJson.DeserializeObject<List<Misp.Kernel.Domain.CellProperty>>(queryResult.Content);
                    if (result == null) return null;
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
        
        public bool applyDesign(int designOid, bool forReport, Kernel.Ui.Office.Range currentRange) 
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/applydesign/" + designOid + "/" + forReport, Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(currentRange);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request);
                try
                {
                    bool result = RestSharp.SimpleJson.DeserializeObject<bool>(queryResult.Content);
                    
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
             

        public event RunInfoEventHandler RunAllocationTableHandler;
        public void RunAllocationTable(TableActionData actionData)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = buildSocket(SocketResourcePath + "/run/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null) 
                {
                    if (RunAllocationTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                           {
                               if(RunAllocationTableHandler != null) RunAllocationTableHandler(runInfo);
                           }
                        );
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            
            };

            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError+= (sender,e) => {logger.Debug("Socket error  : "+ e.Message);};
            socket.OnClose += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.Connect();
            string text = serializer.Serialize(actionData);
            socket.Send(text);
        }

        public event ClearInfoEventHandler ClearAllocationTableHandler;
        public void ClearAllocationTable(TableActionData actionData)
        {

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = buildSocket(SocketResourcePath + "/clear/");
            socket.OnOpen += (sender, e) => { logger.Debug("Socket opened"); };
            socket.OnError += (sender, e) => { logger.Debug("Socket error  : " + e.Message); };
            socket.OnClose += (sender, e) => { logger.Debug("Socket closed : " + e.Reason); };

            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (ClearAllocationTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                if(ClearAllocationTableHandler != null)  ClearAllocationTableHandler(runInfo);
                            }
                        );
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            string text = serializer.Serialize(actionData);
            socket.Send(text);
        }

        public virtual bool changeTableExcelFile(string tableName, string newExcelFile)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var request = new RestRequest(ResourcePath + "/changeexcelFile/table/" + tableName, Method.POST);
                serializer.MaxJsonLength = int.MaxValue;
                string json = serializer.Serialize(newExcelFile);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = RestClient.ExecuteTaskAsync(request);
                try
                {
                    bool result = RestSharp.SimpleJson.DeserializeObject<bool>(response.Result.Content);
                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new BcephalException("Unable to close table : " + newExcelFile, e);
            }
        }

        public  String getNewTableName(string name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/table-new-name/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                String value = Serializer.Deserialize<String>(queryResult.Content);
                return value;
            }
            catch (Exception e)
            {
                logger.Error("Unable to retrieve object from server.", e);
                throw new ServiceExecption("Unable to retrieve object from server.", e);
            }
        }

        public InputTable getTableByName(string name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/tablename/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                InputTableBrowserData value = Serializer.Deserialize<InputTableBrowserData>(queryResult.Content);
                if (value == null) return null;
                return new InputTable()
                {
                    creationDate = value.creationDate,
                    modificationDate = value.modificationDate,
                    name = value.name,
                    oid = value.oid,
                    visibleInShortcut = value.visibleInShortcut,
                    group = new BGroup(value.group, "")
                };
            }
            catch (Exception)
            {
                return null;
            }   
        }

        public string buildExcelFileName(string name)
        {
            try
            {
                var request1 = new RestRequest(ResourcePath + "/build-excel-name/" + name, Method.GET);
                RestResponse queryResult = (RestResponse)RestClient.Execute(request1);

                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                String value = Serializer.Deserialize<String>(queryResult.Content);
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
