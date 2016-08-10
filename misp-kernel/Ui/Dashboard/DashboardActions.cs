using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Util;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using WebSocketSharp;

namespace Misp.Kernel.Ui.Dashboard
{
    public class DashboardActions
    {

        public DashboardBlock BlockToUpdate { get; set; }

        BusyAction action;
        public void Delete(string path, List<int> oids, DashboardBlock block)
        {
            if (oids == null || oids.Count == 0) return;
            int count = oids.Count;
            string message = "You are about to delete " + count + " items.\nDo you want to continue?";
            if (count == 1) message = "You are about to delete " + count + " item.\nDo you want to continue?";
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete", message);
            if (result == MessageBoxResult.Yes)
            {
                action = new BusyAction(false)
                {
                    DoWork = () =>
                    {
                        try
                        {
                            action.ReportProgress(0, message);
                            bool resutl = true;
                            foreach (int oid in oids) if (!Delete(path, oid)) resutl = false;
                            if (!resutl) Kernel.Util.MessageDisplayer.DisplayError("Delete", "Delete fail!");
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { block.RefreshData(); if (this.BlockToUpdate != null) this.BlockToUpdate.RefreshData(); }));
                            action.ReportProgress(100, message);
                        }
                        catch (BcephalException e)
                        {
                            MessageDisplayer.DisplayError("Error", e.Message);
                            action = null;
                            return OperationState.STOP;
                        }
                        return OperationState.CONTINUE;
                    }
                };
                action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationManager.Instance.MainWindow.OnBusyPropertyChanged);
                action.Run();
            }
        }

        protected bool Delete(string path, int oid)
        {
            try
            {
                var request = new RestRequest(path + "/delete/" + oid, Method.DELETE);
                var response = ApplicationManager.Instance.RestClient.ExecuteTaskAsync(request);
                RestResponse queryResult = (RestResponse)response.Result;
                return DashBoardService.ValidateResponse(queryResult);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Model Actions

        public void DeleteModels(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.MODEL_RESOURCE_PATH, oids, block);
        }  
        
        #endregion 


        #region Input Table Actions

        public void DeleteTables(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.INPUT_TABLE_RESOURCE_PATH, oids, block);
        }

        public event RunInfoEventHandler RunTablesHandler;  
        public void RunTables(List<int> oids)
        {
            TableActionData data = new TableActionData(oids);
            Mask(true, "Tables loading...");
            RunTablesHandler += updateRunTablesProgress;

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(ResourcePath.SOCKET_ALLOCATION_RESOURCE_PATH + "/run/all/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (RunTablesHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunTablesHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };
            
            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public event ClearInfoEventHandler ClearTablesHandler;
        public void ClearTables(List<int> oids) 
        {
            TableActionData data = new TableActionData(oids);
            Mask(true, "Clearing tables...");
            ClearTablesHandler += updateClearTablesProgress;

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(ResourcePath.SOCKET_ALLOCATION_RESOURCE_PATH + "/clear/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (ClearTablesHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => ClearTablesHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };
            
            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public void ClearAndRunTables(List<int> oids) 
        {
            
        }


        private void updateClearTablesProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                ClearTablesHandler -= updateClearTablesProgress;
                //this.ApplicationManager.AllocationCount = this.Service.FileService.GetAllocationCount();
                Kernel.Util.MessageDisplayer.DisplayInfo("Clear Tables", "Clear tables ended!");
                Mask(false);
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = runInfo.totalCellCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = runInfo.runedCellCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "Tables clearing: " + rate + " %"
                    + " (" + runInfo.runedCellCount + "/" + runInfo.totalCellCount + ")";
            }
        }

        private void updateRunTablesProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                //ApplicationManager.Instance.AllocationCount = this.Service.FileService.GetAllocationCount();
                RunTablesHandler -= updateRunTablesProgress;
                Kernel.Util.MessageDisplayer.DisplayInfo("Load Tables", "Load Tables ended!");
                Mask(false);
                ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Hidden;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = 0;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = "";
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = runInfo.totalCellCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = runInfo.runedCellCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "Tables loading: " + rate + " %"
                    + " (" + runInfo.runedCellCount + "/" + runInfo.totalCellCount + ")";

                if (runInfo.currentInfo != null)
                {
                    rate = runInfo.currentInfo.totalCellCount != 0 ? (Int32)(runInfo.currentInfo.runedCellCount * 100 / runInfo.currentInfo.totalCellCount) : 0;
                    if (rate > 100) rate = 100;

                    if (runInfo.currentInfo.runedCellCount != 0)
                    {
                        ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Visible;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = runInfo.currentInfo.totalCellCount;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = runInfo.currentInfo.runedCellCount;
                        ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = runInfo.currentInfo.tableName + ": " + rate + " %"
                            + " (" + runInfo.currentInfo.runedCellCount + "/" + runInfo.currentInfo.totalCellCount + ")";
                    }
                }

            }
        }

        public AllocationRunInfo deserializeRunInfo(String json, bool check = true)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                AllocationRunInfo runInfo = Serializer.Deserialize<AllocationRunInfo>(json);
                if (check && (runInfo == null || (runInfo.runedCellCount == 0 && runInfo.currentInfo == null))) return null;
                return runInfo;
            }
            catch (Exception e)
            {

            }
            return null;
        }

        #endregion 
       

        #region Report Actions

        public void DeleteReports(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.REPORT_RESOURCE_PATH, oids, block);
        }

        public void RunReports(List<int> oids)
        {
            TableActionData data = new TableActionData(oids);
            //data.writeInExcel = true;
            Mask(true, "Running Report...");
            RunTablesHandler += updateRunReportsProgress;
            
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(ResourcePath.SOCKET_REPORT_RESOURCE_PATH + "/run/all/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (RunTablesHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunTablesHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        private void updateRunReportsProgress(AllocationRunInfo runInfo)
        {
            if (runInfo != null && runInfo.currentInfo != null)
            {
                foreach (CellAllocationRunInfoBrowserData data in runInfo.currentInfo.infos)
                {
                    if (string.IsNullOrWhiteSpace(data.excelFile)) continue;
                    ExcelLoader loader = null;
                    if (!ExcelLoader.Loaders.ContainsKey(data.table))
                    {
                        loader = new ExcelLoader(data.excelFile);
                        ExcelLoader.Loaders.Add(data.table, loader);
                    }
                    ExcelLoader.Loaders.TryGetValue(data.table, out loader);
                    if (loader == null) continue;
                    loader.setValue(data.sheet, data.row, data.column, data.loadedAmount);
                }
            }

            if (runInfo == null || runInfo.runEnded == true)
            {
                RunTablesHandler -= updateRunReportsProgress;
                foreach (ExcelLoader loader in ExcelLoader.Loaders.Values) loader.saveAndClose();
                ExcelLoader.Loaders.Clear();
                Kernel.Util.MessageDisplayer.DisplayInfo("Run Reports", "Run Reports ended!");
                Mask(false);
                ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Hidden;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = 0;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = "";                
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = runInfo.totalCellCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = runInfo.runedCellCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "Running Report: " + rate + " %"
                    + " (" + runInfo.runedCellCount + "/" + runInfo.totalCellCount + ")";

                if (runInfo.currentInfo != null)
                {
                    rate = runInfo.currentInfo.totalCellCount != 0 ? (Int32)(runInfo.currentInfo.runedCellCount * 100 / runInfo.currentInfo.totalCellCount) : 0;
                    if (rate > 100) rate = 100;

                    if (runInfo.currentInfo.runedCellCount != 0)
                    {
                        ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Visible;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = runInfo.currentInfo.totalCellCount;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = runInfo.currentInfo.runedCellCount;
                        ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = runInfo.currentInfo.tableName + ": " + rate + " %"
                            + " (" + runInfo.currentInfo.runedCellCount + "/" + runInfo.currentInfo.totalCellCount + ")";
                    }
                }

            }
        }

        #endregion


        #region Design Actions

        public void DeleteDesigns(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.DESIGN_RESOURCE_PATH, oids, block);
        }

        #endregion


        #region Automatic Upload Actions

        public void DeleteAutomaticUploads(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.AUTOMATIC_SOURCING_RESOURCE_PATH, oids, block);
        }

        public event SaveInfoEventHandler SaveAutoSourcTableHandler;
        public event OnbuildTableNameEventHandler buildTableNameEventHandler;
        public delegate void OnbuildTableNameEventHandler(Object tableNameIssue);

        public event OnUpdateUniverseEventHandler UpdateUniverse;
        public delegate void OnUpdateUniverseEventHandler(Object tableIssue, bool firstTime);
        public void RunAutomaticUploads(List<int> oids)
        {
            String filePath = openSaveDialog();
            if (string.IsNullOrEmpty(filePath)) return;
            String name = System.IO.Path.GetFileNameWithoutExtension(filePath);

            SaveAutoSourcTableHandler += UpdateAutoSourcSaveInfo;
            UpdateUniverse += OnUpdateUniverse;
            buildTableNameEventHandler += OnBuildTableName;
            Mask(true, "Running ...");
            AutomaticSourcingData data = new AutomaticSourcingData(oids[oids.Count - 1], name, filePath);
           
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;

            Socket socket = new Socket(ResourcePath.SOCKET_AUTOMATIC_SOURCING_RESOURCE_PATH + "/Run/");
            socket.OnMessage += (sender, e) =>
            {
                SaveInfo info = deserializeSaveInfo(e.Data);
                if (info != null)
                {
                    if (SaveAutoSourcTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => SaveAutoSourcTableHandler(info, null));
                    if (info.isEnd) socket.Close(CloseStatusCode.Normal);
                    return;
                }

                AutomaticSourcingData issue = deserializeAutomaticSourcingData(e.Data);
                if (issue != null && !issue.isLast)
                {
                    if (SaveAutoSourcTableHandler != null && issue.tableName != null)
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
                    if (SaveAutoSourcTableHandler != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (UpdateUniverse != null)
                            {
                                UpdateUniverse(TableIssue, true);
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
                    if (SaveAutoSourcTableHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => SaveAutoSourcTableHandler(null, issue));
                    return;
                }
            };
                        
            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);

        }

        private void OnUpdateUniverse(object tableIssue, bool showDialog)
        {
            if (showDialog)
            {
                //ModelModificationDialog dialog = new ModelModificationDialog();
                //dialog.Display((Kernel.Domain.TableSaveIssue)tableIssue);
                //dialog.ShowDialog();
                //((Kernel.Domain.TableSaveIssue)tableIssue).applyToAll = dialog.tableSaveIssue.applyToAll;
                //((Kernel.Domain.TableSaveIssue)tableIssue).decision = dialog.tableSaveIssue.decision;
                //showDialog = false;
            }
        }

        private void OnBuildTableName(object tableissue)
        {
            //AutomaticSourcingTableDialog AutomaticSourcingTableDialog = new AutomaticSourcingTableDialog();
            //AutomaticSourcingTableDialog.SetInputTableName(((AutomaticSourcingData)tableissue).tableName);
            //AutomaticSourcingTableDialog.Owner = ApplicationManager.Instance.MainWindow;
            //AutomaticSourcingTableDialog.ShowDialog();
            //String tableName = ((AutomaticSourcingData)tableissue).tableName;
            //AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            //((AutomaticSourcingData)tableissue).hasDialogName = true;
            //((AutomaticSourcingData)tableissue).tableHasChanged = tableName != AutomaticSourcingTableDialog.inputTableName;
            //((AutomaticSourcingData)tableissue).runTable = AutomaticSourcingTableDialog.requestRunAllocation;
            //((AutomaticSourcingData)tableissue).createTable = AutomaticSourcingTableDialog.requestGenerateInputTable;
            //String documentUrl = page.getAutomaticSourcingForm().SpreadSheet.DocumentUrl;
            //((AutomaticSourcingData)tableissue).excelFilePath = documentUrl;
            //((AutomaticSourcingData)tableissue).tableName = System.IO.Path.GetFileNameWithoutExtension(documentUrl);
            //((AutomaticSourcingData)tableissue).tableGroup = page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.groupGroupField.textBox.Text.Trim();
            //((AutomaticSourcingData)tableissue).dialogTableName = AutomaticSourcingTableDialog.inputTableName;
            //((AutomaticSourcingData)tableissue).excelExtension = Kernel.Util.ExcelUtil.GetFileExtension(page.getAutomaticSourcingForm().SpreadSheet.DocumentUrl).Extension;
            //((AutomaticSourcingData)tableissue).automaticSourcingOid = page.EditedObject.oid.Value;
            //GetAutomaticSourcingService().buildTableNameEventHandler -= OnBuildTableName;
        }

        private void UpdateAutoSourcSaveInfo(SaveInfo info, object automaticSourcing)
        {
            if (automaticSourcing != null && automaticSourcing is AutomaticSourcing)
            {
                return;
            }

            if (automaticSourcing != null && automaticSourcing is Kernel.Domain.InputTable)
            {
                Mask(false);
                MessageDisplayer.DisplayInfo("Run Automatic Sourcing ", "Table  " + ((Kernel.Domain.InputTable)automaticSourcing).name + " created sucessfully !");
                return;
            }

            if (automaticSourcing != null && automaticSourcing is Kernel.Domain.AutomaticSourcingData)
            {
                Mask(false);
                if (((Kernel.Domain.AutomaticSourcingData)automaticSourcing).createTable)
                {
                    MessageDisplayer.DisplayInfo("Run Automatic Sourcing ", "Table  " + ((Kernel.Domain.AutomaticSourcingData)automaticSourcing).tableName + " created sucessfully !");
                    if (((Kernel.Domain.AutomaticSourcingData)automaticSourcing).runTable)
                    {
                        //runTableAfterCreate(((Kernel.Domain.AutomaticSourcingData)automaticSourcing));
                    }
                }
                else MessageDisplayer.DisplayInfo("Run Automatic Sourcing ", "Table creation aborted !");
                return;
            }

            if (info == null || info.isEnd == true)
            {
                SaveAutoSourcTableHandler -= UpdateAutoSourcSaveInfo;
                Mask(false);
            }
            else
            {
                int rate = info.stepCount != 0 ? (Int32)(info.stepRuned * 100 / info.stepCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = info.stepCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = info.stepRuned;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "" + rate + " %";
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
            catch (Exception e) { }
            return null;
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
            catch (Exception e) { }
            return null;
        }

        public SaveInfo deserializeSaveInfo(String json)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer Serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Serializer.MaxJsonLength = int.MaxValue;
                SaveInfo saveInfo = Serializer.Deserialize<SaveInfo>(json);
                if (saveInfo == null || saveInfo.stepCount == null || saveInfo.stepCount < 1) return null;
                return saveInfo;
            }
            catch (Exception e) { }
            return null;
        }

        #endregion
        

        #region Target Actions

        public void DeleteTargets(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.TARGET_RESOURCE_PATH, oids, block);
        }

        #endregion
        

        #region Transformation Tree Actions

        public void DeleteTrees(List<int> oids, DashboardBlock block, DashboardBlock blockToUpdate)
        {
            this.BlockToUpdate = blockToUpdate;
            Delete(ResourcePath.TRANSFORMATION_TREE_RESOURCE_PATH, oids, block);
        }

        public event TransformationTreeRunInfoEventHandler TreeRunHandler;
        public event PowerpointLoadInfoEventHandler PowerpointHandler;
        Socket socket;
        public void RunTrees(List<int> oids, DashboardBlock blockToUpdate)
        {
            this.BlockToUpdate = blockToUpdate;
            TableActionData data = new TableActionData(oids);
            Mask(OnCancelTreeRun, true, "Running...", true);
           
            PowerpointLoader.stop = false;
            TreeRunHandler += updateTreeRunProgress;
            PowerpointHandler += loadPowerpoint;
            PowerpointLoader.PRESENTATIONS.Clear();
           
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string socketHeader = "/run/" + (oids != null && oids.Count > 1 ? "all/" : "");
            socket = new Socket(ResourcePath.SOCKET_TRANSFORMATION_TREE_RESOURCE_PATH + socketHeader);
            socket.OnMessage += (sender, e) =>
            {
                PowerpointLoadInfo pptLoadInfo = deserializePowerpointLoadInfo(e.Data);
                if (pptLoadInfo != null && pptLoadInfo.action != null)
                {
                    if (PowerpointHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => PowerpointHandler(pptLoadInfo));
                    return;
                }

                TransformationTreeRunInfo runInfo = deserializeTreeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (TreeRunHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => TreeRunHandler(runInfo));
                    if (runInfo.runEnded)
                    {
                        socket.Close(CloseStatusCode.Normal);
                        socket = null;
                    }
                    return;
                }

            };
            socket.OnClose += (sender, e) => { socket = null; };
            
            socket.Connect();
            string text = serializer.Serialize(oids);
            socket.Send(text);
        }

        public event ClearInfoEventHandler ClearTreeHandler;
        public void ClearTrees(List<int> oids, DashboardBlock blockToUpdate)
        {
            this.BlockToUpdate = blockToUpdate;
            TableActionData data = new TableActionData(oids);
            Mask(true, "Clearing...");
            ClearTreeHandler += updateClearProgress;

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(ResourcePath.SOCKET_TRANSFORMATION_TREE_RESOURCE_PATH + "/clear/");
            
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data, false);
                if (runInfo != null)
                {
                    if (ClearTreeHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => ClearTreeHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public void ClearAndRunTrees(List<int> oids)
        {

        }


        private void updateClearProgress(AllocationRunInfo runInfo)
        {
            if (runInfo == null || runInfo.runEnded == true)
            {
                ClearTreeHandler -= updateClearProgress;
                if (this.BlockToUpdate != null) this.BlockToUpdate.RefreshData();
                Mask(false);
            }
            else
            {
                int rate = runInfo.totalCellCount != 0 ? (Int32)(runInfo.runedCellCount * 100 / runInfo.totalCellCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = runInfo.totalCellCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = runInfo.runedCellCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "" + rate + " %";
            }
        }

        private void updateTreeRunProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                TreeRunHandler -= updateTreeRunProgress;
                PowerpointHandler -= loadPowerpoint;
                if (this.BlockToUpdate != null) this.BlockToUpdate.RefreshData();
                Mask(OnCancelTreeRun, false);

                ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Collapsed;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = "";
            }
            else
            {
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = info.totalCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = info.runedCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "Tree running: " + rate + " %" + " (" + info.runedCount + "/" + info.totalCount + ")";

                if (info.currentTreeRunInfo != null)
                {
                    rate = info.currentTreeRunInfo.totalCount != 0 ? (Int32)(info.currentTreeRunInfo.runedCount * 100 / info.currentTreeRunInfo.totalCount) : 0;
                    if (rate > 100) rate = 100;

                    if (info.currentTreeRunInfo.runedCount != 0)
                    {
                        ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Visible;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = info.currentTreeRunInfo.totalCount;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = info.currentTreeRunInfo.runedCount;
                        ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = info.currentTreeRunInfo.item + ": " + rate + " %"
                            + " (" + info.currentTreeRunInfo.runedCount + "/" + info.currentTreeRunInfo.totalCount + ")";
                    }
                }
            }
        }

        private void OnCancelTreeRun()
        {
            if (MessageDisplayer.DisplayYesNoQuestion("Stop run", "Do you want to stop the current run ?") == MessageBoxResult.Yes)
            {
                PowerpointLoader.Stop();
                if (socket != null) socket.Send("STOP");
            }
        }

        private void updatePowerpointLoadProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                PowerpointLoader.RunHandler -= updatePowerpointLoadProgress;
                ApplicationManager.Instance.MainWindow.PowerpointProgressBarPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                ApplicationManager.Instance.MainWindow.PowerpointProgressBarPanel.Visibility = Visibility.Visible;
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.Instance.MainWindow.PowerpointProgressBar.Maximum = info.totalCount;
                ApplicationManager.Instance.MainWindow.PowerpointProgressBar.Value = info.runedCount;
                ApplicationManager.Instance.MainWindow.PowerpointProgressBarLabel.Content = info.errorMessage + " [" + rate + " %]";
            }
        }

        private void loadPowerpoint(Kernel.Ui.Office.PowerpointLoadInfo info)
        {
            if (info == null) return;
            PowerpointLoader.RunHandler += updatePowerpointLoadProgress;
            PowerpointLoader.LoopCount = info.items.Count;
            PowerpointLoader.Load(info);
        }


        public TransformationTreeRunInfo deserializeTreeRunInfo(String json)
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
                
            }
            return null;
        }

        #endregion


        #region Calculated Measure Actions

        public void DeleteCalculatedMeasures(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.CALCULATED_MEASURE_RESOURCE_PATH, oids, block);
        }

        #endregion


        #region Combined Tree Actions

        public void DeleteCombinedTrees(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.TRANSFORMATION_COMBINED_RESOURCE_PATH, oids, block);
        }

        public void RunCombinedTrees(List<int> oids, DashboardBlock blockToUpdate)
        {
            this.BlockToUpdate = blockToUpdate;
            TreeRunHandler += updateCombTreeRunProgress;
            PowerpointHandler += loadPowerpoint;            
            Mask(OnCancelTreeRun, true, "Running...", true);
            
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            socket = new Socket(ResourcePath.SOCKET_TRANSFORMATION_TREE_RESOURCE_PATH + "/run/all/");
            socket.OnMessage += (sender, e) =>
            {
                PowerpointLoadInfo pptLoadInfo = deserializePowerpointLoadInfo(e.Data);
                if (pptLoadInfo != null && pptLoadInfo.action != null)
                {
                    if (PowerpointHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => PowerpointHandler(pptLoadInfo));
                    return;
                }

                TransformationTreeRunInfo runInfo = deserializeTreeRunInfo(e.Data);
                if (runInfo != null)
                {
                    if (TreeRunHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => TreeRunHandler(runInfo));
                    if (runInfo.runEnded)
                    {
                        socket.Close(CloseStatusCode.Normal);
                        socket = null;
                    }
                    return;
                }

            };

            socket.OnClose += (sender, e) => { socket = null; };

            socket.Connect();
            string text = serializer.Serialize(oids);
            socket.Send(text);

        }

        public void ClearCombinedTrees(List<int> oids)
        {
            TableActionData data = new TableActionData();
            data.oids.AddRange(oids);
            ClearTreeHandler += updateClearProgress;
            Mask(true, "Clearing...");
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(ResourcePath.SOCKET_TRANSFORMATION_TREE_RESOURCE_PATH + "/clear/");            
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data, false);
                if (runInfo != null)
                {
                    if (ClearTreeHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => ClearTreeHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public void ClearAndRunCombinedTrees(List<int> oids)
        {

        }


        private void updateCombTreeRunProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                TreeRunHandler -= updateCombTreeRunProgress;
                PowerpointHandler -= loadPowerpoint;
                if (this.BlockToUpdate != null) this.BlockToUpdate.RefreshData();
                Mask(OnCancelTreeRun, false);
                ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Hidden;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = 0;
                ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = "";
            }
            else
            {
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = info.totalCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = info.runedCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "" + rate + " % " + " (" + info.runedCount + " / " + info.totalCount + ")";

                if (info.currentTreeRunInfo != null)
                {
                    rate = info.currentTreeRunInfo.runedCount != 0 ? (Int32)(info.currentTreeRunInfo.runedCount * 100 / info.currentTreeRunInfo.totalCount) : 0;
                    if (rate > 100) rate = 100;

                    if (info.currentTreeRunInfo.runedCount != 0)
                    {
                        ApplicationManager.Instance.MainWindow.treeDetails.Visibility = Visibility.Visible;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Maximum = info.currentTreeRunInfo.totalCount;
                        ApplicationManager.Instance.MainWindow.ProgressBarTreeContent.Value = info.currentTreeRunInfo.runedCount;
                        ApplicationManager.Instance.MainWindow.statusTextBlockTreeContent.Content = "" + info.currentTreeRunInfo.item + " :  " + rate + " %" + " (" + info.currentTreeRunInfo.runedCount + " / " + info.currentTreeRunInfo.totalCount + ")";
                    }
                }
            }
        }

        #endregion


        #region Structured Report Actions

        public void DeleteStructuredReports(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.STRUCTURED_REPORT_RESOURCE_PATH, oids, block);
        }

        public event RunInfoEventHandler RunStructuredReportHandler;
        public void RunStructuredReports(List<int> oids)
        {           
            String filePath = openSaveDialog();
            if (string.IsNullOrEmpty(filePath)) return;

            StructuredReportRunData data = new StructuredReportRunData();
            data.filePath = filePath;
            data.oid = oids[oids.Count-1];

            RunStructuredReportHandler += updateStucRepRunProgress;
            Mask(true, "Running...");
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            Socket socket = new Socket(ResourcePath.SOCKET_REPORT_RESOURCE_PATH + "/structured/run/");
            socket.OnMessage += (sender, e) =>
            {
                AllocationRunInfo runInfo = deserializeRunInfo(e.Data, false);
                if (runInfo != null)
                {
                    if (RunStructuredReportHandler != null) System.Windows.Application.Current.Dispatcher.Invoke(() => RunStructuredReportHandler(runInfo));
                    if (runInfo.runEnded) socket.Close(CloseStatusCode.Normal);
                    return;
                }
            };

            socket.Connect();
            string text = serializer.Serialize(data);
            socket.Send(text);
        }

        public String openSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            fileDialog.Title = "Run Structured Report";
            fileDialog.DefaultExt = HistoryHandler.FILE_EXTENSION_CSV;
            fileDialog.Filter = "Excel files (*" + HistoryHandler.FILE_EXTENSION_CSV + ")|*" + HistoryHandler.FILE_EXTENSION_CSV;
            Nullable<bool> result = fileDialog.ShowDialog();
            var fileName = fileDialog.SafeFileName;
            var filePath = fileDialog.FileName;
            if (filePath == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName)) return null;
            return filePath;
        }

        private void updateStucRepRunProgress(AllocationRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                RunStructuredReportHandler -= updateStucRepRunProgress;
                Mask(false);
            }
            else
            {
                int rate = info.totalCellCount != 0 ? (Int32)(info.runedCellCount * 100 / info.totalCellCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = info.totalCellCount;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = info.runedCellCount;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "Structured Report running: " + rate + " %";
            }
        }

        #endregion

        #region Bank Reconciliation Actions

        public void DeleteReconciliationFilters(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.RECONCILIATON_FILTERS_RESOURCE_PATH, oids, block);
        }

        public void DeleteReconciliationPostings(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.RECONCILIATON_POSTING_RESOURCE_PATH, oids, block);
        }

        public void DeleteTransactionFileTypes(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.TRANSACTION_FILE_TYPE_RESOURCE_PATH, oids, block);
        }

        
        #endregion

        #region Grid
            public void DeleteInputGrid(List<int> oids, DashboardBlock block)
            {
                Delete(ResourcePath.INPUT_GRID_RESOURCE_PATH, oids, block);
            }

            public void DeleteReportGrid(List<int> oids, DashboardBlock block)
            {
                Delete(ResourcePath.REPORT_GRID_RESOURCE_PATH, oids, block);
            }

            public void DeleteAutomaticGrid(List<int> oids, DashboardBlock block)
            {
                Delete(ResourcePath.AUTOMATIC_SOURCING_GRID_RESOURCE_PATH, oids, block);
            }

        #endregion

        #region Posting Grid
        
        public void DeletePostingGrid(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.POSTING_GRID_RESOURCE_PATH, oids, block);
        }

        public void DeleteAutomaticPostingGrid(List<int> oids, DashboardBlock block)
        {
            Delete(ResourcePath.AUTOMATIC_POSTING_GRID_RESOURCE_PATH, oids, block);
        }

        #endregion


        protected void Mask(bool mask, string content = "Running...")
        {
            ApplicationManager.Instance.MainWindow.BusyBorder2.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = content;
            if (!mask)
            {
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = 0;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = "";
            }
        }

        protected void Mask( Misp.Kernel.Ui.Base.MainWindow.OnCancelProgressionEventHandler cancelAction, bool mask, string content = "Saving...", bool isRun = false)
        {
            ApplicationManager.Instance.MainWindow.OnCancelProgression -= cancelAction;
            ApplicationManager.Instance.MainWindow.setCloseButton1Visible(mask && isRun);
            ApplicationManager.Instance.MainWindow.BusyBorder2.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Maximum = 100;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Value = 0;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Content = content;
                ApplicationManager.Instance.MainWindow.ProgressBarTree.Visibility = Visibility.Visible;
                ApplicationManager.Instance.MainWindow.statusTextBlockTree.Visibility = Visibility.Visible;
                if (isRun)
                {
                    ApplicationManager.Instance.MainWindow.OnCancelProgression += cancelAction;
                    ApplicationManager.Instance.MainWindow.setCloseButton1ToolTip("Stop run");
                }
            }
        }

    }
}
