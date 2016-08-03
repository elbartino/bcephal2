using Misp.Kernel.Application;
using Misp.Kernel.Service;
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

namespace Misp.Kernel.Administration.User
{
    /// <summary>
    /// Interaction logic for ConnectedUserPanel.xaml
    /// </summary>
    public partial class ConnectedUserPanel : Grid
    {
        public UserService UserService { get; set; }

        public ConnectedUserPanel()
        {
            InitializeComponent();
            SingOutLink.RequestNavigate += OnSingOut;
            UserLink.RequestNavigate += OnViewProfile;
        }

        private void OnViewProfile(object sender, RequestNavigateEventArgs e)
        {
            if (UserService == null) return;
            String login = this.UserTextBlock.Text.Trim();
            Domain.User user = UserService.getUserByLogin(login);
            if (user == null) return;
            NavigationToken token = NavigationToken.GetModifyViewToken(FunctionalitiesCode.ADMINISTRATION_CONNECTED_USER_PROFILE, user.oid);
            if (token != null)
            {
                 HistoryHandler.Instance.openPage(token);
            }
        }
        
        private void OnSingOut(object sender, RequestNavigateEventArgs e)
        {
            if (HistoryHandler.Instance.ActivePage != null)
            {
                if ((HistoryHandler.Instance.ActivePage.ParentController is Kernel.Ui.File.FileController)
                    || (HistoryHandler.Instance.ActivePage.ParentController == null))
                {
                    HistoryHandler.Instance.closePage(HistoryHandler.Instance.ActivePage);
                    HistoryHandler.Instance.ActivePage.Close();
                }
                else
                {
                    HistoryHandler.Instance.ActivePage.Close();
                }
            }
            HistoryHandler.Instance.tryToSingout();
        }
    }
}
