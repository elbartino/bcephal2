using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramDesigner
{

    public delegate void AddBlockEventHandler(DesignerItem sender);

    public delegate void DeleteBlockEventHandler(DesignerItem sender);

    public delegate void ModifyBlockEventHandler(DesignerItem sender);

    public delegate void AddLinkEventHandler(DesignerItem parent, DesignerItem child);

    public delegate void DeleteLinkEventHandler(DesignerItem parent, DesignerItem child);

    public delegate void MoveLinkSourceEventHandler(DesignerItem oldParent, DesignerItem child, DesignerItem newParent);

    public delegate void MoveLinkTargetEventHandler(DesignerItem parent, DesignerItem oldChild, DesignerItem newChild);

    public delegate void SelectionChangeEventHandler(object sender);

    public delegate void ZoomEventHandler();

    public delegate void ChangeEventHandler();

}
