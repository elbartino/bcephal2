using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.LinkedAttribute
{
    public class LinkedAttributeGridEditor : Editor<LinkedAttributeGrid>
    {

        public LinkedAttributeGridEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<LinkedAttributeGrid> getNewPage() 
        {
            LinkedAttributeGridEditorItem item = new LinkedAttributeGridEditorItem(this.SubjectType);
            item.getLinkedAttributeGridForm().Grid.Service = this.Service.InputGridService;
            return item;
        }
        
        public LinkedAttributeGridService Service { get; set; }
        
        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            if (this.ChildrenCount == 2)
                this.Children[0].CanClose = false;

            if (this.ChildrenCount > 2)
                for (int i = 0; i < this.ChildrenCount - 2; i++)
                    this.Children[i].CanClose = true;
        }

        protected override void InitializeNewPage(String functionality)
        {
            
        }

    }
}
