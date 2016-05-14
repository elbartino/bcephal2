using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class ConditionItem
    {

        public static String START 			= "START";
	    public static String ENDSTART 		= "ENDSTART";
	
	    public static String OPEN 			= "(";
	    public static String DOUBLE_OPEN 	= "((";
	    public static String CLOSE 			= ")";
	    public static String DOUBLE_CLOSE 	= "))";
	
	    public static String AND 	= "&";
	    public static String OR 	= "|";
	
	    public static String EQUALS 		= "=";
	    public static String GREAT 			= ">";
	    public static String GREAT_EQUALS 	= ">=";
	    public static String LESS 			= "<";
	    public static String LESS_EQUALS 	= "<=";
	    public static String NOT 			= "<>";
		
	    public String operatorSign { get; set;}
	    public String openBracket { get; set;}
	    public String arg1 { get; set;}
	    public String sign { get; set;}
	    public String arg2 { get; set;}
	    public String closeBracket { get; set;}

        public String comment { get; set; }

        public bool isArgsEmpty()
        {
            return string.IsNullOrWhiteSpace(this.arg1) && string.IsNullOrWhiteSpace(this.arg2);
        }
		
	    public String ToString() {
		    String text = "";
            if (!String.IsNullOrEmpty(operatorSign))
            {
                if (operatorSign.Equals("AND")) text += AND;
                else if (operatorSign.Equals("OR")) text += OR;
                else text += operatorSign;
            }
            if (!String.IsNullOrEmpty(openBracket)) text += " " + openBracket.Trim();	
		    if (!String.IsNullOrEmpty(arg1)) text += " " + arg1.Trim();
            if (!String.IsNullOrEmpty(sign)) text += " " + sign.Trim();
            if (!String.IsNullOrEmpty(arg2)) text += " " + arg2.Trim();
            if (!String.IsNullOrEmpty(closeBracket)) text += " " + closeBracket.Trim();
            return text.Trim();
	    }

    }
}
