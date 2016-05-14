using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Plugin;
using Misp.Kernel.Domain;
using System.Windows;
using log4net;
using Misp.Kernel.Util;

namespace Misp.Kernel.Application
{
    public class ApplicationManager
    {

        public static String DEFAULT_SERVER_START_CMD = "jre7\\bin\\javaw.exe -Xmx800m -jar misp-server-1.0-SNAPSHOT.jar";

        private readonly ILog logger;

        protected ControllerFactory controllerFactory;
        protected HistoryHandler historyHandler;

        /// <summary>
        /// 
        /// </summary>
        public ApplicationManager() 
        {
            logger = LogManager.GetLogger(typeof(ApplicationManager));
            Instance = this;
        }

        public static String FILE_TYPE_FOLDER   = "FOLDER";
        public static String FILE_TYPE_FILE     = "FILE";

        /// <summary>
        /// Gets or sets the ApplicationManager. 
        /// </summary>
        public static ApplicationManager Instance { get; set; }

        /// <summary>
        /// Gets or sets the FileType. 
        /// </summary>
        public String FileType { get; set; }

        /// <summary>
        /// Gets or sets the Plugins list.
        /// </summary>
        public List<IPlugin> Plugins { get; set; }

        public ExcelExtension DefaultExcelExtension { get; set; }

        public PowerPointExtension DefaultPowertPointExtension { get; set; }
        /// <summary>
        /// Gets or sets the ControllerFactory. 
        /// </summary>
        public ControllerFactory ControllerFactory 
        { 
            get 
            {
                if (this.controllerFactory == null)
                {
                    this.controllerFactory = new ControllerFactory(this);
                }
                return this.controllerFactory; 
            }
            set { this.controllerFactory = value; } 
        }

        public ApplcationConfiguration ApplcationConfiguration { get; set; }


        /// <summary>
        /// Gets or sets the historyHandler. 
        /// </summary>
        public HistoryHandler HistoryHandler
        {
            get
            {
                if (this.historyHandler == null)
                {
                    this.historyHandler = new HistoryHandler(this);
                }
                return this.historyHandler;
            }
            set { this.historyHandler = value; }
        }

        /// <summary>
        /// Gets or sets the ServerArg. 
        /// </summary>
        public string[] ServerArg { get; set; }

        /// <summary>
        /// Gets or sets the FilePath. 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the FilePath. 
        /// </summary>
        public File File { get; set; }

        /// <summary>
        /// Gets or sets Allocation count. 
        /// </summary>
        public long AllocationCount { get; set; }

        /// <summary>
        /// Gets or sets the Server Host property
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// Gets or sets the Server Port
        /// </summary>
        public string ServerPort { get; set; }

        /// <summary>
        /// Gets the Server URI
        /// </summary>
        public string ServerUri { get; set; }

        public string ServerWebSocketUri { get; set; }

        /// <summary>
        /// Gets or sets the Application Version
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Gets or sets the 
        /// </summary>
        public string ServerStartCmd { get; set; }
                

        /// <summary>
        /// Gets or sets the Application Workspace
        /// </summary>
        public string Workspace { get; set; }

        /// <summary>
        /// Gets or sets the MainWindow
        /// </summary>
        public MainWindow MainWindow { get; set; }

        /// <summary>
        /// Gets or sets the RestClient
        /// </summary>
        public RestClient RestClient { get; set; }

        public bool useZip()
        {
            return FileType != null
                    && FileType.ToUpper().Equals(ApplicationManager.FILE_TYPE_FILE);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenDefaultFile()
        {
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                NavigationToken token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.FILE_FUNCTIONALITY, FilePath);
                HistoryHandler.openPage(token);
            }
        }
        
        /// <summary>
        /// Start server
        /// </summary>
        public bool StartServer()
        {
            logger.Info("Server starting...");
            try
            {
                bool serverAlive = ControllerFactory.ServiceFactory.GetFileService().IsServerAlive();
                if (serverAlive) return true;

                int heap = 0;
                if (string.IsNullOrWhiteSpace(ServerStartCmd)) this.ServerStartCmd = DEFAULT_SERVER_START_CMD;
                String[] args = this.ServerStartCmd.Split(' ');
                String java = null;
                int javaIndex = -1;
                for (int i = 0; i < args.Length; i++)
                {
                    if (!args[i].Contains("java.exe") && !args[i].Contains("javaw.exe")) continue;
                    javaIndex = i;
                    java = args[i];
                    break;
                }

                if (ServerStartCmd.Contains("-Xmx"))
                {
                    String ServerMaxHeap = null;                    
                    int heapIndex = -1;
                    for (int i = 0; i < args.Length; i++ )
                    {
                        if (!args[i].Contains("-Xmx")) continue;
                        heapIndex = i;
                        ServerMaxHeap = args[i];
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(ServerMaxHeap))
                    {
                        ServerMaxHeap = ServerMaxHeap.Trim();
                        if (ServerMaxHeap.EndsWith("M", StringComparison.CurrentCultureIgnoreCase)) ServerMaxHeap = ServerMaxHeap.Remove(ServerMaxHeap.Length - 1);
                        if (ServerMaxHeap.StartsWith("-Xmx", StringComparison.CurrentCultureIgnoreCase)) ServerMaxHeap = ServerMaxHeap.Remove(0, 4);
                        if (!int.TryParse(ServerMaxHeap, out heap) || heap <= 0)
                        {
                            String error = "Parameter 'server_max_heap' has a wrong value!";
                            logger.Error(error);
                            MessageDisplayer.DisplayError("Unable to start Bcephal", error);
                            return false;
                        }

                        int set = 50;
                        if (heap > 0)
                        {
                            for (int h = heap; h >= 100; h -= set)
                            {
                                args[javaIndex] = "";
                                args[heapIndex] = "-Xmx" + h + "M";
                                string arg = "";
                                for (int i = 0; i < args.Length; i++) arg += args[i] + " ";
                                arg = arg.Trim();
                                serverAlive = startServerProcess(java, arg);
                                if (serverAlive) return true;
                            }                            
                        }
                    }
                    MessageDisplayer.DisplayError("Unable to start Bcephal", "There is not enough free memory to run Bcephal.\nPlease close some applications and try again.");
                    logger.Debug("Server start fail!");
                    return false;
                }
                else
                {
                    args[javaIndex] = "";
                    string arg = "";
                    for (int i = 0; i < args.Length; i++) arg += args[i] + " ";
                    arg = arg.Trim();

                    serverAlive = startServerProcess(java, arg);
                    if (serverAlive) return true;
                    MessageDisplayer.DisplayError("Unable to start Bcephal", "Unable to start Bcephal!\nSee logs for more information.");
                    logger.Error("Unable to start Bcephal server!");
                    return false;
                }                                
            }
            catch (Exception e)
            {
                MessageDisplayer.DisplayError("Unable to start Bcephal", "Unable to start Bcephal!\nSee logs for more information.");
                logger.Error("Unable to start Bcephal server!", e);
                return false;
            }
        }

        protected bool startServerProcess(string javaFile, string StartServerArg)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = javaFile;
            psi.Arguments = StartServerArg;
            psi.WorkingDirectory = Environment.CurrentDirectory;

            Process proc = new Process();
            proc.StartInfo = psi;
            proc.Start();

            bool serverAlive = isServerAlive();
            if(serverAlive) return true;

            try
            {
                proc.CloseMainWindow();
                proc.Close();
            }
            catch { }
            return false;
        }

        protected bool isServerAlive()
        {
            int maxLoop = 3;
            for (int n = 1; n <= maxLoop; n++)
            {                
                bool serverAlive = ControllerFactory.ServiceFactory.GetFileService().IsServerAlive();
                if (serverAlive) return true;
                Thread.Sleep(1000);
            }
            return false;
        }

        /// <summary>
        /// Stop server
        /// </summary>
        public bool StopServer()
        {
            logger.Info("Try to shutdown server...");
            try
            {
                bool ok = ControllerFactory.ServiceFactory.GetFileService().ShutdownApplication();
                return ok;
            }
            catch (Exception e)
            {
                logger.Error("Unable to shutdown server.", e);
                MessageBoxResult response = Util.MessageDisplayer.DisplayYesNoQuestion("B-Cephal shutdown", e.Message + "\nDo you want to force shutdow?");
                if (response != MessageBoxResult.Yes) return false;
                ForceServerToStop();
                return true;
            }
        }

        private void ForceServerToStop()
        {
            logger.Info("Force server shutdown!");
            ControllerFactory.ServiceFactory.GetFileService().ForceShutdownApplication();
        }


    }
}
