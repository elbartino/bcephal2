using DevExpress.Xpf.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office.DevExpressSheet
{
    public class BSpreadsheetControl : SpreadsheetControl
    {
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                base.OnMouseMove(e);
            }
            catch (Exception ex)
            {

            }
        }

    }
}
