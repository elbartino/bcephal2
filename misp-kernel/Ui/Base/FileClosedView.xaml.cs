using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for FileClosedView.xaml
    /// </summary>
    public partial class FileClosedView : Grid
    {
        public FileClosedView()
        {
            InitializeComponent();
            BuildNewFileControl();
            BuildGuidedTourControl();
        }

        /// <summary>
        /// Build Recent Opened Files menu
        /// </summary>
        public void BuildRecentOpenedFiles(StringCollection files)
        {
            this.ItemsPanel.Children.Clear();
            foreach (string filePath in files)
            {
                int n = filePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                string header = filePath.Substring(n + 1);
                NavigationToken token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.FILE_FUNCTIONALITY, filePath);
                this.ItemsPanel.Children.Add(BuildRecentFileControl(filePath, header, token));
            }
        }

        /// <summary>
        /// Build Recent Opened Files menu
        /// </summary>
        public void BuildRecentOpenedFiles(List<String> projects)
        {
            this.ItemsPanel.Children.Clear();
            foreach (string project in projects)
            {
                NavigationToken token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.FILE_FUNCTIONALITY, project);
                this.ItemsPanel.Children.Add(BuildRecentFileControl(project, project, token));
            }
        }


        protected void BuildNewFileControl()
        {
            NavigationToken newToken = NavigationToken.GetCreateViewToken(FunctionalitiesCode.FILE_FUNCTIONALITY);
            Run run1 = new Run("New Project");
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + "New Project"),
                DataContext = newToken
            };
            NewFileTextBlock.Inlines.Add(hyperLink);
            NewFileTextBlock.ToolTip = "Create a new Project";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);

            Run run = new Run("Clear");
            Hyperlink clearHyperLink = new Hyperlink(run)
            {
                NavigateUri = new Uri("http://localhost//" + "Clear list"),
                //DataContext = newToken
            };
            ClearTextBlock.Inlines.Add(clearHyperLink);
            ClearTextBlock.ToolTip = "Clear file list";
            clearHyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnClearRequestNavigate);

        }

        protected void BuildGuidedTourControl()
        {
            Run run1 = new Run("Guided Tour...");
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://www.b-cephal.com"),
                DataContext = "Guided Tour"
            };
            GuidedTourTextBlock.Inlines.Add(hyperLink);
            GuidedTourTextBlock.ToolTip = "Guided Tour";
            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnGuidedTourRequestNavigate);
        }

        private void OnGuidedTourRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        protected TextBlock BuildRecentFileControl(string filePath, string header, NavigationToken token)
        {
            Run run1 = new Run(header);
            Hyperlink hyperLink = new Hyperlink(run1)
            {
                NavigateUri = new Uri("http://localhost//" + header),
                DataContext = token
            };
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(hyperLink);
            textBlock.ToolTip = filePath;

            hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);

            return textBlock;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if(sender is Hyperlink){
                Hyperlink link = (Hyperlink)sender;
                object context = link.DataContext;
                if (context is NavigationToken)
                {
                    NavigationToken token = (NavigationToken)context;
                    HistoryHandler.Instance.openPage(token);
                }
            }
        }

        private void OnClearRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Application.ApplicationManager.Instance.MainWindow.MenuBar.GetFileMenu().RemoveAllRecentFiles();
        }

    }
}
