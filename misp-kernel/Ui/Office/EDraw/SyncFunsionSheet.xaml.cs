using System;
using System.IO;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Spreadsheet;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System.Runtime.InteropServices;

namespace Misp.Kernel.Ui.Office.EDraw
{
    /// <summary>
    /// Interaction logic for SyncFunsionSheet.xaml
    /// </summary>
    public partial class SyncFunsionSheet : Grid
    {

        public event ChangeEventHandler Changed;
        public event EditEventHandler Edited;
        public event SelectionChangedEventHandler SelectionChanged;
        public event SheetActivateEventHandler SheetActivated;
        public event SheetAddedEventHandler SheetAdded;
        public event SheetDeletedEventHandler SheetDeleted;
        public event CopyEventHandler CopyBcephal;
        public event PasteEventHandler PasteBcephal;
        public event PartialPasteEventHandler PartialPasteBcephal;
        public bool ThrowEvent = true;

        public event DisableAddingSheetEventHandler DisableAddingSheet;
        public delegate void DisableAddingSheetEventHandler();

        /// <summary>
        /// Assigne ou retourne l'url du document courant
        /// </summary>
        public string DocumentUrl {
            get { return this.spreadsheetControl.Name; }
            set{}
        }

        private string documentName;

        /// <summary>
        /// Assigne ou retourne le titre du fichier courant
        /// </summary>
        public string DocumentName
        {
            get
            {
                string exeDir = System.IO.Path.GetDirectoryName(DocumentUrl);
                return exeDir;
            }
            set {/** documentName = value; **/}
        }

        /// <summary>
        /// Retourne le nom du fichier ouvert
        /// </summary>
        /// <returns></returns>
        public String GetFilePath() { return this.DocumentName; }

        /// <summary>
        /// Assigne ou retourne le range précédemment édité
        /// </summary>
        public Ui.Office.Range rangePreviousValue { get; set; }


        public SyncFunsionSheet()
        {
            InitializeComponent();
            InitializeHandlers();
            rangePreviousValue = new Range();
        }

        /// <summary>
        /// Provide support for Excel like closing operation when press the close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.spreadsheetControl.Commands.FileClose.Execute(null);
            //if (Application.Current.ShutdownMode != ShutdownMode.OnExplicitShutdown)
            //    e.Cancel = true;
        }


        /// <summary>
        /// Permet de créer un fichier Excel.
        /// </summary>
        /// <returns>
        /// true si l'opération a réussi
        /// false sinon
        /// </returns>
        /// <summary>
        /// Permet de créer un fichier Excel.
        /// </summary>
        /// <returns>
        /// true si l'opération a réussi
        /// false sinon
        /// </returns>
        public String CreateNewExcelFile()
        {
            Close();
            this.spreadsheetControl.Create(2);
            this.spreadsheetControl.Visibility = Visibility.Visible;
            GetSelectedRange();
            this.DocumentUrl = this.spreadsheetControl.Name;


            return this.DocumentUrl;
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
        public OperationState Open(string filePath, string progID)
        {
            Close();
            bool result = false;
            if (System.IO.File.Exists(filePath))
            {
                this.spreadsheetControl.Open(filePath);
                this.spreadsheetControl.Workbook.Activate();
            }
            if (result)
            {
                GetSelectedRange();
                //DocumentUrl = this.Office.Excel.Application.ActiveWorkbook.fullnpa GetDocumentFullName();
                return OperationState.CONTINUE;
            }
            return OperationState.STOP;
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
            //if (this.spreadsheetControl.Workbook != null) this.spreadsheetControl.Workbook.Close(true);
            //this.Office.Dispose();
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
        public OperationState SaveAs(String filePath, bool overwrite)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(filePath) && DocumentUrl != null && DocumentUrl.Equals(filePath)) this.spreadsheetControl.Workbook.Save();
                else this.spreadsheetControl.Workbook.SaveAs(filePath);

                if (result)
                {
                    //DocumentUrl = this.Office.Excel.ActiveWorkbook. GetDocumentFullName();
                    //string name = this.Office.GetDocumentName();
                    //return OperationState.CONTINUE;
                }
                return OperationState.CONTINUE;
            }
            catch (Exception)
            {
                return OperationState.STOP;
            }
        }


        /// <summary>
        /// 
        /// 
        /// </summary>
        protected void InitializeHandlers()
        {
            //this.Office.WorkbookNewSheet += Office_WorkbookNewSheet;
            //this.spreadsheetControl. += SFS_SheetActivate;
            this.spreadsheetControl.WorksheetAdding +=SFS_SheetAddingChange;
            this.spreadsheetControl.PropertyChanged += SFS_PropertyChanged;
            //this.Office.WindowBeforeRightClick +=Office_WindowBeforeRightClick;
            //this.MouseDown += EdrawOffice_MouseDown;
        }

        private void SFS_SheetAddingChange(object sender, Syncfusion.UI.Xaml.Spreadsheet.Helpers.WorksheetAddingEventArgs args)
        {
            if (ThrowEvent && SheetAdded != null) SheetAdded();
            if (ThrowEvent && DisableAddingSheet != null) this.DeleteExcelSheet();   
        }

        private void SFS_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ExcelEventArg eventForRangeEdition = new ExcelEventArg() { };
            eventForRangeEdition.Sheet = getActiveSheet();
            eventForRangeEdition.Range = GetSelectedRange();
            //_clipboardViewerNext = SetClipboardViewer(this.Handle);
            if (ThrowEvent && SelectionChanged != null) SelectionChanged(eventForRangeEdition);
        }

        /// <summary>
        /// Les  cells selectionnées dans le sheet actif
        /// </summary>
        ///       
        public Ui.Office.Range GetSelectedRange()
        {
            try
            {
                IWorkbook xlWorkBook = spreadsheetControl.Workbook;
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                IRange selection = xlWorkSheet.UsedRange;
                if (selection == null) return null;

                Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
                Ui.Office.Range range = new Ui.Office.Range(sheet);

                // Enumerate the Rows within each Area of the Range.
                int numberOfSheets = selection.Application.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {
                        IWorksheet workSheet = (IWorksheet)xlWorkBook.Worksheets[i - 1];
                        IRange xlWorkSheetR = workSheet.UsedRange;
                        foreach (IRange area in xlWorkSheetR)
                        {
                            RangeItem item = new RangeItem(area.Cells[1].Row, area.Cells[area.Cells.Count()].Row,
                            area.Cells[1].Column, area.Cells[area.Cells.Count()].Column);
                            range.Items.Add(item);
                        }
                    }
                    rangePreviousValue = range;
                }
                return range;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get the List of selected range.
        /// </summary>
        /// <returns></returns>
        public List<RangeItem> getListeRange()
        {
            try
            {
                List<RangeItem> rangeRetour = new List<RangeItem>(0);

                IWorkbook xlWorkBook = spreadsheetControl.Workbook;
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                IRange xlRange = xlWorkSheet.UsedRange;


                Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
                Ui.Office.Range range = new Ui.Office.Range(sheet);

                // Enumerate the Rows within each Area of the Range.
                int numberOfSheets = xlRange.Application.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {
                        IWorksheet workSheet = (IWorksheet)xlWorkBook.Worksheets[i - 1];
                        IRange xlWorkSheetR = workSheet.UsedRange;
                        foreach (IRange area in xlWorkSheetR)
                        {
                            RangeItem item = new RangeItem(area.Cells[1].Row, area.Cells[area.Cells.Count()].Row,
                            area.Cells[1].Column, area.Cells[area.Cells.Count()].Column);
                            range.Items.Add(item);
                            rangeRetour.Add(item);
                        }
                    }
                }
                else
                {
                    RangeItem item = new RangeItem(xlRange.Cells[1].Row, xlRange.Cells[xlRange.Cells.Count()].Row,
                        xlRange.Cells[1].Column, xlRange.Cells[xlRange.Cells.Count()].Column);
                    rangeRetour.Add(item);
                }

                rangePreviousValue = range;
                return rangeRetour;
            }
            catch (Exception)
            {
                //xlApp.DisplayAlerts = false;
                return new List<RangeItem>(0);
            }
        }

        /// <summary>
        /// Get the List of selected range.
        /// </summary>
        /// <returns></returns>
        public List<String> getListeRanges()
        {
            try
            {
                List<String> rangeRetour = new List<String>(0);
                IWorkbook xlWorkBook = spreadsheetControl.Workbook;
                IRange xlRange = spreadsheetControl.ActiveSheet.UsedRange;




                // Enumerate the Rows within each Area of the Range.
                int numberOfSheets = spreadsheetControl.Workbook.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {

                        IWorksheet workSheet = (IWorksheet)spreadsheetControl.Workbook.Worksheets[i - 1];
                        IRange xlWorkSheet = workSheet.UsedRange;
                        foreach (IRange area in xlWorkSheet)
                        {
                            RangeItem item = new RangeItem(area.Cells[1].Row, area.Cells[area.Cells.Count()].Row,
                            area.Cells[1].Column, area.Cells[area.Cells.Count()].Column);

                            Range r = new Range();
                            r.Sheet = new Sheet(area.Worksheet.Index, area.Worksheet.Name);
                            r.Items.Add(item);
                            rangeRetour.Add(r.FullName);
                        }
                    }
                }
                else
                {
                    RangeItem item = new RangeItem(xlRange.Cells[1].Row, xlRange.Cells[xlRange.Cells.Count()].Row,
                        xlRange.Cells[1].Column, xlRange.Cells[xlRange.Cells.Count()].Column);

                    Range r = new Range();
                    r.Sheet = new Sheet(xlRange.Worksheet.Index, xlRange.Worksheet.Name);
                    r.Items.Add(item);
                    rangeRetour.Add(r.FullName);
                }
                return rangeRetour;
            }
            catch (Exception)
            {
                return new List<String>(0);
            }
        }

        /// <summary>
        /// Cette méthode permet de supprimer une/plusieurs feuille(s) dans un WorkBook excel
        /// </summary>
        /// <param name="sheetToDeleteName">Si ALLEXCEPTFIRST=> Suppression de toutes les feuilles sauf la première;
        ///  Sinon=> Suppression de la feuille spécifiée.
        ///  </param>
        public void DeleteExcelSheet(string sheetToDeleteName = "ALLEXCEPTFIRST")
        {
            try
            {
                List<String> rangeRetour = new List<String>(0);
                IWorkbook xlWorkBook = spreadsheetControl.Workbook;
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                IRange xlRange = xlWorkSheet.UsedRange;


                int numberOfSheets = spreadsheetControl.Workbook.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {

                        IWorksheet workSheet = (IWorksheet)spreadsheetControl.Workbook.Worksheets[i - 1];
                        if (sheetToDeleteName != "ALLEXCEPTFIRST")
                        {
                            if (workSheet.Name == sheetToDeleteName)
                                workSheet.Remove();
                        }
                        else
                        {
                            workSheet.Remove();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// Retourne la feuille excel courante
        /// </summary>
        /// <returns></returns>
        public Sheet getActiveSheet()
        {
            
            bool val = ThrowEvent;
            ThrowEvent = false;
            ThrowEvent = val;
            try
            {
                if (spreadsheetControl.ActiveSheet is IWorksheet)
                {
                    IWorksheet sheet = spreadsheetControl.ActiveSheet;
                    Range rangeCourant = new Range();
                    if (sheet != null && sheet.Name != null) return new Sheet(sheet.Index, sheet.Name); ;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Sheet getSelectedSheet(int sheetIndex)
        {
            bool val = ThrowEvent;
            ThrowEvent = false;
            ThrowEvent = val;
            try
            {
                if (spreadsheetControl.Workbook.Worksheets == null) return null;
                IWorksheet sheet = (IWorksheet)spreadsheetControl.Workbook.Worksheets[sheetIndex];
                if (sheet != null && sheet.Name != null) return new Sheet(sheet.Index, sheet.Name);
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public String getSheetName(int index)
        {
            if (spreadsheetControl.Workbook.Worksheets == null) return null;
            Object sheet = spreadsheetControl.Workbook.Worksheets[index];
            if (!(sheet is IWorksheet)) return null;
            return ((IWorksheet)sheet).Name;
        }

        public List<String> getSheetNames()
        {
            List<String> names = new List<String>(0);
            if (spreadsheetControl.Workbook.Worksheets == null) return names;
            foreach (Object sheet in spreadsheetControl.Workbook.Worksheets)
            {
                if (!(sheet is IWorksheet)) continue;
                names.Add(((IWorksheet)sheet).Name);
            }
            return names;
        }

        /// <summary>
        /// Convertit la cellule active en Range.
        /// </summary>
        /// <returns></returns>
        public Range getActiveCellAsRange()
        {
            try
            {
                if (spreadsheetControl.ActiveSheet is IWorksheet)
                {
                    IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                    string name = xlWorkSheet.Name;
                    int index = xlWorkSheet.Index + 1;                    
                    Ui.Office.Sheet sheet = new Ui.Office.Sheet(index, name);
                    
                    IRange xlRange = xlWorkSheet.Application.ActiveCell;
                    IRange xlRangeCourant = xlWorkSheet.Range;
                    Range rangeCourant = new Range();

                    RangeItem itemCourant = new RangeItem(xlRange.Cells[1].Row, xlRange.Cells[1].Row,
                        xlRange.Cells[1].Column, xlRange.Cells[1].Column);
                    rangeCourant.Items.Add(itemCourant);
                    return rangeCourant;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;

        }

        /// <summary>
        /// Cette méthode permet de vider les cellules utilisées.
        /// </summary>
        public void ClearUsedExcelCells()
        {
            try
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet as IWorksheet;

                for (int i = 1; i <= xlWorkSheet.Rows.Count(); i++)
                {
                    IRange ir = xlWorkSheet.Rows[i];
                    ir.Clear();
                }
            }
            catch (Exception) { }
        }

        public void deleteExcelRow(int row)
        {
            try
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet as IWorksheet;
                xlWorkSheet.Rows[row].Clear(ExcelMoveDirection.MoveUp);
            }
            catch (Exception) { }
        }

        public void deleteExcelCol(int row, int Col)
        {
            try
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet as IWorksheet;
                xlWorkSheet.Columns[Col].Clear(ExcelMoveDirection.MoveLeft);
            }
            catch (Exception) { }
        }

        public void DisableSheet(bool disable = true)
        {
            try
            {
                IWorksheet workSheet = spreadsheetControl.ActiveSheet as IWorksheet;
                //workSheet.Protect
                //    (Type.Missing, Type.Missing,
                //       disable,
                //       Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //       Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //       Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                //       Type.Missing);
                //xlApp.DisplayAlerts = false;
            }
            catch (Exception) { }
        }

        public void protectSheet(bool protect = true)
        {
            if (protect)
            {
                this.spreadsheetControl.Protect(true, true, "0x00000001") ;
            }
            else this.spreadsheetControl.Unprotect("0x00000001");
        }

        public void SetColorAt(int row, int colunm, string sheetName, int color)
        {
            IRange cell = getCellAt(row, colunm, sheetName);
            if (cell != null) cell.CellStyle.Color = System.Drawing.Color.FromArgb(color);
        }

        public void SetColorAt(int row, int colunm, int color)
        {
            IRange cell = getCellAt(row, colunm);
            if (cell != null) cell.CellStyle.Color = System.Drawing.Color.FromArgb(color);
        }

        public void SetValueAt(int row, int colunm, string sheetName, object value, int color)
        {
            IRange cell = getCellAt(row, colunm, sheetName);
            if (cell != null)
            {
                cell.Value = value.ToString();
                cell.CellStyle.Color = System.Drawing.Color.FromArgb(color);
            }
        }

        /// <summary>
        /// This method is used for design because of problems of native excel setvalueat in excel 2013.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colunm"></param>
        /// <param name="sheetName"></param>
        /// <param name="value"></param>
        public void SetValueAtWithoutColor(int row, int column, string sheetName, object value)
        {
            spreadsheetControl.ActiveSheet.Range[column, row].Text = value.ToString();
        }

        /// <summary>
        /// Affecte une valeur dans une cellule de la feuille excel courante
        /// </summary>
        /// <param name="row">la ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        /// <param name="value">la value à mettre  dans la cellule</param>
        public void SetValueAt(int row, int colunm, string sheetName, object value)
        {
            IRange cell = getCellAt(row, colunm, sheetName);
            if (cell != null) cell.Value = value.ToString();
        }

        public void SetValueAt(int row, int column, String value)
        {
            spreadsheetControl.ActiveSheet.Range[column, row].Text = value;
        }

        public void SetValueAt(int row, int colunm, object value)
        {
            IRange cell = getCellAt(row, colunm);
            if (cell != null) cell.Value = value.ToString();
        }

        public IRange getCellAt(int row, int column)
        {
            if (spreadsheetControl.ActiveSheet is IWorksheet)
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet as IWorksheet;
                IRange xlRange = xlWorkSheet.Range[row, column] as IRange;
                IRange cell = xlRange.Cells[1] as IRange;
                return cell;
            }
            return null;
        }

        /// <summary>
        /// Retourne la valeur d'une cellule
        /// </summary>
        /// <param name="row">la ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        /// <returns>La valeur de la cellule sélectionnée</returns>
        public object getValueAt(int row, int column, string sheetName)
        {
            IRange cell = getCellAt(row, column, sheetName);
            object value = cell != null ? cell.Value : null;
            return value;

        }


        /// <summary>
        /// Retourne la valeur d'une cellule
        /// </summary>
        /// <param name="row">la ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        /// <returns>La valeur de la cellule sélectionnée</returns>
        public IRange getCellAt(int row, int column, string sheetName)
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            if (sheetName == null) return null;
            try
            {
                for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                {
                    if (xlWorkBook.Worksheets[i] is IWorksheet)
                    {
                        IWorksheet sheet = xlWorkBook.Worksheets[i];
                        if (sheet.Name == sheetName)
                        {
                            if (column < 0 || column > sheet.Columns.Count() || row < 0 || row > sheet.Rows.Count()) return null;
                            IRange xlRange = sheet.Range[row, column];
                            IRange cell = xlRange.Cells[1];
                            return cell;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            //}
            return null;
        }


        /// <summary>
        /// Vide une cellule
        /// </summary>
        /// <param name="row">ligne de la cellule</param>
        /// <param name="colunm">la colonne de la cellule</param>
        public void ClearValueAt(int row, int colunm)
        {
            IWorksheet workSheet = spreadsheetControl.ActiveSheet;
            workSheet.Range[colunm, row].Clear();
        }

        /// <summary>
        /// Retourne la cellule active
        /// </summary>
        /// <returns>La cellule active</returns>
        public Cell getActiveCell()
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            if (xlWorkBook == null) return null;
            try
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;

                Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);

                IRange xlRange = xlWorkSheet.Application.ActiveCell;
                IRange xlRangeCourant = xlWorkSheet.Application.Range as IRange;

                Range rangeCourant = new Range(sheet);

                RangeItem itemCourant = new RangeItem(xlRange.Cells[1].Row, xlRange.Cells[xlRange.Cells.Count()].Row,
                    xlRange.Cells[1].Column, xlRange.Cells[xlRange.Cells.Count()].Column);
                rangeCourant.Items.Add(itemCourant);

                Cell cellule = new Cell(xlRange.Cells[1].Row, xlRange.Cells[1].Column);
                RangeItem item = new RangeItem(xlRange.Cells[1].Row, xlRange.Cells[xlRange.Cells.Count()].Row,
                                    xlRange.Cells[1].Column, xlRange.Cells[xlRange.Cells.Count()].Column);

                rangeCourant.Items.Add(item);


                if (rangePreviousValue != rangeCourant)
                {
                    rangePreviousValue = new Range();
                    rangePreviousValue = rangeCourant;
                }
                return cellule;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object[,] getColumnsTitles(IWorksheet sheet, bool IsFirstRow, bool isRangeSelected = false, bool canSelectRange = false)
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            try
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                IRange UsedRange;

                if (isRangeSelected && canSelectRange)
                {
                    UsedRange = xlWorkSheet.UsedRange as IRange;
                }
                else if (!isRangeSelected)
                {
                    UsedRange = sheet.UsedRange;
                }
                else return null;

                int debutLigne = UsedRange.Cells[1].Row;
                int finLigne = UsedRange.Cells[UsedRange.Cells.Count()].Row;

                int debutCol = UsedRange.Cells[1].Column;
                int finCol = UsedRange.Cells[UsedRange.Cells.Count()].Column;
                object[,] ColumnValues = new object[2, (finCol - debutCol + 1)];

                int c = 0;
                if (IsFirstRow)
                {
                    if (debutLigne == 1 && debutCol == 1)
                    {
                        for (int j = debutCol; j <= finCol; j++)
                        {
                            if (this.getValueAt(1, j, sheet.Name) != null)
                            {
                                ColumnValues[0, c] = this.getValueAt(1, j, sheet.Name);
                                ColumnValues[1, c] = j;
                            }
                            else
                            {
                                ColumnValues[0, (j - 1)] = Util.RangeUtil.GetColumnName(j);
                                ColumnValues[1, (j - 1)] = j;
                            }
                            c++;
                        }
                    }
                    else
                    {
                        for (int j = debutCol; j <= finCol; j++)
                        {
                            ColumnValues[0, c] = Util.RangeUtil.GetColumnName(j);
                            ColumnValues[1, c] = j;
                            c++;
                        }
                    }
                    return ColumnValues;
                }
                else
                {
                    for (int j = debutCol; j <= finCol; j++)
                    {
                        ColumnValues[0, c] = Util.RangeUtil.GetColumnName(j);
                        ColumnValues[1, c] = j;
                        c++;
                    }
                    return ColumnValues;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedRange"></param>
        /// <param name="isSelected"></param>
        /// <returns>Rectangle(X,Y,Width,Height)
        ///  X =>startLine
        ///  Y => startCol
        ///  Width => endLine
        ///  Height => endCol
        /// </returns>
        public System.Drawing.Rectangle GetSelectionBounds(Range selectedRange = null, bool isSelected = false, bool FirstRowIsHeader = false)
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            IWorksheet xlWorkSheet = null;
            try
            {
                xlWorkSheet = spreadsheetControl.ActiveSheet;
            }
            catch (Exception)
            {
                return new System.Drawing.Rectangle(1, 1, 1, 1);
            }
            IRange UsedRange = null, UsableRange = null;

            int debutLigne, finLigne, debutCol, finCol;

            Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
            if (isSelected)
            {
                int debutLigneUsedRange = -1,
                    finLigneUsedRange = -1,
                    debutColUsedRange = -1,
                    finColUsedRange = -1,
                    debutLigneUsableRange = -1,
                    finLigneUsableRange = -1,
                    debutColUsableRange = -1,
                    finColUsableRange = -1;
                UsedRange = xlWorkSheet.UsedRange;

                if (selectedRange != null)
                {
                    debutLigneUsedRange = UsedRange.Cells[1].Row;
                    finLigneUsedRange = UsedRange.Cells[UsedRange.Cells.Count()].Row;

                    debutColUsedRange = UsedRange.Cells[1].Column;
                    finColUsedRange = UsedRange.Cells[UsedRange.Cells.Count()].Column;

                    debutLigneUsableRange = selectedRange.Cells[0].Row;
                    finLigneUsableRange = selectedRange.Cells[selectedRange.Cells.Count - 1].Row;

                    debutColUsableRange = selectedRange.Cells[0].Column;
                    finColUsableRange = selectedRange.Cells[selectedRange.Cells.Count - 1].Column;

                }
                else
                {
                    UsableRange = xlWorkSheet.UsedRange as IRange;
                    debutLigneUsedRange = UsedRange.Cells[1].Row;
                    finLigneUsedRange = UsedRange.Cells[UsedRange.Cells.Count()].Row;

                    debutColUsedRange = UsedRange.Cells[1].Column;
                    finColUsedRange = UsedRange.Cells[UsedRange.Cells.Count()].Column;

                    debutLigneUsableRange = UsableRange.Cells[1].Row;
                    finLigneUsableRange = UsableRange.Cells[UsableRange.Cells.Count()].Row;

                    debutColUsableRange = UsableRange.Cells[1].Column;
                    finColUsableRange = UsableRange.Cells[UsableRange.Cells.Count()].Column;

                }
                debutLigne = (debutLigneUsedRange < debutLigneUsableRange) ? debutLigneUsableRange : debutLigneUsedRange;
                finLigne = (finLigneUsedRange > finLigneUsableRange) ? finLigneUsableRange : finLigneUsedRange;

                debutCol = (debutColUsedRange < debutColUsableRange) ? debutColUsableRange : debutColUsedRange;
                finCol = (finColUsedRange > finColUsableRange) ? finColUsableRange : finColUsedRange;
            }
            else
            {
                UsableRange = xlWorkSheet.UsedRange;
                UsedRange = UsableRange;

                debutLigne = UsedRange.Cells[1].Row;
                finLigne = UsedRange.Cells[UsedRange.Cells.Count()].Row;

                debutCol = UsedRange.Cells[1].Column;
                finCol = UsedRange.Cells[UsedRange.Cells.Count()].Column;
            }

            debutLigne = FirstRowIsHeader ? debutLigne == 1 ? debutLigne + 1 : debutLigne : debutLigne;


            return new System.Drawing.Rectangle(debutLigne, debutCol, finLigne, finCol);

        }

        public Range getUsableRange(Range selectedRange = null, bool IsSelected = false, bool IsFirstRow = false)
        {
            System.Drawing.Rectangle selection = GetSelectionBounds(selectedRange, IsSelected, IsFirstRow);
            int debutLigne = selection.X;
            int debutCol = selection.Y;

            int finLigne = selection.Width;
            int finCol = selection.Height;
            RangeItem rangeItem = new RangeItem(debutLigne, finLigne, debutCol, finCol);
            Range UsableRange = new Range();
            UsableRange.Items.Add(rangeItem);
            return UsableRange;
        }

        public object[,] getColumnsTitles(bool IsFirstRow, Range selectedRange = null)
        {

            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            IWorksheet xlWorkSheet = null;
            try
            {
                xlWorkSheet = spreadsheetControl.ActiveSheet;
            }
            catch (Exception)
            {
                return null;
            }

            int debutLigne = -1;
            int debutCol = -1;

            int finLigne = -1;
            int finCol = -1;

            if (selectedRange != null)
            {
                debutLigne = selectedRange.CellCount > 0 ? selectedRange.Cells[0].Row : selectedRange.Items[0].Row1;
                finLigne = selectedRange.CellCount > 0 ? selectedRange.Cells[selectedRange.CellCount - 1].Row : selectedRange.Items[0].Row2;

                debutCol = selectedRange.CellCount > 0 ? selectedRange.Cells[0].Column : selectedRange.Items[0].Column1;
                finCol = selectedRange.CellCount > 0 ? selectedRange.Cells[selectedRange.CellCount - 1].Column : selectedRange.Items[0].Column2;
            }
            else
            {
                IRange UsedRange = xlWorkSheet.UsedRange;
                debutLigne = UsedRange.Cells[1].Row;
                finLigne = UsedRange.Cells[UsedRange.Cells.Count()].Row;

                debutCol = UsedRange.Cells[1].Column;
                finCol = UsedRange.Cells[UsedRange.Cells.Count()].Column;
            }


            Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
            object[,] ColumnValues = new object[2, (finCol - debutCol + 1)];

            int c = 0;
            if (IsFirstRow)
            {

                if (debutLigne == 1)
                {

                    for (int j = debutCol; j <= finCol; j++)
                    {
                        if (this.getValueAt(1, j, sheet.Name) != null)
                        {
                            ColumnValues[0, c] = this.getValueAt(1, j, sheet.Name);
                        }
                        else
                        {
                            ColumnValues[0, c] = Util.RangeUtil.GetColumnName(j);

                        }
                        ColumnValues[1, c] = j;
                        c++;
                    }
                }
                else
                {
                    for (int j = debutCol; j <= finCol; j++)
                    {
                        ColumnValues[0, c] = Util.RangeUtil.GetColumnName(j);
                        ColumnValues[1, c] = j;
                        c++;
                    }
                }
                return ColumnValues;
            }
            else
            {
                for (int j = debutCol; j <= finCol; j++)
                {
                    ColumnValues[0, c] = Util.RangeUtil.GetColumnName(j);
                    ColumnValues[1, c] = j;
                    c++;
                }
                return ColumnValues;
            }
        }

        public List<int> getColumnsIndexes(Range selectedRange = null)
        {
            List<int> returnlist = new List<int>(0);


            System.Drawing.Rectangle selection = GetSelectionBounds(selectedRange, selectedRange != null);
            int debutCol = selection.Y;
            int finCol = selection.Height;

            for (int j = debutCol; j <= finCol; j++)
            {
                returnlist.Add(j);
            }
            return returnlist;
        }

        public int GetSelectedColumn(Range selectedRange = null, bool IsSelected = false, bool isFirstRow = false)
        {
            System.Drawing.Rectangle selection = GetSelectionBounds(selectedRange, IsSelected);

            int debutLigne = selection.X;
            int debutCol = selection.Y;

            int finLigne = selection.Width;
            int finCol = selection.Height;

            Range range = GetSelectedRange();
            if (range == null) return -1;
            if (range.CellCount == 1)
            {
                if ((range.Cells[0].Column >= debutCol && range.Cells[range.CellCount - 1].Column <= finCol) &&
                 (range.Cells[0].Row >= debutLigne && range.Cells[range.CellCount - 1].Row <= finLigne))
                {
                    return range.Cells[0].Column;
                }
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// getActiveSheetName
        /// </summary>
        /// <returns>La sheet active</returns>
        public string getActiveSheetName()
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            try
            {
                IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                if (xlWorkSheet == null) return "";
                Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
                if (sheet != null && sheet.Name != null) return sheet.Name.ToString();
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }

        /// <summary>
        /// getActiveSheetIndex
        /// </summary>
        /// <returns>La sheet active</returns>
        public int getActiveSheetIndex()
        {
            try
            {
                IWorksheet sheet = spreadsheetControl.ActiveSheet;
                if (sheet == null) return -1;
                return sheet.Index;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// getAllWorkbookSheets
        /// </summary>
        /// <returns>La feuille active</returns>
        public List<Sheet> getAllExcelSheets()
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            IWorksheet sheet = null;
            try
            {
                if (spreadsheetControl.ActiveSheet is IWorksheet)
                {
                    sheet = spreadsheetControl.ActiveSheet;
                    List<Sheet> listeSheet = new List<Sheet>(0);
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        IWorksheet foundSheet = xlWorkBook.Worksheets[i];
                        listeSheet.Add(new Sheet(foundSheet.Index, foundSheet.Name));
                    }
                    return listeSheet;
                }
                return new List<Sheet>(0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int getNumberOfSheets()
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            return xlWorkBook.Worksheets.Count;
        }

        public int getNumberOfCellsInCol(int sheetId, int indexCol, bool isFirsRowIn)
        {
            IWorkbook xlWorkBook = spreadsheetControl.Workbook;
            IWorksheet foundSheet = xlWorkBook.Worksheets[sheetId];
            IRange usedRange = foundSheet.UsedRange;

            return !isFirsRowIn ? usedRange.Cells[usedRange.Count].Row : usedRange.Cells[usedRange.Count].Row - 1;
        }

        /// <summary>
        /// Rend visible/invisible la barre d'outils Excel
        /// </summary>
        /// <param name="value">True=>Visible; False=>Invisible</param>
        public void DisableToolBar(bool value)
        {
            //spreadsheetControl.
            //this.Office.Toolbars = !value;
        }

        /// <summary>
        /// Rend visible/invisible la barre de formule
        /// </summary>
        /// <param name="value">True=>InVisible; False=>Visible</param>
        public void DisableFormualaBar(bool value)
        {
             if (value) spreadsheetControl.FormulaBarVisibility = Visibility.Hidden;
             else spreadsheetControl.FormulaBarVisibility = Visibility.Visible;
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        private const int WM_DRAWCLIPBOARD = 0x0308;        // WM_DRAWCLIPBOARD message
        private IntPtr _clipboardViewerNext;     // Our variable that will hold the value to identify the next window in the clipboard viewer chain
    }
}
