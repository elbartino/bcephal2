using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.PresentationView
{
    public class PresentationEditor : Editor<Presentation>
    {
        public event OnRemoveNewPageEventHandler OnRemoveNewPage;
        public delegate void OnRemoveNewPageEventHandler(bool remove = false);

        public PresentationEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        protected override EditorItem<Presentation> getNewPage()
        {
            return new PresentationEditorItem(this.SubjectType);
        }

        protected override void InitializeNewPage(String functionality)
        {
            base.InitializeNewPage(functionality);
            if (NewPage != null && ((PresentationEditorItem)NewPage).getPresentationForm().SlideView != null)
                ((PresentationEditorItem)NewPage).getPresentationForm().SlideView.Close();
        }

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 2)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
            if (OnRemoveNewPage != null)
            {
                if (this.ChildrenCount == 2)
                {
                    for (int i = this.Children.Count - 1; i > 0; i--)
                    {
                        if (i == 1)
                        {
                            this.Children.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        public virtual void removeNewPage()
        {
            if (this.ChildrenCount == 2)
            {
                this.Children.RemoveAt(1);
            }
        }

    }
}
