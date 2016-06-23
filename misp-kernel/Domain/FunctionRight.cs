using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
  public class FunctionRight
  {

      public String function;

      public String view;

      public String edit;

      public FunctionRight(String function, String view, String edit)
      {
          this.function = function;
          this.view = view;
          this.edit = edit;
      }
      
      public static List<FunctionRight> generateDefaultFunction()
      {
          List<FunctionRight> functionRights = new List<FunctionRight>(0);
          functionRights.Add(new FunctionRight("Initialisation", "Initialisation", "Initialisation"));
          //functionRights.Add(new FunctionRight("InputTable", true, true));
          //functionRights.Add(new FunctionRight("Report", true, true));
          //functionRights.Add(new FunctionRight("TransformationTree", true, true));
          //functionRights.Add(new FunctionRight("Target", true, true));
          //functionRights.Add(new FunctionRight("StructuredReport", true, true));
          //functionRights.Add(new FunctionRight("AutomaticSourcing", true, true));
          //functionRights.Add(new FunctionRight("MultipleUplaod", true, true));
          //functionRights.Add(new FunctionRight("Design", true, true));
          //functionRights.Add(new FunctionRight("AutomaticTarget", true, true));
          //functionRights.Add(new FunctionRight("CombinedTranformationTree", true, true));
          //functionRights.Add(new FunctionRight("Load", true, true));
          //functionRights.Add(new FunctionRight("Clear", true, true));
          //functionRights.Add(new FunctionRight("Posting", true, true));
          //functionRights.Add(new FunctionRight("ReconciliationFilter", true, true));
          //functionRights.Add(new FunctionRight("Clear Launch", true, true));
          //functionRights.Add(new FunctionRight("Laod Launch", true, true));

          return functionRights;
        }
    }
}
