using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Administration.UserProfile
{
    public class UserProfileEditor : Editor<Misp.Kernel.Domain.User>
    {
        public UserProfileEditor(Domain.SubjectType subjectType, String fuctionality) : base(subjectType, fuctionality) { }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Misp.Kernel.Domain.User> getNewPage() { return new UserProfileEditorItem(this.SubjectType); }

        public Kernel.Service.GroupService GroupService { get; set; }

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 1)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
        }

        protected override void InitializeNewPage(String fuctionality)
        {
            

        }

        protected override void OnNewPageSelected(object sender, EventArgs args)
        {
            

        }


    }
}
