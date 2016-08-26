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

namespace Misp.Kernel.Ui.Office.DevExpres
{
    /// <summary>
    /// Interaction logic for SpreedSheet.xaml
    /// </summary>
    public partial class SpreedSheet : UserControl
    {
        public SpreedSheet()
        {
            InitializeComponent();
            InitHandlers();
        }

        private void InitHandlers()
        {
            this.SpreadSheet.SelectionChanged += OnSelectionChanged;
            this.SpreadSheet.ActiveSheetChanged += OnSelectionChanged;
            this.SpreadSheet.CellValueChanged += OnCellChanged;
        }

        private void OnCellChanged(object sender, DevExpress.XtraSpreadsheet.SpreadsheetCellEventArgs e)
        {
            Range range = new Range();
            Sheet sheet = new Sheet(this.SpreadSheet.ActiveSheetIndex, this.SpreadSheet.ActiveSheetName);
            List<RangeItem> lis = new List<RangeItem>();
            lis.Add(new RangeItem(e.Cell.RowIndex + 1, e.Cell.RowIndex + 1, e.Cell.ColumnIndex, e.Cell.ColumnIndex));
            range = new Range(sheet, lis);
            ExcelEventArg arg = new ExcelEventArg(sheet, range);
            if (Edited != null) Edited(arg);
        }
        
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null) SelectionChanged(null);
        }

        public event Base.ChangeEventHandler Changed;

        public event EditEventHandler Edited;

        public event SelectionChangedEventHandler SelectionChanged;

        public event SheetActivateEventHandler SheetActivated;

        public event SheetAddedEventHandler SheetAdded;

        public event SheetDeletedEventHandler SheetDeleted;

        public object AddExcelMenu(string Header)
        {
            return null;
        }

        public Application.OperationState Import()
        {
            //this.SpreadSheet.
            return OperationState.CONTINUE;
        }

        public string CreateNewExcelFile()
        {
            return "";
        }

        public Application.OperationState Open(string filePath, string progID)
        {
            return OperationState.CONTINUE;
        }

        public object RemoveExcelMenu(string Header)
        {
            return null;
        }

        public Application.OperationState Close()
        {
            return OperationState.CONTINUE;
        }

        public Application.OperationState SaveAs(string filePath, bool overwrite)
        {
            return OperationState.CONTINUE;
        }

        public Application.OperationState Export(string filePath)
        {
            return OperationState.CONTINUE;
        }

        public string GetFilePath()
        {
            return "";

        }

        public Range GetSelectedRange()
        {
            
            IList<DevExpress.Spreadsheet.Range> currentSelection = this.SpreadSheet.GetSelectedRanges();
            Range range = new Range();
            Sheet sheet = new Sheet(this.SpreadSheet.ActiveSheetIndex, this.SpreadSheet.ActiveSheetName);
            List<RangeItem> lis = new List<RangeItem>();
            foreach (DevExpress.Spreadsheet.Range rang in currentSelection)
            {
                string sheetName = "";
                string rangeName = rang.Name;
                sheet.Name = sheetName;
                RangeItem rangeitem = new RangeItem(rang.TopRowIndex+1, rang.TopRowIndex+1,rang.BottomRowIndex,rang.BottomRowIndex);
                lis.Add(rangeitem);
            }
            range = new Range(sheet,lis);
            return range;
        }

        public void SetValueAt(int row, int colunm, string sheetName, object value)
        {
            
        }

        public object getValueAt(int row, int colunm, string sheetName)
        {
            return null;
        }

        public void ClearValueAt(int row, int colunm)
        {
            
        }

        public Cell getActiveCell()
        {
            return new Cell(this.SpreadSheet.ActiveCell.RowIndex, this.SpreadSheet.ActiveCell.ColumnIndex+1);
        }

        public void DisableToolBar(bool value)
        {
            //SpreadSheet.too
        }

        public void DisableTitleBar(bool value)
        {
            
        }

        public Range getActiveCellAsRange()
        {
            Range range = new Range();
            range.Sheet = new Sheet(this.SpreadSheet.ActiveWorksheet.Index, this.SpreadSheet.ActiveWorksheet.Name);
            Cell active = getActiveCell();
            RangeItem rangItem = new RangeItem(active.Row+1, active.Row+1, active.Column, active.Column);
            List<RangeItem> itemList = new List<RangeItem>();
            itemList.Add(rangItem);
            range.Items.Add(rangItem);
            return range;
        }
    }
}
