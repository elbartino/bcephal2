using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using System.Reflection;
using System.Windows;
using Misp.Kernel.Domain;
using System.Runtime.InteropServices;
using System.IO;

//using Microsoft.Office.Interop.Excel;
//using Excel = Microsoft.Office.Interop.Excel;
//using OfficeProperties = Microsoft.Office.Core;
//using EDOfficeLib;

using Syncfusion.XlsIO;

namespace Misp.Kernel.Ui.Office.EDraw
{
    public partial class SyncFusionOffice : UserControl, ISpreadsheet
    {

        #region Events 

        public event ChangeEventHandler Changed;
        public event EditEventHandler Edited;
        public event SelectionChangedEventHandler SelectionChanged;
        public event SheetActivateEventHandler SheetActivated;
        public event SheetAddedEventHandler SheetAdded;
        public event SheetDeletedEventHandler SheetDeleted;
        public event CopyEventHandler CopyBcephal;
        public event PasteEventHandler PasteBcephal;
        public event PartialPasteEventHandler PartialPasteBcephal;

        public event AuditCellEventHandler AuditCell;

        public event OnAddColumnEventHandler OnAddColumn;
        public delegate void  OnAddColumnEventHandler(int index);

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(int index);

        public event OnBeforeRightClickEventHandler OnBeforeRightClick;
        public delegate void OnBeforeRightClickEventHandler();
        public event CreateDesignEventHandler createDesign;

        public event OnRenameClickEventHandler OnRenameClick;
        public delegate void OnRenameClickEventHandler(string sheetName);

        public event DisableAddingSheetEventHandler DisableAddingSheet;
        public delegate void DisableAddingSheetEventHandler();

        public bool ThrowEvent = true;

        #endregion


        #region Properties
        
        public static string EXCEL_ID = "Excel.Application";
        public static string POWER_POINT_ID = "PowerPoint.Application";
        public static string WORD_ID = "Word.Application";
        public static string EXCEL_EXT = ".xlsx";
        public const string COPY_BCEPHAL_LABEL = "&Copy Bcephal";
        public const string PASTE_BCEPHAL_LABEL = "&Paste Bcephal";
        public const string PARTIAL_PASTE_BCEPHAL_LABEL = "&Partial Paste Bcephal";
        public const string AUDIT_CELL_LABEL = "&Audit Cell";
        public const string CREATE_DESIGN_LABEL = "&Create Design";
        public const string ADD_AUTOMATICCOLUMN_LABEL = "&Add Column";
        public const string REMOVE_AUTOMATICCOLUMN_LABEL = "&Remove Column";
        public const string RENAME_AUTOMATICCOLUMN_LABEL = "&Renommer";

        public ExcelEngine excelEngine;
        public IApplication  Excel ;
        public IApplication  OfficeProperties1;

        public static bool isPaste = false;

        /// <summary>
        /// Assigne ou retourne l'url du document courant
        /// </summary>
        public string DocumentUrl { get; set; }

        //private OfficeProperties.CommandBarButton menuHeader;

        private string documentName;

        /// <summary>
        /// Assigne ou retourne le titre du fichier courant
        /// </summary>
        public string DocumentName 
        {
            get
            {
                return "dsp";//this.Office.GetDocumentName().Split('.')[0];
            }
            set { documentName = value; }
        }

        ///// <summary>
        ///// Retourne le composant office
        ///// </summary>
        //public AxEDOfficeLib.AxEDOffice Office
        //{
        //    get { return this.axEDOffice1; }
        //}

        /// <summary>
        /// Retourne le composant office
        /// </summary>
        public ExcelEngine Office
        {
            get { return this.excelEngine1; }
        }

        /// <summary>
        /// Retourne le nom du fichier ouvert
        /// </summary>
        /// <returns></returns>
        public String GetFilePath() { return this.DocumentUrl; }

        /// <summary>
        /// Assigne ou retourne le range précédemment édité
        /// </summary>
        public Ui.Office.Range rangePreviousValue { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de EdrawOffice.
        /// </summary>
        public SyncFusionOffice()
        {
            InitializeComponent();
            excelEngine = new ExcelEngine();
            Excel = excelEngine.Excel;

            //this.Office.ClearTempFiles();
            InitializeHandlers();
            rangePreviousValue = new Range();
          
        }

    

        /// <summary>
        /// Crée un menuitems dans le menu contextuel d'Excel
        /// </summary>
        /// <param name="Header">Le libelle du menuItem</param>
        /// <returns>l'objet crée</returns>
        public object AddExcelMenu(string Header)
        {
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;
            IRange selection = xlWorkSheet.UsedRange;

            Missing missing = Missing.Value;

            //OfficeProperties.MsoControlType menuItem = OfficeProperties.MsoControlType.msoControlButton;
            //menuHeader = (OfficeProperties.CommandBarButton)xlApp.CommandBars["Cell"].Controls.Add(menuItem, missing, missing, 1, true);
            //menuHeader.Style = OfficeProperties.MsoButtonStyle.msoButtonCaption;
            //menuHeader.Caption = Header;
            //menuHeader.Tag = "0";
            //menuHeader.Click += menuHeader_Click;
            return "AAA"; //menuHeader; 
        }
       
        /// <summary>
        /// Active/désactive le menu Paste.
        /// </summary>
        /// <param name="active"></param>
        public  void ActivatePasteBcephal(bool active)
        {
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
            //    Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;
            //    OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];

                
            //    for (int i = contextMenu.Controls.Count; i > 0; i--)
            //    {
            //        OfficeProperties.CommandBarControl control = contextMenu.Controls[i];
            //        if (control.Caption == PASTE_BCEPHAL_LABEL)
            //        {
            //            control.Enabled = active;
            //            break;
            //        }
            //        if (control.Caption == PARTIAL_PASTE_BCEPHAL_LABEL)
            //        {
            //            control.Enabled = active;
            //            break;
            //        }
            //    }
            //}
        }

        public bool IsExcelPaste()
        {
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
            //    Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
            //    OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];

            //    for (int i = contextMenu.Controls.Count; i > 0; i--)
            //    {
            //        OfficeProperties.CommandBarControl control = contextMenu.Controls[i];
            //        if (i == 3)
            //        {
            //            control.BeginGroup = true;
            //            break;
            //        }
            //    }
            //}
            return false;
        }

        /// <summary>
        /// Ajoute un séparateur.
        /// </summary>
        public void AddSeparatorMenu()
        {
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
            //    Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
            //    OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];

            //    for (int i = contextMenu.Controls.Count; i > 0; i--)
            //    {
            //        OfficeProperties.CommandBarControl control = contextMenu.Controls[i];
            //        if (i == 3)
            //        {
            //            control.BeginGroup = true;
            //            break;
            //        }
            //    }
            //}

        }

              
        //protected virtual void menuHeader_Click(OfficeProperties.CommandBarButton Ctrl, ref bool CancelDefault)
        //{
        //    switch (Ctrl.Caption)
        //    {
        //        //Si l'on choisit la copie Bcephal
        //        case COPY_BCEPHAL_LABEL:
        //        {
        //            //Permet de faire une copy Bcephal
        //            CopyBcephal(new ExcelEventArg(getActiveSheet(), GetSelectedRange()));
        //        }
        //        break;
        //        //Si l'on choisit le paste Bcephal
        //        case PASTE_BCEPHAL_LABEL:
        //        {
        //            //Permet de faire un paste Bcephal
        //            PasteBcephal(new ExcelEventArg(getActiveSheet(), getActiveCellAsRange()));
        //        }
        //            break;
        //        //si l'on choisit partial paste bcephal
        //        case PARTIAL_PASTE_BCEPHAL_LABEL:
        //            {
        //                Util.PasteBcephalDialog pasteBcephalDialog = null;
        //                //bool result ;
        //                System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (System.Action)(() =>
        //                {
        //                    pasteBcephalDialog = new Util.PasteBcephalDialog();
        //                    bool result;
        //                    result = (bool)pasteBcephalDialog.ShowDialog();
        //                    if (pasteBcephalDialog != null && pasteBcephalDialog.ok)
        //                    {
        //                        List<string> selections = new List<string>();
        //                        if ((bool)pasteBcephalDialog.pasteMeasure.IsChecked)
        //                            selections.Add("1");
        //                        if ((bool)pasteBcephalDialog.pastePeriod.IsChecked)
        //                            selections.Add("2");
        //                        if ((bool)pasteBcephalDialog.pasteScope.IsChecked)
        //                            selections.Add("3");

        //                        PartialPasteBcephal(new ExcelEventArg(getActiveSheet(), getActiveCellAsRange()), selections);

        //                    }
        //                }
        //             ));
        //            }
        //            break;

        //        //Si l'on choisit audit cell
        //        case AUDIT_CELL_LABEL:
        //        {
        //            //Permet de lancer l'audit
        //            AuditCell(new ExcelEventArg(getActiveSheet(), GetSelectedRange()));
        //        }
        //        break;
        //        case ADD_AUTOMATICCOLUMN_LABEL:
        //        {
        //            //Automatic Sourcing . Permet de retirer colonne 
        //            // de la liste des colonne à paramètrer.
        //            OnAddColumn(getActiveCell().Column);
        //        }
        //        break;
        //        case REMOVE_AUTOMATICCOLUMN_LABEL:
        //        {
        //           //Automatic Sourcing . Permet d'ajouter une nouvelle colonne 
        //          // dans la liste des colonne à paramètrer.
        //            if(OnRemoveColumn != null)
        //                    OnRemoveColumn(getActiveCell().Column);
        //        }
        //        break;
        //        //Si l'on choisit create design
        //        case CREATE_DESIGN_LABEL:
        //        {
        //            //Permet de definir un design de parametrisation
        //            createDesign(new ExcelEventArg(getActiveSheet(), GetSelectedRange()));
        //        }
        //        break;
        //        default:
        //            break;
        //    }
        //}
    
       
        /// <summary>
        /// Retire un menuitem du menu contextuel d'Excel
        /// </summary>
        /// <param name="Header">le menuItem à retirer</param>
        /// <returns>l'objet retiré</returns>
        public object RemoveExcelMenu(string Header)
        {
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
            //    Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;
            //    OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];
            //        int j = 1;
            //        for (int i = contextMenu.Controls.Count; i > 0; i--)
            //        {
            //            OfficeProperties.CommandBarControl control = contextMenu.Controls[j];
            //            if (control.Caption == Header) control.Delete();
            //            j++;
            //        }
            //}
            return null;
        }

        /// <summary>
        /// Rend invisible un menuitem du menu contextuel d'Excel
        /// </summary>
        /// <param name="Header">le menuItem à retirer</param>
        /// <returns>l'objet retiré</returns>
        public object SetInVisibleExcelMenu(string Header,bool isInVisible=true)
        {
        //    if (this.Office.GetCurrentProgID() == EXCEL_ID)
        //    {
        //        Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;
        //        OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];
        //        int j = 1;
        //        for (int i = contextMenu.Controls.Count; i > 0; i--)
        //        {
        //            OfficeProperties.CommandBarControl control = contextMenu.Controls[j];
        //            if (control.Caption == Header) control.Visible = !isInVisible;
        //            j++;
        //        }
        //    }
            return null;
        }
        
        #endregion


        #region Operations

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
            this.Office.Excel.Workbooks.Create(2);
            GetSelectedRange();

            //this.DocumentUrl = this.Office.GetDocumentName();

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;


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
                IWorkbook actiWorkBook = this.Office.Excel.Workbooks.Open(filePath);
                actiWorkBook.Activate();
            }
            if (result)
            {
                GetSelectedRange();
                //DocumentUrl = this.Office.GetDocumentFullName();
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
            this.Office.Excel.ActiveWorkbook.Close();
            this.Office.Dispose();
            return OperationState.CONTINUE;
        }

        public static String EXCEL_FILTER = "Excel files (*.xls)|*.xlsx";
        public static String POWERPOINT_FILTER = "Powerpoint files (*.ppt)|*.pptx";

        /// <summary>
        /// Ouvre le dialogue permettant de choisir le document à importer.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public OperationState Import()
        {
            //bool result = this.Office.OpenFileDialog(EXCEL_FILTER);
            //if (result)
            //{
            //    this.DocumentUrl = this.Office.GetDocumentFullName();
            //    this.DocumentName = this.Office.GetDocumentName();
            //    if (Changed != null) Changed();
            return OperationState.CONTINUE;
            //}
            //return OperationState.STOP;
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
            bool result = false;
            try
            {
                //String url = this.Office.GetDocumentFullName();
                //SaveAs(url, true);
                //url = this.Office.GetDocumentFullName();

                //this.Office.Excel.ActiveWorkbook.SaveAs(filePath);
                //if (result)
                //{
                //    if (url != this.Office.GetDocumentFullName()) return Open(url, EXCEL_ID);

                return OperationState.CONTINUE;
                //}
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
            bool result = false;
            try
            {
                if (System.IO.File.Exists(filePath) && DocumentUrl != null && DocumentUrl.Equals(filePath)) this.Office.Excel.ActiveWorkbook.Save();
                else this.Office.Excel.ActiveWorkbook.SaveAs(filePath);
                
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
            return OperationState.STOP;
        }



        /// <summary>
        /// 
        /// </summary>
        public void RemoveTempFiles() 
        {
            //this.Office.Excel.ActiveWorkbook.c ClearTempFiles();
        }

        /// <summary>
        /// Les  cells selectionnées dans le sheet actif
        /// </summary>
        ///       
        public Ui.Office.Range GetSelectedRange()
        {
            try
            {
                IApplication xlApp = this.excelEngine1.Excel;
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;
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
                        IWorksheet workSheet = (IWorksheet)xlApp.ActiveWorkbook.Worksheets[i - 1];
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
            catch (COMException)
            {
                return null;
            }
            catch (NullReferenceException)
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
            IApplication xlApp = this.excelEngine1.Excel;
            try
            {
                List<RangeItem> rangeRetour = new List<RangeItem>(0);

                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;
                IRange xlRange = xlWorkSheet.UsedRange;


                Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
                Ui.Office.Range range = new Ui.Office.Range(sheet);

                // Enumerate the Rows within each Area of the Range.
                int numberOfSheets = xlRange.Application.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {
                        IWorksheet workSheet = (IWorksheet)xlApp.ActiveWorkbook.Worksheets[i - 1];
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
            catch (COMException)
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
                IApplication xlApp = this.excelEngine1.Excel;
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IRange xlRange = xlWorkBook.ActiveSheet.UsedRange;
                 
                


                // Enumerate the Rows within each Area of the Range.
                int numberOfSheets = xlRange.Application.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {

                        IWorksheet workSheet = (IWorksheet)xlApp.ActiveWorkbook.Worksheets[i - 1];
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
            catch (COMException)
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
                IApplication xlApp = this.excelEngine1.Excel;
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;
                IRange xlRange = xlWorkSheet.UsedRange;


                int numberOfSheets = xlRange.Application.Worksheets.Count;
                if (numberOfSheets > 1)
                {
                    for (int i = numberOfSheets; i > 1; i--)
                    {

                        IWorksheet workSheet = (IWorksheet)xlApp.ActiveWorkbook.Worksheets[i - 1];
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
            IApplication xlApp = this.excelEngine1.Excel;

            bool val = ThrowEvent;
            ThrowEvent = false;
            ThrowEvent = val;
            try
            {
                if (xlApp.ActiveSheet is IWorksheet)
                {
                    IWorksheet sheet = xlApp.ActiveSheet;
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
            IApplication xlApp = this.excelEngine1.Excel;
            ThrowEvent = val;
            try
            {
                if (xlApp.Worksheets == null) return null;
                IWorksheet sheet = (IWorksheet)xlApp.Worksheets[sheetIndex];
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
            IApplication xlApp = this.excelEngine1.Excel;
            if (xlApp.Worksheets == null) return null;
            Object sheet = xlApp.Worksheets[index];
            if (!(sheet is IWorksheet)) return null;
            return ((IWorksheet)sheet).Name; 
        }

        public List<String> getSheetNames()
        {
            List<String> names = new List<String>(0);
            IApplication xlApp = this.excelEngine1.Excel;
            if (xlApp.Worksheets == null) return names;
            foreach (Object sheet in xlApp.Worksheets)
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
                IApplication xlApp = this.excelEngine1.Excel;
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                try
                {
                    if (xlWorkBook.ActiveSheet is IWorksheet)
                    {
                        IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;
                        Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);
                        Ui.Office.Range range = new Ui.Office.Range(sheet);

                        IRange xlRange = xlWorkSheet.Application.ActiveCell;
                        IRange xlRangeCourant = xlWorkSheet.Range;
                        Range rangeCourant = new Range(sheet);

                        RangeItem itemCourant = new RangeItem(xlRange.Cells[1].Row, xlRange.Cells[1].Row,
                            xlRange.Cells[1].Column, xlRange.Cells[1].Column);
                        rangeCourant.Items.Add(itemCourant);
                        return rangeCourant;
                    }
                }
                catch (Exception) 
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
            IApplication xlApp = this.excelEngine1.Excel;
             try
             {
                 IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                 IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet as IWorksheet;

                 for (int i = 1; i <= xlWorkSheet.Rows.Count();  i++)
                 {
                     IRange ir = xlWorkSheet.Rows[i];
                     ir.Clear();
                 }
             }
             catch (Exception ) { }
        }

        public void deleteExcelRow(int row) {
            IApplication xlApp = this.excelEngine1.Excel;            
            try
            {
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet as IWorksheet;
                xlWorkSheet.Rows[row].Clear(ExcelMoveDirection.MoveUp);
            }
            catch (Exception ) { }
        }

        public void deleteExcelCol(int row,int Col)
        {
            IApplication xlApp = this.excelEngine1.Excel;   
            try
            {
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet as IWorksheet;
                xlWorkSheet.Columns[Col].Clear(ExcelMoveDirection.MoveLeft);
            }
            catch (Exception ) { }
        }
        
        public void DisableSheet(bool disable = true)
        {
            IApplication xlApp = this.excelEngine1.Excel;
            try
            {
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet workSheet = xlWorkBook.ActiveSheet as IWorksheet;
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

        public void protectSheet(bool protect = true) {
            if (protect)
            {
                this.Office.Excel.ActiveWorkbook.PasswordToOpen = "0x00000001";
            }
            else this.Office.Excel.ActiveWorkbook.PasswordToOpen = string.Empty;
        }

        public void SetColorAt(int row, int colunm, string sheetName, int color)
        {
            IRange cell = getCellAt(row, colunm, sheetName);
            if (cell != null) cell.CellStyle.Color = Color.FromArgb(color); 
        }

        public void SetColorAt(int row, int colunm, int color)
        {
            IRange cell = getCellAt(row, colunm);
            if (cell != null) cell.CellStyle.Color = Color.FromArgb(color);
        }

        public void SetValueAt(int row, int colunm, string sheetName, object value, int color)
        {
            IRange cell = getCellAt(row, colunm, sheetName);
            if (cell != null)
            {
                cell.Value = value.ToString();
                cell.CellStyle.Color = Color.FromArgb(color);
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
            this.Office.Excel.ActiveSheet.Range[column, row].Text = value.ToString();
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
            this.Office.Excel.ActiveSheet.Range[column, row].Text = value;
        }
        
        public void SetValueAt(int row, int colunm, object value)
        {
            IRange cell = getCellAt(row, colunm);
            if (cell != null) cell.Value = value.ToString();
        }

        public IRange getCellAt(int row, int column)
        {
            IApplication xlApp = this.excelEngine1.Excel;
            if (xlApp.ActiveSheet is IWorksheet)
            {
                IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet as IWorksheet;
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
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
                IApplication xlApp = this.excelEngine1.Excel;
                
                if (sheetName == null) return null;
                try
                {
                    for (int i = 1; i <= xlApp.Worksheets.Count; i++)
                    {
                        if (xlApp.Worksheets[i] is IWorksheet)
                        {
                            IWorksheet sheet = xlApp.Worksheets[i];
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
                catch (Exception ) 
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
            IWorksheet workSheet = this.Office.Excel.ActiveSheet;
            workSheet.Range[colunm, row].Clear();
        }

        /// <summary>
        /// Retourne la cellule active
        /// </summary>
        /// <returns>La cellule active</returns>
        public Cell getActiveCell()
        {
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            if (xlWorkBook == null) return null;
            try
            {
                IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;

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
            catch (Exception ) 
            {
                return null;
            }
        }

        public object[,] getColumnsTitles(IWorksheet sheet, bool IsFirstRow, bool isRangeSelected = false, bool canSelectRange = false)
       {
           IApplication xlApp = this.excelEngine1.Excel;
           IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
           try
           {
               IWorksheet xlWorkSheet = xlWorkBook.ActiveSheet;
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
           IApplication xlApp = this.excelEngine1.Excel;
           IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
           IWorksheet xlWorkSheet = null;
           try
           {
                xlWorkSheet = xlApp.ActiveSheet;
           }
           catch (Exception ) 
           {
               return new System.Drawing.Rectangle(1,1, 1, 1);
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

            debutLigne = FirstRowIsHeader ? debutLigne == 1 ? debutLigne + 1:debutLigne : debutLigne;
           

           return new System.Drawing.Rectangle(debutLigne, debutCol, finLigne, finCol);
       
       }
               
       public Range getUsableRange(Range selectedRange = null,bool IsSelected=false,bool IsFirstRow=false)
       {
           System.Drawing.Rectangle selection = GetSelectionBounds(selectedRange,IsSelected,IsFirstRow);
           int debutLigne = selection.X;
           int debutCol = selection.Y;

           int finLigne = selection.Width;
           int finCol = selection.Height;
           RangeItem rangeItem = new RangeItem(debutLigne, finLigne, debutCol, finCol);
           Range UsableRange = new Range();
           UsableRange.Items.Add(rangeItem);
           return UsableRange;
       }

       public object[,] getColumnsTitles(bool IsFirstRow,Range selectedRange=null)
       {
           
           IApplication xlApp = this.excelEngine1.Excel;
           IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
           IWorksheet xlWorkSheet = null;
           try
           {
               xlWorkSheet = xlApp.ActiveSheet;
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

       public List<int> getColumnsIndexes(Range selectedRange=null) 
       {
           List<int> returnlist = new List<int>(0);
           

           System.Drawing.Rectangle selection = GetSelectionBounds(selectedRange,selectedRange!=null);
           int debutCol = selection.Y;
           int finCol = selection.Height;
           
           for (int j = debutCol; j <= finCol; j++)
           {
               returnlist.Add(j);
           }
           return returnlist;
       }

       public int GetSelectedColumn(Range selectedRange = null, bool IsSelected = false,bool isFirstRow=false) 
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
               if ((range.Cells[0].Column >= debutCol && range.Cells[range.CellCount-1].Column <= finCol) &&
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
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            try
            {
                IWorksheet xlWorkSheet = xlApp.ActiveSheet;
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
            IApplication xlApp = this.excelEngine1.Excel;
            try
            {
                IWorksheet sheet = xlApp.ActiveSheet;
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
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            IWorksheet sheet = null;
            try
            {
                if (xlApp.ActiveSheet is IWorksheet)
                {
                    sheet = xlApp.ActiveSheet;
                    List<Sheet> listeSheet = new List<Sheet>(0);
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        IWorksheet foundSheet = xlApp.Worksheets[i];
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
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            return xlWorkBook.Worksheets.Count;
        }

        public int getNumberOfCellsInCol(int sheetId,int indexCol,bool isFirsRowIn) 
        {
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            IWorksheet foundSheet = xlApp.Worksheets[sheetId];
            IRange usedRange = foundSheet.UsedRange;

            return !isFirsRowIn ? usedRange.Cells[usedRange.Count].Row : usedRange.Cells[usedRange.Count].Row - 1;
        }
           
        /// <summary>
        /// Rend visible/invisible la barre d'outils Excel
        /// </summary>
        /// <param name="value">True=>Visible; False=>Invisible</param>
        public void DisableToolBar(bool value)
        {
            //this.Office.Toolbars = !value;
        }

        /// <summary>
        /// Rend visible/invisible la barre de formule
        /// </summary>
        /// <param name="value">True=>InVisible; False=>Visible</param>
        public void DisableFormualaBar(bool value) 
        {
            IApplication xlApp = this.excelEngine1.Excel;
            IWorkbook xlWorkBook = xlApp.ActiveWorkbook;
            IWorksheet sheet = xlApp.ActiveSheet;

            if(!value) sheet.EnableSheetCalculations();
            else sheet.DisableSheetCalculations();
        }

        /// <summary>
        /// Rend visible/invisible la barre de titre excel
        /// </summary>
        /// <param name="value">True=>Visible; False=>Invisible</param>
        public void DisableTitleBar(bool value)
        {
            IApplication xlApp = this.Office.Excel;
            
        }

        public void ChangeTitleBarCaption(string title)
        {
            IApplication xlApp = this.Office.Excel;
            //if (xlApp != null) xlApp.ActiveWorkbook. = title;
            
        }
        
        const long COLOR_ACTIVECAPTION = 2;

        [DllImport("user32.dll")]
        private static extern long SetSysColors(long nChanges ,  long lpSysColor,   long lpColorValues);
        
        [DllImport("user32.dll")]
        private static  extern long GetSysColor(long nIndex);
        
        static void test()
        {
            long colorsStatic = GetSysColor(COLOR_ACTIVECAPTION);
        }

        #endregion


        #region Handlers

        /// <summary>
        /// 
        /// 
        /// </summary>
        protected void InitializeHandlers()
        {
            //this.Office.WorkbookNewSheet += Office_WorkbookNewSheet;
            //this.Office.SheetActivate += Office_SheetActivate;
            //this.Office.SheetChange +=Office_SheetChange;
            //this.Office.WindowSelectionChange +=Office_WindowSelectionChange;
            //this.Office.WindowBeforeRightClick +=Office_WindowBeforeRightClick;
            this.MouseDown +=EdrawOffice_MouseDown;
        }

      
        private void EdrawOffice_MouseDown(object sender, MouseEventArgs e)
        {
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
            //    Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;

            //    OfficeProperties.CommandBar sheetMenu = xlApp.ActiveWorkbook.Application.CommandBars["Ply"];
            //    if (sheetMenu.Visible == true)
            //    {
            //        for (int j = sheetMenu.Controls.Count; j > 0; j--)
            //        {
            //            OfficeProperties.CommandBarControl control = sheetMenu.Controls[j];
            //            if (control.Caption == "&Renommer")
            //            {
            //                menuHeader = (OfficeProperties.CommandBarButton)control;
            //                menuHeader.Click += menuHeader_Click;
            //            }
                        
            //        }
            //    }
            //}
        }

        private void Office_WindowBeforeRightClick(object sender, EventArgs e)
        {
            //if (this.Office.GetCurrentProgID() == EXCEL_ID)
            //{
                //if (OnBeforeRightClick != null) OnBeforeRightClick();
            //}
        }

        private void Office_WindowSelectionChange(object sender, EventArgs e)
        {
            //ExcelEventArg eventForRangeEdition = new ExcelEventArg() { };
            //eventForRangeEdition.Sheet = getActiveSheet();
            //eventForRangeEdition.Range = GetSelectedRange();
            //_clipboardViewerNext = SetClipboardViewer(this.Handle);
            //if (ThrowEvent && SelectionChanged != null) SelectionChanged(eventForRangeEdition);
        }

        private void Office_SheetChange(object sender, EventArgs e)
        {
            //ExcelEventArg eventForRangeEdition = new ExcelEventArg() {};
            //Range previousRange = rangePreviousValue;
            //Range range = GetSelectedRange();
            //if (range == null) return;
            //if (range.CellCount > 1) eventForRangeEdition.Range = range;
            //else eventForRangeEdition.Range = previousRange;
            //if (ThrowEvent && Edited != null)
            //{
            //    Edited(eventForRangeEdition);
            //}
            //if (ThrowEvent && Changed != null) Changed();
        }

        private void Office_SheetActivate(object sender, EventArgs e)
        {
            if (ThrowEvent && SheetActivated != null) SheetActivated(); 
        }

        private void Office_WorkbookNewSheet(object sender, EventArgs e)
        {
            if (ThrowEvent && SheetAdded != null) SheetAdded();
            if (ThrowEvent && DisableAddingSheet != null) this.DeleteExcelSheet();   
        }
              


              
        #endregion
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        private const int WM_DRAWCLIPBOARD = 0x0308;        // WM_DRAWCLIPBOARD message
        private IntPtr _clipboardViewerNext;     // Our variable that will hold the value to identify the next window in the clipboard viewer chain


    }
}
