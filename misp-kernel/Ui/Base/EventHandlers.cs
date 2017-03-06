using DevExpress.Xpf.Grid;
using Misp.Kernel.Application;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Misp.Kernel.Ui.Base
{
    public delegate void ChangeEventHandler();

   // public delegate void ChangeShapeEventHandler(int slidePosition, String slideName, int shapePosition, String shapeValue, SlideItemType type);

    public delegate void ChangeShapeEventHandler(int slidePosition, String slideName, int shapePosition, String shapeValue, SlideItemType type,String shapeName);

    public delegate void ChangeItemEventHandler(object item);

    public delegate void SelectedItemChangedEventHandler(object newSelection);
    
    public delegate void AddEventHandler(object item);

    public delegate void UpdateEventHandler(object item);

    public delegate void DeleteEventHandler(object item);

    public delegate void ActivateEventHandler(object item);

    public delegate void ValidateFormulaEventHandler(object item);

    public delegate void SelectedItemDoubleClickEventHandler(object newSelection);

    public delegate void SaveInfoEventHandler(SaveInfo info, Object item);

    public delegate void RunInfoEventHandler(AllocationRunInfo runInfo);

    public delegate void ClearInfoEventHandler(AllocationRunInfo runInfo);

    public delegate void TransformationTreeRunInfoEventHandler(TransformationTreeRunInfo info);

    public delegate void TransformationTreeSaveInfoEventHandler(SaveInfo info, object transformationTree);

    public delegate void PowerpointLoadInfoEventHandler(PowerpointLoadInfo info);

    public delegate bool ActionEventHandler(object item);

    public delegate void RightEventHandler(Right right, bool selected);

    public delegate List<FormatCondition> FormatConditions();
}
