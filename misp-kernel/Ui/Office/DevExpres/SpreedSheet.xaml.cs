using DevExpress.Spreadsheet;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.Xpf.Spreadsheet.Menu.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using Misp.Kernel.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Menu;

namespace Misp.Kernel.Ui.Office.DevExpres
{
    /// <summary>
    /// Interaction logic for SpreedSheet.xaml
    /// </summary>
    public partial class SpreedSheet : Grid
    {

        public const string COPY_BCEPHAL_LABEL = "Copy Bcephal";
        public const string PASTE_BCEPHAL_LABEL = "Paste Bcephal";
        public const string PARTIAL_PASTE_BCEPHAL_LABEL = "&Partial Paste Bcephal";
        public const string AUDIT_CELL_LABEL = "Audit Cell";
        public const string CREATE_DESIGN_LABEL = "Create Design";
        public const string ADD_AUTOMATICCOLUMN_LABEL = "Add Column";
        public const string REMOVE_AUTOMATICCOLUMN_LABEL = "Remove Column";
        public const string RENAME_AUTOMATICCOLUMN_LABEL = "Renommer";

        public string DocumentUrl
        {
            get
            {
                return this.SpreadSheet.Options.Save.CurrentFileName;
            }
            set 
            {
                documenturl = value;
            }
        }

        public List<String> customlistMenuItems { get; set; }

        public string DocumentName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(DocumentUrl);
            }
        }

        private string documenturl;

        public SpreedSheet()
        {
            InitializeComponent();
            InitHandlers();
            customizeToolbar();
            DisableTitleBar(true);
            customlistMenuItems = new List<string>(0);
        }

        private void InitHandlers()
        {
            this.SpreadSheet.SelectionChanged += OnSelectionChanged;
            this.SpreadSheet.ActiveSheetChanged += OnSelectionChanged;
            this.SpreadSheet.CellValueChanged += OnCellChanged;
            this.SpreadSheet.PopupMenuShowing += SpreadSheet_PopupMenuShowing;
        }

        private void SpreadSheet_PopupMenuShowing(object sender, DevExpress.Xpf.Spreadsheet.Menu.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == SpreadsheetMenuType.Cell)
            {
                // Remove the "Clear Contents" menu item.
                //e.Menu.RemoveMenuItem(SpreadsheetCommandId.FormatClearContentsContextMenuItem);
                
                // Disable the "Hyperlink" menu item.
                //e.Menu.DisableMenuItem(SpreadsheetCommandId.InsertHyperlinkContextMenuItem);

                // Create a menu item for the Spreadsheet command, which inserts a picture into a worksheet.
                ISpreadsheetCommandFactoryService service = (ISpreadsheetCommandFactoryService)SpreadSheet.GetService(typeof(ISpreadsheetCommandFactoryService));
                SpreadsheetCommand cmd = service.CreateCommand(SpreadsheetCommandId.InsertPicture);
                //SpreadsheetMenuItemCommandWinAdapter menuItemCommandAdapter = new SpreadsheetMenuItemCommandWinAdapter(cmd);
                //SpreadsheetMenuItem menuItem = (SpreadsheetMenuItem)menuItemCommandAdapter.CreateMenuItem(DevExpress.Utils.Menu.DXMenuItemPriority.Normal);
                //menuItem.BeginGroup = true;
                //e.Menu.Items.Add(menuItem);
                if (customlistMenuItems != null)
                foreach (string s in customlistMenuItems)
                {
                    SpreadsheetMenuItem menuItem = new SpreadsheetMenuItem();
                    //menuItem.Name = s;
                    menuItem.Tag = s;
                    menuItem.Content = s;
                    menuItem.ItemClick +=menuItem_ItemClick;
                    //menuItem.RaiseEvent(new EventHandler());
                    // Insert a new item into the Spreadsheet popup menu and handle its click event.
                    //SpreadsheetMenuItem myItem = new SpreadsheetMenuItem("My Menu Item", new EventHandler(MyClickHandler));
                    e.Menu.Items.Add(menuItem);
                }
                
            }
        }

        private void menuItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            string menu = e.Item.Content.ToString();
            switch (menu) 
            {
                case AUDIT_CELL_LABEL: 
                {
                    break;
                }

                case CREATE_DESIGN_LABEL:
                {
                    break;
                }
                
                case ADD_AUTOMATICCOLUMN_LABEL:
                {
                    break;
                }

                case REMOVE_AUTOMATICCOLUMN_LABEL:
                {
                    break;
                }

                case RENAME_AUTOMATICCOLUMN_LABEL:
                {
                    break;
                }
                default : 
                    break;
            }

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
            if (this.customlistMenuItems == null) customlistMenuItems = new List<string>(0);
            customlistMenuItems.Add(Header);
            return customlistMenuItems; ;
        }

        public Application.OperationState Import()
        {
            biFileOpen.PerformClick();
            return OperationState.CONTINUE;
        }

        public string CreateNewExcelFile()
        {
            return "";
        }

        public Application.OperationState Open(string filePath, string progID)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
                SpreadSheet.LoadDocument(stream, DocumentFormat.OpenXml);
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
            IWorkbook workbook = SpreadSheet.Document;
              
            // Save the modified document to a stream. 
            using (FileStream stream = new FileStream(filePath,
                FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.SaveDocument(stream, DocumentFormat.OpenXml);
            }
            DocumentUrl = SpreadSheet.Options.Save.CurrentFileName;
            biFileSave.IsEnabled = false;
            return OperationState.CONTINUE;
        }

        public Application.OperationState Export(string filePath)
        {
            biFileSaveAs.PerformClick();
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

        public void DisableFormulaBar(bool value) 
        {
            this.FormulaBar.Visibility = !value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.FormulaBar.IsEnabled = !value;
        }

        public void DisableTitleBar(bool value)
        {

            this.SpreadSheet.Ribbon.Visibility = !value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            //this.barManager1.Visibility = !value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            //this.barManager1.IsEnabled = !value;

            //this.ribbonControl1.Visibility = !value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            //this.ribbonControl1.IsEnabled = !value;
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

        private void customizeToolbar() 
        {
            biFileSave.IsVisible = false;
            biFileSaveAs.IsVisible = false;
            biFileOpen.IsVisible = false;
        }
    }
}
