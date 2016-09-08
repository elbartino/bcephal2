using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Sourcing.Table
{
    /// <summary>
    /// Classe utilitaire implémentant l'algorithme d'application d'un design à une table.
    /// </summary>
    public class ApplyDesignUtil
    {

        double getCellDuration = 0;

        #region Properties

        private InputTableEditorItem page;
        private Periodicity periodicity;
        private string sheetName;
        private InputTable table;
        private Design design;

        private Dictionary<string, CellProperty> cellsDictionary;

        #endregion


        #region apply

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="design"></param>
        /// <param name="currentRange"></param>
        /// <param name="_periodicity"></param>
        /// <param name="showHeader"></param>
        public void apply(InputTableEditorItem page, Design design, Range currentRange, Periodicity periodicity, InputTableEditorController.canShowHeader showHeader)
        {
            DateTime time = DateTime.Now;

            this.cellsDictionary = new Dictionary<string, CellProperty>(0);
            this.design = design;
            this.page = page;
            this.table = page.EditedObject;
            this.periodicity = periodicity;
            this.sheetName = currentRange.Sheet.Name;
            System.Windows.Point coord = GetCellsCoord(design);
             
            DateTime time1 = DateTime.Now;
            BuildColunms(currentRange, coord, showHeader.columns);
            var duration = (DateTime.Now - time1).TotalSeconds;
            Console.Out.WriteLine("BuildColunms : " + duration);

            time1 = DateTime.Now;
            BuildRows(currentRange, coord, showHeader.lines);
            duration = (DateTime.Now - time1).TotalSeconds;
            Console.Out.WriteLine("BuildRows : " + duration);

            BuildCentralPart(currentRange,showHeader.central);

            time1 = DateTime.Now;
            BuildTotals(currentRange, coord, showHeader.columns, showHeader.lines);
            duration = (DateTime.Now - time1).TotalSeconds;
            Console.Out.WriteLine("BuildRows : " + duration);

            time1 = DateTime.Now;
            table.cellPropertyListChangeHandler.AddNew(this.cellsDictionary.Values);
            duration = (DateTime.Now - time1).TotalSeconds;
            Console.Out.WriteLine("Add Cells : " + duration);
            
            duration = (DateTime.Now - time).TotalSeconds;
            Console.Out.WriteLine("Get Cells : " + getCellDuration + " secondes");
            Console.Out.WriteLine("Total     : " + duration + " secondes");
        }

        private void BuildTotals(Range range, Point coord, bool showColumnHeader, bool showRowHeader)
        {
            int columnCount = (int)coord.X != 0 ? (int)coord.X : 1;
            int rowCount = (int)coord.Y != 0 ? (int)coord.Y : 1;
            int rowSize = design.rows.lineListChangeHandler.Items.Count;
            int colSize = design.columns.lineListChangeHandler.Items.Count;

            Cell cell = range.Cells[0];

            if (design.addTotalColumnRight)
            {
                int row = cell.Row - 1;
                int col = cell.Column + columnCount;
                if (showColumnHeader)
                {
                    this.page.getInputTableForm().SpreadSheet.SetValueAt(row, col, sheetName, "TOTAL", Designer.DesignerForm.TOTAL_COLOR);
                   // this.page.getInputTableForm().SpreadSheet.SetValueAt2013(row, col, sheetName, "TOTAL");
                }
                for (row = cell.Row; row < cell.Row + rowCount; row++)
                {
                    String formula = "=SUM(" + Kernel.Util.DataFormater.getCellName(row, cell.Column) + ":"
                        + Kernel.Util.DataFormater.getCellName(row, col-1) + ")";
                    this.page.getInputTableForm().SpreadSheet.SetValueAt(row, col, sheetName, formula, Designer.DesignerForm.TOTAL_COLOR);
                    
                }
            }

            if (design.addTotalRowBelow)
            {
                int row = cell.Row + rowCount;
                int col = cell.Column - 1;
                if (showRowHeader)
                {
                    this.page.getInputTableForm().SpreadSheet.SetValueAt(row, col, sheetName, "TOTAL", Designer.DesignerForm.TOTAL_COLOR);
                    
                }
                for (col = cell.Column; col < cell.Column + columnCount; col++)
                {
                    String formula = "=SUM(" + Kernel.Util.DataFormater.getCellName(cell.Row, col) + ":"
                        + Kernel.Util.DataFormater.getCellName(row - 1, col) + ")";
                    this.page.getInputTableForm().SpreadSheet.SetValueAt(row, col, sheetName, formula, Designer.DesignerForm.TOTAL_COLOR);
                }
            }
        }

        protected void BuildCentralPart(Range range,bool showHeader)
        {
            Design design = this.design;
            string sheetName = this.page.getInputTableForm().SpreedSheet.getActiveSheetName();
            string value = "";
            string cont = "";
            foreach (DesignDimensionLine line in design.central.lineListChangeHandler.Items)
            {
                foreach (LineItem item in line.itemListChangeHandler.Items)
                {
                    value += cont + item.GetValue().ToString();
                    cont = " ; ";
                }
            }

            if (showHeader)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //this.page.getInputTableForm().SpreadSheet.deleteExcelRow(1);
                    this.page.getInputTableForm().SpreedSheet.SetValueAt(range.Items[0].Row1 - 1, range.Items[0].Column1 - 1, sheetName,value, Designer.DesignerForm.CENTRAL_COLOR);
                }
            }
        }

        /// <summary>
        ///Build columns
        /// </summary>
        /// <param name="range">Contains the current selected cell and the active sheet</param>
        /// <param name="coords">the number of columns and the number of lines</param>
        public void BuildColunms(Range range, System.Windows.Point coords, bool showHeader)
        {            
            int row = 1, col = 1;
            int columnCount = (int)coords.X;
            int rowCount = (int)coords.Y != 0 ? (int)coords.Y : 1;
            Cell cell = range.Cells[0];

            ObservableCollection<LineItem> centralLine = design.central.lineListChangeHandler.Items.Count > 0
                && design.central.lineListChangeHandler.Items[0].itemListChangeHandler.Items.Count > 0 ?
                design.central.lineListChangeHandler.Items[0].itemListChangeHandler.Items :
                new ObservableCollection<LineItem>();

            ObservableCollection<DesignDimensionLine> Lines = new ObservableCollection<DesignDimensionLine>(design.columns.lineListChangeHandler.Items);
            int lineCount = Lines.Count;
            if (lineCount > 0)
            {
                int totalCardinality = getCartesianProductLineSize(Lines);
                row = lineCount;
                int inc = 1;
                for (int currentLineRang = lineCount; currentLineRang >= 1; currentLineRang--)
                {
                    col = design.rows.lineListChangeHandler.Items.Count > 0 ? design.rows.lineListChangeHandler.Items.Count + 1 : 2;

                    DesignDimensionLine currentLine = Lines[currentLineRang - 1];
                    int currentLineItemCount = currentLine.GetItemCount();

                    ObservableCollection<DesignDimensionLine> AllLines = new ObservableCollection<DesignDimensionLine>(design.columns.lineListChangeHandler.Items);
                    AllLines.Remove(currentLine);
                    int cardinality = getCartesianProductLineSize(AllLines);
                    int addColIndex = 0;
                    if (currentLineRang == 1 && currentLine.GetItemCount() != 0)
                    {
                        for (int j = 1; j <= totalCardinality / currentLine.GetItemCount(); j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                if (showHeader)
                                {
                                    this.page.getInputTableForm().SpreedSheet.SetValueAt(cell.Row - inc, cell.Column + addColIndex, sheetName, item.GetValue().ToString(), Designer.DesignerForm.COLUMNS_COLOR);
                                   // this.page.getInputTableForm().SpreadSheet.SetValueAt(cell.Row - inc, cell.Column + addColIndex, sheetName, item.GetValue().ToString());
                                }
                                addColIndex++;
                                col++;
                            }
                        }
                    }
                    else
                    {
                        int n = totalCardinality - (currentLineItemCount * cardinality) + 1;
                        int m = cardinality;
                        if (currentLineRang != lineCount)
                        {
                            ObservableCollection<DesignDimensionLine> AllLine = design.columns.lineListChangeHandler.Items;
                            ObservableCollection<DesignDimensionLine> UPLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(0, currentLineRang - 1));
                            ObservableCollection<DesignDimensionLine> DOWNLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(currentLineRang, AllLine.Count - currentLineRang));

                            m = getCartesianProductLineSize(UPLines);
                            n = getCartesianProductLineSize(DOWNLines);
                        }
                        for (int j = 1; j <= n; j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                for (int i = 1; i <= m; i++)
                                {
                                    if (showHeader)
                                    {
                                        this.page.getInputTableForm().SpreedSheet.SetValueAt(cell.Row - inc, cell.Column + addColIndex, sheetName, item.GetValue().ToString(), Designer.DesignerForm.COLUMNS_COLOR);
                                        
                                    }
                                   for (int l = 0; l < rowCount; l++)
                                    {
                                        CellProperty cellProperty = GetCellProperty(cell.Row + l, cell.Column + addColIndex, true);
                                        RefreshCellProperty(cellProperty, centralLine);
                                       //this.page.getInputTableForm().SpreadSheet.SetColorAt(cell.Row-1;cell.Column-1,sheetName,item/
                                        RefreshCellProperty(cellProperty, item);
                                    }
                                    addColIndex++;
                                    col++;
                                }
                            }
                        }
                    }
                    inc++;
                    row--;
                }
            }
        }


        /// <summary>
        /// Apply design lines elements as cellproperty
        /// </summary>
        /// <param name="design">Choosen design</param>
        /// <param name="table">The InputTable on which you wish to apply design template</param>
        /// <param name="range">Contains the current selected cell and the active sheet</param>
        /// <param name="coords">the number of columns and the number of lines</param>
        public void BuildRows(Range range, System.Windows.Point coords, bool showHeader)
        {            
            int row = 1, col = 1;
            int columnCount = (int)coords.X != 0 ? (int)coords.X : 1;
            int rowCount = (int)coords.Y;
            Cell cell = range.Cells[0];
            ObservableCollection<DesignDimensionLine> Lines = new ObservableCollection<DesignDimensionLine>(design.rows.lineListChangeHandler.Items);
            int lineCount = Lines.Count;

            if (lineCount > 0)
            {
                int totalCardinality = getCartesianProductLineSize(Lines);
                col = lineCount;
                int inc = 1;
                for (int currentLineRang = lineCount; currentLineRang >= 1; currentLineRang--)
                {
                    row = design.columns.lineListChangeHandler.Items.Count > 0 ? design.columns.lineListChangeHandler.Items.Count + 1 : 2;
                    if (design.concatenateColumnHearder) row = 2;
                    DesignDimensionLine currentLine = Lines[currentLineRang - 1];
                    int currentLineItemCount = currentLine.GetItemCount();

                    ObservableCollection<DesignDimensionLine> AllLines = new ObservableCollection<DesignDimensionLine>(design.rows.lineListChangeHandler.Items);
                    AllLines.Remove(currentLine);
                    int cardinality = getCartesianProductLineSize(AllLines);
                    int addRowIndex = 0;
                    if (currentLineRang == 1 && currentLine.GetItemCount() != 0)
                    {
                        for (int j = 1; j <= totalCardinality / currentLine.GetItemCount(); j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                if (showHeader)
                                {
                                    int colone = cell.Column - inc > 0 ? cell.Column - inc : 1;
                                    this.page.getInputTableForm().SpreedSheet.SetValueAt(cell.Row + addRowIndex, colone, sheetName, item.GetValue().ToString(), Designer.DesignerForm.ROWS_COLOR);
                                    //this.page.getInputTableForm().SpreadSheet.SetColorAt(cell.Row + addRowIndex, colone, sheetName, Designer.DesignerForm.ROWS_COLOR);
                                }
                               for (int c = 0; c < columnCount; c++)
                                {
                                    CellProperty cellProperty = GetCellProperty(cell.Row + addRowIndex, cell.Column + c, false);
                                    RefreshCellProperty(cellProperty, item);
                                }
                                addRowIndex++;
                                row++;
                            }
                        }
                    }
                    else
                    {
                        int n = totalCardinality - (currentLineItemCount * cardinality) + 1;
                        int m = cardinality;

                        if (currentLineRang != lineCount)
                        {
                            ObservableCollection<DesignDimensionLine> AllLine = design.rows.lineListChangeHandler.Items;
                            ObservableCollection<DesignDimensionLine> UPLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(0, currentLineRang - 1));
                            ObservableCollection<DesignDimensionLine> DOWNLines = new ObservableCollection<DesignDimensionLine>(AllLine.ToList().GetRange(currentLineRang, AllLine.Count - currentLineRang));

                            m = getCartesianProductLineSize(UPLines);
                            n = getCartesianProductLineSize(DOWNLines);
                        }

                        for (int j = 1; j <= n; j++)
                        {
                            foreach (LineItem item in currentLine.itemListChangeHandler.Items)
                            {
                                for (int i = 1; i <= m; i++)
                                {
                                    if (showHeader)
                                    {
                                        int colone = cell.Column - inc > 0 ? cell.Column - inc : 1;
                                        this.page.getInputTableForm().SpreedSheet.SetValueAt(cell.Row + addRowIndex, colone, sheetName, item.GetValue().ToString(), Designer.DesignerForm.ROWS_COLOR);
                                        //this.page.getInputTableForm().SpreadSheet.SetColorAt(cell.Row + addRowIndex, colone, sheetName, Designer.DesignerForm.ROWS_COLOR);
                                    }
                                    for (int c = 0; c < columnCount; c++)
                                    {
                                        CellProperty cellProperty = GetCellProperty(cell.Row + addRowIndex, cell.Column + c, false);
                                        RefreshCellProperty(cellProperty, item);
                                    }
                                    addRowIndex++;
                                    row++;
                                }
                            }
                        }
                    }
                    inc++;
                    col--;
                }
            }
        }
    
        #endregion


        #region Utils

        /// <summary>
        /// Determines the number of cells by giving the number of columns and the number of rows in
        /// the design template.
        /// </summary>
        /// <param name="design"></param>
        /// <returns>The coordinates X = number of columns ,Y = number of rows. </returns>
        private System.Windows.Point GetCellsCoord(Design design)
        {
            int cols = getCartesianProductLineSize(design.columns.lineListChangeHandler.Items);
            int rows = getCartesianProductLineSize(design.rows.lineListChangeHandler.Items);
            return new System.Windows.Point((int)cols, (int)rows);
        }

        /// <summary>
        /// Cardinality
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private int getCartesianProductLineSize(ObservableCollection<DesignDimensionLine> lines)
        {
            int size = 1;
            foreach (DesignDimensionLine line in lines) size = size * line.GetItemCount();
            return size;
        }

        /// <summary>
        /// Retourne le CellProperty en fonction des coordonées.
        /// Si le cellProperty n'existe pas, on le crée.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private CellProperty GetCellProperty(int row, int col, bool update)
        {
            DateTime time = DateTime.Now;

            string cellName = Kernel.Util.RangeUtil.GetCellName(row, col);
            if (update)
            {
                CellProperty cp = table.GetCellProperty(cellName, sheetName);
                if (cp != null) table.RemoveCellProperty(cp);
            }

            CellProperty cellProperty = null;
            this.cellsDictionary.TryGetValue(cellName, out cellProperty);
            if (cellProperty != null) return cellProperty;

            object value = page.getInputTableForm().SpreedSheet.getValueAt(row, col, sheetName);
            cellProperty = new CellProperty(Kernel.Util.RangeUtil.GetColumnName(col) + row, row, col);
            cellProperty.nameSheet = sheetName;
            cellProperty.SetDecimalMeasure(value);

            this.cellsDictionary.Add(cellName, cellProperty);

            var duration = (DateTime.Now - time).TotalSeconds;
            getCellDuration += duration;
            Console.Out.WriteLine("Get Cell " + cellName + " : " + duration + " secondes");

            return cellProperty;

        }


        //private CellProperty GetCellProperty(int row, int col, bool update)
        //{
        //    DateTime time = DateTime.Now;

        //    CellProperty CellProperty = table.GetCellProperty(row, col, sheetName);
        //    object value = page.getInputTableForm().SpreadSheet.getValueAt(row, col, sheetName);

        //    if (CellProperty != null)
        //    {
        //        if (update) table.UpdateCellProperty(CellProperty);
        //    }
        //    else
        //    {
        //        CellProperty = new CellProperty(row.ToString(), Kernel.Util.RangeUtil.GetColumnName(col));
        //        CellProperty.nameSheet = sheetName;
        //        table.AddCellProperty(CellProperty);
        //    }
        //    CellProperty.SetDecimalMeasure(value);

        //    var duration = (DateTime.Now - time).TotalSeconds;
        //    getCellDuration += duration;
        //    Console.Out.WriteLine("Get Cell " + CellProperty.name + " : " + duration + " secondes");

        //    return CellProperty;
        //}

        /// <summary>
        /// Vérifie le type d'un objet et l'ajoute à la propriété correspondante 
        /// dans le Cellproperty.
        /// </summary>
        /// <param name="cellProperty"></param>
        /// <param name="item"></param>
        private void RefreshCellProperty(CellProperty cellProperty, ObservableCollection<LineItem> lineItems)
        {
            foreach(LineItem lineItem in lineItems)
            {
                RefreshCellProperty(cellProperty, lineItem);
            }
        }

        /// <summary>
        /// Vérifie le type d'un objet et l'ajoute à la propriété correspondante 
        /// dans le Cellproperty.
        /// </summary>
        /// <param name="cellProperty"></param>
        /// <param name="item"></param>
        private void RefreshCellProperty(CellProperty cellProperty, LineItem lineItem)
        {
            object item = lineItem.GetValue();
            if (item is Target)
            {
                Target target = (Target)item;
                if (cellProperty.cellScope == null)
                {
                    cellProperty.cellScope = GetNewScope();
                    cellProperty.cellScope.name = target.name;
                }
                else cellProperty.cellScope.name += TargetItem.Operator.AND.ToString() + " " + target.name;

                TargetItem targetItem = new TargetItem();
                targetItem.value = target;
                targetItem.operatorType = TargetItem.Operator.AND.ToString();
                cellProperty.cellScope.AddTargetItem(targetItem);
            }

            if (item is Measure)
            {
                if (cellProperty.cellMeasure == null) cellProperty.cellMeasure = new CellMeasure();
                cellProperty.cellMeasure.measure = (Measure)item;
            }

            if (!string.IsNullOrWhiteSpace(lineItem.periodFrom) && !string.IsNullOrWhiteSpace(lineItem.periodTo))
            {
                //lineItem.updatePeriod(this.periodicity);
                //cellProperty.periodFrom = lineItem.periodFrom;
                //cellProperty.periodTo = lineItem.periodTo;
            }
        }

        /// <summary>
        /// New instance of Target
        /// </summary>
        /// <returns></returns>
        public Target GetNewScope()
        {
            Target scope = new Target();
            scope.targetType = Target.TargetType.COMBINED.ToString();
            scope.type = Target.Type.OBJECT_VC.ToString();
            return scope;
        }

        #endregion
            
    }
}
