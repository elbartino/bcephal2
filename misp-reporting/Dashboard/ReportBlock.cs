using Misp.Kernel.Application;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Dashboard;
using Misp.Reporting.Base;
using Misp.Reporting.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Misp.Reporting.Dashboard
{
    public class ReportBlock : NavBlock
    {

        public ReportService Service { get { return new ReportingServiceFactory(ApplicationManager.Instance).GetReportService(); } }

        public ReportBlock() : base("Daily Controls") { }

        public override void BeforeSelection()
        {
            this.Children.Clear();
            List<BrowserData> datas = this.Service.getNavBrowserDatas();
            foreach(BrowserData data in datas)
            {
                NavBlock block = new NavBlock(data.name, NavigationToken.GetModifyViewToken(FunctionalitiesCode.REPORT_EDIT, data.oid));
                block.AllowRemoveHandlersWhenDispose = true;
                block.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#555555");
                this.Children.Add(block);
            }            
        }

    }
}