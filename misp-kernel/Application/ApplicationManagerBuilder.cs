using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using RestSharp;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Kernel.Plugin;
using log4net;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;


namespace Misp.Kernel.Application
{
    /// <summary>
    /// Cette classe permet de construire l'instance de la classe ApplicationManager
    /// qui représente le context de l'application.
    /// </summary>
    public class ApplicationManagerBuilder : IPluginHost
    {

        /// <summary>
        /// Nom du fichier contenant les paramètres d'initialisation de l'application.
        /// </summary>
        public static string APPLICATION_INIT_FILENAME = "bcephal_init.properties";

        public static string DEFAULT_HOST = "localhost";
    
        // Default port
        public static string DEFAULT_PORT = "8080";

        private readonly ILog logger;

        /// <summary>
        /// Construit une nouvelle instance de la classe ApplicationManagerBuilder
        /// </summary>
        public ApplicationManagerBuilder()
        {
            logger = LogManager.GetLogger(typeof(ApplicationManagerBuilder));
        }


        public bool Register(IPlugin plugin)
        {
            return true;
        }

        public void loadPlugins()
        {
            logger.Info("Plugins loading...");
            ApplicationManager.Instance.Plugins = new List<IPlugin>(0);
            string path = getBaseDirectory();
            logger.Debug("Directory to search plugins : " + path);
            string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

            LoadPlugin("Misp.Initiation");
            LoadPlugin("Misp.Sourcing");
            LoadPlugin("Misp.Planification");
            LoadPlugin("Misp.Allocation");
            LoadPlugin("Misp.Reporting");
            if (ApplicationManager.Instance.ApplcationConfiguration.IsReconciliationDomain()) LoadPlugin("Misp.Reconciliation");
            if (ApplicationManager.Instance.ApplcationConfiguration.IsMultiuser()) LoadPlugin("Misp.Administration");

            ApplicationManager.Instance.Plugins.BubbleSort();
            logger.Debug("Plugins loaded!");
        }


        private void LoadPlugin(String name)
        {
            logger.Debug("Adding Plugin : " + name);
            Assembly assembly = null;
            Type ObjType = null;
            try
            {

                assembly = Assembly.Load(name);
                if (assembly != null)
                {
                    ObjType = assembly.GetType(name + ".PlugIn");
                }
            }
            catch (Exception) { }
            try
            {
                if (ObjType != null)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(ObjType);
                    plugin.Host = this;
                    ApplicationManager.Instance.Plugins.Add(plugin);
                    logger.Debug("Plugin : " + plugin.Name + "[Added]");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Unable to load plugin", ex);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void build()
        {
            Dictionary<string, string> data = getProperties(APPLICATION_INIT_FILENAME);
            String host = (String)data["server_host"];
            String port = (String)data["server_port"];
            String version = (String)data["application.version"];
            String filetype = (String)data["bcephal.file.type"];
            
            String serverStartCmd = data.ContainsKey("server_start_command") ? (String)data["server_start_command"] : null;
            
            host = host != null && host.Length > 0 ? host : DEFAULT_HOST;
            port = port != null && port.Length > 0 ? port : DEFAULT_PORT;

            filetype = filetype != null ? filetype.Trim() : ApplicationManager.FILE_TYPE_FILE;

            var baseUri = "http://" + host + ":" + port + "/bcephal/";
            var baseWsUri = "ws://" + host + ":" + port + "/bcephal/websocket/";

            ApplicationManager manager = ApplicationManager.Instance;
            manager.ServerHost = host;
            manager.ServerPort = port;
            manager.ServerUri = baseUri;
            manager.ServerWebSocketUri = baseWsUri;
            manager.FileType = filetype;
            manager.ApplicationVersion = version;

            manager.ServerStartCmd = string.IsNullOrWhiteSpace(serverStartCmd) ? ApplicationManager.DEFAULT_SERVER_START_CMD : serverStartCmd.Trim();

            manager.RestClient = new RestClient(baseUri);
            manager.Plugins = new List<IPlugin>(0);
            manager.HistoryHandler = new HistoryHandler(manager);
                        
        }

        public void tryToconnect()
        {
            SplashScreen screen = new SplashScreen("Resources\\Images\\Splash.png");
            screen.Show(false, true);
            loadApplicationConfig();
            ApplicationManager.Instance.MainWindow = new MainWindow();
            loadPlugins();
            ApplicationManager.Instance.MainWindow.Show();
            screen.Close(TimeSpan.Zero);

            if (ApplicationManager.Instance.ApplcationConfiguration.IsMonouser()) HistoryHandler.Instance.buildUserMenus();
            else HistoryHandler.Instance.tryToLogin();
        }

        protected void loadApplicationConfig()
        {
            ApplcationConfiguration config = ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetFileService().GetApplcationConfiguration();
            if (config == null) config = new ApplcationConfiguration(ApplcationConfiguration.Editon.MONOUSER, ApplcationConfiguration.Domain.RECONCILIATION);
            ApplicationManager.Instance.ApplcationConfiguration = config;
        }

           
        private void setPowerPointExtension()
        {
            logger.Info("MS PowerPoint checking...");
            try
            {
                PowerPointExtension defaultPowerPointExtenstion = PowerPointUtil.GetDefaultPowerPointExtension();
                if (defaultPowerPointExtenstion == null) MessageDisplayer.DisplayWarning("Bcephal - MS PowerPoint not found", "The MS PowerPoint version of your computer is not supported or there is no MS PowerPoint installed. \n You may not be able to use some functionnalities!");
                else ApplicationManager.Instance.DefaultPowertPointExtension = defaultPowerPointExtenstion;
            }
            catch (Exception e)
            {
                logger.Error("MS PowerPoint checking faild: " + e);
            }
            logger.Debug("PowerPoint checking end.");
        }

        
        
        /// <summary>
        /// Le répertoire de base de l'application
        /// </summary>
        /// <returns></returns>
        public static string getBaseDirectory()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string getResourceDirectory()
        {
            string baseDir = getBaseDirectory();
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(baseDir);
            baseDir = dirInfo.FullName;
            return baseDir + "/resources/";
        }

        protected static String getResourceFilePath(String filename)
        {
            if (filename == null)
            {
                
            }
            return getResourceDirectory() + filename;
        }

        protected static Dictionary<string, string> getProperties(String filename)
        {
            var data = new Dictionary<string, string>();
            foreach (var row in System.IO.File.ReadAllLines(getResourceFilePath(filename)))
            {
                if(row.Split('=')[0].Trim().Length > 0)
                    data.Add(row.Split('=')[0].Trim(), string.Join("=", row.Split('=').Skip(1).ToArray()));
            }
            return data;
        }


    }
}
