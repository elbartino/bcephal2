using DevExpress.Xpf.Spreadsheet;
using DevExpress.Xpf.Spreadsheet.Menu.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Misp.Kernel.Ui.Office.DevExpressSheet
{
    /// <summary>
    /// Interaction logic for DESpreadsheet.xaml
    /// </summary>
    public partial class DESpreadsheet : UserControl
    {
       
        #region Events

        public event ChangeEventHandler             Changed;
        public event EditEventHandler               Edited;
        public event SelectionChangedEventHandler   SelectionChanged;
        public event SheetActivateEventHandler      SheetActivated;
        public event SheetAddedEventHandler         SheetAdded;
        public event SheetDeletedEventHandler       SheetDeleted;
        public event CopyEventHandler               CopyBcephal;
        public event PasteEventHandler              PasteBcephal;
        public event PartialPasteEventHandler       PartialPasteBcephal;
        public event AuditCellEventHandler          AuditCell;
        public event CreateDesignEventHandler       createDesign;

        #endregion


        #region Properties

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
            get { 
                String fileName = this.spreadsheetControl.Options.Save.CurrentFileName;
                return fileName;
            }
            set { /**documenturl = value;*/ }
        }

        public List<String> customlistMenuItems { get; set; }

        public string DocumentName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(DocumentUrl);
            }
            set { /**documenturl = value;*/ }
        }

        /// <summary>
        /// Assigne ou retourne le range précédemment édité
        /// </summary>
        public Ui.Office.Range rangePreviousValue { get; set; }

        public bool ThrowEvent = true;

        #endregion


        #region Constructors

        public DESpreadsheet()
        {
            InitializeComponent();
            InitializeHandlers();
            DisableTitleBar(true);
            customlistMenuItems = new List<string>(0);
        }

        #endregion


        #region Toolbar
        
        public void DisableToolBar(bool value)
        {
            
        }

        public void DisableFormualaBar(bool value)
        {
            this.formulaBar.Visibility = !value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.formulaBar.IsEnabled = !value;
        }
        
        public void DisableTitleBar(bool value)
        {
            //this.spreadsheetControl.Ribbon.Visibility = !value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public void ActivatePasteBcephal(bool active)
        {

        }

        public object AddExcelMenu(string Header)
        {
            if (this.customlistMenuItems == null) customlistMenuItems = new List<string>(0);
            customlistMenuItems.Add(Header);
            return customlistMenuItems; ;
        }

        public void ChangeTitleBarCaption(string title)
        {
            
        }

        #endregion


        #region Operations

        public string CreateNewExcelFile()
        {
            return "";
        }

        /// <summary>
        /// Ouvre le fichier dont l'url est passé en paramètre.
        /// </summary>
        /// <param name="filePath">L'url du fichier à ouvrir</param>
        /// <param name="progID">Le type de fichier à ouvrir</param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Open(string filePath)
        {
            try
            {
                ThrowEvent = false;
                DevExpress.Spreadsheet.IWorkbook workbook = this.spreadsheetControl.Document;
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                    this.spreadsheetControl.Options.Save.CurrentFileName = filePath;
                }
                ThrowEvent = true;
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                ThrowEvent = true;
                return OperationState.STOP;
            }
        }

        /// <summary>
        /// Ouvre le dialogue permettant de choisir le document à importer.
        /// </summary>
        /// <returns>cre
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Import()
        {
            SpreadsheetCommand ImportCommand = spreadsheetControl.CreateCommand(SpreadsheetCommandId.FileOpen);
            ImportCommand.Execute();
            //this.biFileOpen.PerformClick();
            this.DocumentUrl = this.spreadsheetControl.Options.Save.CurrentFileName;
            //this.DocumentName = this.Office.GetDocumentName();
            if (Changed != null) Changed();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Sauve le fichier ouvert sous un autre nom.
        /// </summary>
        /// <param name="filePath">L'url du nouveau fichier</param>
        /// <param name="overwrite"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Export(string filePath)
        {
            try
            {
                SpreadsheetCommand FileSaveAsCommand = spreadsheetControl.CreateCommand(SpreadsheetCommandId.FileSaveAs);
                FileSaveAsCommand.Execute();
                //this.biFileSaveAs.PerformClick();
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
            return OperationState.STOP;
        }


        public OperationState Export()
        {
            try
            {
                SpreadsheetCommand FileSaveAsCommand = spreadsheetControl.CreateCommand(SpreadsheetCommandId.FileSaveAs);
                FileSaveAsCommand.Execute();
                //this.biFileSaveAs.PerformClick();
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
            return OperationState.STOP;
        }

        /// <summary>
        /// Sauve le fichier ouvert sous un autre nom.
        /// </summary>
        /// <param name="filePath">L'url du nouveau fichier</param>
        /// <param name="overwrite"></param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState SaveAs(String filePath, bool overwrite)
        {
            try
            {
                DevExpress.Spreadsheet.IWorkbook workbook = this.spreadsheetControl.Document;
                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    workbook.SaveDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                }
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }


        /// <summary>
        /// Ferme le fichier ouvert.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Close()
        {
            //this.spreadsheetControl.close();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Les  cells selectionnées dans le sheet actif
        /// </summary>
        ///       
        public Range GetSelectedRange()
        {
            try
            {
                DevExpress.Spreadsheet.Range selection = this.spreadsheetControl.Selection;
                if (selection == null) return null;
                DevExpress.Spreadsheet.Worksheet worksheet = this.spreadsheetControl.ActiveWorksheet;
                
                if (worksheet == null) return null;

                Sheet sheet = new Sheet(worksheet.Index+1, worksheet.Name);
                Range range = new Range(sheet);

                foreach (DevExpress.Spreadsheet.Range area in selection.Areas)
                {
                    RangeItem item = new RangeItem(area.TopRowIndex + 1
                       , area.BottomRowIndex + 1, area.LeftColumnIndex + 1,
                         area.RightColumnIndex + 1);
                    range.Items.Add(item);
                }
                rangePreviousValue = range;
                return range;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// getActiveSheetIndex
        /// </summary>
        /// <returns>La sheet active</returns>
        public int getActiveSheetIndex()
        {
            DevExpress.Spreadsheet.Worksheet worksheet = this.spreadsheetControl.ActiveWorksheet;            
            if (worksheet != null) return worksheet.Index+1;
            return -1;
        }

        ///// <summary>
        ///// Retourne la valeur d'une cellule
        ///// </summary>
        ///// <param name="row">la ligne de la cellule</param>
        ///// <param name="colunm">la colonne de la cellule</param>
        ///// <returns>La valeur de la cellule sélectionnée</returns>
        //public object getValueAt(int row, int column, string sheetName)
        //{
        //    DevExpress.Spreadsheet.Worksheet worksheet = getWorksheet(sheetName);
        //    if (worksheet == null) return null;
        //    DevExpress.Spreadsheet.Cell cell = worksheet[row, column];
        //    if (cell == null) return null;
        //    DevExpress.Spreadsheet.CellValue value = cell.Value;
        //    if (value == null) return null;

        //    return value.ToObject();
        //}

        public void SetValueAt(int row, int colunm, string sheetName, object value)
        {
            SetValueAt(row, colunm, sheetName, value, -1);
        }

        public void DeleteLine(int row,string sheetName) 
        {
            var sheet = this.spreadsheetControl.Document.Worksheets[sheetName];
            sheet.Rows.Remove(row);
        }


        public void SetValueAt(int row, int colunm, string sheetName, object value, int color)
        {
            var sheet = this.spreadsheetControl.Document.Worksheets[sheetName];
            
            if (value is string)
            {
                sheet.Cells[row - 1, colunm - 1].Value = value.ToString();
            }

            if (value is int)
            {
                sheet.Cells[row - 1, colunm - 1].Value = int.Parse(value.ToString());
            }

            if (value is float)
            {
                sheet.Cells[row - 1, colunm - 1].Value = float.Parse(value.ToString());
            }

            if (value is double)
            {
                sheet.Cells[row - 1, colunm - 1].Value = double.Parse(value.ToString());
            }

            if (value is DateTime)
            {
                sheet.Cells[row - 1, colunm - 1].Value = DateTime.Parse(value.ToString());
            }
            if (color > -1)
                sheet.Cells[row - 1, colunm - 1].FillColor = System.Drawing.ColorTranslator.FromOle(color);
        }

        public object getValueAt(int row, int column, string sheetName)
        {
            var sheet = this.spreadsheetControl.Document.Worksheets[sheetName];
            if (sheet == null) return null;
            DevExpress.Spreadsheet.CellValue value = sheet.GetCellValue(column - 1, row - 1);
            if (value.IsText) return value.TextValue;
            if (value.IsBoolean) return value.BooleanValue;
            if (value.IsDateTime) return value.DateTimeValue.ToShortDateString();
            if (value.IsNumeric) return value.NumericValue;
            return null;
        }

        /// <summary>
        /// Retourne la cellule active
        /// </summary>
        /// <returns>La cellule active</returns>
        public Cell getActiveCell()
        {
            return new Cell(this.spreadsheetControl.ActiveCell.RowIndex + 1, this.spreadsheetControl.ActiveCell.ColumnIndex + 1);
        }

        public Range getActiveCellAsRange()
        {
            Range range = new Range();
            range.Sheet = new Sheet(this.spreadsheetControl.ActiveWorksheet.Index + 1, this.spreadsheetControl.ActiveWorksheet.Name);
            Cell active = getActiveCell();
            RangeItem rangItem = new RangeItem(active.Row, active.Row, active.Column, active.Column);
            List<RangeItem> itemList = new List<RangeItem>();
            itemList.Add(rangItem);
            range.Items.Add(rangItem);
            return range;
        }

        /// <summary>
        /// Retourne la feuille excel courante
        /// </summary>
        /// <returns></returns>
        public Sheet getActiveSheet()
        {
            if (this.spreadsheetControl.ActiveWorksheet != null)
            {
                return new Sheet(this.spreadsheetControl.ActiveWorksheet.Index + 1, this.spreadsheetControl.ActiveWorksheet.Name);                
            }
            return null;
        }

        /// <summary>
        /// getActiveSheetName
        /// </summary>
        /// <returns>La sheet active</returns>
        public string getActiveSheetName()
        {
            if (this.spreadsheetControl.ActiveWorksheet != null) return this.spreadsheetControl.ActiveWorksheet.Name;
            return "";
        }

        public String getSheetName(int index)
        {
            DevExpress.Spreadsheet.IWorkbook workbook = this.spreadsheetControl.Document;
            if (workbook.Worksheets.Count >= index) return workbook.Worksheets[index-1].Name;
            return null;
        }

        public List<String> getSheetNames()
        {
            List<String> names = new List<String>(0); 
            DevExpress.Spreadsheet.IWorkbook workbook = this.spreadsheetControl.Document;
            foreach (DevExpress.Spreadsheet.Worksheet worksheet in workbook.Worksheets)
            {
                names.Add(worksheet.Name);
            }
            return names;
        }

        protected DevExpress.Spreadsheet.Worksheet getWorksheet(string sheetName)
        {
            DevExpress.Spreadsheet.IWorkbook workbook = this.spreadsheetControl.Document;
            foreach (DevExpress.Spreadsheet.Worksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name.Equals(sheetName)) return worksheet;
            }
            return null;
        }

        #endregion


        #region Handlers

        /// <summary>
        /// Initialize Handlers 
        /// </summary>
        protected void InitializeHandlers()
        {
            this.spreadsheetControl.SelectionChanged += OnSelectionChanged;
            this.spreadsheetControl.ActiveSheetChanged += OnSelectionChanged;
            this.spreadsheetControl.SheetInserted += OnSelectionChanged;
            this.spreadsheetControl.PopupMenuShowing += SpreadSheet_PopupMenuShowing;

            this.spreadsheetControl.CellEndEdit += OnCellEdited;
            //this.spreadsheetControl.CellValueChanged += OnCellEdited;
        }

        private void SpreadSheet_PopupMenuShowing(object sender, DevExpress.Xpf.Spreadsheet.Menu.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == SpreadsheetMenuType.Cell)
            {
                ISpreadsheetCommandFactoryService service = (ISpreadsheetCommandFactoryService)spreadsheetControl.GetService(typeof(ISpreadsheetCommandFactoryService));
                SpreadsheetCommand cmd = service.CreateCommand(SpreadsheetCommandId.InsertPicture);
                if (customlistMenuItems != null)
                    foreach (string s in customlistMenuItems)
                    {
                        SpreadsheetMenuItem menuItem = new SpreadsheetMenuItem();
                        menuItem.Tag = s;
                        menuItem.Content = s;
                        menuItem.ItemClick += menuItem_ItemClick;
                        e.Menu.Items.Insert(0,menuItem);
                    }
            }
        }

        private void OnCellEdited(object sender, EventArgs e)
        {  
            ExcelEventArg arg = new ExcelEventArg() { };
            Range previousRange = rangePreviousValue;
            Range range = GetSelectedRange();
            if (range == null) return;
            if (range.CellCount > 1) arg.Range = range;
            else arg.Range = previousRange;
            if (arg.Range == null)
            {
                arg.Range = range;
            }

            if (arg.Sheet == null)
            {
                arg.Sheet = arg.Range.Sheet;
            }

            if (ThrowEvent && Edited != null)
            {
                Edited(arg);
            }
        }


        private void OnSelectionChanged(object sender, EventArgs e)
        {
            ExcelEventArg arg = new ExcelEventArg() { };
            Range previousRange = rangePreviousValue;
            Range range = GetSelectedRange();
            if (range == null) return;
            if (range.CellCount > 1) arg.Range = range;
            else arg.Range = previousRange;
            if (arg.Range == null)
            {
                arg.Range = range;                
            }

            if (arg.Sheet == null)
            {
                arg.Sheet = arg.Range.Sheet;
            }

            //if (ThrowEvent && Edited != null)
            //{
            //    Edited(arg);
            //}
            if (ThrowEvent && SelectionChanged != null) SelectionChanged(arg);
        }

        protected virtual void menuItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            string menu = e.Item.Content.ToString();
            switch (menu)
            {
                case AUDIT_CELL_LABEL:
                    {
                        //Permet de lancer l'audit
                        AuditCell(new ExcelEventArg(getActiveSheet(), GetSelectedRange()));
                        break;
                    }

                case COPY_BCEPHAL_LABEL:
                    {
                        //Permet de faire une copy Bcephal
                        CopyBcephal(new ExcelEventArg(getActiveSheet(), GetSelectedRange()));
                        break;
                    }

                case PASTE_BCEPHAL_LABEL:
                    {

                        //Permet de faire un paste Bcephal
                        PasteBcephal(new ExcelEventArg(getActiveSheet(), getActiveCellAsRange()));
                        break;
                    }

                case CREATE_DESIGN_LABEL:
                    {
                        //Permet de definir un design de parametrisation
                        createDesign(new ExcelEventArg(getActiveSheet(), GetSelectedRange()));
                        break;
                    }

                case RENAME_AUTOMATICCOLUMN_LABEL:
                    {
                        break;
                    }
                default:
                    break;
            }
        }

        #endregion


        public void DeleteExcelSheet()
        {
            //throw new NotImplementedException();
        }
    }
}

