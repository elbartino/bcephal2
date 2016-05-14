using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for HiperlinkPanel.xaml
    /// </summary>
    public partial class HiperlinkPanel : Grid
    {
        public HiperlinkPanel()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Cette fonction ajoute une liste (Model,InputTables,Reports) dans un conteneur visuel(stackPanel)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listeElements"> la liste (Model,InputTables,Reports)</param>
        /// <param name="currentStackPanel">le conteneur dans lequel on veut ajouter la liste </param>
        private void DisplayLinks<T>(List<T> listeElements, StackPanel currentStackPanel) where T : Kernel.Domain.Browser.BrowserData
        {
            ItemsPanel.Children.Clear();
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
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(hyperLink);

                ItemsPanel.Children.Add(textBlock);
                hyperLink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(OnRequestNavigate);
            }
        }

        /// <summary>
        /// Opération qui s'effectue on click d'un lien.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Domain.Browser.BrowserData selectedElement = getDataContext((sender as Hyperlink));
            OpenSelectedElement(selectedElement);
        }


        /// <summary>
        /// Ouvre l'élement selectioné.
        /// </summary>
        /// <param name="selectedElement">l'élément selectionné</param>
        /// <returns>OperationState.STOP en cas d'échec; OperationState.CONTINUE sinon</returns>
        private void OpenSelectedElement(Domain.Browser.BrowserData element)
        {
            if (element != null)
            {
                NavigationToken token = null;

                if (element is Domain.Browser.InputTableBrowserData)
                {
                    token = NavigationToken.GetModifyViewToken("NEW_INPUT_TABLE_FUNCTIONALITY", element.oid);
                    if (((Domain.Browser.InputTableBrowserData)element).isReport)
                        token = NavigationToken.GetModifyViewToken("NEW_REPORT_FUNCTIONALITY", element.oid);
                }
                else if (element is Domain.Browser.BrowserData)
                {
                    token = NavigationToken.GetSearchViewToken("INITIATION_FUNCTIONALITY");
                }

                if (token != null)
                {
                    HistoryHandler.Instance.openPage(token);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        private Domain.Browser.BrowserData getDataContext(Hyperlink selection)
        {
            return (Domain.Browser.BrowserData)selection.DataContext;
        }

    }
}
