using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Ui.Office.DevExpressSheet
{
    public class DESheetPanel : DESpreadsheet
    {
        public event OnAddColumnEventHandler OnAddColumn;
        public delegate void OnAddColumnEventHandler(int index);

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(int index);

        public DESheetPanel(): base()
        {
            this.ribbonControl1.Visibility = System.Windows.Visibility.Collapsed;
            this.DisableFormualaBar(true);
            this.DisableTitleBar(true);
            this.DisableToolBar(true);
        }



        public List<Sheet> getAllExcelSheets()
        {
            List<Sheet> Sheets = new List<Sheet>(0);
            DevExpress.Spreadsheet.IWorkbook workbook = this.spreadsheetControl.Document;
            foreach (DevExpress.Spreadsheet.Worksheet worksheet in workbook.Worksheets)
            {
                Sheets.Add(new Sheet(worksheet.Index+1,worksheet.Name));
            }
            return Sheets;
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
            DevExpress.Spreadsheet.Worksheet workSheet;
            if (this.spreadsheetControl.ActiveWorksheet != null)
            {
                workSheet = this.spreadsheetControl.ActiveWorksheet;
            }
            else
            {
                return new System.Drawing.Rectangle(1, 1, 1, 1);
            }
            DevExpress.Spreadsheet.Range selection = this.spreadsheetControl.Selection;
            //if (selection == null) return  new System.Drawing.Rectangle(1, 1, 1, 1);


            DevExpress.Spreadsheet.Range UsedRange = null, UsableRange = null;

            int debutLigne, finLigne, debutCol, finCol;

            Ui.Office.Sheet sheet = new Ui.Office.Sheet(workSheet.Index+1, workSheet.Name);
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
                UsedRange = workSheet.GetUsedRange();

                if (selectedRange != null)
                {
                    debutLigneUsedRange = UsedRange.TopRowIndex + 1;
                    finLigneUsedRange = UsedRange.BottomRowIndex + 1;
                    debutColUsedRange = UsedRange.LeftColumnIndex + 1;
                    finColUsedRange = UsedRange.RightColumnIndex + 1; ;


                    debutLigneUsableRange = selectedRange.Cells[0].Row;
                    finLigneUsableRange = selectedRange.Cells[selectedRange.Cells.Count - 1].Row;

                    debutColUsableRange = selectedRange.Cells[0].Column;
                    finColUsableRange = selectedRange.Cells[selectedRange.Cells.Count - 1].Column;

                }
                else
                {
                    UsableRange = selection;
                    debutLigneUsedRange = UsedRange.TopRowIndex +1;
                    finLigneUsedRange = UsedRange.BottomRowIndex +1;

                    debutColUsedRange = UsedRange.LeftColumnIndex + 1;
                    finColUsedRange = UsedRange.RightColumnIndex +1;

                    debutLigneUsableRange = UsableRange.TopRowIndex +1;
                    finLigneUsableRange = UsableRange.BottomRowIndex +1;

                    debutColUsableRange = UsableRange.LeftColumnIndex +1;
                    finColUsableRange = UsableRange.RightColumnIndex +1;

                }
                debutLigne = (debutLigneUsedRange < debutLigneUsableRange) ? debutLigneUsableRange : debutLigneUsedRange;
                finLigne = (finLigneUsedRange > finLigneUsableRange) ? finLigneUsableRange : finLigneUsedRange;

                debutCol = (debutColUsedRange < debutColUsableRange) ? debutColUsableRange : debutColUsedRange;
                finCol = (finColUsedRange > finColUsableRange) ? finColUsableRange : finColUsedRange;
            }
            else
            {
                UsableRange = workSheet.GetUsedRange(); 
                UsedRange = UsableRange;

                debutLigne = UsedRange.TopRowIndex+1;
                finLigne = UsedRange.BottomRowIndex+1;

                debutCol = UsedRange.LeftColumnIndex+1;
                finCol = UsedRange.RightColumnIndex +1;
            }

            debutLigne = FirstRowIsHeader ? debutLigne == 1 ? debutLigne + 1 : debutLigne : debutLigne;


            return new System.Drawing.Rectangle(debutLigne, debutCol, finLigne, finCol);

        }

        protected override void menuItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            string menu = e.Item.Content.ToString();
            switch (menu)
            {
                case ADD_AUTOMATICCOLUMN_LABEL:
                    {
                        //Automatic Sourcing . Permet de retirer colonne 
                        // de la liste des colonne à paramètrer.
                        OnAddColumn(getActiveCell().Column);
                    }
                    break;
                case REMOVE_AUTOMATICCOLUMN_LABEL:
                    {
                        //Automatic Sourcing . Permet d'ajouter une nouvelle colonne 
                        // dans la liste des colonne à paramètrer.
                        if (OnRemoveColumn != null)
                            OnRemoveColumn(getActiveCell().Column);
                    }
                    break;
            }
        }


        protected Point GetSelectionColumnBounds(Range selectedRange = null)
        {
            DevExpress.Spreadsheet.Worksheet workSheet = null;
            if (this.spreadsheetControl.ActiveWorksheet != null)
            {
                workSheet = this.spreadsheetControl.ActiveWorksheet;
            }
            
            DevExpress.Spreadsheet.Range UsedRange = null, UsableRange = null;

            int debutCol, finCol;

            Ui.Office.Sheet sheet = new Ui.Office.Sheet(workSheet.Index+1, workSheet.Name);

            if (selectedRange != null)
            {
                UsedRange = workSheet.GetUsedRange();

                int debutLigneUsedRange = UsedRange.TopRowIndex +1;
                int finLigneUsedRange = UsedRange.BottomRowIndex+1;

                int debutColUsedRange = UsedRange.LeftColumnIndex + 1;
                int finColUsedRange = UsedRange.RightColumnIndex +1;

                int debutLigneUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[0].Row : selectedRange.Items[0].Row1;
                int finLigneUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[selectedRange.Cells.Count - 1].Row : selectedRange.Items[0].Row2;

                int debutColUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[0].Column : selectedRange.Items[0].Column1;
                int finColUsableRange = selectedRange.CellCount > 0 ? selectedRange.Cells[selectedRange.Cells.Count - 1].Column : selectedRange.Items[0].Column2;

                debutCol = (debutColUsedRange < debutColUsableRange) ? debutColUsableRange : debutColUsedRange;
                finCol = (finColUsedRange > finColUsableRange) ? finColUsableRange : finColUsedRange;
            }
            else
            {
                UsableRange = workSheet.GetUsedRange();
                UsedRange = UsableRange;

                debutCol = UsedRange.LeftColumnIndex + 1;
                finCol = UsedRange.RightColumnIndex + 1;
            }
            return new Point(debutCol, finCol);

        }

        public bool findInList(int columnIndex, List<int> liste)
        {
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);
                if (liste[mil] == columnIndex)
                    found = true;
                else
                {
                    if (liste[mil] > columnIndex)
                    {
                        fin = mil - 1;
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

        /// <summary>
        /// Verify the presence of the column in the Column listBox.
        /// This in order to able/disable Add/Remove Excel menu.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="isRangeSelected"></param>
        /// <returns>-1=> the selected column is not within the current usable range ;
        /// 0=> the selected column is in the listbox (can be removed) ;1 => the selected column is not in the listbox (can be add)</returns>
        public bool IsColumnPresentInList(int columnIndex, Range selectedRange = null)
        {
            Point bounds = GetSelectionColumnBounds(selectedRange);
            int debutCol = (int)bounds.X;
            int finCol = (int)bounds.Y;
            if (columnIndex < debutCol) return false;
            if (columnIndex > finCol) return false;

            return findInList(columnIndex, getColumnsIndexes(selectedRange)) ? true : false;
        }
    }
}
