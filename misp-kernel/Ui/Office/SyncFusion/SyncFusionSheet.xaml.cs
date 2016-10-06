#region Copyright Syncfusion Inc. 2001 - 2016
// Copyright Syncfusion Inc. 2001 - 2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Application;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Syncfusion.UI.Xaml.CellGrid;
using Syncfusion.UI.Xaml.CellGrid.Helpers;
using Syncfusion.UI.Xaml.Spreadsheet.Helpers;
using Syncfusion.UI.Xaml.Spreadsheet;
using Syncfusion.UI.Xaml.Spreadsheet.Commands;


namespace Misp.Kernel.Ui.Office.SyncFusion
{
    /// <summary>
    /// Interaction logic for SyncFusionSheet.xaml
    /// </summary>
    public partial class SyncFusionSheet : Grid
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

        public bool gridSelectionActive = false;

        public static String EXCEL_FILTER = "Excel files (*.xls)|*.xlsx";

        /// <summary>
        /// Assigne ou retourne l'url du document courant
        /// </summary>
        public string DocumentUrl {
            get { return this.spreadsheetControl.FileName; }
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
                string docN = System.IO.Path.GetDirectoryName(DocumentUrl);
                return docN != "" ? docN : DocumentUrl; ;
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


        public SyncFusionSheet()
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
            this.DocumentUrl = this.spreadsheetControl.FileName; 


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
            this.spreadsheetControl.WorksheetAdding += SFS_SheetAddingChange;
            this.spreadsheetControl.PropertyChanged += grid_PropertyChanged;
            //this.Office.WorkbookNewSheet += Office_WorkbookNewSheet;
            //this.spreadsheetControl. += SFS_SheetActivate;            
            //this.spreadsheetControl. PropertyChanged += SFS_PropertyChanged;            
            //this.Office.WindowBeforeRightClick +=Office_WindowBeforeRightClick;
            //this.MouseDown += EdrawOffice_MouseDown;
        }

        private void grid_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!gridSelectionActive && this.spreadsheetControl.ActiveGrid != null)
            {
                this.spreadsheetControl.ActiveGrid.SelectionChanged += grid_selectionChanged;
                this.spreadsheetControl.ActiveGrid.CurrentCellValueChanged += ActiveGrid_CurrentCellValueChanged;
                gridSelectionActive = true;
            }
        }

        private void grid_selectionChanged(object sender, Syncfusion.UI.Xaml.CellGrid.Helpers.SelectionChangedEventArgs args)
        {
            ExcelEventArg eventForRangeEdition = new ExcelEventArg() { };
            eventForRangeEdition.Sheet = getActiveSheet();
            eventForRangeEdition.Range = GetSelectedRange();
            //_clipboardViewerNext = SetClipboardViewer(this.Handle);
            if (ThrowEvent && SelectionChanged != null) SelectionChanged(eventForRangeEdition);
        }       
        
        private void SFS_SheetAddingChange(object sender, WorksheetAddingEventArgs args)
        {
            if (ThrowEvent && SheetAdded != null) SheetAdded();
            if (ThrowEvent && DisableAddingSheet != null) this.DeleteExcelSheet();   
        }

        private void ActiveGrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs args)
        {
            ExcelEventArg eventForRangeEdition = new ExcelEventArg() { };
            Range previousRange = rangePreviousValue;
            Range range = GetSelectedRange();
            if (range == null) return;
            if (range.CellCount > 1) eventForRangeEdition.Range = range;
            else eventForRangeEdition.Range = previousRange;
            if (ThrowEvent && Edited != null)
            {
                Edited(eventForRangeEdition);
            }
            if (ThrowEvent && Changed != null) Changed();
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

                        GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell ;
                        GridRangeInfoList selectionRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;

                        RangeItem item = new RangeItem(selectionRange[0].Top, selectionRange[0].Bottom,
                                    selectionRange[0].Left, selectionRange[0].Right);
                            range.Items.Add(item);

                        //foreach (IRange area in xlWorkSheetR)
                        //{
                        //    RangeItem item = new RangeItem(area.Cells[1].Row, area.Cells[area.Cells.Count()].Row,
                        //    area.Cells[1].Column, area.Cells[area.Cells.Count()].Column);
                        //    range.Items.Add(item);
                        //}
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

                GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell;
                GridRangeInfoList selectionRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;

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
                            RangeItem item = new RangeItem(selectionRange[0].Top, selectionRange[0].Bottom,
                                selectionRange[0].Left, selectionRange[0].Right);
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
                
                GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell;
                GridRangeInfoList xlRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;


                // Enumerate the Rows within each Area of the Range.
                int numberOfSheets = spreadsheetControl.Workbook.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {
                        IWorksheet workSheet = (IWorksheet)spreadsheetControl.Workbook.Worksheets[i - 1];
                        RangeItem item = new RangeItem(xlRange[0].Top, xlRange[0].Bottom,
                                xlRange[0].Left, xlRange[0].Right);

                        Range r = new Range();
                        r.Sheet = new Sheet(workSheet.Index, workSheet.Name);
                        r.Items.Add(item);
                        rangeRetour.Add(r.FullName);


                        //IWorksheet workSheet = (IWorksheet)spreadsheetControl.Workbook.Worksheets[i - 1];
                        //IRange xlWorkSheet = workSheet.UsedRange;
                        //foreach (IRange area in xlWorkSheet)
                        //{
                        //    RangeItem item = new RangeItem(xlRange[0].Top, xlRange[0].Bottom,
                        //        xlRange[0].Left, xlRange[0].Right);

                        //    Range r = new Range();
                        //    r.Sheet = new Sheet(area.Worksheet.Index, area.Worksheet.Name);
                        //    r.Items.Add(item);
                        //    rangeRetour.Add(r.FullName);
                        //}
                    }
                }
                else
                {
                    RangeItem item = new RangeItem(xlRange[0].Top, xlRange[0].Bottom,
                                xlRange[0].Left, xlRange[0].Right);

                    Range r = new Range();
                    r.Sheet = new Sheet(spreadsheetControl.Workbook.ActiveSheet.Index, spreadsheetControl.Workbook.ActiveSheet.Name);
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
            Object sheet = spreadsheetControl.Workbook.Worksheets[index -1];
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
            if (!gridSelectionActive && this.spreadsheetControl.ActiveGrid != null)
            {
                this.spreadsheetControl.ActiveGrid.SelectionChanged += grid_selectionChanged;
                this.spreadsheetControl.ActiveGrid.CurrentCellValueChanged += ActiveGrid_CurrentCellValueChanged;
                gridSelectionActive = true;
            }
            try
            {
                if (spreadsheetControl.ActiveSheet is IWorksheet)
                {
                    IWorksheet xlWorkSheet = spreadsheetControl.ActiveSheet;
                    string name = xlWorkSheet.Name;
                    int index = xlWorkSheet.Index + 1;                    
                    Ui.Office.Sheet sheet = new Ui.Office.Sheet(index, name);

                    GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell;
                    GridRangeInfoList selectionRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;
                    
                    Range rangeCourant = new Range(sheet);

                    RangeItem itemCourant = new RangeItem(selectionRange[0].Top, selectionRange[0].Bottom,
                                    selectionRange[0].Left, selectionRange[0].Right);
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
            if (cell != null) spreadsheetControl.ActiveGrid.SetCellValue(cell, value.ToString());
            
        }

        public void SetValueAt(int row, int column, String value)
        {
            spreadsheetControl.ActiveGrid.SetCellValue(spreadsheetControl.ActiveSheet.Range[column, row], value.ToString());
        }

        public void SetValueAt(int row, int colunm, object value)
        {
            IRange cell = getCellAt(row, colunm);
            if (cell != null) spreadsheetControl.ActiveGrid.SetCellValue(cell, value.ToString());
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
                    if (xlWorkBook.Worksheets[i-1] is IWorksheet)
                    {
                        IWorksheet sheet = xlWorkBook.Worksheets[i-1];
                        if (sheet.Name == sheetName)
                        {
                            if (column < 0 || row < 0) return null;
                            IRange xlRange = sheet.Range[row, column];
                            IRange cell = xlRange.Cells[0];
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

                GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell ;
                GridRangeInfoList selectionRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;
                               
                IRange xlRange = spreadsheetControl.ActiveSheet.Application.ActiveCell;
                Range rangeCourant = new Range(sheet);

                RangeItem itemCourant = new RangeItem(selectionRange[0].Top, selectionRange[0].Bottom,
                                    selectionRange[0].Left, selectionRange[0].Right);
                rangeCourant.Items.Add(itemCourant);

                Cell cellule = new Cell(selectionRange[0].Top, selectionRange[0].Left);
                RangeItem item = new RangeItem(selectionRange[0].Top, selectionRange[0].Bottom,
                                    selectionRange[0].Left, selectionRange[0].Right);

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
            GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell;
            GridRangeInfoList xlRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;

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

                int debutLigne = xlRange[0].Top;
                int finLigne = xlRange[0].Bottom;

                int debutCol = xlRange[0].Left;
                int finCol = xlRange[0].Right;
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

            GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell;
            GridRangeInfoList xlRange  =  spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;
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
                    debutLigneUsedRange = xlRange[0].Top;
                    finLigneUsedRange = xlRange[0].Bottom;

                    debutColUsedRange = xlRange[0].Left;
                    finColUsedRange = xlRange[0].Right;

                    debutLigneUsableRange = selectedRange.Cells[0].Row;
                    finLigneUsableRange = selectedRange.Cells[selectedRange.Cells.Count - 1].Row;

                    debutColUsableRange = selectedRange.Cells[0].Column;
                    finColUsableRange = selectedRange.Cells[selectedRange.Cells.Count - 1].Column;

                }
                else
                {
                    UsableRange = xlWorkSheet.UsedRange as IRange;
                    debutLigneUsedRange = xlRange[0].Top;
                    finLigneUsedRange = xlRange[0].Bottom;

                    debutColUsedRange = xlRange[0].Left;
                    finColUsedRange = xlRange[0].Right;

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

                debutLigne = xlRange[0].Top;
                finLigne = xlRange[0].Bottom;

                debutCol = xlRange[0].Left;
                finCol = xlRange[0].Right;
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
            GridRangeInfoList xlRange = null;
            try
            {
                xlWorkSheet = spreadsheetControl.ActiveSheet;
                GridCurrentCell cellG = spreadsheetControl.ActiveGrid.SelectionController.CurrentCell;
                xlRange = spreadsheetControl.ActiveGrid.SelectionController.SelectedRanges;
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
                debutLigne = xlRange[1].Top;
                finLigne = xlRange[1].Bottom;

                debutCol = xlRange[1].Left;
                finCol = xlRange[1].Right;
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
                        IWorksheet foundSheet = xlWorkBook.Worksheets[i-1];
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
        /// Ouvre le dialogue permettant de choisir le document à importer.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Import()
        {
            Commands commands = new Commands(spreadsheetControl);
            FileOpenCommand importFile = new FileOpenCommand(spreadsheetControl, EXCEL_FILTER);
            importFile.Execute(EXCEL_FILTER);

            spreadsheetControl = importFile.SfSpreadsheet;
            this.DocumentUrl = importFile.SfSpreadsheet.FileName;
            this.DocumentName = DocumentUrl;

            this.spreadsheetControl.ActiveGrid.SelectionChanged += grid_selectionChanged;
            this.spreadsheetControl.ActiveGrid.CurrentCellValueChanged += ActiveGrid_CurrentCellValueChanged;

            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Ouvre le dialogue permettant de choisir le document à importer.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Export()
        {
            Commands commands = new Commands(spreadsheetControl);
            FileSaveAsCommand exportFile = new FileSaveAsCommand(spreadsheetControl, EXCEL_FILTER);
            exportFile.Execute(EXCEL_FILTER);
            return OperationState.CONTINUE;
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
