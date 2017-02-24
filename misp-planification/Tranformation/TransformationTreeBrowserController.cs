using Misp.Kernel.Application;
using Misp.Kernel.Controller;
using Misp.Kernel.Domain;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Service;
using Misp.Kernel.Task;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Util;
using Misp.Planification.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Misp.Planification.Tranformation
{
    public class TransformationTreeBrowserController : BrowserController<Misp.Kernel.Domain.TransformationTree, BrowserData>
    {

        public TransformationTreeBrowserController() 
        {
            ModuleName = PlugIn.MODULE_NAME;
            this.SubjectType = Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
        }

        /// <summary>
        /// L'éditeur.
        /// </summary>
        public override string GetEditorFuntionality() { return PlanificationFunctionalitiesCode.TRANSFORMATION_TREE_EDIT; }
        
        /// <summary>
        /// Crée et retourne une nouvelle instance de la vue gérée par ce controller.
        /// </summary>
        /// <returns>Une nouvelle instance de la vue</returns>
        protected override IView getNewView() { return new TransformationTreeBrowser(this.SubjectType); }
        
        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }

        public override Misp.Kernel.Domain.SubjectType SubjectTypeFound()
        {
            return Misp.Kernel.Domain.SubjectType.TRANSFORMATION_TREE;
        }

        public TransformationTreeService getTransformationTreeService()
        {
            return (TransformationTreeService)Service;
        }



        /// <summary>
        /// Supprime les objets sélectionnés
        /// </summary>
        /// <returns></returns>
        public override OperationState Delete()
        {
            System.Collections.IList items = GetBrowser().Grid.SelectedItems;
            if (items == null || items.Count == 0) return OperationState.STOP;
            int count = items.Count;
            string message = "You are about to delete " + count + " trees.\nDo you want to continue?";
            if (count == 1)
            {
                object item = GetBrowser().Grid.SelectedItem;
                if (item != null) message = "You are about to delete " + item.ToString() + " .\nDo you want to continue?";
            }
            MessageBoxResult result = Kernel.Util.MessageDisplayer.DisplayYesNoQuestion("Delete", message);
            if (result == MessageBoxResult.Yes)
            {

                action = new BusyAction(false)
                {
                    DoWork = () =>
                    {
                        try
                        {
                            action.ReportProgress(0, message);
                            bool canDelete = true; // getTransformationTreeService().usedByCombinedTree(items);
                            if (!canDelete)
                            {
                                String warning = count == 1 ? "Unable to delete " + items[0].ToString() + " because it's used by a combined tree.\nDelete the combined tree and try again."
                                    : "Unable to delete one or more tree in the selection because there're used by a combined tree.\nDelete the combined tree and try again.";
                                MessageDisplayer.DisplayWarning("Delete", warning);
                            }
                            else
                            {
                                if (!Service.Delete(items)) Kernel.Util.MessageDisplayer.DisplayError("Delete", "Delete fail!");
                                else System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => Search()));
                            }
                            action.ReportProgress(100, message);
                        }
                        catch (BcephalException e)
                        {
                            MessageDisplayer.DisplayError("Error", e.Message);
                            action = null;
                            return OperationState.STOP;
                        }
                        return OperationState.CONTINUE;
                    }

                };
                action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationManager.MainWindow.OnBusyPropertyChanged);
                action.Run();
            }
            return OperationState.CONTINUE;
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
