using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Service;
using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Misp.Reporting.CalculatedMeasures;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Misp.Reporting.Calculated_Measure
{
    public class CalculatedMeasureEditorController : EditorController<Misp.Kernel.Domain.CalculatedMeasure, Misp.Kernel.Domain.Browser.BrowserData>
    {

        #region Properties
        public override void DeleteCommandEnabled(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = false; }
        #endregion

        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de CalculatedMeasureEditorController.
        /// </summary>
        public CalculatedMeasureEditorController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
        }

        #endregion

        #region Editor and Service

        /// <summary>
        /// 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public CalculatedMeasureEditor getCalculatedMeasureEditor()
        {
            return (CalculatedMeasureEditor)this.View;
        }

        /// <summary>
        /// retourne le service associé au calculated measure.
        /// </summary>
        /// <returns>CalculatedMeasureService</returns>
        public CalculatedMeasureService GetCalculatedMeasureService()
        {
            return (CalculatedMeasureService)base.Service;
        }

        #endregion

        #region Operations

        /// <summary>
        /// Cette methode permet de créer une nouvelle page new calculated Measure.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public override OperationState Create()
        {
            Misp.Kernel.Domain.CalculatedMeasure CalculatedMeasure = GetNewCalculatedMeasure();

           ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.AddCalculatedMeasure(CalculatedMeasure);
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().addOrSelectPage(CalculatedMeasure);
            string expression = page.EditedObject.computeExpression();
            this.ApplicationManager.MainWindow.StatusLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression; initializePageHandlers(page);
            page.Title = CalculatedMeasure.name;

            getCalculatedMeasureEditor().ListChangeHandler.AddNew(CalculatedMeasure);
            return OperationState.CONTINUE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public override OperationState Open(CalculatedMeasure CalculatedMeasure)
        {
            EditorItem<CalculatedMeasure> page = getEditor().addOrSelectPage(CalculatedMeasure);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(CalculatedMeasure);
            string expression = page.EditedObject.computeExpression();
            this.ApplicationManager.MainWindow.StatusLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression; initializePageHandlers(page);
            return OperationState.CONTINUE;
        }

        protected virtual Misp.Kernel.Domain.CalculatedMeasure GetNewCalculatedMeasure()
        {
            Misp.Kernel.Domain.CalculatedMeasure CalculatedMeasure = new CalculatedMeasure();
            CalculatedMeasure.name = getNewPageName("CalculatedMeasure");
            CalculatedMeasure.group = GetCalculatedMeasureService().GroupService.getDefaultGroup();
            return CalculatedMeasure;
        }

        protected override string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                CalculatedMeasure CalculatedMeasure = GetObjectByName(name);
                if (CalculatedMeasure == null) return name;
                i++;
            }
            return name;
        }

        /// <summary>
        /// validate operatuions syntax
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public OperationState validateOperations(EditorItem<CalculatedMeasure> page)
        {
            if (page.EditedObject.GetCalculatedMeasureItems().Count == 0)
            {
                string message = "Item can't be empty"; 
                Kernel.Util.MessageDisplayer.DisplayError("Syntax Error", message);
                return OperationState.STOP;
            }
            int countEqualUsed = 0;
            foreach (CalculatedMeasureItem item in page.EditedObject.GetCalculatedMeasureItems())
            {                
                
                 if (item.measure != null && (item.sign == null || item.sign.Equals("")) && !item.openPar && item.position > 0)
                {
                    string message = "sign of measure " + item.measure.name + " can't be empty ";
                    Kernel.Util.MessageDisplayer.DisplayError("Syntax Error", message);
                    return OperationState.STOP;
                }
                if (item.sign != null && item.sign.Equals("="))
                {
                    countEqualUsed = countEqualUsed + 1;
                }
              
            }
            if (countEqualUsed > 1)
            {
                string message = "Cannot save with more than one operator = ";
                Kernel.Util.MessageDisplayer.DisplayError("Syntax Error", message);
                return OperationState.STOP;
            }

            return OperationState.CONTINUE;
        }

        public OperationState validateSyntax(EditorItem<CalculatedMeasure> page)
        {
            int countOpenPar = 0;
            int countClosePar = 0;
            CalculatedMeasureItem valuePrev = null;            
            CalculatedMeasureItem valueAft = null;
            bool status = true;
            int sizePage = page.EditedObject.GetCalculatedMeasureItems().Count;

            foreach (CalculatedMeasureItem item in page.EditedObject.GetCalculatedMeasureItems())
            {
                if (status && item.position >= 0 && item.position <= sizePage - 1)
                {
                    if (item.position == sizePage - 1)
                    {
                        valuePrev = (CalculatedMeasureItem)page.EditedObject.GetItemByPosition(item.position - 1);
                        valueAft = null;
                    }
                    else if (item.position == 0)
                    {
                        valuePrev = null;
                        valueAft = (CalculatedMeasureItem)page.EditedObject.GetItemByPosition(item.position + 1); ;
                    }
                    else
                    {
                        valuePrev = (CalculatedMeasureItem)page.EditedObject.GetItemByPosition(item.position - 1);
                        valueAft = (CalculatedMeasureItem)page.EditedObject.GetItemByPosition(item.position + 1);
                    }                    
                    if (item.openPar)
                    {
                        countOpenPar++;
                        if ((item.amount == 0 && item.measure == null) && item.closePar) status = false;
                        if (valuePrev != null && item.sign == null) status = false;
                        //if (valuePrev != null && (valuePrev.measure != null || valuePrev.amount != 0 || valuePrev.closePar)) status = false;
                        //if (valueAft != null && (item.measure == null && item.amount == 0 && (valueAft.closePar || valueAft.sign != null))) status = false;

                    }
                    if (status && item.closePar)
                    {
                        countClosePar++;
                        if (valueAft != null && valueAft.sign == null) status = false;
                        if (item.amount == 0 && item.measure == null) status = false;
                       // if (valuePrev != null && (valuePrev.measure == null && item.amount == 0 && (valuePrev.sign != null || valuePrev.openPar))) status = false;
                       // if (valueAft != null && (valueAft.openPar || ((valueAft.measure != null || valueAft.amount!= 0) && valuePrev.sign == null))) status = false;
                    }
                    if (status && item.sign != null)
                    {
                        if (valuePrev != null && (item.amount == 0 && item.measure == null)) status = false;
                        if (item.amount == 0 && item.measure == null) status = false;
                       // if (valuePrev != null && ((valuePrev.sign != null || valuePrev.openPar) && (valuePrev.measure == null && valuePrev.amount == 0))) status = false;
                       // if (valueAft != null && ((item.measure == null && item.amount == 0) && (valueAft.closePar || valueAft.sign != null))) status = false;
                    }
                    if (status && (item.measure != null || item.amount != 0))
                    {
                        if (valueAft != null && valueAft.sign == null ) status = false;
                        //if (item.sign == null && valuePrev != null && ((valuePrev.measure != null || valuePrev.amount != 0 || valuePrev.closePar))) status = false;
                       // if (valueAft != null && (valueAft.openPar || (valueAft.sign == null && (valueAft.measure != null || valueAft.amount != 0)))) status = false;
                    }
                }
            }
            if (status && countOpenPar != countClosePar) status = false;

            if (!status)
            {
                string message = "FORMULA SYNTAX ERROR ! Correct you formula ";
                Kernel.Util.MessageDisplayer.DisplayError("Syntax Error", message);
                return OperationState.STOP;
            }

            return OperationState.CONTINUE;
        }

        
        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        CalculatedMeasureEditorItem currentPage = new CalculatedMeasureEditorItem();

        public override OperationState Save(EditorItem<CalculatedMeasure> page)
        {
            try
            {

                if (ValidateEditedNewName() == OperationState.STOP)
                    return OperationState.STOP;


                if (validateOperations(page) == OperationState.STOP)
                    return OperationState.STOP;

                if (validateSyntax(page) == OperationState.STOP)
                    return OperationState.STOP;

                CalculatedMeasureItem last = page.EditedObject.GetItemByPosition(page.EditedObject.GetCalculatedMeasureItems().Count -1);
                if (last.sign == null || (last.sign != null && !last.sign.Equals("=")))
                {
                    CalculatedMeasureItem equalsSign = new CalculatedMeasureItem();
                    equalsSign.sign = "=";
                    page.EditedObject.AddItem(equalsSign);
                }

                currentPage = (CalculatedMeasureEditorItem)page;
                if (base.Save(page) == OperationState.STOP) return OperationState.STOP;
               //((CalculatedMeasureSideBar)SideBar).MeasureGroup.MeasureTreeview.AddOrUpdateCalculateMeasure(page.EditedObject);
                
            }
            catch (Exception)
            {
                DisplayError("Save", "Unable to save CalculatedMeasure");
                return OperationState.STOP;
            }
          //( (CalculatedMeasureEditorItem) page).getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.setIgnorePropertiesGridVisibility(false);
          string expression = page.EditedObject.computeExpression();
          if (expression.IndexOf("=") < 0) expression = expression + "=";
          this.ApplicationManager.MainWindow.StatusLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression;

            return OperationState.CONTINUE;
        }

        public OperationState Create(string name, CalculatedMeasure CalculatedMeasureInEdition)
        {
            CalculatedMeasure CalculatedMeasure = null;//CalculatedMeasureInEdition.getCopy(name);
            if (CalculatedMeasure == null) return OperationState.STOP;

            EditorItem<CalculatedMeasure> page = getEditor().addOrSelectPage(CalculatedMeasure);

           ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.AddCalculatedMeasure(CalculatedMeasure);
            return Open(CalculatedMeasure);
        }

        public bool isSaveAs = false;

        
        private CalculatedMeasure GetCalculatedMeasure(string name)
        {
            if (!IsNameUsed(name))
            {
                CalculatedMeasure calculatedMeasure = new CalculatedMeasure();
                calculatedMeasure.name = name;
                calculatedMeasure.group = GetCalculatedMeasureService().GroupService.getDefaultGroup();
                return calculatedMeasure;
            }
            return null;
        }
        
        private bool IsNameUsed(string name)
        {
            CalculatedMeasure obj = GetObjectByName(name);
            if (obj != null)
            {
                DisplayError("Duplicate Name", "There is another CalculatedMeasure named: " + name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void OnPageSelected(EditorItem<CalculatedMeasure> page)
        {
            if (page == null) return;
            CalculatedMeasureForm form = ((CalculatedMeasureEditorItem)page).getCalculatedMeasureForm();
           ((CalculatedMeasurePropertyBar)this.PropertyBar).TableLayoutAnchorable.Content = form.CalculatedMeasurePropertiesPanel;
           string expression = page.EditedObject.computeExpression();
          // if (expression.IndexOf("=") < 0)  expression = expression + "=";
           //this.ApplicationManager.MainWindow.StatusBarLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression;
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
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            string expression = page.EditedObject.computeExpression();
            this.ApplicationManager.MainWindow.StatusLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression;
            return OperationState.CONTINUE;
        }
        
        public override OperationState Rename()
        {
            if (base.Rename() != OperationState.CONTINUE)
                return OperationState.STOP;

            IsRenameOnDoubleClick = true;
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            return ValidateEditedNewName(page.EditedObject.name);
        }

        protected override void Rename(string name)
        {
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Text = name;
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
        protected override IView getNewView() { return new CalculatedMeasureEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar() { return new CalculatedMeasureToolBar(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de ToolBarHandlerBuilder liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de ToolBarHandlerBuilder</returns>
        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new CalculatedMeasureSideBar(); }

        protected override PropertyBar getNewPropertyBar() { return new CalculatedMeasurePropertyBar(); }

        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData()
        {

        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.CALCULATED_MEASURE;
        }
        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// En plus des handlers de base, on initialise les handlers sur :
        /// - DesignerPropertiesPanel
        /// - 
        /// - SpreadSheet
        /// - 
        /// </summary>
        protected override void initializePageHandlers(EditorItem<CalculatedMeasure> page)
        {
            base.initializePageHandlers(page);
            CalculatedMeasureEditorItem editorPage = (CalculatedMeasureEditorItem)page;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.groupField.GroupService = GetCalculatedMeasureService().GroupService;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.groupField.subjectType = SubjectTypeFound();
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.KeyUp += onNameTextChange;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.LostFocus += onNameTextLostFocus;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.groupField.Changed += onGroupFieldChange;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.IgnorePropertiesGridChanged += onIgnorePropertiesChange;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePanel.Changed += onCalculatedMeasureOperationsGridPanelChange;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePanel.ItemChanged += onCalculatedMeasurePanelItemChange;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePanel.ItemCloseParOrEqualSelected += onItemCloseParOrEqualSelected;
            editorPage.getCalculatedMeasureForm().CalculatedMeasurePanel.ItemDeleted += onCalculatedMeasurePanelItemDeleted;
        }

        private void onCalculatedMeasurePanelItemChange(object item)
        {
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            if (page == null) return;
            page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.displayCalculatedMeasureItemIgnoreProperties((CalculatedMeasureItem)item);
        }

        private void onItemCloseParOrEqualSelected(object newSelection)
        {
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            if (page == null) return;
            page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.setIgnorePropertiesGridVisibility(true);
            if (newSelection.ToString().Equals(")"))
            {
                CalculatedMeasureItem item = new CalculatedMeasureItem();
                item.measureType = CalculatedMeasureItem.MeasureType.MEASURE.ToString();
                page.getCalculatedMeasureForm().CalculatedMeasurePanel.SetCalculatedMeasureItemValue(item);
            }
            else
            {
                string expression = page.EditedObject.computeExpression();
               // expression = expression + "=";
                this.ApplicationManager.MainWindow.StatusLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression;
            }
        }

        /// <summary>
        /// Initialisation des donnée sur la SideBar.
        /// </summary>
        protected override void initializeSideBarData()
        {
           Measure rootMeasure  = GetCalculatedMeasureService().MeasureService.getRootMeasure();

           List<CalculatedMeasure> CalculatedMeasures = GetCalculatedMeasureService().getAllCalculatedMeasure();
           if(CalculatedMeasures!=null)
           ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.fillTree(new ObservableCollection<CalculatedMeasure>(CalculatedMeasures));

           ((CalculatedMeasureSideBar)SideBar).MeasureGroup.InitializeData();

           BGroup group = GetCalculatedMeasureService().GroupService.getDefaultGroup();
        }

        /// <summary>
        /// Initialisation des Handlers sur la SideBar.
        /// </summary>
        protected override void initializeSideBarHandlers()
        {
            ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.SelectionChanged += onSelectCalculatedMeasureFromSidebar;
            ((CalculatedMeasureSideBar)SideBar).MeasureGroup.Tree.Click += onSelectOperationItemFromSidebar;
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne une Input Table sur la sidebar.
        /// Cette opération a pour but d'ouvrir une page pour la calculatedMeasure selectionnée dans l'éditeur.
        /// </summary>
        /// <param name="sender">La calculatedMeasure sélectionnée</param>
        protected void onSelectCalculatedMeasureFromSidebar(object sender)
        {
            if (sender != null && sender is CalculatedMeasure)
            {
                CalculatedMeasure CalculatedMeasure = (CalculatedMeasure)sender;
                EditorItem<CalculatedMeasure> page = getCalculatedMeasureEditor().getPage(CalculatedMeasure.name);
                if (page != null)
                {
                    page.fillObject();
                    getCalculatedMeasureEditor().selectePage(page);
                    string expression = page.EditedObject.computeExpression();
                    this.ApplicationManager.MainWindow.StatusLabel.Content = String.IsNullOrEmpty(expression) ? "" : "Operation: " + expression; initializePageHandlers(page);

                }
                else if (CalculatedMeasure.oid != null && CalculatedMeasure.oid.HasValue)
                {

                    this.Open(CalculatedMeasure.oid.Value);
                }
                else
                {
                    page = getCalculatedMeasureEditor().addOrSelectPage(CalculatedMeasure);
                    initializePageHandlers(page);
                    page.Title = CalculatedMeasure.name;

                    getCalculatedMeasureEditor().ListChangeHandler.AddNew(CalculatedMeasure);
                }
                CalculatedMeasureEditorItem pageOpen = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            }
        }

        /// <summary>
        /// Cette méthode est exécutée lorsqu'on sélectionne un element sur la sidebar.
        /// ces instructions ont pour but de rajouté cet élément ,
        /// dans l'operation de la mesure calculé en édition
        /// </summary>
        /// <param name="sender">l operateur sélectionné</param>
        protected void onSelectOperationItemFromSidebar(object sender)
        {
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            if (page == null) return;
            if (page.EditedObject.name != (sender as Measure).name)
            {
                CalculatedMeasureItem item = createNewCalculatedMeasureItem(sender, page);
                if (page.getCalculatedMeasureForm().CalculatedMeasurePanel.SetCalculatedMeasureItemValue(item) == true)
                    OnChange();
            }
        }

        private CalculatedMeasureItem createNewCalculatedMeasureItem(object sender, CalculatedMeasureEditorItem page)
        {
            CalculatedMeasureItem item = new CalculatedMeasureItem();
            item.measureType = CalculatedMeasureItem.MeasureType.AMOUNT.ToString();
            if (sender != null)
            {
                page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.setIgnorePropertiesGridVisibility(true);
                if (sender is CalculatedMeasure)
                {
                    item.measureType = CalculatedMeasureItem.MeasureType.CALCULATED_MEASURE.ToString();
                    item.measure = (CalculatedMeasure)sender;
                }
                else if (sender is Measure)
                {
                    item.measure = (Measure)sender;
                    item.measureType = CalculatedMeasureItem.MeasureType.MEASURE.ToString();
                }
            }
            return item;
        }

        /// <summary>
        /// Cette methode est exécuté lorsqu'on édit le nom de la calculatedMeasure active.
        /// Si l'utilisateur tappe sur la touche ENTER, le nouveau nom est validé.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            if (args.Key == Key.Escape)
            {
                page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Text = page.Title;
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
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            string name = page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.groupField.textBox.Text;
            ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.updateCalculatedMeasure(name, page.Title, true);
            OnChange();
        }

        protected void onIgnorePropertiesChange()
        {
            /*  CalculatedMeasureItem item = page.EditedObject.calculatedMeasureItemListChangeHandler.Items.Last();
             if(item.sign!=null && item.sign.Equals("="))
             {
                item = page.EditedObject.GetItemByPosition(page.EditedObject.calculatedMeasureItemListChangeHandler.Items.Count - 2);
             }*/
            
            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            CalculatedMeasureItem item = page.getCalculatedMeasureForm().CalculatedMeasurePanel.ActiveItemPanel.CalculatedMeasureItem;
            page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.fillCalculatedMeasureItemIgnoreProperties(item);



            OnChange();
        }

        protected void onCalculatedMeasureOperationsGridPanelChange()
        {
            OnChange();
            

        }

      
        protected void onCalculatedMeasurePanelItemDeleted(object item)
        {

            CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            page.EditedObject.RemoveItem((CalculatedMeasureItem)item);
            OnChange();
           
        }



        #endregion

        public override bool validateName(EditorItem<CalculatedMeasure> page, string name)
        {
            if (!base.validateName(page, name)) return false;
            return ValidateEditedNewName() == OperationState.CONTINUE;
        }

        private bool IsRenameOnDoubleClick = false;
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual OperationState ValidateEditedNewName(string newName = "")
        {
           CalculatedMeasureEditorItem page = (CalculatedMeasureEditorItem)getCalculatedMeasureEditor().getActivePage();
            CalculatedMeasure calculatedMeasure = page.EditedObject;
            if (string.IsNullOrEmpty(newName))
                newName = page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                DisplayError("Empty Name", "The CalculatedMeasure name can't be empty!");
                page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.SelectAll();
                page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Focus();
                return OperationState.STOP;
            }

            bool found = false;
            if (GetCalculatedMeasureService().getByName(newName) != null) found = true;

            foreach (CalculatedMeasureEditorItem calculatedMeasurePage in getCalculatedMeasureEditor().getPages())
            {
                if ((found && newName != getCalculatedMeasureEditor().getActivePage().Title) || (calculatedMeasurePage != getCalculatedMeasureEditor().getActivePage() && newName == calculatedMeasurePage.Title))
                {
                    DisplayError("Duplicate Name", "There is another CalculatedMeasure named: " + newName);
                    page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Text = page.Title;
                    page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.SelectAll();
                    page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Focus();
                    return OperationState.STOP;
                }
                Kernel.Domain.Measure root = GetCalculatedMeasureService().MeasureService.getRootMeasure();
                if (calculatedMeasurePage == getCalculatedMeasureEditor().getActivePage() && root.GetChildByName(newName) != null)
                {
                    DisplayError("Duplicate Name", "There is another Measure named: " + newName);
                    page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Text = page.Title;
                    page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.SelectAll();
                    page.getCalculatedMeasureForm().CalculatedMeasurePropertiesPanel.nameTextBox.Focus();
                    return OperationState.STOP;
                }
            }

             if (!IsRenameOnDoubleClick)
                if (calculatedMeasure.name.ToUpper().Equals(newName.ToUpper())) return OperationState.CONTINUE;

            ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.updateCalculatedMeasure(newName, calculatedMeasure.name, false);
            //((CalculatedMeasureSideBar)SideBar).MeasureGroup.MeasureTreeview.updateCalculatedMeasure(newName, page.EditedObject.name, false);
            
            calculatedMeasure.name = newName;
            page.Title = newName;
            OnChange();
            return OperationState.CONTINUE;
        }
              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override CalculatedMeasure GetObjectByName(string name)
        {
            return ((CalculatedMeasureSideBar)SideBar).CalculatedMeasureGroup.CalculatedMeasureTreeview.getCalculatedMeasureByName(name);

        }

        public override Kernel.Application.OperationState Search(object oid)
        {
            return Kernel.Application.OperationState.CONTINUE;
        }

        public override OperationState RenameItem(string newName) { return OperationState.CONTINUE; }

        /// <summary>
        /// 
        /// </summary>
        protected override void AfterClose()
        {
            base.AfterClose();
            this.ApplicationManager.MainWindow.StatusLabel.Content = "";
        }
    }
}
