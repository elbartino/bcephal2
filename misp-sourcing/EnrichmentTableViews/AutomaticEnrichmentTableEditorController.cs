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


       #region Editor and Service

       protected override IView getNewView() { return new AutomaticEnrichmentTableEditor(); }

       AutomaticGridDataDialog dialog;
       protected override void performRun(AutomaticSourcingEditorItem page)
       {
           if (validateColumns(page))
           {
               dialog = new AutomaticGridDataDialog();
               dialog.InputGridService = ApplicationManager.ControllerFactory.ServiceFactory.GetInputGridService();
               dialog.loadGrids();
               dialog.NewGridNameTextBox.Text = page.getAutomaticSourcingForm().SpreadSheet.DocumentName;
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
               foreach (AutomaticSourcingColumn column in sheet.automaticSourcingColumnListChangeHandler.Items)
               {
                   if (!column.isValid()) columns.Add(column.Name);
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
               MessageBoxResult response = MessageDisplayer.DisplayYesNoQuestion("Run", message);
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

       public override bool isAutomaticGrid()
       {
           return true;
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
           sideBar.RemoveGroup(sideBar.PeriodNameGroup);
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
           automaticGrid.name = getNewPageName("Enrichment Table");
           automaticGrid.isGrid = false;
           automaticGrid.isAutomaticGrid = false;
           automaticGrid.isEnrichmentTable = true;
           automaticGrid.group = GetAutomaticEnrichmentTableService().GroupService.getDefaultGroup();
           return automaticGrid;
       }             


   }
}
