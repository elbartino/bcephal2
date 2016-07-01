using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Group;
using Misp.Kernel.Ui.File;
using Misp.Kernel.Application;
using System.Windows.Controls;
using System.Threading;
using Misp.Kernel.Ui.Office.EDraw;
using Misp.Kernel.Ui.Office;
using System.Windows.Navigation;
using Misp.Kernel.Util;
using Misp.Kernel.Service;
using System.Windows.Data;
using System.Collections;
using Misp.Kernel.Domain.Browser;
using Misp.Sourcing.Table;

namespace Misp.Sourcing.Designer
{
    public class DesignerEditorController : EditorController<Design, Misp.Kernel.Domain.Browser.BrowserData>
    {

        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
        #endregion

        #region Constructor



        public DesignerEditorController()
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        #endregion


        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public DesignerEditor getDesignerEditor()
        {
            return (DesignerEditor)this.View;
        }

        /// <summary>
        /// Service pour acceder aux opérations liés aux Designs.
        /// </summary>
        /// <returns>DesignService</returns>
        public DesignService GetDesignService()
        {
            return (DesignService)base.Service;
        }

        #endregion


        #region Operations
        
        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Design design = GetNewDesign();
            ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.AddDesign(design);
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().addOrSelectPage(design);
            initializePageHandlers(page);
            page.Title = design.name;
            getDesignerEditor().ListChangeHandler.AddNew(design);
            page.getDesignerForm().SpreadSheet.protectSheet();
            return OperationState.CONTINUE;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.DESIGN;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(Design design)
        {
            EditorItem<Design> page = getEditor().addOrSelectPage(design);
            UpdateStatusBar();
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(design);
            return OperationState.CONTINUE;
        }

        protected virtual Design GetNewDesign()
        {
            Design design = new Design();
            design.name = getNewPageName("Design");
            design.group = GetDesignService().GroupService.getDefaultGroup();
            return design;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                Design design = GetObjectByName(name);
                if (design == null) return name; 
                i++;
            }
            return name;
        }

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        DesignerEditorItem currentPage = new DesignerEditorItem();
        public DesignWindow DesignWindow { get; set; }
        public override OperationState Save(EditorItem<Design> page)
        {
            try
            {
                currentPage = (DesignerEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
            }
            catch (Exception ex)
            {
                DisplayError("Unable to save Design", "Unable to save Excel file.");
                return OperationState.STOP;
            }
            Design editedObject = page.EditedObject;
            if (DesignWindow != null)
                DesignWindow.curentDesign = editedObject;
            int CountOpeningDesigner = getEditor().ListChangeHandler.Items.Count;
            
            return OperationState.CONTINUE;
        }

        public OperationState Create(string name, Design DesignInEdition)
        {
            Design design = DesignInEdition.getCopy(name);
            if (design == null) return OperationState.STOP;
           
            EditorItem<Design> page = getEditor().addOrSelectPage(design);
            
            ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.AddDesign(design);
            return Open(design);
        }

        //public bool isSaveAs = false;

        //public override OperationState SaveAs(string name)
        //{
        //    isSaveAs = true;

        //    DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
        //    Design design = page.EditedObject;
        //    if (design.oid.HasValue)
        //    {
        //        return Create(name, page.EditedObject);
        //    }
        //    else
        //    {
        //        isSaveAs = false;
        //        Rename(name);
        //        if (Save(page) != OperationState.CONTINUE)
        //            return OperationState.STOP;
        //        ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.updateDesign(name, design.name, false);
        //    }
        //    return OperationState.CONTINUE;
        //}


        private Design GetDesign(string name)
        {
            if (!IsNameUsed(name))
            {
                Design design = new Design();
                design.name = name;
                design.group = GetDesignService().GroupService.getDefaultGroup();
                return design;
            }
            return null;
        }


        private bool IsNameUsed(string name)
        {
            Design obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another Table named: " + name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Close all opened Excel files
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();

            foreach (DesignerEditorItem page in getDesignerEditor().getPages())
            {
                if (page.getDesignerForm().SpreadSheet != null)
                {
                    page.getDesignerForm().SpreadSheet.Close();
                }   
            }
            if (getDesignerEditor().NewPage != null && ((DesignerEditorItem)getDesignerEditor().NewPage).getDesignerForm().SpreadSheet != null)
                ((DesignerEditorItem)getDesignerEditor().NewPage).getDesignerForm().SpreadSheet.Close();
            ApplicationManager.MainWindow.StatusLabel.Content = "";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnPageClosing(object sender, CancelEventArgs args)
        {
            base.OnPageClosing(sender, args);
            if (!args.Cancel)
            {
                DesignerEditorItem page = (DesignerEditorItem)sender;
                if (page.getDesignerForm().SpreadSheet != null && OperationState.STOP == page.getDesignerForm().SpreadSheet.Close())
                {
                    try
                    {
                    
                        args.Cancel = true;
                    }
                    catch (Exception)
                    {
                        DisplayError("Unable to save Input Table", "Unable to save Excel file.");
                     
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<Design> page)
        {
            if (page == null) return;
            DesignerForm form = ((DesignerEditorItem)page).getDesignerForm();
            ((DesignerPropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = form.DesignerPropertiesPanel;
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
            UpdateStatusBar();
            return OperationState.CONTINUE;
        }
                      

        public override OperationState Rename()
        {
            if (base.Rename() != OperationState.CONTINUE)
                return OperationState.STOP;

            IsRenameOnDoubleClick = true;
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();

            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.Text = name;
            page.EditedObject.name = name;
            base.Rename(name);
        }

        public override OperationState Delete() { return OperationState.CONTINUE; }

        #endregion


        #region Others

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new DesignerEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar(){ return new DesignerToolBar();  }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new DesignerSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new DesignerPropertyBar(); }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData()
        {
            DimensionField.Periodicity = GetDesignService().PeriodicityService.getPeriodicity(); 
        }

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<Design> page)
        {
            base.initializePageHandlers(page);
            DesignerEditorItem editorPage = (DesignerEditorItem)page;
            editorPage.getDesignerForm().DesignerPropertiesPanel.groupField.GroupService = GetDesignService().GroupService;
            editorPage.getDesignerForm().DesignerPropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getDesignerForm().DesignerPropertiesPanel.NameTextBox.KeyUp += onNameTextChange;
            editorPage.getDesignerForm().DesignerPropertiesPanel.NameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getDesignerForm().DesignerPropertiesPanel.groupField.Changed += onGroupFieldChange;

            editorPage.getDesignerForm().DesignerPropertiesPanel.Changed += OnDesignerPropertiesChange;
            //editorPage.getDesignerForm().DesignerPropertiesPanel.ColumnsField.ItemDeleted += OnFilterChange;
            //editorPage.getDesignerForm().DesignerPropertiesPanel.RowsField.ItemDeleted += OnFilterDelete;;
            //editorPage.getDesignerForm().DesignerPropertiesPanel.CentralField.ItemDeleted += OnFilterDelete;

        }


        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
            List<BrowserData> designs = Service.getBrowserDatas();
            ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.fillTree(new ObservableCollection<BrowserData>(designs));
            
            List<Model> models = GetDesignService().ModelService.getModelsForSideBar();
            ((DesignerSideBar)SideBar).EntityGroup.EntityTreeview.setDisplacherInterval(new TimeSpan(0, 0,0,1));
            ((DesignerSideBar)SideBar).EntityGroup.EntityTreeview.DisplayModels(models);

            Measure rootMeasure = GetDesignService().MeasureService.getRootMeasure();
            ((DesignerSideBar)SideBar).MeasureGroup.MeasureTreeview.setDisplacherInterval(new TimeSpan(0, 0,1));
            ((DesignerSideBar)SideBar).MeasureGroup.MeasureTreeview.DisplayRoot(rootMeasure);

            
            List<Kernel.Domain.CalculatedMeasure> CalculatedMeasures = GetDesignService().CalculatedMeasureService.getAllCalculatedMeasure();
            if (CalculatedMeasures != null)
                ((DesignerSideBar)SideBar).CalculateMeasureGroup.CalculatedMeasureTreeview.fillTree(new ObservableCollection<CalculatedMeasure>(CalculatedMeasures));
      
            //Periodicity periodicity = GetDesignService().PeriodicityService.getPeriodicity();
            //((DesignerSideBar)SideBar).PeriodicityGroup.PeriodicityTreeview.DisplayPeriodicity(periodicity);

            /*List<PeriodName> periodNames = GetDesignService().PeriodNameService.getAll();
            ((DesignerSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(periodNames);*/

            PeriodName rootPeriodName = GetDesignService().PeriodNameService.getRootPeriodName();
            ((DesignerSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.DisplayPeriods(rootPeriodName);
            
            Target targetAll = GetDesignService().ModelService.getTargetAll();
            List<Target> targets = new List<Target>(0);
            targets.Add(targetAll);
            ((DesignerSideBar)SideBar).TargetGroup.TargetTreeview.DisplayTargets(targets);

            List<Target> CustomizedTargets = GetDesignService().TargetService.getAll();
            ((DesignerSideBar)SideBar).CustomizedTargetGroup.TargetTreeview.fillTree(new ObservableCollection<Target>(CustomizedTargets));


            BGroup group = GetDesignService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.SelectionChanged += onSelectDesignFromSidebar;

            ((DesignerSideBar)SideBar).MeasureGroup.MeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
            ((DesignerSideBar)SideBar).MeasureGroup.MeasureTreeview.SelectionDoubleClick +=onDoubleClickSelectMeasureFromSidebar;

            ((DesignerSideBar)SideBar).CalculateMeasureGroup.CalculatedMeasureTreeview.SelectionChanged += onSelectMeasureFromSidebar;
            ((DesignerSideBar)SideBar).EntityGroup.EntityTreeview.SelectionChanged += onSelectTargetFromSidebar;
            ((DesignerSideBar)SideBar).EntityGroup.EntityTreeview.SelectionDoubleClick += onDoubleClickSelectTargetFromSidebar;
            ((DesignerSideBar)SideBar).EntityGroup.EntityTreeview.ExpandAttribute += onExpandAttribute;
           
            ((DesignerSideBar)SideBar).TargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;

            ((DesignerSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionChanged += onSelectPeriodFromSidebar;
            ((DesignerSideBar)SideBar).PeriodNameGroup.PeriodNameTreeview.SelectionDoubleClick += onDoubleClickPeriodFromSidebar;

            ((DesignerSideBar)SideBar).CustomizedTargetGroup.TargetTreeview.SelectionChanged += onSelectTargetFromSidebar;
        }

        private void onExpandAttribute(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                if (!attribute.LoadValues)
                {
                    List<Kernel.Domain.AttributeValue> values = GetDesignService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    attribute.valueListChangeHandler.Items.Clear();
                    foreach (Kernel.Domain.AttributeValue value in values)
                    {
                        attribute.valueListChangeHandler.Items.Add(value);
                    }
                    attribute.LoadValues = true;
                }
            }
        }
        
        private void onDoubleClickPeriodFromSidebar(object sender)
        {
            if (sender != null && sender is PeriodInterval)
            {
                PeriodInterval period = (PeriodInterval)sender;
                if (period.childrenListChangeHandler.Items.Count <= 0)
                {
                    onSelectPeriodFromSidebar(period);
                    return;
                }
                DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
                if (page == null) return;
                page.getDesignerForm().SpreadSheet.protectSheet(false);
                page.getDesignerForm().DesignerPropertiesPanel.SetValue(period.childrenListChangeHandler.Items);
                page.getDesignerForm().SpreadSheet.protectSheet();
            }
            if (sender != null && sender is PeriodName)
            {
                return;
            }
           
        }

        private void onDoubleClickSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && sender is  Measure)
            {
                Measure measure = (Measure)sender;
                if (measure.childrenListChangeHandler.Items.Count <= 0) {
                    onSelectMeasureFromSidebar(measure);
                    return;
                }
                DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
                if (page == null) return;
                page.getDesignerForm().DesignerPropertiesPanel.SetValue(measure.childrenListChangeHandler.Items);
            }
        }
             
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la table selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La table sélectionnée</param>
        protected void onSelectDesignFromSidebar(object sender)
        {
            if (sender != null && sender is Design)
            {
                Design design = (Design)sender;
                EditorItem<Design> page = getDesignerEditor().getPage(design.name);
                if (page != null)
                {
                    ((DesignerEditorItem)page).getDesignerForm().SpreadSheet.DisableSheet(false);
                    page.fillObject();
                    getDesignerEditor().selectePage(page);
                    ((DesignerEditorItem)page).getDesignerForm().SpreadSheet.DisableSheet();
                    
                }
                else if (design.oid != null && design.oid.HasValue)
                {
                    this.Open(design.oid.Value);

                }
                else
                {
                    ((DesignerEditorItem)page).getDesignerForm().SpreadSheet.DisableSheet(false);
                    page = getDesignerEditor().addOrSelectPage(design);
                    initializePageHandlers(page);
                    page.Title = design.name;

                    getDesignerEditor().ListChangeHandler.AddNew(design);
                    ((DesignerEditorItem)page).getDesignerForm().SpreadSheet.DisableSheet();
                }
                DesignerEditorItem pageOpen = (DesignerEditorItem)getDesignerEditor().getActivePage();

                UpdateStatusBar();
            }
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une mesure sur la sidebar.
        /// Cette opération a pour but d'assigner la mesure sélectionnée 
        /// aux cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La mesure sélectionnée</param>
        protected void onSelectMeasureFromSidebar(object sender)
        {
            if (sender != null && sender is Measure)
            {
                Measure measure = (Measure)sender;
                DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
                if (page == null) return;
                //page.getDesignerForm().SpreadSheet.DisableSheet(false);
                page.getDesignerForm().SpreadSheet.protectSheet(false);
                page.getDesignerForm().DesignerPropertiesPanel.SetValue(measure);
                page.getDesignerForm().SpreadSheet.protectSheet();
                //page.getDesignerForm().SpreadSheet.DisableSheet(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected void onSelectPeriodFromSidebar(object sender)
        {
            
            if (sender != null && sender is PeriodInterval)
            {
                PeriodInterval period = (PeriodInterval)sender;
                DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
                if (page == null) return;
                page.getDesignerForm().SpreadSheet.protectSheet(false);
                Object value = null;
                if (period.isStandardPeriod) value = period.childrenListChangeHandler.Items;
                else value = period;
                page.getDesignerForm().DesignerPropertiesPanel.SetValue(value);
                page.getDesignerForm().SpreadSheet.protectSheet();
            }
            if (sender != null && sender is PeriodName)
            {
                return;
            }
        }
        
        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une target sur la sidebar.
        /// Cette opération a pour but de rajouté la target sélectionnée au filtre de la table en édition,
        /// ou au scope des cellProperties correspondants à la sélection Excel.
        /// </summary>
        /// <param name="sender">La target sélectionné</param>
        protected void onSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                object value = null;
                if (sender is Entity) value = sender;
                if (sender is AttributeValue) value = sender;
                if (sender is Kernel.Domain.Attribute)
                {
                    Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                    if (attribute.valueListChangeHandler.Items.Count <= 0) return;
                    if (!attribute.LoadValues)
                    {
                        List<Kernel.Domain.AttributeValue> values = GetDesignService().ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                        attribute.valueListChangeHandler.Items.Clear();
                        foreach (Kernel.Domain.AttributeValue attrValue in values)
                        {
                            attribute.valueListChangeHandler.Items.Add(attrValue);
                        }
                        attribute.LoadValues = true;
                    }
                    value = attribute.valueListChangeHandler.Items;
                }
                else value = sender;
                DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
                if (page == null) return;
                page.getDesignerForm().DesignerPropertiesPanel.SetValue(value);
            }
        }

        protected void onDoubleClickSelectTargetFromSidebar(object sender)
        {
            if (sender != null && sender is Target)
            {
                if (!(sender is AttributeValue))
                {
                    onSelectTargetFromSidebar(sender);
                    return;
                }
                AttributeValue value = (AttributeValue)sender;
                if (value.childrenListChangeHandler.Items.Count <= 0) return;
                DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
                if (page == null) return;
                page.getDesignerForm().DesignerPropertiesPanel.SetValue(value.childrenListChangeHandler.Items);
             }
        }
                
        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la table active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.Text = page.Title;
            }
            else if (args.Key == Key.Enter)
            {
                ValidateEditedNewName();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextLostFocus(object sender, RoutedEventArgs args)
        {
            ValidateEditedNewName();
        }

        protected void onGroupFieldChange()
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            string name = page.getDesignerForm().DesignerPropertiesPanel.groupField.textBox.Text;
            ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.updateDesign(name, page.Title, true);
            OnChange();
        }
        
        private void OnDesignerPropertiesChange()
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            if (page == null) return;
            BuildSheetTable();
            OnChange();
        }

        protected void BuildSheetTable()
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            if (page == null) return;
            //page.getDesignerForm().SpreadSheet.DisableSheet(false);
            page.getDesignerForm().SpreadSheet.protectSheet(false);
            page.getDesignerForm().BuildSheetTable();
            page.getDesignerForm().SpreadSheet.protectSheet();
            //page.getDesignerForm().SpreadSheet.DisableSheet();
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateStatusBar()
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            Kernel.Ui.Office.Range range = page.getDesignerForm().SpreadSheet.GetSelectedRange();
            
        }


        #endregion

        public override bool validateName(EditorItem<Design> page, string name)
        {
            if(!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        private bool IsRenameOnDoubleClick = false;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
            DesignerEditorItem page = (DesignerEditorItem)getDesignerEditor().getActivePage();
            Design table = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
            newName = page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.Text.Trim();
            string excelName = page.getDesignerForm().SpreadSheet.Office.GetDocumentFullName();
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The Design name can't be mepty!");
                page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.SelectAll();
                page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.Focus();
                return OperationState.STOP;
            }

            bool found = false;
            if (GetDesignService().getByName(newName) != null) found = true;

            foreach (DesignerEditorItem unInputTable in getDesignerEditor().getPages())
            {
                if ((found && newName != getDesignerEditor().getActivePage().Title) || (unInputTable != getDesignerEditor().getActivePage() && newName == unInputTable.Title))
                {
                    DisplayError("Duplicate Name", "There is another Design named: " + newName);
                    page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.Text = page.Title;
                    page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.SelectAll();
                    page.getDesignerForm().DesignerPropertiesPanel.NameTextBox.Focus();
                    return OperationState.STOP;
                }
                    }
            if(!IsRenameOnDoubleClick)
            if (table.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.updateDesign(newName, table.name, false);
            table.name = newName;
            page.getDesignerForm().SpreadSheet.ChangeTitleBarCaption(newName);
            page.Title = newName;
            OnChange();
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override Design GetObjectByName(string name)
        {
            return ((DesignerSideBar)SideBar).DesignerGroup.DesignerTreeview.getDesignByName(name);
        }


        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

    }
}
