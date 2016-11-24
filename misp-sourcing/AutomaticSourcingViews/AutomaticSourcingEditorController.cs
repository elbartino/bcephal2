using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Office;
using Misp.Kernel.Ui.Office.DevExpressSheet;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Util;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Misp.Sourcing.Base
{
    public class AutomaticSourcingEditorController : EditorController<Misp.Kernel.Domain.AutomaticSourcing, Misp.Kernel.Domain.Browser.BrowserData>
    {

        #region Properties
        private bool refreshMode = false;
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
        #endregion





        #region Editor and Service

        public virtual bool isAutomaticTarget()
        {
            return false;
        }

        public virtual bool isAutomaticGrid()
        {
            return false;
        }

        public virtual bool isGrid()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public AutomaticSourcingEditor getAutomaticSourcingEditor()
        {
            return (AutomaticSourcingEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés à l'automaticSourcing.
        /// </summary>
        /// <returns>DesignService</returns>
        public virtual AutomaticSourcingService GetAutomaticSourcingService()
        {
            return (AutomaticSourcingService)base.Service;
        }

        /// <summary>
        /// Service pour accéder aux opérations liées à l'inputTable.
        /// </summary>
        public InputTableService InputTableService { get; set; }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() {
            return new AutomaticSourcingEditor();
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new AutomaticSourcingToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        protected override SideBar getNewSideBar()
        {
            return new AutomaticSourcingSideBar();
        }

        protected override Kernel.Ui.Base.PropertyBar getNewPropertyBar()
        {
            AutomaticSourcingPropertyBar.isAutomaticTarget = isAutomaticTarget();
            AutomaticSourcingPropertyBar.isAutomaticGrid = isAutomaticGrid();
            return new AutomaticSourcingPropertyBar();
        }

        #endregion

        public override Kernel.Application.OperationState RenameItem(string newName)
        {
            return OperationState.CONTINUE;
        }

        public override Kernel.Application.OperationState Delete()
        {
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return  Misp.Kernel.Domain.SubjectType.AUTOMATIC_SOURCING;
        }

        private void onNameTextLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            String name = page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Text;
            if (validateName(page, name))
            {
                Rename(name);
            }
        }

        /// <summary>
        /// definit l'action a effectue si le name text box est edité
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onNameTextChange(object sender, System.Windows.Input.KeyEventArgs args)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                String name = page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Text;
                if (validateName(page, name))
                {
                    Rename(name);
                }
            }
        }

        protected override void Rename(string name)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Text = name;
            UpdateAutomaticSourcingSidebarName(name, page.Title, false);
            base.Rename(name);
            page.EditedObject.name = name;
        }

        #region Initializers

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// </summary>
        protected override void initializePageHandlers(EditorItem<AutomaticSourcing> page)
        {
            base.initializePageHandlers(page);
            AutomaticSourcingEditorItem editorPage = (AutomaticSourcingEditorItem)page;

            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.groupGroupField.GroupService = GetAutomaticSourcingService().GroupService;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.groupGroupField.subjectType = SubjectTypeFound();
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.groupGroupField.Changed += onGroupFieldChange;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.SelectRangeOption += OnSelectRange;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.SetFirstRowAsHeader += OnSetFirtRowAsHeader;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSelectColumn += OnSelectColumn;

            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.Changed += OnChanged;

            if (isAutomaticTarget())
            {
                editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSelectTarget += OnSelectTarget;
            }
            else
            {
                editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSelectColumnItem += OnSelectColumnTargetItem;
            }

            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnTypeChange += OnColumnParameterTypeChanged;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSetTagName += OnSetColumnTagName;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSetPeriod += OnSetPeriod;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnChooseTarget += OnChooseTarget;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnRemoveColumnItem += OnRemoveColumnTargetItem;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnChooseTargetType += OnChooseTargetType;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnRemoveColumn += OnRemoveColumn;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.KeyUp += onNameTextChange;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.LostKeyboardFocus += onNameTextLostFocus;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.KeyUp += onNameTextChange;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.KeyUp += onNameTextLostFocus;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnAllocationPanelChange += OnAllocationDataChange;
           // editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSelectedRangeChange += OnSelectedRangeChange;
            editorPage.getAutomaticSourcingForm().AutomaticSourcingPanel.OnSetTargetGroup += OnSetTargetGroup;
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.groupGroupField.GroupService = GetAutomaticSourcingService().GroupService;
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.groupGroupField.subjectType = Misp.Kernel.Domain.SubjectType.INPUT_TABLE;
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.groupGroupField.Changed += onTableGroupFieldChange;
            
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.periodPanel.ItemChanged += OnTablePeriodChange;
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.periodPanel.ItemDeleted += OnTablePeriodDelete;
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.periodPanel.PeriodHyperlink.RequestNavigate += OnNewPeriodName;

            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.filterScopePanel.ItemChanged += OnFilterChange;
            editorPage.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.filterScopePanel.ItemDeleted += OnFilterDelete;
            
            if (editorPage.getAutomaticSourcingForm().SpreadSheet != null)
            {
                editorPage.getAutomaticSourcingForm().SpreadSheet.SelectionChanged += OnSpreadSheetSelectionChanged;
                editorPage.getAutomaticSourcingForm().SpreadSheet.SheetActivated += OnSheetActivated;
                editorPage.getAutomaticSourcingForm().SpreadSheet.OnAddColumn += SpreadSheet_OnAddColumn;
                editorPage.getAutomaticSourcingForm().SpreadSheet.OnRemoveColumn += SpreadSheet_OnRemoveColumn;
            //    editorPage.getAutomaticSourcingForm().SpreadSheet.OnBeforeRightClick += SpreadSheet_OnBeforeRightClick;
            //
            }
        }

        private void OnNewPeriodName(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            EditorItem<AutomaticSourcing> page = getAutomaticSourcingEditor().getActivePage();
            if (page == null) return;
            page.InitializeCustomDialog("Create Period Name");
            if (page.CustomDialog.ShowCenteredToMouse().Value)
            {
                string name = page.namePanel.NameTextBox.Text;
                if (name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Create Period Name", "The name can't be empty!");
                    return;
                }
                PeriodName tagName = new PeriodName(name);
                List<PeriodName> tagNames = GetAutomaticSourcingService().PeriodNameService.getAll();
                try
                {
                    foreach (PeriodName tagname in tagNames)
                    {
                        if (tagname.name == tagName.name)
                        {
                            Kernel.Util.MessageDisplayer.DisplayInfo("Duplicate Period name", "Item named: " + name + " already exist!");
                            return;
                        }
                    }
                    tagName = GetAutomaticSourcingService().PeriodNameService.Save(tagName);
                }
                catch (BcephalException) { }

                tagNames = GetAutomaticSourcingService().PeriodNameService.getAll();

                ((AutomaticSourcingSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(tagNames);

                AutomaticSourcingPropertyBar propertyBar = (AutomaticSourcingPropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.AutomaticTablePropertiesLayoutAnchorable)
                    ((AutomaticSourcingEditorItem)page).getAutomaticSourcingForm().AutomaticTablePropertiesPanel.periodPanel.SetPeriodItemName(tagName.name);
            }
        }

        private void OnTablePeriodDelete(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            page.EditedObject.period.SynchronizeDeletePeriodItem(periodItem);
            if (page.EditedObject.period != null) page.EditedObject.period.itemListChangeHandler.Items = page.EditedObject.period.itemListChangeHandler.getItems();
            page.EditedObject.isModified = true;
            page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.periodPanel.DisplayPeriod(page.EditedObject.period);
            OnChange();
        }

        private void OnTablePeriodChange(object item)
        {
            if (item == null || !(item is PeriodItem)) return;
            PeriodItem periodItem = (PeriodItem)item;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            page.EditedObject.isModified = true;
            if (page.EditedObject.period == null) page.EditedObject.period = new Period();
            page.EditedObject.period.SynchronizePeriodItems(periodItem);
            page.EditedObject.period.itemListChangeHandler.Items = page.EditedObject.period.itemListChangeHandler.getItems();
            page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.periodPanel.DisplayPeriod(page.EditedObject.period);
            OnChange();
        }
      
        private void OnSetTargetGroup(string groupName)
        {
            if (groupName != "")
            {
                AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                BGroup bgroup = GetAutomaticSourcingService().GroupService.getGroupByName(groupName);
                if (bgroup == null)
                {
                    bgroup = new BGroup();
                    bgroup.name = groupName;
                    bgroup.subjectType = SubjectTypeFound().label;
                    BGroup rootGroup = GetAutomaticSourcingService().GroupService.getRootGroup(SubjectTypeFound());
                    bgroup.SetParent(rootGroup);
                    bgroup.SetPosition(rootGroup.childrenListChangeHandler.Items.Count);
                    rootGroup.AddChild(bgroup);
                    bgroup = GetAutomaticSourcingService().GroupService.Save(rootGroup);
                    if (bgroup == null) return;
                }
                page.EditedObject.ActiveSheet.ActiveColumn.targetGroup = bgroup;
            }
        }

        private void OnSelectColumn(object item, int index)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (page.EditedObject.ActiveSheet != null)
            {
                page.EditedObject.ActiveSheet.ActiveColumn = (AutomaticSourcingColumn)item;
                page.EditedObject.ActiveSheet.ActiveColumn.indexInListBox = index;
                List<AutomaticSourcingColumn> listeObjects = page.EditedObject.ActiveSheet.automaticSourcingColumnListChangeHandler.Items.ToList();
                page.getAutomaticSourcingForm().displayColumn((AutomaticSourcingColumn)item, listeObjects);
            }
        }

        private void SpreadSheet_OnRemoveColumn(int index)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            AutomaticSourcingColumn columnToRemove = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(index);
            int listboxIndex = page.getAutomaticSourcingForm().getColumnInListBox(columnToRemove.columnIndex);
            if (columnToRemove == null) return;
            page.EditedObject.ActiveSheet.listColumnToDisplay.Remove(columnToRemove);

            int numberElements = page.EditedObject.ActiveSheet.listColumnToDisplay.Count;
            Action UpdateAction = new Action((Action)(() =>
            {
                page.displayObject();

                if (numberElements > 0)
                {
                    int newCurrentIndex = listboxIndex < numberElements ? listboxIndex : listboxIndex - 1;
                    page.getAutomaticSourcingForm().AutomaticSourcingPanel.SetSelectedIndex(newCurrentIndex);
                }
                OnChange();
            }
             )
            );
            System.Windows.Application.Current.Dispatcher.Invoke(UpdateAction);
        }

        private void OnSelectedRangeChange(object range)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (page.getAutomaticSourcingForm().GetSelectionRangeState())
            {
                if (page.EditedObject.ActiveSheet.oid.HasValue) page.EditedObject.ActiveSheet.setToUpdate();

                page.EditedObject.updateSheetParams(page.EditedObject.ActiveSheet, (Kernel.Ui.Office.Range)range);
                InitializeColumnOnRangeChange();
                if (page.EditedObject.ActiveSheet.ActiveColumn != null){
                    page.getAutomaticSourcingForm().SetSelectedIndex(0);
                    page.getAutomaticSourcingForm().AutomaticSourcingPanel.SheetPanel.Display(page.EditedObject.ActiveSheet);
                }
                OnChange();
            }
        }
        
        private void OnFilterDelete(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            Target scope = page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.filterScopePanel.Scope;
            if (page.EditedObject.filter == null) page.EditedObject.filter = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            page.EditedObject.filter.SynchronizeDeleteTargetItem(targetItem);
            page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.filterScopePanel.DisplayScope(page.EditedObject.filter);
            OnChange();
        }

        private void OnFilterChange(object item)
        {
            if (item == null || !(item is TargetItem)) return;
            TargetItem targetItem = (TargetItem)item;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            Target scope = page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.filterScopePanel.Scope;
            if (page.EditedObject.filter == null) page.EditedObject.filter = new Target(Target.Type.OBJECT_VC, Target.TargetType.COMBINED);
            page.EditedObject.filter.SynchronizeTargetItems(targetItem);
            OnChange();
        }

        private void OnSheetActivated()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();

            Sheet activeSheet = page.getAutomaticSourcingForm().SpreadSheet.getActiveSheet();
            if (activeSheet == null) return;
            page.EditedObject.ActiveSheet = page.EditedObject.getAutomaticSourcingSheet(activeSheet.Index);
            if (page.EditedObject.ActiveSheet == null) return;
            String selecteRange = page.EditedObject.ActiveSheet.selectedRange;
            Range rangeSelected;
            if (selecteRange == "") rangeSelected = null;
            else
            {
                rangeSelected = page.EditedObject.ActiveSheet.buildRange(selecteRange);
                rangeSelected.Sheet = activeSheet;
            }
            page.EditedObject.ActiveSheetIndex = page.EditedObject.ActiveSheet.position;
            page.EditedObject.ActiveSheet.rangeSelected = rangeSelected;
            page.EditedObject.ActiveSheet.Name = activeSheet.Name;

            InitializeColumn();
            page.displayObject();
            page.getAutomaticSourcingForm().SetSelectedRange(selecteRange != "");

        }

        protected override void initializeViewData()
        {

        }

        protected override void initializeSideBarData()
        {
            List<Kernel.Domain.Browser.BrowserData> datas = this.Service.getBrowserDatas();
            ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.fillTree(new ObservableCollection<Kernel.Domain.Browser.BrowserData>(datas));

            ((AutomaticSourcingSideBar)SideBar).EntityGroup.InitializeData();

            if (!isAutomaticTarget())
            {
                ((AutomaticSourcingSideBar)SideBar).MeasureGroup.InitializeMeasure(false);

                List<CalculatedMeasure> ListCalculatedMeasure = GetAutomaticSourcingService().CalculatedMeasureService.getAllCalculatedMeasure();
                ((AutomaticSourcingSideBar)SideBar).CaculatedMeasureGroup.CalculatedMeasureTreeview.fillTree(new ObservableCollection<CalculatedMeasure>(ListCalculatedMeasure));

                PeriodName rootPeriodName = GetAutomaticSourcingService().PeriodNameService.getRootPeriodName();
                ((AutomaticSourcingSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);               
            }
            BGroup group = GetAutomaticSourcingService().GroupService.getDefaultGroup();
        }

        protected virtual void UpdateAutomaticSourcingSidebarName(string newName, string tableName, bool updateGroup)
        {
            ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.updateAutomaticSourcing(newName, tableName, updateGroup);
        }

        protected override void initializePropertyBarData()
        {

        }

        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            ((AutomaticSourcingToolBar)this.ToolBar).RunButton.Click += OnRun;
        }

        protected override void initializePropertyBarHandlers()
        {

        }

        protected override void initializeSideBarHandlers()
        {
            ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.SelectionChanged += SidebarAutomaticSourcingSelected;
            ((AutomaticSourcingSideBar)SideBar).MeasureGroup.Tree.Click += SidebarMeasureSelected;
            ((AutomaticSourcingSideBar)SideBar).EntityGroup.Tree.Click += SidebarTargetSelected;
            //((AutomaticSourcingSideBar)SideBar).EntityGroup.Tree.OnRightClick += onRightClickFromSidebar;
            ((AutomaticSourcingSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += SidebarPeriodNameSelected;
        }

        private void onRightClickFromSidebar(object sender)
        {
            if (sender != null && sender is Kernel.Ui.Popup.EntityPopup)
            {
                Kernel.Ui.Popup.EntityPopup popup = (Kernel.Ui.Popup.EntityPopup)sender;
                popup.OnValidate += OnValidate;
                Kernel.Domain.Attribute attribute = null;

                if (popup.Tag is Kernel.Domain.Attribute)
                {
                    attribute = (Kernel.Domain.Attribute)popup.Tag;
                    popup.selectedItem.Clear();
                    popup.selectedNames.Clear();


                    popup.ItemSource.Clear();
                    List<Kernel.Domain.AttributeValue> values = GetAutomaticSourcingService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    values.BubbleSortByName();
                    popup.ItemSource.AddRange(values);
                    popup.selectedItem.AddRange(attribute.FilterAttributeValues);
                    popup.FillSelectedNames();
                    popup.Tag = attribute;
                }
                //else if (popup.Tag is Kernel.Domain.AttributeValue) 
                //{
                //    popup.IsChildren = true;
                //    Kernel.Domain.AttributeValue value = (Kernel.Domain.AttributeValue)popup.Tag;
                //    popup.ItemSource.AddRange(value.childrenListChangeHandler.Items);
                //    popup.Tag = value;
                //}
                popup.IsOpen = true;
                popup.Display();
            }
        }

        private void OnValidate(object sender)
        {
            if (sender == null) return;
            if (!(sender is Array)) return;
            object[] senderArray = (object[])sender;
            bool isAttribute;
            Kernel.Domain.Attribute attribute = null;
            Kernel.Domain.AttributeValue value = null;
            List<Kernel.Domain.AttributeValue> listValues = new List<AttributeValue>(0);

            isAttribute = senderArray[1] is Kernel.Domain.Attribute;
            if (senderArray[0] is IList && senderArray[1] is Kernel.Domain.Target)
            {
                List<object> liste = (List<object>)senderArray[0];
                listValues.AddRange(liste.Cast<Kernel.Domain.AttributeValue>().ToList());
                attribute = isAttribute ? (Kernel.Domain.Attribute)senderArray[1] : null;
                value = !isAttribute ? (Kernel.Domain.AttributeValue)senderArray[1] : null;
            }

            if (isAttribute)
            {
                attribute.valueListChangeHandler.Items.Clear();
                attribute.FilterAttributeValues.Clear();
                attribute.FilterAttributeValues.AddRange(listValues);
            }
            else
            {
                attribute.FilterAttributeValues.Clear();
                attribute.FilterAttributeValues.AddRange(listValues);
            }

            foreach (Kernel.Domain.AttributeValue avalue in listValues)
            {
                attribute.valueListChangeHandler.Items.Add(avalue);
            }
        }

        private Kernel.Domain.Attribute currentAttribute;
                       
       

        private void SidebarPeriodNameSelected(object sender)
        {
            if (sender != null)
            {
                AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                if (IsColumnOutOfBounds(page)) return;
                page.getAutomaticSourcingForm().DisplayPeriod();
                PeriodName periodName = null;
                if (sender is Kernel.Domain.PeriodName)
                {
                    periodName = (Kernel.Domain.PeriodName)sender;
                }
                else if(sender is Kernel.Domain.PeriodInterval)
                {
                    periodName = ((Kernel.Domain.PeriodInterval)sender).periodName;
                }

                if (periodName == null) return;
                page.getAutomaticSourcingForm().SetPeriodName(periodName);
           }
        }

       

        #endregion

        #region AutomaticSourcingController Methods

        protected virtual Kernel.Domain.AutomaticSourcing GetNewAutomaticSourcing()
        {
            Kernel.Domain.AutomaticSourcing automaticSourcing = new Kernel.Domain.AutomaticSourcing();
            automaticSourcing.name = getNewPageName("Automatic Sourcing");
            automaticSourcing.group = GetAutomaticSourcingService().GroupService.getDefaultGroup(SubjectType.AUTOMATIC_SOURCING.label);
            automaticSourcing.tableGroup = GetAutomaticSourcingService().GroupService.getDefaultGroup(SubjectType.INPUT_TABLE.label);
            return automaticSourcing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Kernel.Domain.AutomaticSourcing GetObjectByName(string name)
        {
            return ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.getAutomaticSourcingByName(name);
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;

                Kernel.Domain.AutomaticSourcing automaticSourcing = GetObjectByName(name);
                if (automaticSourcing == null) return name;

                i++;
            }
            return name;
        }

        private void OnAllocationDataChange()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            CellPropertyAllocationData data = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetAllocationData();
            AutomaticSourcingColumn columnActivate = page.getAutomaticSourcingForm().GetSelectedColumn();
            if (columnActivate.measure != null)
            {
                columnActivate.allocationData = data;
                setColumnParams(data);
            }
            page.getAutomaticSourcingForm().DisplayAllocationData(data != null ? data : null);
            refreshMeasureInSideBar(data.measureRef);

            OnChange();
        }

        private void refreshMeasureInSideBar(Measure measure)
        {
            ((AutomaticSourcingSideBar)SideBar).MeasureGroup.InitializeMeasure(false);
        }

        protected virtual void onGroupFieldChange()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            string name = page.getAutomaticSourcingForm().AutomaticSourcingPanel.groupGroupField.textBox.Text;
            page.EditedObject.group = GetAutomaticSourcingService().GroupService.getGroupByNameAndType(name,SubjectType.AUTOMATIC_SOURCING.label);
            ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.updateAutomaticSourcing(name, page.Title, true);
        }

        protected virtual void onTableGroupFieldChange()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            string name = page.getAutomaticSourcingForm().AutomaticTablePropertiesPanel.groupGroupField.textBox.Text;
            page.EditedObject.tableGroup = GetAutomaticSourcingService().GroupService.getGroupByNameAndType(name,SubjectType.INPUT_TABLE.label);
        }

        private void OnChooseTargetType(object parameters)
        {
            if (parameters == null) return;
            setColumnParams((Kernel.Application.TargetType)parameters);
            OnChange();

        }

        private void OnChooseTarget(object parameters)
        {
            if (parameters == null) return;
            setColumnParams((Kernel.Application.TargetType)parameters);
            OnChange();
        }

        private void OnSetPeriod(object parameters,bool isFormatDate)
        {
            if (parameters == null) return;
            setColumnParams(parameters,isFormatDate);
            OnChange();
        }

        /// <summary>
        /// Set the tag name value to the selected column.
        /// </summary>
        /// <param name="tagName"></param>
        private void OnSetColumnTagName(string tagName)
        {
            if (!string.IsNullOrEmpty((String)tagName))
            {
                setColumnParams(tagName);
                OnChange();
            }
        }

        /// <summary>
        /// Set the ParameterType value to the selected column.
        /// </summary>
        /// <param name="typeValue"></param>
        private void OnColumnParameterTypeChanged(ParameterType typeValue)
        {
            setColumnParams(typeValue);
            OnChange();
        }

        private void OnChanged()
        {
            OnChange();
        }

        protected virtual void setColumnParams(object param,object targetType=null,bool isFormatDate = false)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            AutomaticSourcingSheet sheet = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedSheet();
            if (sheet == null)
            {
                int index = page.getAutomaticSourcingForm().SpreadSheet.getActiveSheetIndex();
                sheet = page.EditedObject.getAutomaticSourcingSheet(index);
                page.EditedObject.ActiveSheet = sheet;
            }

            AutomaticSourcingColumn column = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedColumn();
            if (column == null)
            {
                column = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(0);
                page.EditedObject.ActiveSheet.ActiveColumn = column;
            }

            if (sheet == null) return;
            if (sheet.oid.HasValue) page.EditedObject.UpdateSheet(sheet);
            sheet.updateColumnParam(column, param,targetType,isFormatDate);

            if (column.oid.HasValue) sheet.UpdateColumn(column);
        }

        /// <summary>
        /// Build the columnTargetItem  when creating a new target.
        /// </summary>
        /// <param name="item"></param>
        private void OnSelectColumnTargetItem(object item)
        {
            if (item != null && item is AutomaticSourcingColumn)
            {
                AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                AutomaticSourcingColumn col = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedColumn();
                if (col == null) return;

                ColumnTargetItem columnTargetItem = col.getColumnTargetItem((item as AutomaticSourcingColumn).columnIndex);

                if (columnTargetItem == null)
                {
                    columnTargetItem = new ColumnTargetItem((item as AutomaticSourcingColumn).columnIndex);
                    columnTargetItem.setToNew();
                }
                else if (columnTargetItem.oid.HasValue)
                {
                    columnTargetItem.setToUpdate();
                }
                setColumnParams(columnTargetItem);
            }
        }

        private void OnSelectTarget(object item,object operatorValue) 
        {
            if (item != null && item is AutomaticSourcingColumn)
            {
                AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                AutomaticSourcingColumn col = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedColumn();
                if (col == null) return;

                ColumnTargetItem columnTargetItem = col.getColumnTargetItem((item as AutomaticSourcingColumn).columnIndex);
                
                if (columnTargetItem == null)
                {
                    columnTargetItem = new ColumnTargetItem((item as AutomaticSourcingColumn).columnIndex);
                    columnTargetItem.setToNew();
                }
                else if (columnTargetItem.oid.HasValue)
                {
                    columnTargetItem.setToUpdate();
                }
                setColumnParams(columnTargetItem, operatorValue);
            }
        }

        private void OnRemoveColumnTargetItem(object item)
        {
            if (item != null)
            {
                AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                AutomaticSourcingColumn col = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedColumn();
                if (col == null) return;

                ColumnTargetItem columnTargetItem = new ColumnTargetItem();
                if (item is AutomaticSourcingColumn)
                {
                    columnTargetItem = col.getColumnTargetItem((item as AutomaticSourcingColumn).columnIndex);
                }
                if (item is ColumnTargetItem)
                {
                    columnTargetItem = col.getColumnTargetItem((item as ColumnTargetItem).columnIndex);
                }

                if (columnTargetItem.oid.HasValue) columnTargetItem.setToDelete();
                else columnTargetItem.setToForget();

                setColumnParams(columnTargetItem);
                OnChange();
            }
        }

        /// <summary>
        /// Remove a specified AutomaticSourcingcolumn in an automaticSourcingSheet
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        private void OnRemoveColumn(object item, int index)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            AutomaticSourcingColumn columnToRemove = null;
            if (item != null) columnToRemove = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedColumn();

            int activeSheetIndex = page.getAutomaticSourcingForm().SpreadSheet.getActiveSheetIndex();
            page.EditedObject.ActiveSheet = new AutomaticSourcingSheet();
            page.EditedObject.ActiveSheet = page.EditedObject.getAutomaticSourcingSheet(activeSheetIndex);

            Range UsableRange = page.EditedObject.ActiveSheet.rangeSelected;
            Action UpdateAction = new Action((Action)(() =>
            {
                if (columnToRemove == null) columnToRemove = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(index);

                int sheetIndex = page.EditedObject.getAutomaticSourcingSheetIndex(page.EditedObject.ActiveSheet.position);

                if (columnToRemove.oid.HasValue) columnToRemove.setToDelete();
                else columnToRemove.setToForget();

                page.EditedObject.UpdateSheet(page.EditedObject.ActiveSheet);

                page.EditedObject.ActiveSheet.listColumnToDisplay.Remove(columnToRemove);
                page.EditedObject.automaticSourcingSheetListChangeHandler.Items[sheetIndex].updateColumnParam(columnToRemove, null);
                int numberElements = page.EditedObject.ActiveSheet.getColumnsCount();
                page.getAutomaticSourcingForm().displayColumns(page.EditedObject.ActiveSheet.listColumnToDisplay);

                if (numberElements > 0)
                {
                    int newCurrentIndex = index < numberElements ? index : index - 1;
                    page.getAutomaticSourcingForm().AutomaticSourcingPanel.SetSelectedIndex(newCurrentIndex);
                    page.EditedObject.ActiveSheet.ActiveColumn = page.getAutomaticSourcingForm().GetSelectedColumn();
                }

                OnChange();
            }
                )
            );
            System.Windows.Application.Current.Dispatcher.Invoke(UpdateAction);
        }

        /// <summary>
        /// Set first line of the file as Header.
        /// </summary>
        /// <param name="set"></param>
        private void OnSetFirtRowAsHeader(bool set)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (page.EditedObject.ActiveSheet == null)
            {
                int position = page.getAutomaticSourcingForm().GetActiveSheetIndex();
                page.EditedObject.ActiveSheet = page.EditedObject.getAutomaticSourcingSheet(position);
                if (page.EditedObject.ActiveSheet == null) return;
                if (page.EditedObject.ActiveSheet.ActiveColumn == null)
                {
                    if (page.EditedObject.ActiveSheet.getColumnsCount() > 0)
                    {
                        page.EditedObject.ActiveSheet.ActiveColumn = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(1);
                        if (page.EditedObject.ActiveSheet.ActiveColumn == null) return;
                    }
                }
            }

            page.EditedObject.ActiveSheet.firstRowColumn = set;
            if (page.EditedObject.ActiveSheet.oid.HasValue) page.EditedObject.ActiveSheet.setToUpdate();
            page.EditedObject.updateSheetParams(page.EditedObject.ActiveSheet, set);

            FillAutomaticSourcingColumn();

            page.getAutomaticSourcingForm().displaySheet(page.EditedObject.ActiveSheet);
            if (page.EditedObject.ActiveSheet.getColumnsCount() > 0)
            {
                int index = page.getAutomaticSourcingForm().getColumnInListBox(page.EditedObject.ActiveSheet.ActiveColumn.columnIndex);
                page.getAutomaticSourcingForm().SetSelectedIndex(index >= 0 ? index : 0);
                page.getAutomaticSourcingForm().SetSelectedIndex(index);

                if (!refreshMode) OnChange();
            }
        }


        /// <summary>
        /// Consider a specific selection in the excel file.
        /// </summary>
        /// <param name="set"></param>
        private void OnSelectRange(bool selectedRange)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();

            int sheetIndex = page.getAutomaticSourcingForm().GetActiveSheetIndex();
            String sheetName = page.getAutomaticSourcingForm().GetActiveSheetName();

            if (page.EditedObject.ActiveSheet == null)
            {
                int position = page.getAutomaticSourcingForm().GetActiveSheetIndex();
                page.EditedObject.ActiveSheet = page.EditedObject.getAutomaticSourcingSheet(position);
                if (page.EditedObject.ActiveSheet == null) return;
            }
            page.EditedObject.ActiveSheet.SetSelectedRange = selectedRange;

            
            if (selectedRange)
            {
                OnSelectedRangeChange(page.getAutomaticSourcingForm().SpreadSheet.GetSelectedRange());
            }
            else 
            {
                page.EditedObject.ActiveSheet.selectedRange = "";
            }

            if (page.EditedObject.ActiveSheet.ActiveColumn == null)
            {
                int activeCol = page.getAutomaticSourcingForm().SpreadSheet.getActiveCell().Column;
                page.EditedObject.ActiveSheet.ActiveColumn = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(activeCol);
                int listboxPos = page.getAutomaticSourcingForm().getColumnInListBox(page.EditedObject.ActiveSheet.ActiveColumn.columnIndex);
                if (listboxPos > 0) page.getAutomaticSourcingForm().SetSelectedIndex(listboxPos);
                else page.getAutomaticSourcingForm().SetSelectedIndex(0);
            }

            if (!refreshMode) OnChange();
        }

        /// <summary>
        /// Fill the column List when the first row is set as the header.
        /// </summary>
        private void FillAutomaticSourcingColumn()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();

            if (page.EditedObject.ActiveSheet.selectedRange != "" && page.getAutomaticSourcingForm().GetSelectionRangeState())
            {
                InitializeColumnOnRangeChange();
            }
            else
            {
                page.EditedObject.ActiveSheet.listColumnToDisplay = new List<AutomaticSourcingColumn>(0);
                int j = 0;
                for (int i = page.EditedObject.ActiveSheet.getColumnsCount() - 1; i >= 0; i--)
                {
                    AutomaticSourcingColumn col = page.EditedObject.ActiveSheet.automaticSourcingColumnListChangeHandler.Items[j];
                    String excelName = getColumName(col.columnIndex);
                    page.EditedObject.ActiveSheet.setColumnName(col, excelName);
                    page.EditedObject.ActiveSheet.listColumnToDisplay.Add(col);
                    j++;
                }
            }
        }

        #endregion

        #region Side bar methods

        private void SidebarTargetSelected(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Target)
            {
                AutomaticSourcingPropertyBar propertyBar = (AutomaticSourcingPropertyBar)this.PropertyBar;
                if (propertyBar.Pane.SelectedContent == propertyBar.AutomaticTablePropertiesLayoutAnchorable)
                {
                    Target target = (Target)sender;
                    AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                    if (IsColumnOutOfBounds(page)) return;
                    page.getAutomaticSourcingForm().SetTargetItemValue(target);
                }
                else
                {
                    Kernel.Domain.Attribute attribute = null;
                    if (sender != null && sender is Kernel.Domain.Attribute)
                    {
                        attribute = (Kernel.Domain.Attribute)sender;
                    }
                    else if (sender != null && sender is Kernel.Domain.AttributeValue)
                    {
                        attribute = ((Kernel.Domain.AttributeValue)sender).attribut;
                    }
                    if (attribute == null) return;
                    AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                    if (IsColumnOutOfBounds(page)) return;
                    page.getAutomaticSourcingForm().DisplayScope();
                    setColumnParams(attribute);
                    page.getAutomaticSourcingForm().SetSelectedAttribute(attribute);


                }
                OnChange();
            }
        }

        protected virtual bool IsColumnOutOfBounds(AutomaticSourcingEditorItem page)
        {
            bool result = false;
            int selecteSheetIndex = page.getAutomaticSourcingForm().SpreadSheet.getActiveSheetIndex();
            if (page.EditedObject.ActiveSheet == null) page.EditedObject.ActiveSheet = page.EditedObject.getAutomaticSourcingSheet(selecteSheetIndex);
            if (page.EditedObject.ActiveSheet == null) result = true;
            int selectedColumn = page.getAutomaticSourcingForm().SpreadSheet.getActiveCell().Column;
            if (!result && page.EditedObject.ActiveSheet.ActiveColumn == null) page.EditedObject.ActiveSheet.ActiveColumn = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(selectedColumn);
            if (!result && page.EditedObject.ActiveSheet.ActiveColumn == null) page.EditedObject.ActiveSheet.ActiveColumn = page.getAutomaticSourcingForm().GetSelectedColumn();
            if (!result && page.EditedObject.ActiveSheet.ActiveColumn == null) page.EditedObject.ActiveSheet.ActiveColumn = page.EditedObject.ActiveSheet.getFirstInList();
            if (!result && page.EditedObject.ActiveSheet.ActiveColumn == null) result = true;

            if (result) MessageDisplayer.DisplayInfo("Automatic Sourcing ", "There is not active  column !");
            return result;
        }

        private void SidebarMeasureSelected(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Measure)
            {
                Kernel.Domain.Measure measure = (Kernel.Domain.Measure)sender;

                AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
                if (IsColumnOutOfBounds(page)) return;
                page.getAutomaticSourcingForm().DisplayMeasure();
                setColumnParams(measure);
                page.getAutomaticSourcingForm().SetSelectedMeasure(measure);
                OnChange();
            }
        }

        private void SidebarAutomaticSourcingSelected(object sender)
        {
            if (sender != null)
            {
                AutomaticSourcing automaticSourcing = null;
                if (sender is AutomaticSourcing)
                {
                    automaticSourcing = (AutomaticSourcing)sender;
                }
                if (sender is Kernel.Domain.Browser.BrowserData)
                {
                    automaticSourcing = new AutomaticSourcing();
                    automaticSourcing.oid = ((Kernel.Domain.Browser.BrowserData)sender).oid;
                    automaticSourcing.name = ((Kernel.Domain.Browser.BrowserData)sender).name;
                }
                EditorItem<AutomaticSourcing> page = getAutomaticSourcingEditor().getPage(automaticSourcing.name);
                Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
                string excelDir = fileDirs != null ? fileDirs.AutomaticSourcingDir : "";
                string filePath = excelDir + automaticSourcing.excelFile;

                if (automaticSourcing.oid != null && automaticSourcing.oid.HasValue && automaticSourcing.oid > 0)
                {
                    this.Open(automaticSourcing.oid.Value);
                }
                else if (page != null)
                {
                    page.fillObject();
                    getAutomaticSourcingEditor().selectePage(page);
                }
                else
                {
                    page = getAutomaticSourcingEditor().addOrSelectPage(automaticSourcing);
                    initializePageHandlers(page);
                    page.Title = automaticSourcing.name;
                    getAutomaticSourcingEditor().ListChangeHandler.AddNew(automaticSourcing);
                    ((AutomaticSourcingEditorItem)page).getAutomaticSourcingForm().SpreadSheet.Open(filePath);
                }

            }
        }
        
        #endregion

        #region name action
        protected virtual string GetProvidedNewName(string newName, AutomaticSourcingEditorItem page)
        {
            if (string.IsNullOrEmpty(newName))
                newName = page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Text.Trim();
            return newName;
        }

        /// <summary>
        /// verifie le text saisie au nivo du textbox name
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        private OperationState ValidateEditedNewName(string newName = "")
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            AutomaticSourcing automaticSourcing = page.EditedObject;
            newName = GetProvidedNewName(newName, page);
            if (String.IsNullOrWhiteSpace(newName))
            {
                DisplayError("Empty Name", "The " + page.EditedObject.GetType().Name + " name can't be mepty!");
                setNameTextBox(page);
                return OperationState.STOP;
            }


            if (VerifyDuplicateName(newName, page, automaticSourcing) != OperationState.CONTINUE)
                return OperationState.STOP;

            AutomaticSourcing automaticSourcingExist = GetObjectByName(newName);
            if (automaticSourcingExist != null && automaticSourcingExist.name != page.EditedObject.name)
            {
                DisplayError("Duplicate Name", "There is another AutomaticSourcing named: " + newName);
                return OperationState.STOP;
            }
            automaticSourcing.name = newName;
            OnChange();

            page.displayObject();
            return OperationState.CONTINUE;
        }

        private OperationState VerifyDuplicateName(string newName, AutomaticSourcingEditorItem page, AutomaticSourcing automaticSourcing)
        {
            foreach (AutomaticSourcingEditorItem automaticSourcingItem in getAutomaticSourcingEditor().getPages())
            {
                if (automaticSourcingItem != getAutomaticSourcingEditor().getActivePage() && newName == automaticSourcingItem.Title)
                {
                    DisplayError("Duplicate Name", "There is another " + automaticSourcing.GetType().Name + " named: " + newName);
                    page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Text = page.Title;
                    setNameTextBox(page);
                    return OperationState.STOP;
                }
            }

            if (automaticSourcing.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;
            return OperationState.CONTINUE;
        }

        private void setNameTextBox(AutomaticSourcingEditorItem page)
        {
            page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.SelectAll();
            page.getAutomaticSourcingForm().AutomaticSourcingPanel.NameTextBox.Focus();
        }

        #endregion

        #region Editor actions

        public override Kernel.Application.OperationState Search(object oid)
        {
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// Methode à exécuter lorsqu'il y a un changement sur la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState OnChange()
        {
            base.OnChange();
            return OperationState.CONTINUE;
        }

        BusyAction action;
        public virtual OperationState Run()
        {
            OperationState state = OperationState.CONTINUE;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (page == null) return state;
            state = beforeRun(page);
            if (state == OperationState.STOP) return state;
            performRun(page);
            this.AfterRun();
            return OperationState.CONTINUE;
        }

        protected virtual OperationState beforeRun(AutomaticSourcingEditorItem page)
        {
            OperationState state = OperationState.CONTINUE;
            if (page == null) return state;
            if (page.IsModify)
            {
                state = Save(page);
                this.AfterSave();
            }
            return state;
        }

        private string getBaseName(AutomaticSourcingEditorItem page) 
        {
            string baseName = page.getAutomaticSourcingForm().SpreadSheet.DocumentName.Trim();
            baseName = System.IO.Path.GetFileNameWithoutExtension(page.EditedObject.excelFile);
            if (!string.IsNullOrEmpty(baseName)) return baseName;
            if (isAutomaticGrid()) baseName = "Grid";
            else if (isAutomaticTarget()) baseName = "Target";
            else baseName = "Table";
            return baseName;
        }

        protected virtual void performRun(AutomaticSourcingEditorItem page)
        {
            string baseName = getBaseName(page);
            String name = baseName;
            int i = 1;
            string filePath = page.EditedObject.excelFile;
            string path = System.IO.Path.GetDirectoryName(filePath) + System.IO.Path.DirectorySeparatorChar;
            string fileName = GetAutomaticSourcingService().FileService.FileTransferService.AutomaticActionsUpload(System.IO.Path.GetFileName(filePath), path);
            if (fileName == null) return;
             
            AutomaticSourcingTableDialog AutomaticSourcingTableDialog = null;
            if(isAutomaticTarget())
            {
                AutomaticSourcingTableDialog =  new AutomaticSourcingTableDialog();
                AutomaticSourcingTableDialog.isTarget = true;
            }
            else
            {
                while (GetAutomaticSourcingService().InputTableService.getByName(name) != null)
                {
                    name = baseName + i++;
                }
                AutomaticSourcingTableDialog =  new AutomaticSourcingTableDialog();
            }
            AutomaticSourcingTableDialog.Customize();
            AutomaticSourcingTableDialog.AutomaticSourcingService = GetAutomaticSourcingService();
            AutomaticSourcingTableDialog.SetInputTableName(name);
            AutomaticSourcingTableDialog.Owner = ApplicationManager.Instance.MainWindow;
            AutomaticSourcingTableDialog.ShowDialog();
            if (AutomaticSourcingTableDialog.requestGenerateInputTable)
            {
                name = AutomaticSourcingTableDialog.inputTableName;
                GetAutomaticSourcingService().SaveTableHandler += UpdateSaveInfo;
                GetAutomaticSourcingService().OnUpdateUniverse += OnUpdateUniverse;
                //GetAutomaticSourcingService().buildTableNameEventHandler += OnBuildTableName;
                Mask(true, "Running ...");
                String docUrl = page.getAutomaticSourcingForm().SpreadSheet.DocumentUrl;
                if (string.IsNullOrEmpty(docUrl)) docUrl = fileName;
                AutomaticSourcingData data = new AutomaticSourcingData(page.EditedObject.oid.Value, name, docUrl);
                data.runTable = AutomaticSourcingTableDialog.requestRunAllocation;
                data.createTable = true;
                data.excelExtension = Kernel.Util.ExcelUtil.GetFileExtension(docUrl).Extension;
                GetAutomaticSourcingService().Run(data);               
            }
        }

        protected void OnUpdateUniverse(object tableIssue, bool showDialog)
        {
            if (showDialog)
            {
                ModelModificationDialog dialog = new ModelModificationDialog();
                dialog.Display((Kernel.Domain.TableSaveIssue)tableIssue);
                dialog.ShowDialog();
                ((Kernel.Domain.TableSaveIssue)tableIssue).applyToAll = dialog.tableSaveIssue.applyToAll;
                ((Kernel.Domain.TableSaveIssue)tableIssue).decision = dialog.tableSaveIssue.decision;
                showDialog = false;
            }
        }
        
        private void runTableAfterCreate(Kernel.Domain.AutomaticSourcingData data)
        {
            InputTableRunProcess process = new InputTableRunProcess();
            process.Service = this.InputTableService;
            process.RunTable(data.tableOid);
            // MessageDisplayer.DisplayInfo("Automatic  Sourcing", "Allocation runned successfully on " + data.tableName + " !");
            AutomaticSourcingEditorItem currentPage = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            int activeSheet = currentPage.getAutomaticSourcingForm().SpreadSheet.getActiveSheetIndex();
            int activeCol = currentPage.getAutomaticSourcingForm().SpreadSheet.getActiveCell().Column;
            RefreshView(currentPage, activeSheet, activeCol);
        }

        public override OperationState Open(AutomaticSourcing automaticSourcing)
        {

            string[] fileAttribute = openFileDialog("Open File To Upload", null);
            automaticSourcing.refresh();
            if (fileAttribute == null)
            {
                this.ApplicationManager.HistoryHandler.closePage(this);
                return OperationState.STOP;
            }

            if (base.Open(automaticSourcing) != OperationState.CONTINUE) return OperationState.STOP;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (automaticSourcing.tableGroup == null)
            {
                page.getAutomaticSourcingForm().tableGroup = automaticSourcing.tableGroup = GetAutomaticSourcingService().GroupService.getDefaultGroup(SubjectType.INPUT_TABLE.label);
            }
            page.EditedObject = automaticSourcing;
            page.EditedObject.excelFile = fileAttribute[1];
            InitializeExcelFile(page.EditedObject.excelFile);
            OnSheetActivated();
            if (page.EditedObject.ActiveSheet == null) return OperationState.CONTINUE;
            if (page.EditedObject.ActiveSheet.listColumnToDisplay.Count > 0) page.getAutomaticSourcingForm().SetSelectedIndex(0);
            return OperationState.CONTINUE;
        }

        public override Kernel.Application.OperationState Create()
        {
            string[] fileAttribute = openFileDialog("Open File To Upload", null);
            if (fileAttribute == null)
            {
                if (this.ApplicationManager.HistoryHandler.ActivePage == this)
                {
                    if (getAutomaticSourcingEditor().ListChangeHandler.Items.Count > 1)
                    {
                        int last = getAutomaticSourcingEditor().getPages().Count - 1;
                        this.OnPageSelected(getAutomaticSourcingEditor().getPages()[last]);
                        return OperationState.STOP;
                    }
                    else
                    {
                        this.ApplicationManager.HistoryHandler.closePage(this);
                        return OperationState.STOP;
                    }
                }
            }
            AutomaticSourcing automaticSourcing = GetNewAutomaticSourcing();
            ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.AddAutomaticSourcing(automaticSourcing);
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().addOrSelectPage(automaticSourcing);
            initializePageHandlers(page);

            page.Title = automaticSourcing.name;
            page.EditedObject = automaticSourcing;
            page.EditedObject.excelFile = fileAttribute[1];
            getAutomaticSourcingEditor().ListChangeHandler.AddNew(automaticSourcing);

            InitializeExcelFile(page.EditedObject.excelFile);
            OnSheetActivated();
            if (page.EditedObject.ActiveSheet != null)
            {
                if (page.EditedObject.ActiveSheet.listColumnToDisplay.Count > 0) page.getAutomaticSourcingForm().SetSelectedIndex(0);
            }
            OnChange();
            return OperationState.CONTINUE;

        }

        protected virtual string[] openFileDialog(string title, string initialDirectory)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Title = title;
            if (!string.IsNullOrWhiteSpace(initialDirectory)) fileDialog.InitialDirectory = initialDirectory;

            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = HistoryHandler.FILE_EXTENSION_EXCEL;
            string listeExtension = "Excel files (";
            string listeExtension1 = "";
            foreach (string ext in HistoryHandler.TAB_FILE_EXTENSION_EXCEL)
            {
                listeExtension += "*" + ext + ",";
                listeExtension1 += "*" + ext + ";";
            }
            int lastComa = listeExtension.LastIndexOf(",");
            listeExtension = listeExtension.Remove(lastComa);
            listeExtension += ")|";

            int lastSemiColon = listeExtension1.LastIndexOf(";");
            listeExtension1 = listeExtension1.Remove(lastSemiColon);

            fileDialog.Filter = listeExtension + listeExtension1;
            Nullable<bool> result = fileDialog.ShowDialog(this.ApplicationManager.MainWindow);

            var fileName = fileDialog.SafeFileName;
            var filePath = fileDialog.FileName;

            string[] strings = { fileName, filePath };
            return result == true ? strings : null;
        }

        public override void OnPageSelected(EditorItem<Misp.Kernel.Domain.AutomaticSourcing> page)
        {
            if (page == null) return;
            AutomaticSourcingForm form = ((AutomaticSourcingEditorItem)page).getAutomaticSourcingForm();
           ((AutomaticSourcingPropertyBar)this.PropertyBar).AutomaticSourcingLayoutAnchorable.Content = form.AutomaticSourcingPanel;
           bool canAddTableProperty = isAutomaticGrid() || isAutomaticTarget();
           if (!canAddTableProperty)
           {
               ((AutomaticSourcingPropertyBar)this.PropertyBar).AutomaticTablePropertiesLayoutAnchorable.Content = form.AutomaticTablePropertiesPanel;
           }
         
        }

        public int activeSheetIndex { get; set; }
        public int activeColIndex { get; set; }

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>    
        public override OperationState Save(EditorItem<AutomaticSourcing> page)
        {

            if (page.IsModify)
            {
                if (!page.validateEdition()) return OperationState.STOP;
                try
                {
                    Mask(true);
                    AutomaticSourcingEditorItem currentPage = (AutomaticSourcingEditorItem)page;
                    
                    string groupName = currentPage.getAutomaticSourcingForm().AutomaticSourcingPanel.groupGroupField.textBox.Text.Trim();

                    BGroup bgroup = getGroup(groupName, SubjectTypeFound());

                    page.EditedObject.group = bgroup;

                    if (isAutomaticTarget())
                    {
                        groupName = currentPage.getAutomaticSourcingForm().getTargetGroupName();
                        if (groupName != null)
                        {
                            bgroup = getGroup(groupName, SubjectType.TARGET);
                            page.EditedObject.ActiveSheet.ActiveColumn.targetGroup = bgroup;
                            setColumnParams(bgroup);
                        }
                    }
                   
                    currentPage.EditedObject.name = currentPage.EditedObject.name.TrimEnd();

                    if (currentPage.EditedObject.ActiveSheet == null)
                    {
                        currentPage.EditedObject.ActiveSheet = currentPage.EditedObject.getAutomaticSourcingSheet(this.activeSheetIndex);
                    }

                    activeSheetIndex = currentPage.EditedObject.ActiveSheet.position;
                    if (currentPage.EditedObject.ActiveSheet.ActiveColumn == null)
                    {
                        int currentIndex = currentPage.getAutomaticSourcingForm().GetSelectedListIndex();
                        if (currentIndex == -1) return OperationState.STOP;
                        currentPage.EditedObject.ActiveSheet.ActiveColumn = currentPage.EditedObject.ActiveSheet.listColumnToDisplay[currentIndex];
                    }
                    activeColIndex = currentPage.EditedObject.ActiveSheet.ActiveColumn.columnIndex;
                    GetAutomaticSourcingService().SaveTableHandler += UpdateSaveInfo;
                    GetAutomaticSourcingService().Save(currentPage.EditedObject);
                }
                catch (Exception)
                {
                    DisplayError("Unable to save " + page.EditedObject.typeName, "Unable to save " + page.EditedObject.typeName + " named : " + page.EditedObject.name);
                    OnChange();
                    Mask(false);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        private BGroup getGroup(String groupName, SubjectType subjectType) 
        {
            groupName = !string.IsNullOrEmpty(groupName) ? groupName : subjectType.label;
            BGroup bgroup = GetAutomaticSourcingService().GroupService.getGroupByName(groupName);
            if (bgroup == null)
            {
                bgroup = new BGroup();
                bgroup.name = groupName;
                bgroup.subjectType = subjectType.label;
                BGroup rootGroup = GetAutomaticSourcingService().GroupService.getRootGroup(subjectType);
                bgroup.SetParent(rootGroup);
                bgroup.SetPosition(rootGroup.childrenListChangeHandler.Items.Count);
                rootGroup.AddChild(bgroup);
                bgroup = GetAutomaticSourcingService().GroupService.Save(bgroup);
            }
            return bgroup;
        }

        private SaveInfo lastSaveInfo { get; set; }
        protected void UpdateSaveInfo(SaveInfo info, object automaticSourcing)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (automaticSourcing != null && automaticSourcing is AutomaticSourcing)
            {
                page.EditedObject = GetAutomaticSourcingService().getByOid(((AutomaticSourcing)automaticSourcing).oid.Value);
                //page.displayObject();
                RefreshView(page, activeSheetIndex, activeColIndex);
                //OnSheetActivated();
                page.IsModify = false;
                return;
            }

            if (automaticSourcing != null && automaticSourcing is Kernel.Domain.InputTable)
            {
                Mask(false);
                MessageDisplayer.DisplayInfo("Run Automatic Sourcing ", "Table  " + ((Kernel.Domain.InputTable)automaticSourcing).name + " created sucessfully !");
                page.IsModify = false;
                return;
            }

            if (automaticSourcing != null && automaticSourcing is Kernel.Domain.AutomaticSourcingData)
            {
                Mask(false);
                if (((Kernel.Domain.AutomaticSourcingData)automaticSourcing).createTable)
                {
                    MessageDisplayer.DisplayInfo("Run Automatic Sourcing ", "Table  " + ((Kernel.Domain.AutomaticSourcingData)automaticSourcing).tableName + " created sucessfully !");
                    if (((Kernel.Domain.AutomaticSourcingData)automaticSourcing).runTable)
                    {
                        runTableAfterCreate(((Kernel.Domain.AutomaticSourcingData)automaticSourcing));
                    }
                }
                else MessageDisplayer.DisplayInfo("Run Automatic Sourcing ", "Table creation aborted !");
                return;
            }

            if (info == null || info.isEnd == true)
            {
                GetAutomaticSourcingService().SaveTableHandler -= UpdateSaveInfo;
                Mask(false);
                Service.FileService.SaveCurrentFile();
            }
            else
            {
                int rate = info.stepCount != 0 ? (Int32)(info.stepRuned * 100 / info.stepCount) : 0;
                if (rate > 100) rate = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = info.stepCount;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = info.stepRuned;
                //ApplicationManager.MainWindow.LoadingLabel.Content = "" + rate + " %";
                ApplicationManager.MainWindow.LoadingLabel.Content = "Running : " + rate + " %" + " (" + info.stepRuned + "/" + info.stepCount + ")";
            }
        }

        protected void Mask(bool mask, string content = "Saving...")
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            page.getAutomaticSourcingForm().Mask(mask);
            ApplicationManager.MainWindow.BusyBorder.Visibility = mask ? Visibility.Visible : Visibility.Hidden;
            if (mask)
            {
                ApplicationManager.MainWindow.LoadingProgressBar.Maximum = 100;
                ApplicationManager.MainWindow.LoadingProgressBar.Value = 0;
                ApplicationManager.MainWindow.LoadingLabel.Content = content;

                ApplicationManager.MainWindow.LoadingProgressBar.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingLabel.Visibility = Visibility.Visible;
                ApplicationManager.MainWindow.LoadingImage.Visibility = Visibility.Hidden;
            }

        }

        private void RefreshView(AutomaticSourcingEditorItem currentPage, int activeSheetIndex, int activeColunIndex)
        {
            currentPage.EditedObject.refresh();
            currentPage.EditedObject.ActiveSheet = currentPage.EditedObject.getAutomaticSourcingSheet(activeSheetIndex);
            currentPage.EditedObject.ActiveSheet.ActiveColumn = currentPage.EditedObject.ActiveSheet.getAutomaticSourcingColumn(activeColIndex);

            String selecteRange = currentPage.EditedObject.ActiveSheet.selectedRange;
            Range rangeSelected;
            if (selecteRange == "") rangeSelected = null;
            else
            {
                rangeSelected = currentPage.EditedObject.ActiveSheet.buildRange(selecteRange);
                rangeSelected.Sheet = currentPage.getAutomaticSourcingForm().SpreadSheet.getActiveSheet();
                currentPage.EditedObject.ActiveSheet.rangeSelected = rangeSelected;
                currentPage.EditedObject.ActiveSheet.SetSelectedRange = currentPage.EditedObject.ActiveSheet.SetSelectedRange;
                ((AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage()).EditedObject = currentPage.EditedObject;
                OnSelectedRangeChange(rangeSelected);
            }
            currentPage.EditedObject.ActiveSheet.rangeSelected = rangeSelected;
            if (currentPage.getAutomaticSourcingForm().SpreadSheet == null) return;
            if (currentPage.getAutomaticSourcingForm().SpreadSheet.getActiveSheet() == null) return;

            currentPage.EditedObject.ActiveSheet.Name = currentPage.getAutomaticSourcingForm().SpreadSheet.getActiveSheet().Name;
            refreshMode = true;
            //this.OnSetFirtRowAsHeader(currentPage.EditedObject.ActiveSheet.firstRowColumn);
            //this.OnSelectRange(currentPage.EditedObject.ActiveSheet.SetSelectedRange);
            refreshMode = false;
        }

        /// <summary>
        /// Close all opened Excel files
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();

            foreach (AutomaticSourcingEditorItem page in getAutomaticSourcingEditor().getPages())
            {
                if (page.getAutomaticSourcingForm().SpreadSheet != null)
                {
                    page.getAutomaticSourcingForm().SpreadSheet.Close();
                }
            }
            if (getAutomaticSourcingEditor().NewPage != null && ((AutomaticSourcingEditorItem)getAutomaticSourcingEditor().NewPage).getAutomaticSourcingForm().SpreadSheet != null)
                ((AutomaticSourcingEditorItem)getAutomaticSourcingEditor().NewPage).getAutomaticSourcingForm().SpreadSheet.Close();
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {
            this.Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosed(object sender, EventArgs args)
        {
            AutomaticSourcingEditorItem removedPage = (AutomaticSourcingEditorItem)sender;
            Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
            string excelDir = fileDirs != null ? fileDirs.InputTableDir : "";
            string filePath = excelDir + removedPage.EditedObject.excelFile;
            if (!removedPage.EditedObject.oid.HasValue && !System.IO.File.Exists(filePath))
            {
                ((AutomaticSourcingSideBar)SideBar).AutomaticSourcingGroup.AutomaticSourcingTreeview.RemoveAutomaticSourcing(removedPage.EditedObject);
            }
            removedPage.getAutomaticSourcingForm().SpreadSheet.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosing(object sender, System.ComponentModel.CancelEventArgs args)
        {
            base.OnPageClosing(sender, args);
            if (args.Cancel) return;
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)sender;
            if (page.getAutomaticSourcingForm().SpreadSheet != null && OperationState.STOP == page.getAutomaticSourcingForm().SpreadSheet.Close())
            {
                try { args.Cancel = true; }
                catch (Exception)
                { DisplayError("Unable to save Input Table", "Unable to save Excel file."); }
            }
        }

        #endregion

        #region Excel Methods

        private void SpreadSheet_OnBeforeRightClick()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            UpdateExcelContextMenu(page, page.EditedObject.ActiveSheet.rangeSelected);
        }

        private void InitializeExcelFile(string fileToOpen)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            CustomizeSpreedSheet(page.getAutomaticSourcingForm().SpreadSheet, fileToOpen);

            if (!page.EditedObject.oid.HasValue)
            {
                //List<string> listeSheets = GetAutomaticSourcingService().getListColumns(fileToOpen, firstRow);
                List<Sheet> listeSheets = page.getAutomaticSourcingForm().SpreadSheet.getAllExcelSheets();
                foreach (Sheet sheet in listeSheets)
                {
                    page.EditedObject.AddSheet(new AutomaticSourcingSheet(sheet.Name, sheet.Index, false, "", null));
                }
            }

        }

        private void UpdateExcelContextMenu(AutomaticSourcingEditorItem page, Range UsableRange)
        {
            string rangeName = UsableRange != null ? UsableRange.Name : "";
            Range selectedRange = UsableRange;

            //SheetPanel sheetPanel = page.getAutomaticSourcingForm().SpreadSheet;
            //int index = sheetPanel.getActiveCell().Column;
            //int sheetIndex = sheetPanel.getActiveSheetIndex();

            //bool isPresent;

            //if (page.EditedObject.ActiveSheet.oid.HasValue)
            //    isPresent = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(index) != null ? true :
            //       page.EditedObject.ActiveSheet.oid.HasValue && page.EditedObject.ActiveSheet.findInDeleted(index) != null ? true : false;
            //else
            //    isPresent = sheetPanel.IsColumnPresentInList(index);

            //if (isPresent)
            //{
            //    bool isInListBox = page.EditedObject.ActiveSheet.getColumnInListToDisplay(index) != null ? true : false;
            //    sheetPanel.SetInVisibleExcelMenu(EdrawOffice.ADD_AUTOMATICCOLUMN_LABEL, false);
            //    sheetPanel.SetInVisibleExcelMenu(EdrawOffice.REMOVE_AUTOMATICCOLUMN_LABEL, false);

            //    sheetPanel.ActivateExcelContextMenuItem(!isInListBox, EdrawOffice.ADD_AUTOMATICCOLUMN_LABEL);
            //    sheetPanel.ActivateExcelContextMenuItem(isInListBox, EdrawOffice.REMOVE_AUTOMATICCOLUMN_LABEL);
            //}
            //else
            //{
            //    sheetPanel.SetInVisibleExcelMenu(EdrawOffice.ADD_AUTOMATICCOLUMN_LABEL);
            //    sheetPanel.SetInVisibleExcelMenu(EdrawOffice.REMOVE_AUTOMATICCOLUMN_LABEL);
            //}
        }

        private void SpreadSheet_OnAddColumn(int index)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            Range selectedRange = null;
            Action AddColumnAction = new Action((Action)(() =>
            {
                AutomaticSourcingSheet currentSheet = page.getAutomaticSourcingForm().AutomaticSourcingPanel.GetSelectedSheet();
                if (currentSheet == null) return;
                selectedRange = currentSheet.rangeSelected;
                bool isPresent = page.getAutomaticSourcingForm().SpreadSheet.IsColumnPresentInList(index, selectedRange);
                if (isPresent)
                {
                    AutomaticSourcingColumn columnToAdd = currentSheet.getAutomaticSourcingColumn(index);
                    if (columnToAdd == null) columnToAdd = new AutomaticSourcingColumn(index, getColumName(index));

                    if (columnToAdd.oid.HasValue) columnToAdd.setToUpdate();
                    else columnToAdd.setToNew();
                    int sheetIndex = page.EditedObject.getAutomaticSourcingSheetIndex(page.EditedObject.ActiveSheet.position);
                    page.EditedObject.ActiveSheet.listColumnToDisplay.Add(columnToAdd);
                    page.EditedObject.ActiveSheet.listColumnToDisplay.BubbleSortColumnAndColumnTargetItem();
                    page.EditedObject.automaticSourcingSheetListChangeHandler.Items[sheetIndex].updateColumnParam(columnToAdd, null);

                    page.getAutomaticSourcingForm().displayObject();
                    int indexAddedColumn = currentSheet.getAutomaticSourcingColumnIndex(index);
                    if (indexAddedColumn < 0) return;
                    page.getAutomaticSourcingForm().AutomaticSourcingPanel.SetSelectedIndex(indexAddedColumn);
                }
            }));
            System.Windows.Application.Current.Dispatcher.Invoke(AddColumnAction);
        }

        protected virtual void InitializeColumn()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            page.EditedObject.ActiveSheet.listColumnToDisplay = new List<AutomaticSourcingColumn>(0);
            if (!page.EditedObject.ActiveSheet.oid.HasValue)
            {
               List<int> listeIndexColumn = page.getAutomaticSourcingForm().SpreadSheet.getColumnsIndexes(page.EditedObject.ActiveSheet.rangeSelected);

               foreach (int column in listeIndexColumn)
                {
                   AutomaticSourcingColumn addedColumn = new AutomaticSourcingColumn(column, getColumName(column));
                    page.EditedObject.ActiveSheet.AddColumn(addedColumn);
                   page.EditedObject.ActiveSheet.listColumnToDisplay.Add(addedColumn);
                }
            }
            else
            {
                foreach (AutomaticSourcingColumn addedColumn in page.EditedObject.ActiveSheet.automaticSourcingColumnListChangeHandler.getItems())
                {
                    addedColumn.Name = getColumName(addedColumn.columnIndex);
                    if (addedColumn.oid.HasValue) page.EditedObject.ActiveSheet.listColumnToDisplay.Add(addedColumn);
                }
            }
            page.EditedObject.ActiveSheet.ActiveColumn = page.EditedObject.ActiveSheet.getFirstInList();
            if (page.EditedObject.ActiveSheet.ActiveColumn == null) return;
        }

        private string getColumName(int colPosition)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            string colName;
            string activeSheetName = page.getAutomaticSourcingForm().GetActiveSheetName();
            if (page.EditedObject.ActiveSheet.firstRowColumn)
            {
                try
                {
                    colName = page.getAutomaticSourcingForm().SpreadSheet.getValueAt(1, colPosition, activeSheetName).ToString();
                }
                catch (Exception)
                {
                    colName = Kernel.Util.RangeUtil.GetColumnName(colPosition);
                }
            }
            else
            {
                colName = Kernel.Util.RangeUtil.GetColumnName(colPosition);
            }
            return colName;
        }

        private void InitializeColumnOnRangeChange()
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            Range rangeSelected = page.EditedObject.ActiveSheet.rangeSelected != null
                            && page.getAutomaticSourcingForm().GetSelectionRangeState() ? page.EditedObject.ActiveSheet.rangeSelected : null;

            if (rangeSelected == null) return;
            page.EditedObject.ActiveSheet.listColumnToDisplay = new List<AutomaticSourcingColumn>(0);
            List<int> listeIndexColumn = page.getAutomaticSourcingForm().SpreadSheet.getColumnsIndexes(rangeSelected);
            foreach (int colPosition in listeIndexColumn)
            {
                AutomaticSourcingColumn colSourcing = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(colPosition);
                if (colSourcing == null)
                {
                    colSourcing = new AutomaticSourcingColumn(colPosition, "");
                    page.EditedObject.ActiveSheet.AddColumn(colSourcing);
                }
                colSourcing.Name = getColumName(colPosition);
                if (colSourcing.oid.HasValue)
                {
                    colSourcing.setToUpdate();
                    page.EditedObject.ActiveSheet.updateColumnParam(colSourcing, null);
                }
                page.EditedObject.ActiveSheet.listColumnToDisplay.Add(colSourcing);
            }
        }

        private void OnSpreadSheetSelectionChanged(Kernel.Ui.Office.ExcelEventArg arg)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();

            page.EditedObject.ActiveSheet = page.EditedObject.getAutomaticSourcingSheet(arg.Sheet.Index);
            int selectedCol = page.getAutomaticSourcingForm().SpreadSheet.getActiveCell().Column;
            if (page.EditedObject.ActiveSheet == null) return;
            page.EditedObject.ActiveSheet.ActiveColumn = page.EditedObject.ActiveSheet.getAutomaticSourcingColumn(selectedCol);

            bool selectedRange = page.getAutomaticSourcingForm().GetSelectionRangeState();
            bool hasFocus = page.getAutomaticSourcingForm().AutomaticSourcingPanel.SheetPanel.RangeTextBox.IsFocused;

            Range UsableRange = arg.Range;
            if (selectedRange && UsableRange != null && hasFocus)
            {
                page.EditedObject.ActiveSheet.rangeSelected = UsableRange;
                page.EditedObject.ActiveSheet.selectedRange = UsableRange.Name;

                page.getAutomaticSourcingForm().AutomaticSourcingPanel.SheetPanel.RangeTextBox.SelectAll();
                InitializeColumnOnRangeChange();

                page.getAutomaticSourcingForm().displayObject();
            }

            int selectedIndex = page.getAutomaticSourcingForm().getColumnInListBox(selectedCol);

            if (selectedCol >= 0) page.getAutomaticSourcingForm().SetSelectedIndex(selectedIndex);
            else if (page.EditedObject.ActiveSheet != null && page.EditedObject.ActiveSheet.getColumnsCount() > 0)
                page.getAutomaticSourcingForm().SetSelectedIndex(0);
        }

        protected virtual string buildExcelFilePath(string name)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            if (page == null) return null;
            Kernel.Service.FileDirs fileDirs = this.Service.FileService.GetFileDirs();
            string excelDir = fileDirs != null ? fileDirs.InputTableDir : "";
            string filePath = excelDir + name + System.IO.Path.GetExtension(page.EditedObject.excelFile);
            return filePath;
        }

        /// <summary>
        /// Customize Spreedsheet
        /// </summary>
        /// <param name="page"></param>
        private void CustomizeSpreedSheet(DESheetPanel sheetPanel, string fileToOpen)
        {
            if(!string.IsNullOrWhiteSpace(fileToOpen))sheetPanel.Open(fileToOpen);
            //sheetPanel.BuildSheetPanelMethod(1);
            sheetPanel.AddExcelMenu(SheetConst.REMOVE_AUTOMATICCOLUMN_LABEL);
            sheetPanel.AddExcelMenu(SheetConst.ADD_AUTOMATICCOLUMN_LABEL);
        }

        #endregion

        #region InputTable methods

        private void OnAutomaticSourcingDateChanged(string dateFrom, string dateTo)
        {
            AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();
            page.EditedObject.periodFrom = dateFrom;
            page.EditedObject.periodTo = dateTo;
            OnChange();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetNewInputTableName(string prefix)
        {
            List<Misp.Kernel.Domain.Browser.InputTableBrowserData> listeInputTables = GetAutomaticSourcingService().InputTableService.getBrowserDatas();

            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;

                bool IsInputTableNameUsed = InputTableNameExist(name);
                if (!IsInputTableNameUsed) return name;

                i++;
            }
            return name;
        }

        public bool InputTableNameExist(string inputTableName)
        {
            List<Misp.Kernel.Domain.Browser.InputTableBrowserData> listeInputTables = GetAutomaticSourcingService().InputTableService.getBrowserDatas();

            foreach (Kernel.Domain.Browser.InputTableBrowserData inputTableBd in listeInputTables)
            {
                if (inputTableBd.name.ToUpper() == inputTableName.ToUpper()) return true;
            }
            return false;
        }

        #endregion
    }
}
