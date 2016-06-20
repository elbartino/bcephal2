using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Domain;

namespace Misp.Administration.Role
{
    public class RoleTree : EditableTree
    {

        protected override object GetDataType() { return typeof(Misp.Kernel.Domain.Role); }
        protected override string GetChildrenBindingName() { return "childrenListChangeHandler.Items"; }
        protected override string GetRendererBindingName() { return "name"; }
        protected override string GetClipbordDataFormat() { return Kernel.Util.ClipbordUtil.MEASURE_CLIPBOARD_FORMAT; }
        protected override IHierarchyObject GetNewTreeViewModel()
        {
            Kernel.Domain.Role role = new Kernel.Domain.Role();
            role.name = "Role1";
            if (Root != null)
            {
                Kernel.Domain.Measure m = null;
                int i = 1;
                do
                {
                    role.name = "Role" + i++;
                    m = (Kernel.Domain.Measure)Root.GetChildByName(role.name);
                }
                while (m != null);
            }
            return role;
        }
    }
}
