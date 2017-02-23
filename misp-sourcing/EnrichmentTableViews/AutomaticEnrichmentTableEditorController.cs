using Misp.Kernel.Domain;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.Sidebar;
using Misp.Kernel.Util;
using Misp.Sourcing.AutomaticSourcingViews;
using Misp.Sourcing.Base;
using Misp.Sourcing.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Sourcing.EnrichmentTableViews
{
   public class AutomaticEnrichmentTableEditorController : AutomaticSourcingEditorController
   {

        public AutomaticEnrichmentTableEditorController()
           : base()
        {
            this.SubjectType = Kernel.Domain.SubjectType.AUTOMATIC_ENRICHMENT_TABLE;
        }


       #region Editor and Service

       public override bool isEnrichmentTable()
       {
           return true;
       }

       protected override IView getNewView() { return new AutomaticEnrichmentTableEditor(); }

       AutomaticEnrichmentTableDataDialog dialog;
       protected override void performRun(AutomaticSourcingEditorItem page)
       {
           if (validateColumns(page))
           {
               dialog = new AutomaticEnrichmentTableDataDialog();
               dialog.EnrichmentTableService = ApplicationManager.ControllerFactory.ServiceFactory.GetEnrichmentTableService();
               dialog.loadTables(page.getAutomaticSourcingForm().SpreadSheet.DocumentName);
               //dialog.NewTableNameTextBox.Text = page.getAutomaticSourcingForm().SpreadSheet.DocumentName;
               dialog.cancelButton.Click += OnCancelAutomaticGridDataDialog;
               dialog.runButton.Click += OnRunAutomaticDataDialog;
               dialog.ShowDialog();
           }
       }

       protected virtual bool validateColumns(AutomaticSourcingEditorItem page)
       {
           List<String> columns = new List<string>(0);
           foreach (AutomaticSourcingSheet sheet in page.EditedObject.automaticSourcingSheetListChangeHandler.Items)
           {
               bool? hasPrimaryKey = null;
               if (sheet.firstRowColumn) 
               {
                   FillAutomaticSourcingColumn();
               }
               foreach (AutomaticSourcingColumn column in sheet.automaticSourcingColumnListChangeHandler.Items)
               {
                   if (!hasPrimaryKey.HasValue) hasPrimaryKey = false;
                   if (column.primary) hasPrimaryKey = true;
                   if (!column.isValid()) columns.Add(column.ToString());
               }
               if (hasPrimaryKey.HasValue && !hasPrimaryKey.Value)
               {
                   MessageDisplayer.DisplayWarning("Automatic Enrichment Table Run", "There is no primary column in sheet : '" + sheet.Name + "'!\nYou have to set at least one column as primary.");
                   return false;
               }
           }
           if (columns.Count > 0)
           {
               String message = "There is columns with no definition : ";
               String coma = "";
               foreach (String column in columns)
               {
                   message += coma + "\"" + column + "\"";
                   coma = ", ";
               }
               message += "\nDo you want to continue?";
               MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Automatic Enrichment Table Run", message);
               if (response == MessageBoxResult.Yes) return true;
               return false;
           }
           return true;
       }

       private void OnRunAutomaticDataDialog(object sender, System.Windows.RoutedEventArgs e)
       {
           AutomaticGridData data = dialog.Fill();
           if (data == null) return;
           AutomaticSourcingEditorItem page = (AutomaticSourcingEditorItem)getAutomaticSourcingEditor().getActivePage();

           string filePath = page.EditedObject.excelFile;
           string path = System.IO.Path.GetDirectoryName(filePath) + System.IO.Path.DirectorySeparatorChar;
           string fileName = GetAutomaticSourcingService().FileService.FileTransferService.AutomaticActionsUpload(System.IO.Path.GetFileName(filePath), path);
           if (fileName == null) return;

           GetAutomaticSourcingService().SaveTableHandler += UpdateSaveInfo;
           GetAutomaticSourcingService().OnUpdateUniverse += OnUpdateUniverse;
           Mask(true, "Running ...");
           data.automaticSourcingOid = page.EditedObject.oid.Value;

           data.setExcelFilePath(fileName);
           data.excelExtension = Kernel.Util.ExcelUtil.GetFileExtension(fileName).Extension;
           OnCancelAutomaticGridDataDialog(sender, e);
           GetAutomaticSourcingService().Run(data);
       }

       private void OnCancelAutomaticGridDataDialog(object sender, System.Windows.RoutedEventArgs e)
       {
           dialog.cancelButton.Click -= OnCancelAutomaticGridDataDialog;
           dialog.runButton.Click -= OnRunAutomaticDataDialog;
           dialog.Close();
           dialog = null;
       }


       /// <summary>
       /// Service pour acceder aux opérations liés à l'automaticSourcing.
       /// </summary>
       /// <returns>DesignService</returns>
       public virtual AutomaticEnrichmentTableService GetAutomaticEnrichmentTableService()
       {
           return (AutomaticEnrichmentTableService)base.Service;
       }

      

       /// <summary>
       /// Crée et retourne une nouvelle instance de la ToolBar liée à ce controller.
       /// </summary>
       /// <returns>Une nouvelle instance de la ToolBar</returns>
       protected override Kernel.Ui.Base.ToolBar getNewToolBar()
       {
           AutomaticSourcingToolBar toolBar = new AutomaticSourcingToolBar();
           toolBar.RunButton.ToolTip = "Run Automatic Enrichment Table";
           toolBar.SaveButton.ToolTip = "Save Automatic Enrichment Table";
           toolBar.CloseButton.ToolTip = "Exit Automatic Enrichment Table";
           return toolBar;
       }

       protected override SideBar getNewSideBar()
       {
           AutomaticSourcingSideBar sideBar = new AutomaticSourcingSideBar();
           sideBar.AutomaticSourcingGroup.Header = "Automatic Enrichment Tables";
           sideBar.RemoveGroup(sideBar.MeasureGroup);
           sideBar.RemoveGroup(sideBar.CaculatedMeasureGroup);
           sideBar.RemoveGroup(sideBar.PeriodGroup);
           return sideBar;
       }


       public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
       {
           return Misp.Kernel.Domain.SubjectType.AUTOMATIC_ENRICHMENT_TABLE;
       }

       #endregion


       protected override Kernel.Domain.AutomaticSourcing GetNewAutomaticSourcing()
       {
           Kernel.Domain.AutomaticSourcing automaticGrid = new Kernel.Domain.AutomaticSourcing();
           automaticGrid.name = getNewPageName("Automatic Enrichment");
           automaticGrid.isGrid = false;
           automaticGrid.isAutomaticGrid = false;
           automaticGrid.isEnrichmentTable = true;
           automaticGrid.group = GetAutomaticEnrichmentTableService().GroupService.getDefaultGroup();
           return automaticGrid;
       }             


   }
}
