using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Domain;

namespace Misp.Initiation.Model
{
    public class AttributeValueTree : EditableTree
    {

        protected override object GetDataType() { return typeof(Misp.Kernel.Domain.AttributeValue); }
        protected override string GetChildrenBindingName() { return "childrenListChangeHandler.Items"; }
        protected override string GetRendererBindingName() { return "name"; }
        protected override string GetClipbordDataFormat() { return Kernel.Util.ClipbordUtil.ATTRIBUTE_VALUE_CLIPBOARD_FORMAT; }
        protected override IHierarchyObject GetNewTreeViewModel() 
        {
            Kernel.Domain.AttributeValue value = new Kernel.Domain.AttributeValue();
            value.name = "Value1";
            if (Root != null)
            {
                Kernel.Domain.AttributeValue m = null;
                int i = 1;
                do
                {
                    value.name = "Value" + i++;
                    m = (Kernel.Domain.AttributeValue)Root.GetChildByName(value.name);
                }
                while (m != null);
            }
            return value;
        }

    }
}
