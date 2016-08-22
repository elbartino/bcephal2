using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Administration.Role
{
    public class RoleEditor :Editor<Domain.Role>
    {
        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Domain.Role> getNewPage() { return new RoleEditorItem(); }

        public Kernel.Service.GroupService GroupService { get; set; }


        protected override void InitializeNewPage()
        {
            //NewPage = getNewPage();
            //NewPage.CanClose = false;
            //NewPage.CanFloat = false;
            //NewPage.Title = "+";

            //newPageEventHandler = new EventHandler(this.OnNewPageSelected);
            //NewPage.IsActiveChanged += newPageEventHandler;

        }

        protected override void OnNewPageSelected(object sender, EventArgs args)
        {
            //if (NewPageSelected != null) NewPageSelected();

        }

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 1)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
        }
    }
}
