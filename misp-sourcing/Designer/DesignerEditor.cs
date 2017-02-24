using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.Designer
{
    public class DesignerEditor : Editor<Design>
    {

        public DesignerEditor(Kernel.Domain.SubjectType subjectType) : base(subjectType) { }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Design> getNewPage() { return new DesignerEditorItem(this.SubjectType); }

        public Kernel.Service.GroupService GroupService { get; set; }

        protected override void InitializeNewPage()
        {
            base.InitializeNewPage();
            ((DesignerEditorItem)NewPage).getDesignerForm().SpreadSheet.Close();
        }

        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 2)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
        }

    }

}

