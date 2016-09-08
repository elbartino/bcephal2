using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Kernel.Util
{
    [Serializable]
    public class RangeUtil
    {

        public enum Type { ROW, COL }

        public static String WORKBOOK_END = "]";
        public static String SHEET_SEPARATOR = ";";
        public static String SHEET_DELIMITOR = "!";
        public static String RANGE_SEPARATOR = ":";
        public static String CELL_SEPARATOR = "$";

        static String[] alphabet = {"A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
        static String[] numeric = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public static String GetCellName(int row, int col)
        {
            return GetColumnName(col) + row;
        }

        public static String GetColumnName(int value)
        {
            return getName(value);
        }


        public static int GetColumnIndex(string name)
        {
            char[] chars = name.ToCharArray();
            int length = chars.Length;
            if (length == 1)
            {
                return getPosition(chars[0]);
            }
            return getPosition(chars, length - 1);
        }


        protected static String getName(int value)
        {
            String[] tab = {"A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
            if (value <= 26)
            {
                return tab[value > 0 ? value - 1 : value];
            }
            int r = value % 26;
            r = r == 0 ? tab.Length : r;
            int d = (int)(value - r) / 26;
            String lastName = r > 0 ? tab[r - 1] : "";
            String name = getName(d);

            return name + lastName;
        }

        protected static int getPosition(char[] chars, int index)
        {
            int length = chars.Length;
            if (index == 0)
            {
                return getPosition(chars[0]);
            }
            int precPos = getPosition(chars, index - 1);
            int pos = getPosition(chars[index]);
            return (26 * precPos) + pos;
        }


        protected static int getPosition(char c)
        {
            char[] tab = {'A','B','C','D','E','F','G','H','I','J','K','L','M',
    	            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
            int length = 26;
            for (int i = 1; i <= length; i++)
            {
                if (tab[i - 1] == c)
                {
                    return i;
                }
            }
            return 0;
        }

        public static List<Point> getCellsCoord(String cellRange)
        {
            List<Point> result = new List<Point>();
            String[] rangeTab = cellRange.Split(RANGE_SEPARATOR.ToCharArray()[0]);
      
            Point borneInf = getCoord(rangeTab[0].ToCharArray());
            result.Add(borneInf);

            if (result.Count > 1)
            {
                Point borneSup = getCoord(rangeTab[1].ToCharArray());
                result.Add(borneSup);
            }
            return result;
        }

        public static List<Kernel.Ui.Office.RangeItem> getRangeItem(String Range)
        {
            if (!Range.Contains(RANGE_SEPARATOR)) return null;
            String range = Range.ToUpper().Trim();
            bool canContinue = range.Length >= 4;
            if (!canContinue) return null;

            canContinue = Regex.Matches(Range, RANGE_SEPARATOR, RegexOptions.IgnoreCase).Count < 2;
            if (!canContinue) return  null; 

            String[] rangeTab = range.Trim().Split(RANGE_SEPARATOR.ToCharArray()[0]);
            canContinue = rangeTab.Length >= 1 && rangeTab.Length <= 3;
            if (!canContinue) return  null;
            int length = rangeTab.Length - 1;
           // rangeTab = length > 0 ? new String[] { rangeTab[length - 1] + rangeTab[length] } : new String[] { rangeTab[length] };
            List<Kernel.Ui.Office.RangeItem> RangeItems = new List<Ui.Office.RangeItem>();
            for (int i = 0 ; i<=rangeTab.Length - 1; i++) 
            {
                Point? coords = getCoords(rangeTab[i]);
                if (coords == null) return null;
                Kernel.Ui.Office.RangeItem rangeItem = new Ui.Office.RangeItem((int)coords.Value.X, (int)coords.Value.X, (int)coords.Value.Y, (int)coords.Value.Y);
                RangeItems.Add(rangeItem);
            }
            return RangeItems;
        }

        private static Point? getCoords(string rangeItemString) 
        {
            char[] values = rangeItemString.ToCharArray();
            int j = 0;
            String cols = "";
            String lines = "";
            bool canContinue = true;
            for (int i = values.Length - 1; i >= 0; i--)
            {
                string val = "" + values[j];
                val = val.ToUpper();
                if (alphabet.Contains(val))
                {
                    if (canContinue)
                        cols += val;
                    else return null;
                }
                else if (numeric.Contains(val))
                {
                    lines += "" + val;
                    canContinue = false;
                }
                else
                {
                    return null;
                }
                j++;
            }

            if (lines.StartsWith("0") || String.IsNullOrEmpty(lines) || String.IsNullOrEmpty(cols))
                return null;

            int row =0;
            bool ok = int.TryParse(lines, out row);
            if (!ok) return null;

            return new Point(row,GetColumnIndex(cols));
        }

        private static Point getCoord(char[] rangeTab)
        {
            String colValue = "";

            int j = 0;
            for (int i = rangeTab.Length - 1; i >= 0; i--)
            {
                Object index;
                try
                {
                    index =  Convert.ToInt64(rangeTab[j].ToString());
                    break;
                }
                catch (Exception exce)
                {
                    colValue += rangeTab[j];
                }
                j++;
            }


            String rowValue = "";
            for (int p = j; p <= rangeTab.Length - 1; p++)
            {
                rowValue += rangeTab[p];
            }
            Point cellule1 = new Point(GetColumnIndex(colValue.ToUpper()), Convert.ToInt64((rowValue)));

            return cellule1;
        }

        static int startLine=0;
        static int endLine = 0;
        static int startCol = 0;
        static int endCol = 0;

        public static Kernel.Ui.Office.RangeItem getCount(String range)
        {
            List<Point> borne = getCellsCoord(range);
            List<Kernel.Ui.Office.Cell> listCel = new List<Ui.Office.Cell>(0);
            
            startLine = (int)borne[0].Y;
            endLine = startLine;

            startCol = (int)borne[0].X;
            endCol = startCol;
            try
            {
                endLine = (int)borne[1].Y;
                endCol = (int)borne[1].X;
            }
            catch (Exception) { }
   
            Kernel.Ui.Office.RangeItem rangeCount = new Ui.Office.RangeItem(startLine, endLine, startCol, endCol);

            return rangeCount;
        }

        public struct Resultat
        {
            public List<Kernel.Ui.Office.Cell> liste;
            public int row1;
            public int row2;
            public int Col1;
            public int Col2;
        }


        public static Resultat listeCell(Kernel.Ui.Office.RangeItem rangeCount, int pageSize = 500) 
        {
           // Kernel.Ui.Office.RangeItem rangeCount = new Ui.Office.RangeItem(startLine, endLine, startCol, endCol);
            

             return  getListCell(rangeCount.Row1, rangeCount.Row2, rangeCount.Column1, rangeCount.Column2,pageSize);
                
        }

        public static Resultat getListCell(int startLine, int endLine, int startCol, int endCol, int pageSize = 500) 
        {
            List<Kernel.Ui.Office.Cell> liste = new List<Ui.Office.Cell>(0);
            Resultat rest = new Resultat();
            Kernel.Ui.Office.RangeItem rangeCount = new Ui.Office.RangeItem(startLine, endLine, startCol, endCol);
            int reste = rangeCount.CellCount % pageSize;
            int pageCount = (rangeCount.CellCount / pageSize) + (reste > 0 ? 1 : 0);
            int count = 0;
            int l=0;
            int c=0;
            for (l = startLine; l <= endLine; l++)
            {
                for (c = startCol; c <= endCol; c++)
                {
                            
                            Kernel.Ui.Office.Cell cell = new Kernel.Ui.Office.Cell(l, c);
                            liste.Add(cell);
                            count++;
                            if (count == pageSize)
                            {

                                startLine =l+1 ;
                                startCol = c+1;

                                rest.liste = liste;
                                rest.row1 = l + 1;
                                rest.row2 = endLine;
                                rest.Col1 = c + 1;
                                rest.Col2 = endCol;
                                
                                return rest;
                            }
                }
            }
            rest.liste = liste;
            rest.row1 = l + 1;
            rest.row2 = endLine;
            rest.Col1 = c + 1;
            rest.Col2 = endCol;
            return rest;
        }

        public static bool isCellInRange(Kernel.Ui.Office.Cell cell, String range) 
        {
            List<Point> borne = getCellsCoord(range);
            int startLine = (int)borne[0].Y;
            int endLine = startLine;

            int startCol = (int)borne[0].X;
            int endCol = startCol;
            try
            {
                endLine = (int)borne[1].Y;
                endCol = (int)borne[1].X;
            }
            catch (Exception) { }

            if (startLine <= cell.Row && endLine >= cell.Row)
                if (startCol <= cell.Column && endCol >= cell.Column)
                    return true;
 

            return false;
        }

        public static bool isCellInRange(String rangeIn, String range)
        {
            List<Point> borne = getCellsCoord(range);
            List<Point> borneIn = getCellsCoord(rangeIn);

            int startLine = (int)borne[0].Y;
            int endLine = startLine;

            int startLineIn = (int)borne[0].Y;
            int endLineIn = startLineIn;

            int startColIn = (int)borne[0].X;
            int endColIn = startColIn;

            int startCol = (int)borne[0].X;
            int endCol = startCol;
            try
            {
                endLine = (int)borne[1].Y;
                endCol = (int)borne[1].X;
            }
            catch (Exception) { }

            try
            {
                endLineIn = (int)borneIn[1].Y;
                endColIn = (int)borneIn[1].X;
            }
            catch (Exception) { }

            if (startLine <= startLineIn && endLine >= endLineIn)
                if (startCol <= startColIn && endCol >= startColIn)
                    return true;


            return false;
        }
    }
}
