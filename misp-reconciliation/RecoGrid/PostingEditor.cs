using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Reconciliation.Posting;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.InputGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Reconciliation.RecoGrid
{
    public class PostingEditor : PostingGridEditor
    {

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            this.Children[0].CanClose = false;
        }

        protected override void InitializeNewPage() { }

    }
}
