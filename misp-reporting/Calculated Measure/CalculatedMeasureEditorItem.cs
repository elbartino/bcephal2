using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Reporting.Calculated_Measure
{
   public  class CalculatedMeasureEditorItem : EditorItem <CalculatedMeasure>
    {
       

       /// <summary>
       /// UNe nouvelle instance de la form.
       /// </summary>
       /// <returns></returns>
       protected override IEditableView<CalculatedMeasure> getNewEditorItemForm()
       {

           return new CalculatedMeasureForm();
       }

       public virtual CalculatedMeasureForm getCalculatedMeasureForm()
       {
           return (CalculatedMeasureForm)editorItemForm;
       }

       protected override void OnClosing(System.ComponentModel.CancelEventArgs args)
       {

       }


       public Kernel.Service.GroupService GroupService { get; set; }
    }
}
