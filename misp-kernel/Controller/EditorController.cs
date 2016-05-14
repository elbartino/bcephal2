using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Application;
using Misp.Kernel.Util;
using Misp.Kernel.Task;

namespace Misp.Kernel.Controller
{

    /// <summary>
    /// Un EditorController est un controller qui gère l'édition d'objets d'un type précis.
    /// L'EditorController possède une vue d'édition (Editor).
    /// 
    /// On peut ouvrir plusieurs objets dans l'éditeur (un objet par page).
    /// 
    /// </summary>
    /// <typeparam name="T">Le type d'objet manager par ce controller</typeparam>
    public abstract class EditorController<T, B> : Controller<T, B>
        where T : Domain.Persistent
        where B : Misp.Kernel.Domain.Browser.BrowserData
    {

        #region Constructors

        /// <summary>
        /// Construit une nouvelle instance de EditorController
        /// </summary>
        public EditorController() : base() { }

        #endregion


        #region Properties

        
        #endregion


        #region Operations

        /// <summary>
        /// Ouvre l'objet identifié par l'oid dans une page de l'éditeur.
        /// 1. Si il existe déjà une page ouverte pour cet objet, on la sélectionne.
        /// 2. Sinon
        /// 2.1. On récupère l'objet à ouvrir via le service
        /// 2.2. On rajoute une nouvelle page pour cette objet dans l'éditeur.
        /// 2.3. On initialise les handlers sur la nouvelle page
        /// 2.4.
        /// </summary>
        /// <param name="oid">L'identifiant de l'objet à ouvrir</param>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        //public override OperationState Open(object oid)
        //{
        //    DateTime start = DateTime.Now;

        //    EditorItem<T> page = getEditor().getPage((int)oid);
        //    if (page != null)
        //    {
        //        getEditor().selectePage(page);
        //        return OperationState.CONTINUE;
        //    }
        //    try
        //    {
        //        DateTime time = DateTime.Now;
        //        T item = Service.getByOid((int)oid);
        //        TimeSpan read = DateTime.Now - time;

        //        time = DateTime.Now;
        //        OperationState state = Open(item);
        //        TimeSpan open = DateTime.Now - time;
        //        TimeSpan total = DateTime.Now - start;
        //        Console.Out.WriteLine("Read : " + read);
        //        Console.Out.WriteLine("Open : " + open);
        //        Console.Out.WriteLine("Total : " + total);
        //        return state;
        //    }
        //    catch (Kernel.Service.ServiceExecption)
        //    {
        //        DisplayError("Unable to read item", "Unable to read item identified by : " + oid);
        //    }


        //    return OperationState.STOP;
        //}

        BusyAction action;

        public override OperationState Open(object oid)
        {
            
            action = new BusyAction(false)
            {
                DoWork = () =>
                {
                    try
                    {
                        DateTime start = DateTime.Now;
                        DateTime time = DateTime.Now;
                        T item = Service.getByOid((int)oid);
                        TimeSpan read = DateTime.Now - time;

                        time = DateTime.Now;
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => Open(item)));
                        TimeSpan open = DateTime.Now - time;
                        TimeSpan total = DateTime.Now - start;

                        Console.Out.WriteLine("Read  = " + read);
                        Console.Out.WriteLine("Open  = " + open);
                        Console.Out.WriteLine("Total = " + total);
                        return OperationState.CONTINUE;
                    }
                    catch (Kernel.Service.ServiceExecption e)
                    {
                        MessageDisplayer.DisplayError("Error", e.Message);
                        action = null;
                        return OperationState.STOP;
                    }
                },
                
                EndWork = () =>
                {
                    try
                    {

                    }
                    catch (Exception e)
                    {
                        MessageDisplayer.DisplayError("Error", e.Message);
                        return OperationState.STOP;
                    }
                    finally
                    {
                        action = null;
                    }
                    return OperationState.CONTINUE;
                }

            };

            action.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationManager.MainWindow.OnBusyPropertyChanged);
            action.Run();

            return OperationState.STOP;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public virtual OperationState Open(T item)
        {
            EditorItem<T> page = getEditor().addOrSelectPage(item);
            initializePageHandlers(page);
            getEditor().ListChangeHandler.AddNew(item);
            return OperationState.CONTINUE;    
        }

        /// <summary>
        /// Sauve l'objet en cours d'édition sur la page active.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Save() { return SaveAll(); }

        /// <summary>
        /// Sauve les objets en cours d'édition sur toutes les pages ouvertes.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState SaveAll()
        {
            foreach (EditorItem<T> page in getEditor().getPages())
            {
                if (page.IsModify && !page.validateEdition()) return OperationState.STOP;
            }
            SavePages();
            return saveSuccess ? OperationState.CONTINUE : OperationState.STOP;
        }

        bool saveSuccess { get; set; }

        protected virtual void SavePages()
        {
            saveSuccess = true;
            foreach (EditorItem<T> page in getEditor().getPages())
            {
                OperationState resultSave =  Save(page);
                if (resultSave == OperationState.STOP) saveSuccess = false;
            }
        }

        /// <summary>
        /// Sauve les objets en cours d'édition sur la page.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public virtual OperationState Save(EditorItem<T> page)
        {
            if (page.IsModify)
            {
                if (!page.validateEdition()) return OperationState.STOP;
                page.fillObject();
                T editedObject = page.EditedObject;
                try
                {
                    editedObject = Service.Save(editedObject);
                    page.EditedObject = editedObject;
                    page.displayObject();
                    page.IsModify = false;
                }
                catch (Domain.BcephalException)
                {
                    DisplayError("Unable to save item", "Unable to save : " + editedObject.ToString());
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        public virtual OperationState SaveAs(string name, EditorItem<T> page)
        {
            page = getEditor().getActivePage();
            if (page != null && validateName(page, name))
            {
                try
                {

                    T item = Service.SaveAs(page.EditedObject.oid.Value, name);
                    Open(item);
                }
                catch (Domain.BcephalException)
                {
                    DisplayError("Unable to save item", "Unable to save : " + name);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }

        public override OperationState SaveAs(string name)
        {
            EditorItem<T> page = getEditor().getActivePage();
            page.InitializeCustomDialog("Save as");
            if (!page.EditedObject.oid.HasValue || page.EditedObject.oid.Value == null)
            {
                Rename(name);
                return Save(page);
            }

            if (page != null && validateName(page, name))
            {
                try
                {
                
                    T item = Service.SaveAs(page.EditedObject.oid.Value, name);
                    Open(item);
                }
                catch (Domain.BcephalException)
                {
                    DisplayError("Unable to save item", "Unable to save : " + name);
                    return OperationState.STOP;
                }
            }
            return OperationState.CONTINUE;
        }


        public virtual OperationState Delete(EditorItem<T> page)
        {
            page.Content = null;
            /* if (page.IsModify)
             {
                 if (!page.validateEdition()) return OperationState.STOP;
                 page.fillObject();
                 T editedObject = page.EditedObject;
                 try
                 {
                     page.Content = null;
                     editedObject = Service.Save(editedObject);
                     page.EditedObject = editedObject;
                     page.displayObject();
                     page.IsModify = false;
                 }
                 catch (Domain.BcephalException)
                 {
                     DisplayError("Unable to save item", "Unable to save : " + editedObject.ToString());
                     return OperationState.STOP;
                 }
             }*/
            return OperationState.CONTINUE;
        }

        public override OperationState SaveAs()
        {
            EditorItem<T> page = getEditor().getActivePage();
            if (page == null) return OperationState.STOP;
            string newName = getNewPageName(page.Title);
            page.InitializeCustomDialog("Save as");
            page.namePanel.NameTextBox.Text = newName;
            page.namePanel.NameTextBox.SelectAll();
            if (page.CustomDialog.ShowCenteredToMouse().Value)
            {
                string name = page.namePanel.NameTextBox.Text;
                if (name == null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Util.MessageDisplayer.DisplayError("Save as", "The name can't be empty!");
                    return OperationState.STOP;
                }
                return SaveAs(name);
            }

            return OperationState.CONTINUE;
        }
        
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override OperationState Rename()
        {
            EditorItem<T> page = getEditor().getActivePage();
            if (page == null) return OperationState.STOP;
            page.InitializeRenameField();
            page.RenameTextBox.Text = page.Title;           
            if (page.RenameDialog.ShowCenteredToMouse().Value)
            {
                string name = page.RenameTextBox.Text;
                if (validateName(page, name))
                {
                    Rename(name);
                }
                else
                    return OperationState.STOP;
            }
            return OperationState.CONTINUE;
        }

        

        protected virtual void Rename(string name)
        {
            EditorItem<T> page = getEditor().getActivePage();
            page.Title = name;
            page.IsModify = true;
            page.EditedObject.isModified = true;
            if (this.ChangeEventHandler != null) this.ChangeEventHandler.change();
            else OnChange();
        }

        public virtual bool validateName(EditorItem<T> page, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Kernel.Util.MessageDisplayer.DisplayError("Empty Name", "Name can't be empty!");
                return false;
            }

            foreach (EditorItem<T> pageItem in getEditor().getPages())
            {
                if (page != pageItem && name.ToUpper() == pageItem.Title.ToUpper())
                {
                    Kernel.Util.MessageDisplayer.DisplayError("Duplicate Name", "Another Object named " + name + " already exists!");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Cette méthode est appelée avant de fermer la vue contôlée.
        /// Elle demande à l'utilisateur s'il veut sauver les éventuels 
        /// modifications avant la fermeture de la vue.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'opération a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState TryToSaveBeforeClose()
        {
            if (!IsModify) { RemoveCommands(); return OperationState.CONTINUE; }
            MessageBoxResult result = Util.MessageDisplayer.DisplayYesNoCancelQuestion("Close", "Do you want to save change before close?");
            if (result == MessageBoxResult.Cancel) return OperationState.STOP;
            if (result == MessageBoxResult.No) 
            {
                RemoveCommands();
                this.IsModify = false;
                return OperationState.CONTINUE; 
            }

            OperationState p = Save();
            if (p == OperationState.CONTINUE) RemoveCommands();
            return p;
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
            //Save();
            EditorItem<T> page = getEditor().getActivePage();
            if (page != null) page.IsModify = true;
            return OperationState.CONTINUE;
        }
      
        public override OperationState Search() { return OperationState.CONTINUE; }
        public override OperationState Open() { return OperationState.CONTINUE; }
        public override Kernel.Application.OperationState Search(object oid) { return Kernel.Application.OperationState.CONTINUE; }
        public override OperationState RenameItem(String name) { return OperationState.CONTINUE; }

        #endregion


        #region Initializations

        /// <summary>
        /// Effectue l'initialisation du controller.
        /// On initialise la vue et le bouton + pour rajouté une nouvelle page à l'éditeur.
        /// </summary>
        /// <returns>
        /// OperationState.CONTINUE si l'initialisation a réussi
        /// OperationState.STOP sinon
        /// </returns>
        public override OperationState Initialize()
        {
            base.Initialize();
            InitializeNewPageHandler();
            return OperationState.CONTINUE;
        }

        public Editor<T>.NewPageEventHandler NewPageSelectedHandler;

        /// <summary>
        /// Initialise le bouton + utilisé pour rajouté une nouvelle page à l'éditeur.
        /// </summary>
        protected void InitializeNewPageHandler()
        {
            NewPageSelectedHandler = new Editor<T>.NewPageEventHandler(this.OnNewPageSelected);
            getEditor().NewPageSelected += NewPageSelectedHandler;
        }

        /// <summary>
        /// Cette méthode est appelée lorsqu'on click sur le bouton + dans l'éditeur 
        /// pour rajouter une nouvelle page.
        /// </summary>
        protected virtual void OnNewPageSelected()
        {
            //SaveAll();
            Create();
        }

        /// <summary>
        /// Retourne l'Editor géré par ce controller 
        /// </summary>
        /// <returns>L'Editor géré par ce controller</returns>
        public Editor<T> getEditor()
        {
            return (Editor<T>)this.View;
        }

        /// <summary>
        /// Initialisation des Handlers sur une nouvelle page.
        /// </summary>
        protected virtual void initializePageHandlers(EditorItem<T> page) 
        {
            page.Closing += new EventHandler<CancelEventArgs>(OnPageClosing);
            page.Closed += new EventHandler(OnPageClosed);
            page.PageTabDoubleClick += this.PageTabDoubleClick;
        }

        public virtual void PageTabDoubleClick(object sender, MouseButtonEventArgs args)
        { 
          
        }


        protected override void initializePropertyBarData() { }

        protected override void initializePropertyBarHandlers() { }

        /// <summary>
        /// Initialisation des donnée sur la vue.
        /// </summary>
        protected override void initializeViewData() { }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnPageClosing(object sender, CancelEventArgs args)
        {
            if (sender is EditorItem<T>)
            {
                EditorItem<T> page = (EditorItem<T>) sender;
                if(page.IsModify)
                {
                    MessageBoxResult result = Util.MessageDisplayer.DisplayYesNoCancelQuestion("Close Page", 
                        "You are about closing page : " + page.Title +"\nDo you want to save change before close?");
                    if (result == MessageBoxResult.Yes)
                    {
                        OperationState state = Save(page);
                        if (state == OperationState.STOP) args.Cancel = true;
                    }
                    else if (result == MessageBoxResult.Cancel) args.Cancel = true;
                }
                else {
                    MessageBoxResult result = Util.MessageDisplayer.DisplayYesNoQuestion("Close Page",
                        "You are about closing page : " + page.Title + "\nDo you want to continue?");
                    if (result != MessageBoxResult.Yes) args.Cancel = true;
                }

                this.IsModify = this.IsModify;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnPageClosed(object sender, EventArgs args)
        {
            foreach (EditorItem<T> page in getEditor().getPages())
            {
                if (page.IsModify)
                {
                    this.IsModify = true;
                    if (this.ToolBar != null) this.ToolBar.SaveButton.IsEnabled = true;
                    return;
                }
            }
            this.IsModify = false;
            if (this.ToolBar != null) this.ToolBar.SaveButton.IsEnabled = false;
        }

        /// <summary>
        /// Initialisation des Handlers sur la vue.
        /// </summary>
        protected override void initializeViewHandlers()
        {
            getEditor().ActivePageChangedEventHandler = new EventHandler(OnPageSelected);
           /* getEditor().getEditorContextMenu().DeleteItemMenu.Click+= new RoutedEventHandler(DeleteItemMenu_Click);
            getEditor().getEditorContextMenu().RenameItemMenu.Click += new RoutedEventHandler(RenameItemMenu_Click);
            getEditor().getEditorContextMenu().SaveItemMenu.Click += new RoutedEventHandler(SaveItemMenu_Click);
            getEditor().getEditorContextMenu().SaveAsItemMenu.Click += new RoutedEventHandler(SaveAsItemMenu_Click);
            */
        }


        protected override void initializeCommands()
        {
            base.initializeCommands();
            if (ApplicationManager != null)
            {
                //ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, new Separator());
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, DeleteMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, SaveAsMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, SaveMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, RenameMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Insert(0, NewMenuItem);

                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(NewCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(RenameCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(SaveCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(SaveAsCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Add(DeleteCommandBinding);
            }
        }

        /// <summary>
        /// Remove Commands
        /// </summary>
        protected override void RemoveCommands()
        {
            if (ApplicationManager != null)
            {
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RefreshMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(DeleteMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveAsMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(SaveMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(RenameMenuItem);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.Items.Remove(NewMenuItem);

                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(NewCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RenameCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(SaveAsCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(DeleteCommandBinding);
                ApplicationManager.MainWindow.dockingManager.DocumentContextMenu.CommandBindings.Remove(RefreshCommandBinding);
            }
        }


        /*private void SaveAsItemMenu_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SaveItemMenu_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RenameItemMenu_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        */


        public virtual void OnPageSelected(object sender, EventArgs args)
        {
                OnPageSelected(getEditor().getActivePage());
        }

        /// <summary>
        /// Cette méthode est appelée lorsqu'une page est sélectionnée dans l'éditeur.
        /// </summary>
        /// <param name="page"></param>
        public virtual void OnPageSelected(EditorItem<T> page)
        {
            
        }

        #endregion


        protected virtual string getNewPageName(string prefix)
        {
            int i = 1;
            string name = prefix + i;
            bool valid = false;
            while (!valid)
            {
                name = prefix + i;
                T obj = GetObjectByName(name);
                if (obj == null) return name;
                i++;
            }
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual T GetObjectByName(string name)
        {
            foreach (EditorItem<T> item in this.getEditor().getPages())
            {
                if (item.EditedObject != null && item.EditedObject.ToString().ToUpper().Equals(name.ToUpper()))
                {
                    return item.EditedObject;
                }
            }
            return null;
        }


        public RoutedEventHandler DeleteItemMenu_Click { get; set; }
    }
}
