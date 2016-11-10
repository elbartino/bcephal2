using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;


namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Cette page représente une page d'un éditeur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EditorItem<T> : LayoutDocument, IEditableView<T> where T : Domain.Persistent
    {

        #region Attributes

        /// <summary>
        /// Le contenu de la page
        /// </summary>
        protected IEditableView<T> editorItemForm;

        #endregion


        public MouseButtonEventHandler PageTabDoubleClick;


        #region Constructors



        /// <summary>
        /// Construit une nouvelle instance de EditorItem.
        /// </summary>
        public EditorItem()
        {
            ListChangeHandler = new Domain.PersistentListChangeHandler<T>();
            initializeForm();            
        }

        /// <summary>
        /// Construit une nouvelle instance de EditorItem
        /// et initialise l'objet à éditer
        /// </summary>
        /// <param name="editedObject">L'objet à éditer</param>
        public EditorItem(T editedObject) : this()
        {
            this.EditedObject = editedObject;
            
        }

        #endregion


        #region Properties

        public TextBox RenameTextBox { get; set; }
        public Util.Dialog RenameDialog { get; set; }
        public Util.Dialog CustomDialog { get; set; }
        public NamePanel namePanel { get; set; }
        /// <summary>
        /// Assigne ou retourne la liste des objets en édition
        /// </summary>
        public Domain.PersistentListChangeHandler<T> ListChangeHandler { get; set; }

        /// <summary>
        /// Assigne ou retourne l'objet en édition
        /// </summary>
        public T EditedObject { get { return editorItemForm.EditedObject; } set { editorItemForm.EditedObject = value; } }

        /// <summary>
        /// Assigne ou retourne la valeur indiquant
        /// qu'une modification est survenue dans la page.
        /// </summary>
        public bool IsModify { get { return editorItemForm.IsModify; } set { editorItemForm.IsModify = value; } }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        #endregion


        #region Operations

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public virtual bool validateEdition() { return editorItemForm.validateEdition(); }

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public virtual void fillObject() { editorItemForm.fillObject(); }

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public virtual void displayObject()
        {
            RemoveChangeHandlers();
            editorItemForm.displayObject();
            AddChangeHandlers();
        }

        #endregion


        #region Initializations

        /// <summary>
        /// UNe nouvelle instance de la form.
        /// </summary>
        /// <returns></returns>
        protected abstract IEditableView<T> getNewEditorItemForm();

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public T getNewObject() { return editorItemForm.getNewObject(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls() { return editorItemForm.getEditableControls(); }
           
        /// <summary>
        /// Initialisation de la form
        /// </summary>
        protected void initializeForm()
        {
            editorItemForm = getNewEditorItemForm();
            this.Content = editorItemForm;
            InitializeRenameField();
            InitializeCustomDialog("");
        }

        public virtual void InitializeRenameField()
        {
            RenameDialog = new Util.Dialog();
            RenameTextBox = new TextBox();
            RenameDialog.Content = RenameTextBox;
            RenameDialog.Height = 23;
            RenameDialog.Width = 90;
            RenameDialog.ResizeMode = System.Windows.ResizeMode.NoResize;
            RenameDialog.WindowStyle = System.Windows.WindowStyle.None;
            RenameDialog.Loaded += RenameDialogLoaded;
            RenameTextBox.KeyDown += RenameDialogKeyDown;
        }

        public virtual void InitializeCustomDialog(String title){
            RenameTextBox = new TextBox();
            namePanel = new NamePanel();
            namePanel.Height =30;
            CustomDialog = new Util.Dialog(title, namePanel);
            CustomDialog.buttonPanel.Visibility = Visibility.Visible;
            CustomDialog.Height = 140;
            CustomDialog.Width = 300;
            namePanel.NameTextBox.Focus();
            namePanel.NameTextBox.SelectAll();
            CustomDialog.ResizeMode = System.Windows.ResizeMode.NoResize;
            CustomDialog.Loaded += CustomDialogLoaded;
            CustomDialog.KeyDown += CustomDialogKeyDown;            
        }


        #endregion


        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        /// <summary>
        /// Initialisation de la form
        /// </summary>
        protected virtual void AddChangeHandlers()
        {
            if (this.ChangeEventHandler != null)
            {
                foreach (object control in getEditableControls())
                {
                    if (control is TextBoxBase) ((TextBoxBase)control).TextChanged += this.ChangeEventHandler.TextChangedEventHandler;
                    else if (control is ToggleButton)
                    {
                        ((ToggleButton)control).Checked += this.ChangeEventHandler.RoutedEventHandler;
                        ((ToggleButton)control).Unchecked += this.ChangeEventHandler.RoutedEventHandler;
                    }
                    else if (control is ListView.PeriodListBox) ((ListView.PeriodListBox)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Selector) ((Selector)control).SelectionChanged += this.ChangeEventHandler.SelectionChangedEventHandler;
                    else if (control is DatePicker) ((DatePicker)control).SelectedDateChanged += this.ChangeEventHandler.EventHandler;
                    else if (control is TreeView.EditableTree) ((TreeView.EditableTree)control).Change += this.ChangeEventHandler.EditableTreeChangeHandler;
                    else if (control is DiagramDesigner.DesignerCanvas) ((DiagramDesigner.DesignerCanvas)control).Changed += this.ChangeEventHandler.DiagramChange;
                    else if (control is Measure.MeasureTreeView) ((Measure.MeasureTreeView)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is TreeView.PeriodicityTreeview) ((TreeView.PeriodicityTreeview)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
              
                    else if (control is Attribute.AttributeTreeView) ((Attribute.AttributeTreeView)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Attribute.AttributeValueTreeView) ((Attribute.AttributeValueTreeView)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Group.GroupTreeView) ((Group.GroupTreeView)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Group.GroupField) ((Group.GroupField)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Office.ISpreadsheet) ((Office.ISpreadsheet)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is IChangeable) ((IChangeable)control).Changed += this.ChangeEventHandler.ChangeEventHandler;

                    else if (control is EditableTree.AttributeEditableTree) ((EditableTree.AttributeEditableTree)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is EditableTree.AttributeValueEditableTree) ((EditableTree.AttributeValueEditableTree)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is EditableTree.MeasureEditableTree) ((EditableTree.MeasureEditableTree)control).Changed += this.ChangeEventHandler.ChangeEventHandler;
                }
            }
        }

        /// <summary>
        /// Initialisation de la form
        /// </summary>
        protected virtual void RemoveChangeHandlers()
        {
            if (this.ChangeEventHandler != null)
            {
                foreach (object control in getEditableControls())
                {
                    if (control is TextBoxBase) ((TextBoxBase)control).TextChanged -= this.ChangeEventHandler.TextChangedEventHandler;
                    else if (control is ToggleButton)
                    {
                        ((ToggleButton)control).Checked -= this.ChangeEventHandler.RoutedEventHandler;
                        ((ToggleButton)control).Unchecked -= this.ChangeEventHandler.RoutedEventHandler;
                    }
                    else if (control is ListView.PeriodListBox) ((ListView.PeriodListBox)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Selector) ((Selector)control).SelectionChanged -= this.ChangeEventHandler.SelectionChangedEventHandler;
                    else if (control is DatePicker) ((DatePicker)control).SelectedDateChanged -= this.ChangeEventHandler.EventHandler;
                    else if (control is TreeView.EditableTree) ((TreeView.EditableTree)control).Change -= this.ChangeEventHandler.EditableTreeChangeHandler;
                    else if (control is DiagramDesigner.DesignerCanvas) ((DiagramDesigner.DesignerCanvas)control).Changed -= this.ChangeEventHandler.DiagramChange;
                    else if (control is Measure.MeasureTreeView) ((Measure.MeasureTreeView)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Attribute.AttributeTreeView) ((Attribute.AttributeTreeView)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Attribute.AttributeValueTreeView) ((Attribute.AttributeValueTreeView)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Group.GroupTreeView) ((Group.GroupTreeView)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Group.GroupField) ((Group.GroupField)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is Office.ISpreadsheet) ((Office.ISpreadsheet)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is IChangeable) ((IChangeable)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                    else if (control is TreeView.PeriodicityTreeview) ((TreeView.PeriodicityTreeview)control).Changed -= this.ChangeEventHandler.ChangeEventHandler;
                 }
            }
        }

        private void RenameDialogKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) RenameDialog.OkButtonClick(null, null);
            if (e.Key == Key.Escape) RenameDialog.CancelButtonClick(null, null);
        }

        private void RenameDialogLoaded(object sender, RoutedEventArgs e)
        {
            RenameTextBox.SelectAll();
            RenameTextBox.Focus();
        }

        private void SaveAsDialogLoaded(object sender, RoutedEventArgs e)
        {
            RenameTextBox.SelectAll();
            RenameTextBox.Focus();
        }

        private void CustomDialogKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) CustomDialog.OkButtonClick(null, null);
            if (e.Key == Key.Escape) CustomDialog.CancelButtonClick(null, null);
        }

        private void CustomDialogLoaded(object sender, RoutedEventArgs e)
        {
            RenameTextBox.SelectAll();
            RenameTextBox.Focus();
        }


        #endregion

    }
}
