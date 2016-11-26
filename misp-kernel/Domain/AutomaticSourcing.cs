using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Misp.Kernel.Util;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class AutomaticSourcing : Persistent,IComparable
    {
        public string name { get; set; }

        public string excelFile { get; set; }

        public BGroup group { get; set; }

        public BGroup tableGroup { get; set; }

        public string periodFrom { get; set; }

        public string periodTo { get; set; }

        public Target filter { get; set; }

        public Period period { get; set; }

        public PersistentListChangeHandler<AutomaticSourcingSheet> automaticSourcingSheetListChangeHandler { get; set; }

        public bool isTarget { get; set; }

        public bool visibleInShortcut { get; set; }

        public bool isGrid { get; set; }

        public bool isAutomaticGrid { get; set; }

        public bool isPosting { get; set; }
                
        /// <summary>
        /// La date de début
        /// </summary>
        [ScriptIgnore]
        public DateTime periodFromDateTime
        {
            get { return DateTime.Parse(periodFrom); }
            set { periodFrom = value.ToShortDateString(); }
        }

        /// <summary>
        /// La date de fin
        /// </summary>
        [ScriptIgnore]
        public DateTime periodToDateTime
        {
            get { return DateTime.Parse(periodTo); }
            set { periodTo = value.ToShortDateString(); }
        }

        [ScriptIgnore]
        public AutomaticSourcingSheet ActiveSheet {get;set;}

        public int ActiveSheetIndex { get; set; }

        public int GetCountSheets() 
        {
            if (this.automaticSourcingSheetListChangeHandler == null) return 0;
            return this.automaticSourcingSheetListChangeHandler.Items.Count();
        }

        public AutomaticSourcing()
        {
            this.automaticSourcingSheetListChangeHandler = new PersistentListChangeHandler<AutomaticSourcingSheet>();
            this.visibleInShortcut = true;
        }


        public void updateSheetParams(AutomaticSourcingSheet sheet, object param) 
        {

            int index = this.getAutomaticSourcingSheetIndex(sheet.position);
            if (index == -1) return;
            if (param is bool)
            {
                this.automaticSourcingSheetListChangeHandler.Items[index].firstRowColumn = (bool)param;
            }
            else if (param is string)
            {
                this.automaticSourcingSheetListChangeHandler.Items[index].selectedRange = param.ToString();
            }
            else if (param is Kernel.Ui.Office.Range)
            {
                this.automaticSourcingSheetListChangeHandler.Items[index].selectedRange = ((Kernel.Ui.Office.Range)param).Name;
                this.automaticSourcingSheetListChangeHandler.Items[index].rangeSelected = ((Kernel.Ui.Office.Range)param);
            }
            else
            {
                this.automaticSourcingSheetListChangeHandler.Items[index].selectedRange = null;
                this.automaticSourcingSheetListChangeHandler.Items[index].rangeSelected = null;
            }

            if (sheet.toUpdate) this.UpdateSheet(sheet);
            if (sheet.toNew) this.AddSheet(sheet);
            if (sheet.toForget) this.ForgetSheet(sheet);
            if (sheet.toDelete) this.RemoveSheet(sheet);
            
        }

        /// <summary>
        /// Rajoute un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void AddSheet(AutomaticSourcingSheet sheet)
        {
            sheet.parent = this;
            automaticSourcingSheetListChangeHandler.AddNew(sheet);
            OnPropertyChanged("automaticSourcingSheetListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateSheet(AutomaticSourcingSheet sheet)
        {
            automaticSourcingSheetListChangeHandler.AddUpdated(sheet);
            OnPropertyChanged("automaticSourcingSheetListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveSheet(AutomaticSourcingSheet sheet)
        {
            automaticSourcingSheetListChangeHandler.AddDeleted(sheet);
            OnPropertyChanged("automaticSourcingSheetListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetSheet(AutomaticSourcingSheet sheet)
        {
            automaticSourcingSheetListChangeHandler.forget(sheet);
            OnPropertyChanged("automaticSourcingSheetListChangeHandler.Items");
        }
          
        public AutomaticSourcingSheet getAutomaticSourcingSheet(int position)
        {
            List<AutomaticSourcingSheet> liste = this.automaticSourcingSheetListChangeHandler.Items.ToList();

            if (liste.Count == 0) return null;
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].position == position)
                {
                    found = true;
                    return this.automaticSourcingSheetListChangeHandler.Items[mil];
                }
                else
                {
                    if (liste[mil].position > position)
                    {
                        fin = mil - 1;

                    }
                    else
                    {
                        debut = mil + 1;

                    }
                }
            }
            while (!found && debut <= fin);
            return null;
        }

        public int getAutomaticSourcingSheetIndex(int position)
        {
            List<AutomaticSourcingSheet> liste = this.automaticSourcingSheetListChangeHandler.Items.ToList();

            if (liste.Count == 0) return -1;
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].position == position)
                {
                    found = true;
                    return mil;
                }
                else
                {
                    if (liste[mil].position > position)
                    {
                        fin = mil - 1;

                    }
                    else
                    {
                        debut = mil + 1;

                    }
                }
            }
            while (!found && debut <= fin);
            return -1;
        }

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is AutomaticSourcing)) return 1;
            return this.name.CompareTo(((AutomaticSourcing)obj).name);
        }

        public void refresh()
        {
            foreach (AutomaticSourcingSheet sheet in this.automaticSourcingSheetListChangeHandler.Items)
            {
                sheet.parent = this;
                sheet.refresh();
            }
        }
    }
}
