using Misp.Kernel.Ui.Office.EDraw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Misp.Kernel.Util;
using Misp.Kernel.Ui.Office;
using System.Text.RegularExpressions;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class AutomaticSourcingSheet : Persistent,IComparable
    {
        public int position { get; set; }

        public string selectedRange { get; set; }
        
        
        public bool firstRowColumn { get; set; }

         
        public String startPosition{get;set;}
    
    
        public String endPosition{get;set;}

        [ScriptIgnore]
        public AutomaticSourcing parent { get; set; }

        public PersistentListChangeHandler<AutomaticSourcingColumn> automaticSourcingColumnListChangeHandler { get; set; }

        [field:NonSerialized]
        private bool _setSelectedRange;

        [field: NonSerialized]
        private bool _canSelectRange;

       /// <summary>
        /// This method is used to modify(Update and set) AutomaticSourcingColumn properties.
        ///Update/Set ParameterType
        ///Update/Set PeriodType
        ///Update/Set TargetType
        ///Update/Set Measure value
        ///Update/Set Attribute
        ///Update/Set CellPropertyAllocationData
        ///Update/Set group
        ///Update/Set ColumnTargetItem
        ///Update/Set dateFormat
        ///Update/Set TagName
        ///finally put the column in the convenient Persistent List.
       /// </summary>
       /// <param name="col">The given column</param>
       /// <param name="param">The property we want to modify</param>
        public void updateColumnParam(AutomaticSourcingColumn col,object param,object targetOperator = null,bool isDateFormat=false)
        {
            if (col == null) return;
            int index = this.getAutomaticSourcingColumnIndex(col.columnIndex);
            if (index == -1 && !col.toNew) return ;

            //Update/Set ParameterType
            if (param is Kernel.Application.ParameterType)
            {
                if ((Kernel.Application.ParameterType)param == Application.ParameterType.NULL)
                this.automaticSourcingColumnListChangeHandler.Items[index].RestoreDefault();
                else
                this.automaticSourcingColumnListChangeHandler.Items[index].parameterType = (Kernel.Application.ParameterType)param;
            }
          
            //Update/Set TargetType
            else if (param is Kernel.Application.TargetType && param != null)
            {
                this.automaticSourcingColumnListChangeHandler.Items[index].targetType = (Kernel.Application.TargetType)param;
            }

            //Update/Set Measure value
            else if (param is Kernel.Domain.Measure && this.automaticSourcingColumnListChangeHandler.Items[index].parameterType == Application.ParameterType.MEASURE)
                this.automaticSourcingColumnListChangeHandler.Items[index].measure = (Kernel.Domain.Measure)param;

           //Update/Set Attribute
            else if (param != null && param is Kernel.Domain.Attribute && this.automaticSourcingColumnListChangeHandler.Items[index].parameterType == Application.ParameterType.SCOPE)
                this.automaticSourcingColumnListChangeHandler.Items[index].attribute = (Kernel.Domain.Attribute)param;

            ///Update/Set CellPropertyAllocationData
            else if (param is CellPropertyAllocationData)
                this.automaticSourcingColumnListChangeHandler.Items[index].allocationData = (CellPropertyAllocationData)param;

            ///Update/Set group
            else if (param is BGroup && param != null)
                this.automaticSourcingColumnListChangeHandler.Items[index].targetGroup = (BGroup)param;

            ///Update/Set ColumnTargetItem
            else if (param != null && param is ColumnTargetItem && this.automaticSourcingColumnListChangeHandler.Items[index].parameterType == Application.ParameterType.TARGET)
            {
                ColumnTargetItem colTargetItem = param as ColumnTargetItem;
                if (targetOperator != null)
                colTargetItem.targetOperator = TargetItem.getOperatorByStringValue(targetOperator.ToString());
                if (colTargetItem.oid.HasValue)
                {
                    if (colTargetItem.toUpdate)
                    {
                        this.automaticSourcingColumnListChangeHandler.Items[index].UpdateColumnTargetItem(colTargetItem);
                    }
                    else if (colTargetItem.toDelete)
                    {
                        this.automaticSourcingColumnListChangeHandler.Items[index].RemoveColumnTargetItem(colTargetItem);
                    }
                }
                else
                {
                    if (colTargetItem.toNew)
                    {
                        this.automaticSourcingColumnListChangeHandler.Items[index].ForgetColumnTargetItem(colTargetItem);
                        this.automaticSourcingColumnListChangeHandler.Items[index].AddColumnTargetItem(colTargetItem);
                    }
                    else if (colTargetItem.toForget) this.automaticSourcingColumnListChangeHandler.Items[index].ForgetColumnTargetItem(colTargetItem);
                }
            }
            ///Update/Set dateFormat and TagName
            else if (param is string)
            {
                if (this.automaticSourcingColumnListChangeHandler.Items[index].parameterType == Application.ParameterType.PERIOD)
                {
                    if (isDateFormat) this.automaticSourcingColumnListChangeHandler.Items[index].dateFormat = param.ToString();
                    else this.automaticSourcingColumnListChangeHandler.Items[index].periodName = param.ToString();
                }
                if (this.automaticSourcingColumnListChangeHandler.Items[index].parameterType == Application.ParameterType.TAG)
                    this.automaticSourcingColumnListChangeHandler.Items[index].tagName = param.ToString();
            }

            ///Putting the column in the convenient Persistent List.
            if (col.toUpdate) this.UpdateColumn(col);
            else if (col.toDelete) this.RemoveColumn(col);
            else if (col.toNew) this.AddColumn(col);
            else if (col.toForget) this.ForgetColumn(col);
        }

        public void setColumnName(AutomaticSourcingColumn col, String name) 
        {
            if (col == null) return;
            int index = this.getAutomaticSourcingColumnIndex(col.columnIndex);
            this.automaticSourcingColumnListChangeHandler.Items[index].Name = name;
        }


        [ScriptIgnore]
        public Dictionary<int, AutomaticSourcingColumn> listeColumn;

        
        public Dictionary<int, AutomaticSourcingColumn> getListeColumn()
        {
            foreach (AutomaticSourcingColumn col in this.automaticSourcingColumnListChangeHandler.Items) 
            {
                listeColumn.Add(col.columnIndex, col);        
            }
            return listeColumn;
        }

          #region Managing Persitent  new , update, delete Lists

        public void setToUpdate()
        {
            this.toUpdate = true;
            this.toNew = false;
            this.toDelete = false;
            this.toForget = false;
        }

        public void setToNew()
        {
            this.toUpdate = false;
            this.toNew = true;
            this.toDelete = false;
            this.toForget = false;
        }

        public void setToDelete()
        {
            this.toDelete = true;
            this.toUpdate = false;
            this.toNew = false;
            this.toForget = false;
        }

        public void setToForget()
        {
            this.toForget = true;
            this.toDelete = false;
            this.toUpdate = false;
            this.toNew = false;
        }
        #endregion

        [ScriptIgnore]
        public Range rangeSelected
        {
            get;
            set;
        }


        public Range buildRange(String rangeTemp)
        {
            string patternRange = @"^[a-zA-Z]+[0-9]+(:[a-zA-Z]+[0-9]+){0,1}$";
            if (!Regex.IsMatch(rangeTemp, patternRange)) return null;
            RangeItem rangeItem = new RangeItem();

            if (rangeTemp.Contains(':'))
            {
                string[] bloc = rangeTemp.Split(':');

                string bloc1 = bloc[0];
                string bloc2 = bloc[1];
                startPos = Coord(bloc1.ToCharArray());
                endPos = Coord(bloc2.ToCharArray());

                rangeItem.Column1 = (int)startPos.X == 0 ? 1 : (int)startPos.X;
                rangeItem.Row1 = (int)startPos.Y == 0 ? 1 : (int)startPos.Y;

                rangeItem.Column2 = (int)endPos.X == 0 ? 1 : (int)endPos.X;
                rangeItem.Row2 = (int)endPos.Y == 0 ? 1 : (int)endPos.Y;
            }
            else 
            {
                System.Windows.Point coords1 = Coord(rangeTemp.ToCharArray());
                rangeItem.Column1 = rangeItem.Column2 = (int)coords1.X == 0 ? 1 : (int)coords1.X;
                rangeItem.Row1 = rangeItem.Row2 = (int)coords1.Y == 0 ? 1 : (int)coords1.Y;
            }
            Range range = new Range();
            range.Items.Add(rangeItem);

            return range;
        }

        [ScriptIgnore]
        public System.Windows.Point startPos { get; set; }

        [ScriptIgnore]
        public System.Windows.Point endPos { get; set; }


        System.Windows.Point Coord(char[] blocChar1)
        {
            int j = 0;

            string colValue = "";
            for (int i = blocChar1.Count() - 1; i >= 0; i--)
            {
                object index = null;
                try
                {
                    index = int.Parse(blocChar1[j].ToString());
                    break;
                }
                catch (Exception)
                {
                    colValue += blocChar1[j];
                }

                j++;
            }
            string rowValue = "";
            for (int p = j; p <= blocChar1.Count() - 1; p++)
            {
                rowValue += blocChar1[p];
            }

            return new System.Windows.Point(Kernel.Util.RangeUtil.GetColumnIndex(colValue.ToUpper()), int.Parse(rowValue));
        }

        [ScriptIgnore]
        public bool CanSelectRange
        {
            get { return this._canSelectRange; }
            set { this._canSelectRange = value; }
        }

        [ScriptIgnore]
        public bool SetSelectedRange 
        {
            get { return this._setSelectedRange; }
            set { this._setSelectedRange = value; }
        }

        [field: NonSerialized]
        private string name;

        [ScriptIgnore]
        public string Name
        {
            get 
            {
                return this.name;
            }
            set 
            {
                this.name = value;
            }
        }


        [field:NonSerialized]
        private AutomaticSourcingColumn activeColumn;

        [ScriptIgnore]
        public AutomaticSourcingColumn ActiveColumn
        {
            get 
            {
                 return this.activeColumn;
            }
            set 
            {
                this.activeColumn = value;
            }
        }

        public AutomaticSourcingSheet()
        {
            this.automaticSourcingColumnListChangeHandler = new PersistentListChangeHandler<AutomaticSourcingColumn>();
        }

        public AutomaticSourcingSheet(string _name,int _position,bool firstRow,string range,PersistentListChangeHandler<AutomaticSourcingColumn> listeColumns) 
        {
            this.automaticSourcingColumnListChangeHandler = new PersistentListChangeHandler<AutomaticSourcingColumn>();
            this.Name = _name;
            this.position = _position;
            this.selectedRange = range;
            this.firstRowColumn = firstRow;
            if(listeColumns != null)
            this.automaticSourcingColumnListChangeHandler = listeColumns ;
        }

        [ScriptIgnore]
        public List<AutomaticSourcingColumn> listColumnToDisplay { get; set; }

        public AutomaticSourcingColumn getColumnInListToDisplay(int position) 
        {
            List<AutomaticSourcingColumn> liste = this.listColumnToDisplay;

            if (liste.Count == 0) return null;
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].columnIndex == position)
                {
                    found = true;
                    return this.listColumnToDisplay[mil];
                }
                else
                {
                    if (liste[mil].columnIndex > position)
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

        public AutomaticSourcingColumn getFirstInList() 
        {
            if (this.listColumnToDisplay == null || this.listColumnToDisplay.Count() == 0)
                return null;
            return this.listColumnToDisplay[0];
        }

        public AutomaticSourcingColumn getAutomaticSourcingColumn(int position)
        {
            List<AutomaticSourcingColumn> liste = this.automaticSourcingColumnListChangeHandler.Items.ToList();

            if (liste.Count == 0) return null;
            if (position == 0) return this.automaticSourcingColumnListChangeHandler.Items[position];
            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].columnIndex == position)
                {
                    found = true;
                    return this.automaticSourcingColumnListChangeHandler.Items[mil];
                }
                else
                {
                    if (liste[mil].columnIndex > position)
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

        public AutomaticSourcingColumn findInDeleted(int position) 
        {
            foreach (AutomaticSourcingColumn col in this.automaticSourcingColumnListChangeHandler.deletedItems) 
            {
                if (col.columnIndex == position) return col;
            }
            return null;
        }

        public int getAutomaticSourcingColumnIndex(int position)
        {
            List<AutomaticSourcingColumn> liste = this.automaticSourcingColumnListChangeHandler.Items.ToList();
            if (liste.Count == 0) return -1;

            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].columnIndex == position)
                {
                    found = true;
                    return mil;
                }
                else
                {
                    if (liste[mil].columnIndex > position)
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

        public int getAutomaticSourcingColumnIndex(string columnName) 
        {
            List<AutomaticSourcingColumn> liste = this.automaticSourcingColumnListChangeHandler.Items.ToList();
            if (liste.Count == 0) return -1;

            bool found = false;
            int fin = liste.Count - 1;
            int debut = 0;
            int mil = 0;
            do
            {
                mil = (int)((fin + debut) / 2);

                if (liste[mil].columnIndex == position)
                {
                    found = true;
                    return mil;
                }
                else
                {
                    if (liste[mil].columnIndex > position)
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

        public Dictionary<int,AutomaticSourcingColumn> getListColumnScopeTarge() 
        {
            Dictionary<int, AutomaticSourcingColumn> result = new Dictionary<int, AutomaticSourcingColumn>(0);

            foreach (AutomaticSourcingColumn col in this.automaticSourcingColumnListChangeHandler.Items) 
            {
                if((col.parameterType == Application.ParameterType.SCOPE && col.attribute != null)
                || (col.parameterType == Application.ParameterType.TARGET))
                {
                    result.Add(col.columnIndex, col);
                }
            }
            return result;

        }

        public int getColumnsCount() 
        {
            if (this.automaticSourcingColumnListChangeHandler == null)
                return 0;
            return this.automaticSourcingColumnListChangeHandler.getItems().Count();
        }

        /// <summary>
        /// Rajoute un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void AddColumn(AutomaticSourcingColumn column)
        {
            column.parent = this;
            automaticSourcingColumnListChangeHandler.AddNew(column);
            OnPropertyChanged("automaticSourcingColumnListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateColumn(AutomaticSourcingColumn column)
        {
            automaticSourcingColumnListChangeHandler.AddUpdated(column);
            OnPropertyChanged("automaticSourcingColumnListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void RemoveColumn(AutomaticSourcingColumn column)
        {
            automaticSourcingColumnListChangeHandler.AddDeleted(column);
            OnPropertyChanged("automaticSourcingColumnListChangeHandler.Items");
        }

        /// <summary>
        /// Oublier un Sheet
        /// </summary>
        /// <param name="cell"></param>
        public void ForgetColumn(AutomaticSourcingColumn column)
        {
            automaticSourcingColumnListChangeHandler.forget(column);
            OnPropertyChanged("automaticSourcingColumnListChangeHandler.Items");
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is AutomaticSourcingSheet)) return 1;
            return this.position.CompareTo(((AutomaticSourcingSheet)obj).position);
        }

        [ScriptIgnore]
        public bool toUpdate { get; set; }

        [ScriptIgnore]
        public bool toNew { get; set; }

        [ScriptIgnore]
        public bool toDelete { get; set; }

        [ScriptIgnore]
        public bool toForget { get; set; }

        public void refresh()
        {
            foreach (AutomaticSourcingColumn column in this.automaticSourcingColumnListChangeHandler.Items) 
            {
                column.parent = this;
            }
        }
    }
}
