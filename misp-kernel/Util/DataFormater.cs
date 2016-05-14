using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class DataFormater
    {

        public enum Type {
            ROW,
            COL
        }

    public static String WORKBOOK_END = "]";
    public static String SHEET_SEPARATOR = ";";
    public static String SHEET_DELIMITOR = "!";
    public static String RANGE_SEPARATOR = ":";
    public static String CELL_SEPARATOR = "$";

    
    /**
     * 
     * @param row
     * @param col
     * @return
     */
    public static String getCellName(int row, int col){
    	return getColumnName(col) + row;
    }

    /**
     * Calculate and returns the row of the given cell.
     * <P>
     * The cell looks like: "A$1"
     * @param cell The cell.
     * @return
     */
    public static int getRow(String cell){
        String[] tab = cell.Split(CELL_SEPARATOR.ToArray());
        return int.Parse(tab[tab.Length-1]);
    }

    /**
     * Calculate and returns the column of the given cell.
     * <P>
     * The cell looks like: "A$1"
     * @param cell The cell.
     * @return
     */
    public static String getColumn(String cell){
        if(cell.StartsWith(CELL_SEPARATOR)){
                cell = cell.Substring(1);
            }
        String[] tab = cell.Split(CELL_SEPARATOR.ToArray());
        return tab[0];
    }

    /**
     * Column name
     * @param value Column value
     * @return
     */
    public static String getColumnName(int value){
    	return getName(value);
    }
    
    protected static String getName(int value){
        String[] tab = {"A","B","C","D","E","F","G","H","I","J","K","L","M",
        "N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
        if(value <= 26){
        	return tab[value > 0 ? value - 1 : value];
        }        
        int r = value % 26;
        r = r == 0 ? tab.Length : r;
        int d = (int)(value-r)/26;
        String lastName = r > 0 ? tab[r - 1] : "";
        String name = getName(d);
        
        return name + lastName;        
    }

    /**
     * Column index
     * @param name Column name
     * @return
     */
    public static int getColumnIndex(String name){
    	char[] chars = name.ToUpper().ToCharArray();
    	int length = chars.Length;
    	if(length == 1){
    		return getPosition(chars[0]);
    	}
    	return getPosition(chars, length-1);
    }
    
    
    protected static int getPosition(char[] chars, int index){
    	int length = chars.Length;
    	if(index == 0){
    		return getPosition(chars[0]);
    	}
    	int precPos = getPosition(chars, index-1);
    	int pos = getPosition(chars[index]);
    	return (26 * precPos) + pos;
    }
    
    /**
     * Position dans l'aphabet
     * @param c
     * @return
     */
    protected static int getPosition(char c){  
    	char[] tab = {'A','B','C','D','E','F','G','H','I','J','K','L','M',
    	        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
    	int length = 26;
    	for(int i = 1; i <= length; i++){
    		if(tab[i-1] == c){
    			return i;
    		}
    	}
    	return 0;
    }


    }
}
