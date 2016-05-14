using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Domain;

namespace Misp.Initiation.Model
{
    public class AttributeTree : EditableTree
    {

        public AttributeTree()
        {
            CanCreateSubNode = false;
        }

        protected override object GetDataType() { return typeof(Misp.Kernel.Domain.Attribute); }
        protected override string GetChildrenBindingName() { return "childrenListChangeHandler.Items"; }
        protected override string GetRendererBindingName() { return "name"; }
        protected override string GetClipbordDataFormat() { return Kernel.Util.ClipbordUtil.ATTRIBUTE_CLIPBOARD_FORMAT; }
        protected override IHierarchyObject GetNewTreeViewModel() 
        {
            Kernel.Domain.Attribute attribute  = new Kernel.Domain.Attribute();
            attribute.name = "Attrinute1";
            if (Root != null)
            {
                Kernel.Domain.Attribute m = null;
                int i = 1;
                do
                {
                    attribute.name = "Attrinute" + i++;
                    m = (Kernel.Domain.Attribute)Root.GetChildByName(attribute.name);
                }
                while (m != null);
            }
            return attribute; 
        }

    }
}
