using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base.Menu;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.File;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Misp.Kernel.Domain;
using Misp.Kernel.Controller;
using Misp.Kernel.Util;
namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : UserControl, IView
    {
        DashboardData dashboardData;

        public DashBoard()
        { 
            InitializeComponent();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashboardData"></param>
        public void DisplayDashboard(DashboardData dashboardData) 
        {
            if (dashboardData == null) dashboardData = new DashboardData();
            this.dashboardData = dashboardData;
            addElementInPanel(dashboardData.models, stackPanModel);
            addElementInPanel(dashboardData.tables, stackPanTable);
            addElementInPanel(dashboardData.reports, stackPanReport);
        }


        /// <summary>
        /// Cette fonction ajoute une liste (Model,InputTables,Reports) dans un conteneur visuel(stackPanel)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listeElements"> la liste (Model,InputTables,Reports)</param>
        /// <param name="currentStackPanel">le conteneur dans lequel on veut ajouter la liste </param>
        private void addElementInPanel<T>(List<T> listeElements, StackPanel currentStackPanel) where T : Kernel.Domain.Browser.BrowserData
        {
            currentStackPanel.Children.Clear();
            foreach (T element in listeElements)
            {
                int oid = element.oid;
                string name = element.name;
                          
                Run run1 = new Run(name);
                Hyperlink hyperLink = new Hyperlink(run1)
                {
                    NavigateUri = new Uri("http://localhost//" + name),
                    DataContext = element
                };
                TextBlock txblkNewModel = new TextBlock();
                txblkNewModel.Inlines.Add(hyperLink);

                currentStackPanel.Children.Add(txblkNewModel);
                hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperLink_RequestNavigate);
            }
        }

        /// <summary>
        /// Opération qui s'effectue on click d'un lien.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Domain.Browser.BrowserData selectedElement = getNameElement((sender as Hyperlink));
            var result =  OpenSelectedElement(selectedElement);
            if (result != OperationState.STOP)
                this.Visibility = System.Windows.Visibility.Hidden;

        }


        /// <summary>
        /// Ouvre l'élement selectioné.
        /// </summary>
        /// <param name="selectedElement">l'élément selectionné</param>
        /// <returns>OperationState.STOP en cas d'échec; OperationState.CONTINUE sinon</returns>
        private OperationState OpenSelectedElement(Domain.Browser.BrowserData element) 
        {
            if (element != null)
            {
                NavigationToken token = null;

                if (element is Domain.Browser.InputTableBrowserData)
                {
                    token = NavigationToken.GetModifyViewToken("NEW_INPUT_TABLE_FUNCTIONALITY", element.oid);
                    if(((Domain.Browser.InputTableBrowserData)element).isReport)
                        token = NavigationToken.GetModifyViewToken("NEW_REPORT_FUNCTIONALITY", element.oid);                    
                } 
                else if (element is Domain.Browser.BrowserData)
                {
                    token = NavigationToken.GetSearchViewToken("INITIATION_FUNCTIONALITY");
                }
                
                if (token != null)
                {
                    return HistoryHandler.Instance.openPage(token);
                }
            }
            return OperationState.STOP;
        }


        /// <summary>
        /// Récupère l'élément (model,table,report) à partir l'url cliquée
        /// </summary>
        /// <param name="urlClicked">l'url cliquée</param>
        /// <returns>l'élément selectionné</returns>
        private Domain.Browser.BrowserData getNameElement(Hyperlink urlClicked) 
        {
            Hyperlink selection = urlClicked;
            return (Domain.Browser.BrowserData)selection.DataContext;          
        }

        public void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            
        }

       
    }    
}
