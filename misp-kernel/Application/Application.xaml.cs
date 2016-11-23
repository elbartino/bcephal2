using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using log4net;
using log4net.Config;
using log4net.Appender;
using System.IO;
using WebSocketSharp;
using System.Threading;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Office;
using System.Collections;

namespace Misp.Kernel.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Entry point.
    /// </summary>
    public partial class Application : System.Windows.Application
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(Application));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">Le chemin vers le fichier à ouvrir</param>
        public Application(string filePath, string[] serverArg)
            : base()
        {
            ApplicationManager manager = new ApplicationManager();
            ApplicationManager.Instance = manager;            
            ApplicationManager.Instance.FilePath = filePath;
            ApplicationManager.Instance.ServerArg = serverArg;
        }

        /// <summary>
        /// La méthode à exécuter au demarrage de l'application;
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //TestPowerPoint();

            base.OnStartup(e);

            ApplicationManagerBuilder builder = new ApplicationManagerBuilder();
            builder.build();
            bool ok = ApplicationManager.Instance.StartServer();
            if (!ok)
            {
                Application.Current.Shutdown();
                return;
            }
            SplashScreen screen = new SplashScreen("Resources\\Images\\Splash.png");
            screen.Show(false, true);
            builder.tryToconnect();
            ApplicationManager.Instance.MainWindow.Show();
            ApplicationManager.Instance.OpenDefaultFile();
            screen.Close(TimeSpan.Zero);
        }
        

        [STAThread]
        public static void Main(string[] args)
        {            
            ConfigureLog4Net();
            string filePath = null;
            string[] serverArg = null;
            //if (args != null && args.Length > 0) {
            //    filePath = args[0];
            //}

            if (args != null && args.Length > 1) {
                serverArg = new string[2];
                serverArg[0] = args[0];
                serverArg[1] = args[1];
            }

            var application = new Application(filePath, serverArg);
            application.InitializeComponent();
            application.Run();
        }

        static void ConfigureLog4Net(){

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            String resourcesDir = dir.FullName + "\\resources";
            String log4netConfigFile = resourcesDir + "\\log4net.xml";
            ICollection collection = XmlConfigurator.Configure(new System.IO.FileInfo(log4netConfigFile));
            
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).FullName;
            }
            path = Path.Combine(path, ".bcephal"); 
            path = Path.Combine(path, "log"); 

            var fileAppender = LogManager.GetRepository().GetAppenders().First(appender => appender is RollingFileAppender);
            if (fileAppender != null)
            {
                ((RollingFileAppender)fileAppender).File = Path.Combine(path, Path.GetFileName(((RollingFileAppender)fileAppender).File));
                ((RollingFileAppender)fileAppender).ActivateOptions();
            }
        }

        private void OnApplicationStarting(object sender, StartupEventArgs args)
        {
            //DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName = "Aqua";
            //DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName = "Office2016White";
            //DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
            //ResourceDictionary res = new ResourceDictionary();
            //res.Source = new Uri("../Resources/Styles/ButtonStyle.xaml", UriKind.Relative);
            //this.Resources.MergedDictionaries.Add(res);
        }

    }
}
