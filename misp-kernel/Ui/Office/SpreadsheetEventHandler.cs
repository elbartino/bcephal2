using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office
{
        
    public delegate void EditEventHandler(ExcelEventArg arg);
    public delegate void SelectionChangedEventHandler(ExcelEventArg arg);
    public delegate void SheetActivateEventHandler();
    public delegate void SheetAddedEventHandler();
    public delegate void SheetDeletedEventHandler();
    public delegate void CopyEventHandler(ExcelEventArg arg);
    public delegate void PasteEventHandler(ExcelEventArg arg);
    public delegate void AuditCellEventHandler(ExcelEventArg arg);
    public delegate void CreateDesignEventHandler(ExcelEventArg arg);
    public delegate void PartialPasteEventHandler(ExcelEventArg arg, List<string> selections);
    public delegate void EditReportEventHander();
    public delegate void EditReportShapeEventHander(string shapeName);
}
