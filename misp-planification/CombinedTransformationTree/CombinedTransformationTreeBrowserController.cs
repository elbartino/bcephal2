using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Planification.CombinedTransformationTree
{
    public class CombinedTransformationTreeBrowserController : BrowserController<Misp.Kernel.Domain.CombinedTransformationTree, BrowserData>
    {

        public CombinedTransformationTreeBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.COMBINED_TRANSFORMATION_TREE;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return Kernel.Application.FunctionalitiesCode.COMBINED_TRANSFORMATION_TREES_EDIT; }


        /// <summary>
        /// Service pour acceder aux opérations liés aux Designs.
        /// </summary>
        /// <returns>DesignService</returns>
        public CombineTransformationTreeService GetCombineTransformationTreeService()
        {
            return (CombineTransformationTreeService)base.Service;
        }

        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new CombinedTransformationTreeBrowser(this.SubjectType, this.FunctionalityCode); }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.COMBINED_TRANSFORMATION_TREE;
        }


        protected override Misp.Kernel.Ui.Base.ToolBar getNewToolBar() { return new CombinedTransformationTreeBrowserToolBar(); }

        protected override ToolBarHandlerBuilder getNewToolBarHandlerBuilder() { return new ToolBarHandlerBuilder(this); }



        protected override void initializeToolBarHandlers()
        {
            base.initializeToolBarHandlers();
            if (this.ToolBar == null) return;
            ((CombinedTransformationTreeBrowserToolBar)this.ToolBar).RunButton.Click += OnRun;
            ((CombinedTransformationTreeBrowserToolBar)this.ToolBar).ClearButton.Click += OnClear;
        }

        private void OnClear(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.GetBrowser().Grid.SelectedItems.Count == 0) return;
            TableActionData tableActionData = new TableActionData();
            foreach (object tree in this.GetBrowser().Grid.SelectedItems)
            {
                BrowserData bData = (BrowserData)tree;
                int oid = (int)bData.oid;
                Kernel.Domain.CombinedTransformationTree ctree = GetCombineTransformationTreeService().getByOid(oid);
            }

            //tableActionData.oids.Add(oid);
            //GetCombineTransformationTreeService().TransformationTreeService.ClearTreeHandler += updateClearProgress;
            GetCombineTransformationTreeService().TransformationTreeService.ClearTree(tableActionData);
        }

        private void OnRun(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.GetBrowser().Grid.SelectedItems.Count == 0) return;

            List<int> listTreeToRun = new List<int>(0);
            foreach (object obj in this.GetBrowser().Grid.SelectedItems)
            {
                int oid = ((BrowserData)obj).oid;
                listTreeToRun.AddRange(GetCombineTransformationTreeService().getByOid(oid).getTransformationTreesOids());
            }
           
            GetCombineTransformationTreeService().TransformationTreeService.RunHandler += updateRunProgress;
            GetCombineTransformationTreeService().TransformationTreeService.Run(listTreeToRun, true);
            Mask(true, RunMessageUtil.getMaskStartText());
        }

        private void updateRunProgress(TransformationTreeRunInfo info)
        {
            if (info == null || info.runEnded == true)
            {
                GetCombineTransformationTreeService().TransformationTreeService.RunHandler -= updateRunProgress;
                Mask(false, RunMessageUtil.getMaskEndedText());
                Service.FileService.SaveCurrentFile();
                ApplicationManager.MainWindow.treeDetails.Visibility = System.Windows.Visibility.Hidden;
                ApplicationManager.MainWindow.ProgressBarTreeContent.Maximum = 0;
                ApplicationManager.MainWindow.ProgressBarTreeContent.Value = 0;
                ApplicationManager.MainWindow.statusTextBlockTreeContent.Content = "";
            }
            else
            {
                int rate = info.totalCount != 0 ? (Int32)(info.runedCount * 100 / info.totalCount) : 0;
                if (rate > 100) rate = 100;

                ApplicationManager.MainWindow.ProgressBarTree.Maximum = info.totalCount;
                ApplicationManager.MainWindow.ProgressBarTree.Value = info.runedCount;
                ApplicationManager.MainWindow.statusTextBlockTree.Content = "" + rate + " % " + " (" + info.runedCount + " / " + info.totalCount + ")";

                if (info.currentTreeRunInfo != null)
                {
                    rate = info.currentTreeRunInfo.runedCount != 0 ? (Int32)(info.currentTreeRunInfo.runedCount * 100 / info.currentTreeRunInfo.totalCount) : 0;
                    if (rate > 100) rate = 100;

                    if (info.currentTreeRunInfo.runedCount != 0)
                    {
                        ApplicationManager.MainWindow.treeDetails.Visibility = System.Windows.Visibility.Visible;
                        ApplicationManager.MainWindow.ProgressBarTreeContent.Maximum = info.currentTreeRunInfo.totalCount;
                        ApplicationManager.MainWindow.ProgressBarTreeContent.Value = info.currentTreeRunInfo.runedCount;
                        ApplicationManager.MainWindow.statusTextBlockTreeContent.Content = "" + info.currentTreeRunInfo.item + " :  " + rate + " %";
                    }
                }
            }
        }

        protected void Mask(bool mask, string content = "Saving...")
        {
            ApplicationManager.MainWindow.BusyBorder2.Visibility = mask ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }


        protected override OperationState EditProperty(BrowserData item, String header, Object value)
        {
            if (item == null || String.IsNullOrWhiteSpace(header) || value == null) return OperationState.STOP;
            if (header.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
            {
                return RenameItem(item, (String)value);
            }
            if (header.Equals("Visible in shortcut", StringComparison.InvariantCultureIgnoreCase))
            {
                BrowserData data = new BrowserData(item);
                data.visibleInShortcut = (bool)value;
                try
                {
                    item = Service.SaveBrowserData(data);
                    if (item == null) return OperationState.STOP;
                    item.visibleInShortcut = (bool)value;
                }
                catch (Misp.Kernel.Domain.BcephalException)
                {
                    DisplayError("Unable edit item", "Unable edit : " + item.name);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }


    }
}

