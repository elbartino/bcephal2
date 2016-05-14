using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Kernel.Util
{
    public class TagFormulaUtil
    {
        
        public enum Type {
            ROWREF,
            COLREF,
            CELLREF,
            CELL
        }

        public static String[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public static String[] numeric = {"0","1","2","3","4","5","6","7","8","9" };

        public static String CELL_REF_SEPARATOR = "$";
    
        public static String ERROR_CELL= "REF_NOT_FOUND";
    
        public static String SIGN_EQUAL = "=";
    
        private static String MIN_CHAR = "A";
    
        private static String MAX_CHAR = "Z";
    
        private static String MIN_CHAR_NUM = "0";
    
        private static String MAX_CHAR_NUM = "9";

        private static String DOLLARD = "$";
    
        private static String POURCENTAGE = "%";


        /// <summary>
        /// la forme generale d'une formule est de la forme:
        /// -  =A1
        /// -  =$A1
        /// -  =A$1
        /// -  =$A$1
        /// </summary>
        /// <param name="formula"></param>
        /// <returns>true si c'est une formule et false sinon</returns>
        public static bool isFormula(String formula)
        {
            return formula != null && formula.StartsWith(SIGN_EQUAL);
        }

        public static String getFormulaWithoutEqualSign(String formula)
        {
            return formula.StartsWith(SIGN_EQUAL) ? formula.Substring(1) : formula;
        }

        public static bool isSyntaxeFormulaCorrectly(String form)
        {
            if (!isFormula(form)) return false;
            String formula = getFormulaWithoutEqualSign(form.ToUpper()).Trim().ToUpper();
            bool canContinue = formula.Length >= 2;
            if (!canContinue) return canContinue;

            canContinue = Regex.Matches(formula, CELL_REF_SEPARATOR, RegexOptions.IgnoreCase).Count <= 2;
            if (!canContinue) return canContinue;
            
            String[] formulaTab = formula.Trim().Split(CELL_REF_SEPARATOR.ToCharArray()[0]);
            canContinue = formulaTab.Length >= 1 && formulaTab.Length <= 3 ;
            if (!canContinue) return canContinue;
            int length = formulaTab.Length - 1;
            formulaTab = length > 0 ? new String[] { formulaTab[length - 1] + formulaTab[length] } : new String[] {formulaTab[length] };
            
            if (formulaTab.Length==1){
                char[] values = formulaTab[0].ToCharArray();
                int j = 0;
                String cols = "";
                String lines = "";
               
                for (int i = values.Length - 1; i >= 0; i--)
                {
                    string val = "" + values[j];
                    val = val.ToUpper();
                    if (alphabet.Contains(val))
                    {
                        if (canContinue)
                            cols += val;
                        else return false;
                    }
                    else if (numeric.Contains(val))
                    {
                        lines += "" + val;
                        canContinue = false;
                    }
                    else 
                    {
                        return false;
                    }
                    j++;
                }

                if (lines.StartsWith("0") || String.IsNullOrEmpty(lines) || String.IsNullOrEmpty(cols)) 
                return false;
                
                return true;
            }
            return false;
        }

      /*  public static bool isSyntaxeFormulaCorrectly(String form)
        {
            String formula = getFormulaWithoutEqualSign(form).Trim().ToUpper();
            int length = formula.Length;
            if (length < 2) return false;
            String first = formula.Substring(0, 1);
            if (first.CompareTo(CELL_REF_SEPARATOR) != 0 && (first.CompareTo(MIN_CHAR) < 0 || first.CompareTo(MAX_CHAR) > 0)) return false;

            String last = formula.Substring(length-1, 1);
            if (last.CompareTo(MIN_CHAR_NUM) < 0 || last.CompareTo(MAX_CHAR_NUM) > 0) return false;

            if (first.CompareTo(CELL_REF_SEPARATOR) == 0) formula = formula.Substring(1, formula.Length - 1);
            if (formula.Length < 2) return false;

            for (int i = 0; i < formula.Length; i++)
            {
                String car = formula.Substring(i, 1);
                if (car.CompareTo(MIN_CHAR) >= 0 && car.CompareTo(MAX_CHAR) <= 0) formula = formula.Substring(1, formula.Length - 1);
                else break;                
            }

            if (formula.Length < 1) return false;
            first = formula.Substring(0, 1);
            if (first.CompareTo(CELL_REF_SEPARATOR) == 0) formula = formula.Substring(1, formula.Length - 1);
            if (formula.Length < 1) return false;
            for (int i = 0; i < formula.Length; i++)
            {
                String car = formula.Substring(i, 1);
                if (car.CompareTo(MIN_CHAR_NUM) >= 0 && car.CompareTo(MAX_CHAR_NUM) <= 0) formula = formula.Substring(1, formula.Length - 1);
                else return false;
            }
            return true;
        }*/
    
    
    /**
     * les different type de format sont :
     * - CELL    : A1
     * - CELLREF : $A$1
     * - COLREF  : $A1
     * - ROWREF  : A$1
     * @param formula
     * @return le type de la cellule suivant un format.
     */
    public static Type gettType(String formula){
        int firstOccurence = formula.IndexOf(CELL_REF_SEPARATOR);
        int lastOccurence = formula.LastIndexOf(CELL_REF_SEPARATOR);
        if(firstOccurence < 0 && lastOccurence < 0){
            return Type.CELL;
        }else if(firstOccurence == 0 && lastOccurence >0 ){
            return Type.CELLREF;
        }if(firstOccurence == 0 && lastOccurence == 0 ){
            return Type.COLREF;
        }if(firstOccurence >0 && lastOccurence > 0 ){
            return Type.ROWREF;
        }
        return Type.CELL;
    }
    
    /**
     * 
     * @param formulRef
     * @param rowRef
     * @param colRef
     * @param row
     * @param col
     * @return 
     */
    public static String getFormula( String formulRef, int rowRef, int colRef, int row , int col){
        Type type = gettType(formulRef);
        Point formul = getCoordonne(formulRef); 
        if(type.Equals(Type.CELLREF)){
            return SIGN_EQUAL + formulRef;
        }
        else if (type.Equals(Type.CELL))
        {   
            Point ecart = new Point(col-colRef, row-rowRef);
            if((ecart.X + formul.X)>0 && (ecart.Y + formul.Y)>0){
                return SIGN_EQUAL + DataFormater.getColumnName((int)ecart.X + (int)formul.X) + ((int)ecart.Y + (int)formul.Y);
            }else{
            	return ERROR_CELL;
            }
        }
        else if (type.Equals(Type.COLREF))
        {
            Point ecart = new Point(0, row-rowRef);
            if((ecart.X + formul.X)>0 && (ecart.Y+formul.Y)>0){
                return SIGN_EQUAL + CELL_REF_SEPARATOR + DataFormater.getColumnName((int)ecart.X + (int)formul.X) + ((int)ecart.Y + (int)formul.Y);
            }else{
            	return ERROR_CELL;
            }
        }
        else if (type.Equals(Type.ROWREF))
        {
            Point ecart = new Point(col-colRef, 0);
            if((ecart.X + formul.X)>0 && (ecart.Y + formul.Y)>0){
                return SIGN_EQUAL + DataFormater.getColumnName((int)ecart.X + (int)formul.X) + CELL_REF_SEPARATOR + ((int)ecart.Y + (int)formul.Y);
            }else{
            	return ERROR_CELL;
            }
        }
        return null;
        
    }
    
    /**
     * 
     * @param formula
     * @return 
     */
    public static Point getCoordonne(String formula){    	
        Point point = new Point();
        Type type = gettType(formula);
        if (type.Equals(Type.CELLREF) || type.Equals(Type.ROWREF))
        {
            point.X = DataFormater.getColumnIndex(DataFormater.getColumn(formula));
            point.Y = DataFormater.getRow(formula);
        }
        else if (type.Equals(Type.COLREF) || type.Equals(Type.CELL))
        {
            if(formula.StartsWith(CELL_REF_SEPARATOR)){
                formula = formula.Substring(1);
            }
            String row="";
            String col="" ;            
            for(int i = 0 ; i < formula.Length  ; i++){
                String car = formula.Substring(i, 1);
                if ((car.ToUpper().CompareTo(MIN_CHAR) >= 0 && car.ToUpper().CompareTo(MAX_CHAR) <= 0))
                {
                    col += car;
                }else{
                    row += car;
                }
            }            
            point.X = DataFormater.getColumnIndex(col.ToUpper());
            try
            {
                point.Y = int.Parse(row);
            }
            catch (Exception ex) 
            {
                return new Point(-1,-1);
            }
        }
       return point;
    }   
    
    /**
     * 
     * @param car
     * @return 
     */
    private static bool isCaratereSpecial(String car){
        return (car != null && (car.Equals(DOLLARD) || car.Equals(POURCENTAGE)));
    }
    
    private static bool  isChangeCaractere(String car, String car1){
        return ((car.Equals(DOLLARD) && car1.Equals(DOLLARD))
                || (car.Equals(DOLLARD) && car1.Equals(POURCENTAGE))
                || (car.Equals(POURCENTAGE) && car1.Equals(DOLLARD)));
    }

    }
}
