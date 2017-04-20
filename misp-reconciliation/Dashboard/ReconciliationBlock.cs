using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Dashboard;
using Misp.Reconciliation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Misp.Reconciliation.Dashboard
{
    public class ReconciliationBlock : NavBlock
    {

        public ReconciliationFilterTemplateService Service { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetReconciliationFilterTemplateService(); } }

        public ReconciliationBlock() : base("Reconciliation")
        {
            
        }

        public override void BeforeSelection()
        {
            this.Children.Clear();
            List<BrowserData> datas = this.Service.getNavBrowserDatas();
            foreach(BrowserData data in datas)
            {
                NavBlock block = new NavBlock(data.name, NavigationToken.GetModifyViewToken(ReconciliationFunctionalitiesCode.RECONCILIATION_FILTER_EDIT, data.oid));
                block.Background = data.ok ? Brushes.Green : (SolidColorBrush)new BrushConverter().ConvertFrom("#555555");
                block.AllowRemoveHandlersWhenDispose = true;
                this.Children.Add(block);
            }            
        }

    }
}
