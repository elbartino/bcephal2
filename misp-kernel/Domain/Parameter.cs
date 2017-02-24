using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
   public class Parameter
   {

       public Parameter() 
       {
       }

       public Parameter(int tableoid)
       {
           this.tableOid = tableoid;
       }

       public Parameter(String tableName)
       {
           this.tableName = tableName;
       }

       public Parameter(String tableName, Kernel.Ui.Office.Range range) 
       {
           this.tableName = tableName;
           this.range = range;         
       }

       public Parameter(String tableName, Kernel.Ui.Office.Range range, string activeCellName)
       {
           this.tableName = tableName;
           this.range = range;
           this.activeCellName = activeCellName;
       }
      
       public Kernel.Ui.Office.Range range { get; set; }
       public string activeCellName { get; set; }
       public String tableName { get; set; }
       public int? tableOid { get; set; }
       public String excelFileName { get; set;}
       public Measure measure { get; set; }
       public TargetItem targetItem { get; set; }

       public PeriodItem periodItem { get; set; }
       
       public bool forAllocation { get; set; }
       public CellPropertyAllocationData cellPropertyAllocationData {get;set;}
       public BGroup groupe { get; set; }
       public int rowRef { get; set; }
       public int colRef { get; set; }
       public bool removeTargetItem { get; set; }
       public bool removeTagItem { get; set; }
       public bool removePeriodItem { get; set; }
       public bool active { get; set; }
       public bool template { get; set; }
       public bool visibleInShortcut { get; set; }
       public bool isValueChanged { get; set; }
       public bool isTarget { get; set; }
       public bool isMeasure { get; set; }
       public bool isForAllocation { get; set; }
       public bool isPeriod { get; set; }
       public bool isCellPropertyAllocationData { get; set; }
       public bool isGroup { get; set; }
       public bool isActive { get; set; }
       public bool isTemplate { get; set; }
       public bool isVisibleInShortcut { get; set; }
       public bool isRename { get; set; }
       public bool isResetAllCells { get; set; }
       public bool isRight { get; set; }
       public bool isTransformationTree { get; set; }
       public int transformationTreeOid { get; set; }

       public CellProperty activeCell { get; set; }
       public int cellCount { get; set; }
       public int cellCountInRange { get; set; }
       public PersistentListChangeHandler<Right> rightsListChangeHandler { get; set; }

       public void setVisibleInShortcut(bool active)
       {
           this.isVisibleInShortcut = true;
           this.visibleInShortcut = active;
       }

       public void setActive(bool active) 
       {
           this.isActive = true;
           this.active = active;
       }

       public void setTemplate(bool template) 
       {
           this.isTemplate = true;
           this.template = template;
       }

       public void setMeasure(Kernel.Domain.Measure measure)
       {
           this.isMeasure = true;
           this.measure = measure;
       }

       public void setScope(Kernel.Domain.TargetItem targetItem) 
       {
           this.isTarget = true;
           this.targetItem = targetItem;
       }

       public void setPeriod(PeriodItem item, String excelFile, int rowRef = -1, int colRef = -1, bool remove = false)
       {
           this.isPeriod = true;
           this.periodItem = item;
           this.excelFileName = excelFile;
           this.rowRef = rowRef;
           this.colRef = colRef;
           this.removePeriodItem = remove;
       }

       public void setAllocationData(CellPropertyAllocationData data) 
       {
           this.isCellPropertyAllocationData = true;
           this.cellPropertyAllocationData = data;
       }
       public void setGroup(BGroup group) 
       {
           this.isGroup = true;
           this.groupe = group;
       }

       public void setTransformationTree(int oid)
       {
           this.isTransformationTree = true;
           this.transformationTreeOid = oid;
       }

       
       public void setForAllocation(bool forAllocation) 
       {
           this.isForAllocation = true;
           this.forAllocation = forAllocation;
       }
       public void removeScope(Kernel.Domain.TargetItem targetItem) 
       {
           this.isTarget = true;
           this.targetItem = targetItem;
           this.removeTargetItem = true;
       }

       public void setRights(PersistentListChangeHandler<Right> listRights) 
       {
           this.isRight = true;
           this.rightsListChangeHandler = listRights;
       }

       public void removePeriod(Kernel.Domain.PeriodItem item)
       {
           this.isPeriod = true;
           this.periodItem = item;
           this.removePeriodItem = true;
       }
   }
}
