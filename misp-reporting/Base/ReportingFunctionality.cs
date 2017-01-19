using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reporting.Base
{
    public class ReportingFunctionality : Functionality
    {

        public ReportingFunctionality()
        {
            this.Code = FunctionalitiesCode.REPORTING;
            this.Name = "Reporting";
            buildChildren();
        }

        private void buildChildren()
        {
            Functionality report = new Functionality(this, FunctionalitiesCode.REPORT, "Report", true);
            /*
            report.Children.Add(new Functionality(this, FunctionalitiesCode.REPORT_LIST, "Report List", true));
            report.Children.Add(new Functionality(this, FunctionalitiesCode.REPORT_VIEW, "Report View", true));
            report.Children.Add(new Functionality(this, FunctionalitiesCode.REPORT_EDIT, "Report Edit", true));
            */
            this.Children.Add(report);

            Functionality grid = new Functionality(this, FunctionalitiesCode.REPORT_GRID, "Report Grid", true);
            /*
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.REPORT_GRID_LIST, "Report Grid List", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.REPORT_GRID_VIEW, "Report Grid View", true));
            grid.Children.Add(new Functionality(this, FunctionalitiesCode.REPORT_GRID_EDIT, "Report Grid Edit", true));
            */
            this.Children.Add(grid);

            Functionality sReport = new Functionality(this, FunctionalitiesCode.STRUCTURED_REPORT, "Structured Report", true);
            /*
            sReport.Children.Add(new Functionality(this, FunctionalitiesCode.STRUCTURED_REPORT_LIST, "Structured Report List", true));
            sReport.Children.Add(new Functionality(this, FunctionalitiesCode.STRUCTURED_REPORT_VIEW, "Structured Report View", true));
            sReport.Children.Add(new Functionality(this, FunctionalitiesCode.STRUCTURED_REPORT_EDIT, "Structured Report Edit", true));
            */
            this.Children.Add(sReport);

            Functionality measure = new Functionality(this, FunctionalitiesCode.CALCULATED_MEASURE, "Calculated Measure", true);
            /*
            measure.Children.Add(new Functionality(this, FunctionalitiesCode.CALCULATED_MEASURE_LIST, "Calculated Measure List", true));
            measure.Children.Add(new Functionality(this, FunctionalitiesCode.CALCULATED_MEASURE_VIEW, "Calculated Measure View", true));
            measure.Children.Add(new Functionality(this, FunctionalitiesCode.CALCULATED_MEASURE_EDIT, "Calculated Measure Edit", true));
            */
            this.Children.Add(measure);

            Functionality pivot = new Functionality(this, FunctionalitiesCode.PIVOT_TABLE, "Pivot Table", true);
            /*
            pivot.Children.Add(new Functionality(this, FunctionalitiesCode.PIVOT_TABLE_LIST, "Pivot Table List", true));
            pivot.Children.Add(new Functionality(this, FunctionalitiesCode.PIVOT_TABLE_VIEW, "Pivot Table View", true));
            pivot.Children.Add(new Functionality(this, FunctionalitiesCode.PIVOT_TABLE_EDIT, "Pivot Table Edit", true));
            */
            this.Children.Add(pivot);

        }


    }
}
