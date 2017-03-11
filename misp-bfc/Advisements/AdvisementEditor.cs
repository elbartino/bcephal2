using Misp.Bfc.Model;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Bfc.Advisements
{
    public class AdvisementEditor : Editor<Advisement>
    {
        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Advisement> getNewPage()
        {
            return new AdvisementEditorItem(this.SubjectType); 
        }

        public event OnRemoveNewPageEventHandler OnRemoveNewPage;
        public delegate void OnRemoveNewPageEventHandler(bool remove = false);

        public AdvisementEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }
                
        public Kernel.Service.GroupService GroupService { get; set; }        

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
                if (this.ChildrenCount == 2){
                    for (int i = this.Children.Count-1; i > 0; i--)
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
