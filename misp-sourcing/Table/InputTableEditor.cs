using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;

namespace Misp.Sourcing.Table
{

    /// <summary>
    /// Cette classe représente l'éditeur pour les InputTables.
    /// On peut ouvrir plusieurs pages à la fois; une page correspondant à une seule InputTable.
    /// </summary>
    public class InputTableEditor : Editor<InputTable>
    {

        public event OnRemoveNewPageEventHandler OnRemoveNewPage;
        public delegate void OnRemoveNewPageEventHandler(bool remove = false);

        public InputTableEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }
        
        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<InputTable> getNewPage() { return new InputTableEditorItem(this.SubjectType); }

        public Kernel.Service.GroupService GroupService { get; set; }



        protected override void InitializeNewPage(String functionality)
        {
            base.InitializeNewPage(functionality);
            if (NewPage != null && ((InputTableEditorItem)NewPage).getInputTableForm().SpreadSheet != null)
                ((InputTableEditorItem)NewPage).getInputTableForm().SpreadSheet.Close();
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
