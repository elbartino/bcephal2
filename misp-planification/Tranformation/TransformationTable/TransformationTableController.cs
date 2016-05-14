using Misp.Kernel.Application;
using Misp.Kernel.Ui.Base;
using Misp.Reporting.StructuredReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.Tranformation.TransformationTable
{
    public class TransformationTableController : StructuredReportEditorController
    {
        public Kernel.Domain.TransformationTable EditedObject { get; set; }

        public TransformationTableController() { }


        /// <summary>
        /// Cette methode permet de créer une nouvelle table.
        /// </summary>
        /// <returns>CONTINUE si la création du nouveau Model se termine avec succès. STOP sinon</returns>
        public OperationState Create(Misp.Kernel.Domain.TransformationTable report)
        {
            ((StructuredReportSideBar)SideBar).StructuredReportGroup.StructuredReportTreeview.AddStructuredReport(report);
            StructuredReportEditorItem page = (StructuredReportEditorItem)getEditor().addOrSelectPage(report);
            initializePageHandlers(page);
            page.Title = report.name;
            page.IsModify = true;
            getEditor().ListChangeHandler.AddNew(report);
            page.getStructuredReportForm().StructuredReportPropertiesPanel.ColumnForms.ItemForm.PeriodicityService = GetStructuredReportService().PeriodicityService;
            this.EditedObject = (Kernel.Domain.TransformationTable)page.EditedObject;
            DisplayActiveColumn();
            return OperationState.CONTINUE;
        }


        /// <summary>
        /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la ToolBar</returns>
        protected override Kernel.Ui.Base.ToolBar getNewToolBar()
        {
            StructuredReportToolBar toolBar = new StructuredReportToolBar();
            toolBar.RunButton.ToolTip = "Run Transformation Table";
            toolBar.SaveButton.ToolTip = "Save Transformation Table";
            toolBar.CloseButton.ToolTip = "Exit Transformation Table Editor";
            return toolBar;
        }


        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new TransformationTableEditor(); }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la SideBar liée à ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la SideBar</returns>
        protected override SideBar getNewSideBar() { return new TransformationTableSideBar(); }


        /// <summary>
        /// Service pour acceder aux opérations liés aux InputTables.
        /// </summary>
        /// <returns>ReportService</returns>
        public TransformationTableService GetTransformationTableService()
        {
            return (TransformationTableService)base.Service;
        }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
        }

        public override void OnPageSelected(EditorItem<Kernel.Domain.StructuredReport> page)
        {
            if (page == null) return;
            StructuredReportForm form = ((StructuredReportEditorItem)page).getStructuredReportForm();
            form.StructuredReportPropertiesPanel.gridEachLoop.Visibility = System.Windows.Visibility.Visible;
            form.StructuredReportPropertiesPanel.checkboxAllocateEach.Visibility = System.Windows.Visibility.Visible;
            ((StructuredReportPropertyBar)this.PropertyBar).DesignLayoutAnchorable.Title = "Transformation Table Properties";
            ((StructuredReportPropertyBar)this.PropertyBar).DesignLayoutAnchorable.Content = form.StructuredReportPropertiesPanel;
        }
        

        protected override void initializeSideBarHandlers()
        {
            base.initializeSideBarHandlers();
            
        }

        protected override Kernel.Domain.StructuredReport GetNewStructuredReport()
        {
            Kernel.Domain.TransformationTable report = new Kernel.Domain.TransformationTable();
            report.name = getNewPageName("Transformation Table");
            report.group = GetStructuredReportService().GroupService.getDefaultGroup();
            return report;
        }

        protected override OperationState ValidateEditedNewName(string newName = "")
        {
            Kernel.Domain.TransformationTable report = GetTransformationTableService().getByName(newName);
            if (report != null)
            {
                StructuredReportEditorItem page = (StructuredReportEditorItem)getStructuredReportEditor().getActivePage();            
                DisplayError("Duplicate Name", "There is another Transformation table named: " + newName);
                page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Text = page.Title;
                page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.SelectAll();
                page.getStructuredReportForm().StructuredReportPropertiesPanel.NameTextBox.Focus();
                return OperationState.STOP;
            }
            return base.ValidateEditedNewName(newName);
        }

        public override OperationState Save(EditorItem<Kernel.Domain.StructuredReport> page)
        {
            if (page.IsModify)
            {
                if (!page.validateEdition()) return OperationState.STOP;
                Kernel.Domain.StructuredReport table;
                if (page.EditedObject is Kernel.Domain.TransformationTable) table = (Kernel.Domain.TransformationTable)page.EditedObject;
                else table = page.EditedObject;
                try
                {
                    StructuredReportEditorItem currentPage = (StructuredReportEditorItem)page;
                    this.EditedObject = (Kernel.Domain.TransformationTable)GetTransformationTableService().Save(table);
                }
                catch (Exception)
                {
                    return OperationState.STOP;
                }
                this.EditedObject =(Kernel.Domain.TransformationTable)page.EditedObject;
            }
            return OperationState.CONTINUE;
        }
    }
}
