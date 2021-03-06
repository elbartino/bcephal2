﻿using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Sourcing.AutomaticSourcingViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Sourcing.InputGrid
{
    public class InputGridEditor : Editor<Grille>
    {

        public InputGridEditor(Kernel.Domain.SubjectType subjectType, String functionality) : base(subjectType, functionality) { }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected override EditorItem<Grille> getNewPage() 
        { 
            InputGridEditorItem item = new InputGridEditorItem(this.SubjectType);
            if (this.Service != null)
            {
                PeriodName name = this.Service.PeriodNameService.getRootPeriodName();
                PeriodName defaultName = name.getDefaultPeriodName();
                item.getInputGridForm().GridForm.filterForm.periodFilter.DefaultPeriodName = defaultName;
                item.getInputGridForm().GridForm.filterForm.periodFilter.DisplayPeriod(null);
            }
            item.getInputGridForm().GridForm.gridBrowser.Service = this.Service;
            return item;
        }

        public Kernel.Service.GroupService GroupService { get; set; }

        public InputGridService Service { get; set; }
        
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
