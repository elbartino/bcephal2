using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Domain;

namespace Misp.Initiation.Measure
{
    public class MeasureTree : EditableTree
    {

        protected override object GetDataType() { return typeof(Misp.Kernel.Domain.Measure); }
        protected override string GetChildrenBindingName() { return "childrenListChangeHandler.Items"; }
        protected override string GetRendererBindingName() { return "name"; }
        protected override string GetClipbordDataFormat() { return Kernel.Util.ClipbordUtil.MEASURE_CLIPBOARD_FORMAT; }
        protected override IHierarchyObject GetNewTreeViewModel()
        {
            Kernel.Domain.Measure measure = new Kernel.Domain.Measure();
            measure.name = "Measure1";
            if (Root != null)
            {
                Kernel.Domain.Measure m = null;
                int i = 1;
                do
                {
                    measure.name = "Measure" + i++;
                    m = (Kernel.Domain.Measure)Root.GetChildByName(measure.name);
                }
                while (m != null);
            }
            return measure;
        }
    }
}
