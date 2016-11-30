using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Misp.Sourcing.AutomaticSourcingViews
{
    /// <summary>
    /// Interaction logic for AutomaticSourcingPanel.xaml
    /// </summary>
    public partial class AutomaticSourcingPanel : ScrollViewer
    {
        public AutomaticSourcing AutomaticSourcing { get; private set; }

        

        private bool throwChange = true;
        #region Events
        public event ChangeEventHandler Changed;

        public event OnSetFirstRowAsHeaderEventHandler SetFirstRowAsHeader;
        public delegate void OnSetFirstRowAsHeaderEventHandler(bool set);

        public event OnSelectRangeOptionChangeEventHandler SelectRangeOption;
        public delegate void OnSelectRangeOptionChangeEventHandler(bool set);


        public event OnFocusRangeTextBoxEventHandler FocusTextBox;
        public delegate void OnFocusRangeTextBoxEventHandler(bool set);

        public event OnRemoveColumnItemEventHandler OnRemoveColumnItem;
        public delegate void OnRemoveColumnItemEventHandler(object item);

        public event OnSelectColumnEventHandler OnSelectColumn;
        public delegate void OnSelectColumnEventHandler(object item,int index);


        public event OnNewColumnEventHandler OnNewColumn;
        public delegate void OnNewColumnEventHandler();

        public event OnRemoveColumnEventHandler OnRemoveColumn;
        public delegate void OnRemoveColumnEventHandler(object item,int index);

        public event OnRemoveExcelColumnEventHandler OnRemoveExcelColumn;
        public delegate void OnRemoveExcelColumnEventHandler(object item, int index);

        public event OnSelectionChangeEventHandler OnSelectColumnItem;
        public delegate void OnSelectionChangeEventHandler(object item);

        public event OnSelectionTargetChangeEventHandler OnSelectTarget;
        public delegate void OnSelectionTargetChangeEventHandler(object item, object operatorValue);

        public event OnSetTextBoxValueEventHandler OnSetTextBoxValue;
        public delegate void OnSetTextBoxValueEventHandler();

        public event OnChooseTargetEventHandler OnChooseTarget;
        public delegate void OnChooseTargetEventHandler(object parameters);

        public event OnChooseTargetTypeEventHandler OnChooseTargetType;
        public delegate void OnChooseTargetTypeEventHandler(object parameters);

        public event OnTypeChangeEventHandler OnTypeChange;
        public delegate void OnTypeChangeEventHandler(Kernel.Application.ParameterType typeValue);

        public event OnSetPeriodEventHandler OnSetPeriod;
        public delegate void OnSetPeriodEventHandler(object parameters,bool isDateFormat);

        public event OnSetTagNameEventHandler OnSetTagName;
        public delegate void OnSetTagNameEventHandler(string tagName);

        public event OnAllocationPanelChangeEventHandler OnAllocationPanelChange;
        public delegate void OnAllocationPanelChangeEventHandler();

        public event OnSelectedRangeChangeEventHandler OnSelectedRangeChange;
        public delegate void OnSelectedRangeChangeEventHandler(object range);

        public event OnSetTargetGroupEventHandler OnSetTargetGroup;
        public delegate void OnSetTargetGroupEventHandler(string groupName);

        #endregion

        #region Constructor
        public AutomaticSourcingPanel()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        #endregion

        #region Handlers
        private void InitializeHandlers()
        {
            this.SheetPanel.SelectRangeOption +=SelectRangeOptionHandler;
            this.SheetPanel.SetFirstRowAsHeader +=SetFirstRowAsHeaderHandler;

            this.SheetPanel.FocusTextBox += FocusTextBoxHandler;
            this.SheetPanel.OnNewColumn +=AutomaticSourcingSheetPanel_OnNewColumn;
         
            this.SheetPanel.OnRemoveColumn +=AutomaticSourcingSheetPanel_OnRemoveColumn;

            this.SheetPanel.SelectColumn +=AutomaticSourcingSheetPanel_SelectColumn;
            this.SheetPanel.OnSelectTarget += AutomaticSourcingSheetPanel_OnSelectTarget;
            this.SheetPanel.OnSelectColumnItem +=AutomaticSourcingSheetPanel_OnSelectColumnItem;
            
            this.SheetPanel.OnTypeChange +=AutomaticSourcingSheetPanel_OnTypeChange;

            this.SheetPanel.OnSetTagName +=AutomaticSourcingSheetPanel_OnSetTagName;

            this.SheetPanel.OnSetPeriod +=AutomaticSourcingSheetPanel_OnSetPeriod;
            this.SheetPanel.OnChooseTarget +=AutomaticSourcingSheetPanel_OnChooseTarget;
            this.SheetPanel.OnRemoveColumnItem +=AutomaticSourcingSheetPanel_OnRemoveColumnItem;
            this.SheetPanel.OnChooseTargetType +=AutomaticSourcingSheetPanel_OnChooseTargetType;
            this.SheetPanel.OnSelectedRangeChange +=AutomaticSourcingSheetPanel_OnSelectedRangeChange;
            this.SheetPanel.OnAllocationPanelChange +=AutomaticSourcingSheetPanel_OnAllocationPanelChange;
            this.SheetPanel.OnSetTargetGroup +=SheetPanel_OnSetTargetGroup;

            this.SheetPanel.Changed += OnChanged;
        }

        private void SheetPanel_OnSetTargetGroup(string groupName)
        {
            if (OnSetTargetGroup != null) OnSetTargetGroup(groupName);
        }

        private void AutomaticSourcingSheetPanel_OnSelectedRangeChange(object range, bool focusvalue)
        {
            
        }

        private void AutomaticSourcingSheetPanel_OnSelectedRangeChange(object range)
        {
            if (OnSelectedRangeChange != null) OnSelectedRangeChange(range);
        }

        private void AutomaticSourcingSheetPanel_OnAllocationPanelChange()
        {
            if (OnAllocationPanelChange != null)
                OnAllocationPanelChange();
        }

        private void AutomaticSourcingSheetPanel_OnChooseTargetType(object parameters)
        {
            if (OnChooseTargetType != null)
                OnChooseTargetType(parameters);
        }

        private void AutomaticSourcingSheetPanel_OnRemoveColumnItem(object item)
        {
            if (OnRemoveColumnItem != null)
                OnRemoveColumnItem(item);
        }

        private void AutomaticSourcingSheetPanel_OnChooseTarget(object parameters)
        {
            if (OnChooseTarget != null)
                OnChooseTarget(parameters);
        }

        private void AutomaticSourcingSheetPanel_OnSetPeriod(object parameters,bool isDateFormat)
        {
            if (OnSetPeriod != null)
                OnSetPeriod(parameters,isDateFormat);
        }

        private void AutomaticSourcingSheetPanel_OnSetTagName(string tagName)
        {
            if (OnSetTagName != null)
                OnSetTagName(tagName);

        }

        private void AutomaticSourcingSheetPanel_OnTypeChange(Kernel.Application.ParameterType typeValue)
        {
            if (OnTypeChange != null)
                OnTypeChange(typeValue);

        }

        private void AutomaticSourcingSheetPanel_OnSetTextBoxValue()
        {
            if (OnSetTextBoxValue != null) OnSetTextBoxValue();
        }

        private void AutomaticSourcingSheetPanel_OnSelectColumnItem(object item)
        {
            if (OnSelectColumnItem != null) OnSelectColumnItem(item);
        }

        private void AutomaticSourcingSheetPanel_SelectColumn(object item,int index)
        {
            if (OnSelectColumn != null)
                OnSelectColumn(item,index);
        }

        private void AutomaticSourcingSheetPanel_OnSelectTarget(object item, object operatorValue) 
        {
            if (OnSelectTarget != null) OnSelectTarget(item, operatorValue);
        }

        private void AutomaticSourcingSheetPanel_OnRemoveColumn(object item,int index)
        {
            if (OnRemoveColumn != null)
                OnRemoveColumn(item,index);
        }

        private void AutomaticSourcingSheetPanel_OnNewColumn()
        {
            if (OnNewColumn != null)
                OnNewColumn();
        }

        private void FocusTextBoxHandler(bool set)
        {
            if (FocusTextBox != null) 
            {
                FocusTextBox(set);
            }

        }

        private void SetFirstRowAsHeaderHandler(bool set)
        {
           if (SetFirstRowAsHeader != null)
            {
                SetFirstRowAsHeader(set);
            } 
        }

        private void SelectRangeOptionHandler(bool set)
        {
            if (SelectRangeOption != null)
            {
                SelectRangeOption(set);
            }
        }

        #endregion

        #region Public Methods
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        public void SetSelectedAttribute(Kernel.Domain.Attribute attribute) 
        {
            this.SheetPanel.SetSelectedAttribute(attribute);
        }

        public void DisplayMeasure()
        {
            this.SheetPanel.DisplayMeasure();
        }

        public void DisplayScope()
        {
            this.SheetPanel.DisplayScope();
        }

        public void DisplayPeriod()
        {
            this.SheetPanel.DisplayPeriod();
        }

        public void DisplayTag() 
        {
            this.SheetPanel.DisplayTag();
        }

        public void SetSelecteColumn(int columnIndex) 
        {
            this.SheetPanel.SetSelecteColumn(columnIndex);
        }

        public List<int> GetListBoxItems() 
        {
          return  this.SheetPanel.getListBoxItems();
        }

        public int getColumnInListBox(int columnIndex) 
        {
            return this.SheetPanel.getColumnInListBox(columnIndex);
        }
        public bool GetSelectionRangeState()
        {
            return this.SheetPanel.GetSelectionRangeState();
        }

        public bool GetFirstRowSelectionState() 
        {
            return this.SheetPanel.GetFirstRowSelectionState();
        }

        public void DisplayAllocationData(CellPropertyAllocationData CellPropertyAllocationData)
        {
            this.SheetPanel.DisplayAllocationData(CellPropertyAllocationData);
        }

        public void DisplayColumn(AutomaticSourcingColumn col, List<AutomaticSourcingColumn> liste) 
        {
            this.SheetPanel.DisplayColumn(col,liste);
        }

        public AutomaticSourcingColumn GetSelectedColumn() 
        {
           return  this.SheetPanel.GetSelectedColumn();
        }

        public AutomaticSourcingSheet GetSelectedSheet() 
        {
            return this.SheetPanel.GetSelectedSheet();
        }


        public int GetSelectedListIndex() 
        {
            return this.SheetPanel.GetSelectedListIndex();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure"></param>
        public void SetSelectedMeasure(Kernel.Domain.Measure measure) 
        {
            this.SheetPanel.SetSelectedMeasure(measure);
        }


        public CellPropertyAllocationData GetAllocationData() 
        {
            return this.SheetPanel.GetAllocationData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="automaticSourcing"></param>
        public void Display(AutomaticSourcing automaticSourcing) 
        {
            throwChange = false;
            
            this.AutomaticSourcing = automaticSourcing;
            NameTextBox.Text = this.AutomaticSourcing != null ? this.AutomaticSourcing.name : "";
            groupGroupField.textBox.Text = this.AutomaticSourcing != null ? this.AutomaticSourcing.group != null ? this.AutomaticSourcing.group.name : "":"";
            
            if(this.AutomaticSourcing != null && this.AutomaticSourcing.GetCountSheets() > 0){
                int activeSheetIndex = this.AutomaticSourcing.ActiveSheetIndex;
                this.AutomaticSourcing.ActiveSheet = this.AutomaticSourcing.getAutomaticSourcingSheet(activeSheetIndex);
                SheetPanel.Display(automaticSourcing != null ? this.AutomaticSourcing.ActiveSheet : null);
            }
            
            throwChange = true;
        }


        public void SetSelectedIndex(int index)
        {
            this.SheetPanel.SetSelectedIndex(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<System.Windows.UIElement> getEditableControls()
        {
            List<UIElement> controls = new List<UIElement>(0);
            controls.Add(this.NameTextBox);
            controls.Add(this.groupGroupField);
            controls.Add(this.SheetPanel);
            return controls;
        }

        #endregion

        #region Private Method
     
        private void OnChanged()
        {
            if (Changed != null && throwChange) Changed();
        }
     
        #endregion

        public void displaySheet(AutomaticSourcingSheet sheet)
        {
            this.SheetPanel.Display(sheet);
        }

        public void displayColumns(List<AutomaticSourcingColumn> liste)
        {
            this.SheetPanel.FillListColumns(liste);
        }

        public void setPeriodName(PeriodName periodName)
        {
            this.SheetPanel.SetPeriod(periodName);
        }

        public void CustomizeForTarget()
        {
            this.SheetPanel.customizeForTarget();
        }

        public void customizeForEnrichmentTable()
        {
            this.SheetPanel.customizeForEnrichmentTable();
        }

        public string getTargetGroupName()
        {
            return this.SheetPanel.getTargetGroupName();
        }
    }
}
