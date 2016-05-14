using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain;
using Misp.Sourcing.Table;
using System.Windows.Controls;

namespace Misp.Reporting.Report
{
    public class ReportBrowser : InputTableBrowser
    {
        
        protected override string getTitle() { return "Reports"; }

    }
}
