using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using System.Runtime.InteropServices;
using OfficeProperties = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using Misp.Kernel.Util;

namespace Misp.Kernel.Ui.Office.EDraw
{
    /// <summary>
    /// Cette classe permet permet l'affichage d'un classeur excel avec rien que la grille des lignes et des colonnes dans
    ///  une unique feuille.
    /// </summary>
    public class SheetPanel: EdrawOffice
    {

        public event OnRightClickEventHandler OnRightClick;
        public delegate void  OnRightClickEventHandler();

        #region SheetPanel Constructor
        public SheetPanel() 
        {

            this.Office.WorkbookNewSheet +=Office_WorkbookNewSheet;
            this.Office.SheetChange += Office_SheetChange;
            
            if (this.Office.GetCurrentProgID() == EXCEL_ID) 
            {
               Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
               xlApp.SheetBeforeRightClick +=xlApp_SheetBeforeRightClick;
                
               Excel.Worksheet workSheet = xlApp.ActiveWorkbook.ActiveSheet as Excel.Worksheet;
               Excel.Workbook workBook = xlApp.ActiveWorkbook as Excel.Workbook;
               
            }
            
        }

     

        private void xlApp_SheetBeforeRightClick(object Sh, Excel.Range Target, ref bool Cancel)
        {
            if (OnRightClick != null)
                OnRightClick();
        }

         
        public Range selectedRange { get; set; }
 

        public string currentSheetName { get; set; }
 
  
        private void xlApp_SheetSelectionChange(object Sh, Excel.Range Target)
        {

            Console.WriteLine("La selection change");

        }

        private void xlApp_SheetBeforeDoubleClick(object Sh, Excel.Range Target, ref bool Cancel)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
              //  xlApp.ActiveCell.ClearContents();
                Cancel = true;
            }
        }
   
        #endregion
   
        #region SheetPanel Handlers
        /// <summary>
        /// Cette méthode permet de supprimer la feuille excel nouvellement créée.
        /// Ceci nous permet de n'avoir qu'une seule feuille présente dans le SheetPanel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Office_WorkbookNewSheet(object sender, EventArgs e)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;
                xlApp.DisplayAlerts = false;
                Excel._Worksheet workSheet = (Excel._Worksheet)xlApp.ActiveWorkbook.ActiveSheet;
                //workSheet.Delete();
            }
        }


        private void Office_SheetChange(object sender, EventArgs e)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
                xlApp.DisplayAlerts = false;
                if (xlApp.ActiveWorkbook.Sheets.Count > 1)
                {
                    Excel.Worksheet workSheet = (Excel.Worksheet)xlApp.ActiveWorkbook.ActiveSheet;
                    workSheet.Delete();
                }
            }            
        }
        #endregion

        #region SheetPanel methods

     

        /// <summary>
        /// Building of a SheetPanel component.
        /// SheetPanel component is a minimun view excel sheet.
        /// No TitleBar
        /// No ToolBar
        /// No Status Bar
        /// No Formula Bar
        /// No Context menu
        /// Just a sheet.
        /// </summary>
        public void BuildSheetPanelMethod() 
        {
            if (isExcel2013())
            {
                return;
            }
            this.DisableTitleBar(true);
            this.DisableToolBar(true);
            this.DisplayExcelFormulaBar(false);
            this.DisplayExcelStatusBar(false);
            this.DisableExcelSheetContextMenuItems();
           // this.DisableSheet();
            this.DeleteExcelSheet();
        }

        public bool isExcel2013()
        {
           return ExcelUtil.excelVersion == "15.0";
        }

        /// <summary>
        /// Build the sheetPanel component without removing Sheets.
        /// </summary>
        /// <param name="i"></param>
        public void BuildSheetPanelMethod(int i =1)
        {
            this.DisableTitleBar(true);
            this.DisableToolBar(true);
            this.DisplayExcelFormulaBar(false);
            this.DisplayExcelStatusBar(false);
            this.DisableSheet();
            this.DisableExcelSheetContextMenuItems();
        }

        /// <summary>
        /// Cette fonction permet de cacher/afficher la barre de formule d'Excel.
        /// </summary>
        /// <param name="isShowed"></param>
        public void DisplayExcelFormulaBar(bool isShowed)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
                xlApp.DisplayFormulaBar = isShowed;
                
            }
        }
       
        /// <summary>
        /// Cette méthode permet de rendre read-only une feuille Excel.
        /// </summary>
        /// <param name="password"></param>
        public void DisableExcelSheet(string password = "PASSWORD")
        {
            this.Office.ProtectDoc(0x00000002, password);

        }
    
        public void ProtectSheet(String password ="PASSWORD"){
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;
                
                xlApp.ActiveSheet.Protect(password, true, false, true,
                        true, true, true,
                        true, false, false,
                        false, false, false,
                        false, false, true);
            }
        }
       
        
   
        /// <summary>
        /// Cette fonction permet de désactiver/activer le menu contextuel d'une feuille.
        /// Lorsqu'on clique-droit sur une feuille.
        /// </summary>
        /// <param name="isDisable"></param>
        public void DisableExcelSheetContextMenuItems(bool isDisable = true)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;

                OfficeProperties.CommandBar sheetMenu = xlApp.ActiveWorkbook.Application.CommandBars["Ply"];
                
                for (int j = sheetMenu.Controls.Count; j > 0; j--)
                {
                    OfficeProperties.CommandBarControl control = sheetMenu.Controls[j];
                    control.Enabled = !isDisable;
                }
            }
        }
   
        /// <summary>
        /// Cette fonction permet de désactiver/activer le menu contexte d'Excel. Lorqu'on clique droit sur une cellule.
        /// </summary>
        /// <param name="isDisable"></param>
        public void DisableExcelContextMenuItems(bool isDisable = true)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel._Application xlApp = this.Office.GetApplication() as Excel._Application;
                OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];

                for (int i = contextMenu.Controls.Count; i > 0; i--)
                {
                    OfficeProperties.CommandBarControl control = contextMenu.Controls[i];
                    control.Enabled = !isDisable;
                }
            }
        }
     
        /// <summary>
        /// Cette fonction permet de cacher/afficher la barre de status d'Excel.
        /// </summary>
        /// <param name="isShowed"></param>
        public void DisplayExcelStatusBar(bool isShowed)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
                xlApp.DisplayStatusBar = isShowed;
            }
        }

        /// <summary>
        /// Verify the presence of the column in the Column listBox.
        /// This in order to able/disable Add/Remove Excel menu.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="isRangeSelected"></param>
        /// <returns>-1=> the selected column is not within the current usable range ;
        /// 0=> the selected column is in the listbox (can be removed) ;1 => the selected column is not in the listbox (can be add)</returns>
        public bool IsColumnPresentInList(int columnIndex, Range selectedRange=null) 
        {
            Point bounds = GetSelectionColumnBounds(selectedRange);
            int debutCol = (int)bounds.X;
            int finCol = (int)bounds.Y;
            if (columnIndex < debutCol) return false;
            if (columnIndex > finCol) return false;

            return findInList(columnIndex,getColumnsIndexes(selectedRange)) ? true : false;
        
        }

        public bool findInList(int columnIndex,List<int> liste) 
        {
            bool found = false;
            int fin =liste.Count-1;
            int debut =0;
            int mil = 0;
            do
            {
               mil = (int)((fin+debut)/2);
               if (liste[mil] == columnIndex)
                   found = true;
               else
               {
                   if (liste[mil] > columnIndex)
                   {
                       fin =mil - 1;
                       if (liste[fin] == columnIndex)
                           found = true;
                   }
                   else
                   {
                       debut = mil + 1;
                       if (liste[debut] == columnIndex)
                           found = true;
                   }
               }
            }
            while (!found && debut <= fin);
            return found;
        }
   
        private  Point GetSelectionLinesBounds(Range selectedRange=null) 
        {
            Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
            Excel.Worksheet xlWorkSheet = xlApp.ActiveSheet as Excel.Worksheet;
            Excel.Range UsedRange = null, UsableRange = null;
            int debutLigne , finLigne;

            if (selectedRange!=null)
            {
                //UsableRange = xlApp.Selection as Excel.Range;
                UsedRange = xlWorkSheet.UsedRange;

                int debutLigneUsedRange = UsedRange.Cells[1].Row;
                int finLigneUsedRange = UsedRange.Cells[UsedRange.Cells.Count].Row;

                int debutColUsedRange = UsedRange.Cells[1].Column;
                int finColUsedRange = UsedRange.Cells[UsedRange.Cells.Count].Column;

                int debutLigneUsableRange = selectedRange.Cells[0].Row;
                int finLigneUsableRange = selectedRange.Cells[selectedRange.Cells.Count-1].Row;

                int debutColUsableRange = selectedRange.Cells[0].Column;
                int finColUsableRange = selectedRange.Cells[selectedRange.Cells.Count-1].Column;


                debutLigne = (debutLigneUsedRange < debutLigneUsableRange) ? debutLigneUsableRange : debutLigneUsedRange;
                finLigne = (finLigneUsedRange > finLigneUsableRange) ? finLigneUsableRange : finLigneUsedRange;
            }
            else
            {
                UsableRange = xlWorkSheet.UsedRange;
                UsedRange = UsableRange;

                debutLigne = UsedRange.Cells[1].Row;
                finLigne = UsedRange.Cells[UsedRange.Cells.Count].Row;
            }
            return new Point(debutLigne, finLigne);
        }

        protected Point GetSelectionColumnBounds(Range selectedRange=null)
        {
            Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
            Excel.Workbook xlWorkBook = xlApp.ActiveWorkbook;
            Excel.Worksheet xlWorkSheet = xlApp.ActiveSheet;

            Excel.Range UsedRange = null, UsableRange = null;

            int debutCol, finCol;

            Ui.Office.Sheet sheet = new Ui.Office.Sheet(xlWorkSheet.Index, xlWorkSheet.Name);

            if (selectedRange != null)
            {
                UsedRange = xlWorkSheet.UsedRange;

                int debutLigneUsedRange = UsedRange.Cells[1].Row;
                int finLigneUsedRange = UsedRange.Cells[UsedRange.Cells.Count].Row;

                int debutColUsedRange = UsedRange.Cells[1].Column;
                int finColUsedRange = UsedRange.Cells[UsedRange.Cells.Count].Column;

                int debutLigneUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[0].Row:selectedRange.Items[0].Row1;
                int finLigneUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[selectedRange.Cells.Count - 1].Row : selectedRange.Items[0].Row2;

                int debutColUsableRange = selectedRange.CellCount > 0 ?  selectedRange.Cells[0].Column : selectedRange.Items[0].Column1;
                int finColUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[selectedRange.Cells.Count - 1].Column : selectedRange.Items[0].Column2;

                debutCol = (debutColUsedRange < debutColUsableRange) ? debutColUsableRange : debutColUsedRange;
                finCol = (finColUsedRange > finColUsableRange) ? finColUsableRange : finColUsedRange;
            }
            else
            {
                UsableRange = xlWorkSheet.UsedRange;
                UsedRange = UsableRange;
                
                debutCol = UsedRange.Cells[1].Column;
                finCol = UsedRange.Cells[UsedRange.Cells.Count].Column;
            }
            return new Point(debutCol,finCol);

        } 
        
        public List<string> GetSelectedColumnLinesValues(int columnIndex, Range selectedRange=null,bool isFirstRowHeader=false) 
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID) 
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
                Excel.Worksheet xlWorkSheet = xlApp.ActiveSheet as Excel.Worksheet;
                List<string> listeRetour = new List<string>(0);

                Point bounds = GetSelectionLinesBounds(selectedRange);

                int debutLigne = (int)bounds.X, finLigne = (int)bounds.Y;

                if (isFirstRowHeader) debutLigne = debutLigne == 1 ? debutLigne + 1 : debutLigne;
                

                for (int i = debutLigne; i <= finLigne; i++)
                {
                    if (this.getValueAt(i, columnIndex, xlWorkSheet.Name) != null)
                    {
                        string lineValue = this.getValueAt(i, columnIndex, xlWorkSheet.Name).ToString();

                        for (int p = listeRetour.Count - 1; p >= 0; p--) 
                        {
                            if (lineValue == listeRetour[p])
                            {
                                listeRetour.RemoveAt(p);
                                break;
                            }
                        }
                        listeRetour.Add(lineValue);

                    }
                }
                return listeRetour;
            }
            return null;
        }

        public int GetLinesBounds(int columnIndex,Range selectedRange=null) 
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
                Excel.Worksheet xlWorkSheet = xlApp.ActiveSheet as Excel.Worksheet;
                List<string> listeRetour = new List<string>(0);

                Point bounds = GetSelectionLinesBounds(selectedRange);

                int debutLigne = (int)bounds.X, finLigne = (int)bounds.Y;

                for (int i = debutLigne; i <= finLigne; i++)
                {
                    if (this.getValueAt(i, columnIndex, xlWorkSheet.Name) != null)
                    {
                        string lineValue = this.getValueAt(i, columnIndex, xlWorkSheet.Name).ToString();

                        for (int p = listeRetour.Count - 1; p >= 0; p--)
                        {
                            if (lineValue == listeRetour[p])
                            {
                                listeRetour.RemoveAt(p);
                                break;
                            }
                        }
                        listeRetour.Add(lineValue);

                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Active/désactive un menu item du menu contextuel d'excel.
        /// </summary>
        /// <param name="active"></param>
        public void ActivateExcelContextMenuItem(bool active,string label)
        {
            if (this.Office.GetCurrentProgID() == EXCEL_ID)
            {
                Excel.Application xlApp = this.Office.GetApplication() as Excel.Application;
                OfficeProperties.CommandBar contextMenu = xlApp.CommandBars["Cell"];
                int j = 1;
                for (int i = contextMenu.Controls.Count; i > 0; i--)
                {
                    
                    OfficeProperties.CommandBarControl control = contextMenu.Controls[j];
                    if (control.Caption == label)
                    {
                        control.Enabled = active;
                        break;
                    }
                    j++;
                }
            }
        }
        #endregion

       
    }
}
