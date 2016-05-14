using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Instruction
    {
        public static String IF 	= "IF";
	    public static String THEN 	= "THEN";
	    public static String ELSE 	= "ELSE";
	
	    public static String ENDIF 		= "ENDIF";
	    public static String ENDTHEN 	= "ENDTHEN";
	    public static String ENDELSE 	= "ENDELSE";
	
	    public static String BLOCK 	= "BLOCK";
	    public static String COND 	= "COND";
	    public static String END 	= "END";
	
	    public static String CONTINUE 		= "CONTINUE";
	    public static String STOP 			= "STOP";
	    public static String NEXT_VALUE 	= "NEXT_VALUE";
	    public static String CREATE_TABLE 	= "CREATE_TABLE";
        public static String CREATE_SLIDE   = "CREATE_SLIDE";

        public static string[] lOGICAL_OPERATORS = new string[] { "AND", "OR" };
        public static string[] lOGICAL_OPERATORS_SYMBOLS = new string[] { "&", "|" };
        public static string[] COMP_OPERATORS = new string[] { ConditionItem.EQUALS, ConditionItem.NOT, ConditionItem.LESS, ConditionItem.LESS_EQUALS, ConditionItem.GREAT, ConditionItem.GREAT_EQUALS };
        public static string[] ACTIONS = new string[] {CREATE_SLIDE, CREATE_TABLE, CONTINUE, NEXT_VALUE, STOP, IF };
        public static string[] OPEN_BRACKETS = new string[] { ConditionItem.OPEN, ConditionItem.DOUBLE_OPEN, "" };
        public static string[] CLOSE_BRACKETS = new string[] { ConditionItem.CLOSE, ConditionItem.DOUBLE_CLOSE, "" };


        public int position { get; set; }
	    public String start { get; set; }
	    public String end { get; set; }
		
	    public String args { get; set; }

        public String comment { get; set; }
	
	    public bool isIfBloc { get; set; }
	    public bool isThenBloc { get; set; }
	    public bool isElseBloc { get; set; }
	    public bool isAction { get; set; }
	    public bool isCondition { get; set; }
	
	    public List<Instruction> subInstructions { get; set; }

        public List<ConditionItem> conditionItems { get; set; }
	
	    public Instruction() {
		    subInstructions = new List<Instruction>(0);
            conditionItems = new List<ConditionItem>(0);
	    }
	
	    public Instruction(String start, String end) : this() {
		    setStartAndEnd(start, end);
	    }
	
	    public Instruction(String start, String end, String args) : this(start, end) {
		    setStartAndEnd(start, end);
		    this.args = args;
	    }
			
	    public void setStartAndEnd(String start, String end) {
		    this.start = start;
		    this.end = end;
		    isIfBloc = false;
		    isThenBloc = false;
		    isElseBloc = false;
		    isAction = false;
		    isCondition = false;
		    if(start == null) return;
		    if(start.Equals(Instruction.IF, StringComparison.OrdinalIgnoreCase)) isIfBloc = true;
		    else if(start.Equals(Instruction.THEN, StringComparison.OrdinalIgnoreCase)) isThenBloc = true;
		    else if(start.Equals(Instruction.ELSE, StringComparison.OrdinalIgnoreCase)) isElseBloc = true;
		
		    else if(start.Equals(Instruction.CONTINUE, StringComparison.OrdinalIgnoreCase)) isAction = true;
		    else if(start.Equals(Instruction.STOP, StringComparison.OrdinalIgnoreCase)) isAction = true;
		    else if(start.Equals(Instruction.NEXT_VALUE, StringComparison.OrdinalIgnoreCase)) isAction = true;
		    else if(start.Equals(Instruction.CREATE_TABLE, StringComparison.OrdinalIgnoreCase)) isAction = true;
            else if (start.Equals(Instruction.CREATE_SLIDE, StringComparison.OrdinalIgnoreCase)) isAction = true;
		
		    else if(start.Equals(Instruction.COND, StringComparison.OrdinalIgnoreCase)) isCondition = true;
	    }
	
	
	    public String toString() {
		    String text = "";
		    if(!string.IsNullOrEmpty(start)) text += start;
		    if(args != null && !string.IsNullOrEmpty(args.Trim())) text += " " + args.Trim();		
		    foreach(Instruction child in subInstructions){
			    String childText = child.toString();
			    text += " " + childText.Trim();			
		    }
		    if(!string.IsNullOrEmpty(end)) text += " " + end;
		    return text.Trim();
	    }
	
	    public bool isContinue(){
            return !string.IsNullOrEmpty(start)
                && start.Equals(Instruction.CONTINUE, StringComparison.OrdinalIgnoreCase);
	    }
	
        public bool isStop(){
            return !string.IsNullOrEmpty(start)
                && start.Equals(Instruction.STOP, StringComparison.OrdinalIgnoreCase);
	    }
		    
	    public bool isCreateTable(){
            return !string.IsNullOrEmpty(start)
                && start.Equals(Instruction.CREATE_TABLE, StringComparison.OrdinalIgnoreCase);
	    }
        public bool isCreateSlide()
        {
            return !string.IsNullOrEmpty(start)
                && start.Equals(Instruction.CREATE_SLIDE, StringComparison.OrdinalIgnoreCase);
        }
		    
	    public bool isNextValue(){
            return !string.IsNullOrEmpty(start)
                && start.Equals(Instruction.NEXT_VALUE, StringComparison.OrdinalIgnoreCase);
	    }
		    
	    public bool isBlock(){
		    return !string.IsNullOrEmpty(start)
                && start.Equals(Instruction.BLOCK, StringComparison.OrdinalIgnoreCase);
	    }
		    
	    public Instruction getIfInstruction(){
            foreach (Instruction child in subInstructions)
            {
			    if(child.isIfBloc) return child;
		    }
		    return null;
	    }
		    	
	    public Instruction getThenInstruction(){
            foreach (Instruction child in subInstructions)
            {
			    if(child.isThenBloc) return child;
		    }
		    return null;
	    }
		    
	    public Instruction getElseInstruction(){
            foreach (Instruction child in subInstructions)
            {
			    if(child.isElseBloc) return child;
		    }
		    return null;
	    }

    }
}
