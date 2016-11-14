using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Misp.Kernel.Ui.Base;
using DiagramDesigner;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;
using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Util;
using Misp.Kernel.Application;

namespace Misp.Initiation.Model
{
    /// <summary>
    /// Interaction logic for ModelForm.xaml
    /// </summary>
    public partial class ModelForm : Grid, IEditableView<Misp.Kernel.Domain.Model>
    {
        
        public ModelForm()
        {
            InitializeComponent();
            InitializeHandlers();
        }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }
        public Kernel.Domain.Attribute defaultValue { get; set; }
        public Kernel.Domain.AttributeValue defaultAttributValue { get; set; }

        public Kernel.Service.ModelService ModelService { get { return ApplicationManager.Instance.ControllerFactory.ServiceFactory.GetModelService(); } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        //private TextChangedEventHandler nameTextChangedEventHandler;
        private KeyEventHandler nameKeyEventHandler;

        /// <summary>
        /// Initialisation des handlers
        /// </summary>
        protected void InitializeHandlers()
        {
            diagramEditor.designerCanvas.SelectionService.SelectionChanged += new SelectionService.SelectionChangedEventHandler(onSelectBlok);
            diagramEditor.designerCanvas.SelectionService.Clear += new SelectionService.ClearEventHandler(onClearSelection);

            diagramEditor.designerCanvas.AddBlock += new DiagramDesigner.AddBlockEventHandler(onAddBlock);
            diagramEditor.designerCanvas.DeleteBlock += new DiagramDesigner.DeleteBlockEventHandler(onDeleteBlock);
            diagramEditor.designerCanvas.ModifyBlock += new DiagramDesigner.ModifyBlockEventHandler(onModifyBlock);
            diagramEditor.designerCanvas.AddLink += new DiagramDesigner.AddLinkEventHandler(onAddLink);
            diagramEditor.designerCanvas.DeleteLink += new DiagramDesigner.DeleteLinkEventHandler(onDeleteLink);

            attributeEditableTree.Changed += onAttributeChange;
            attributeEditableTree.treeView.SelectedItemChanged += OnSelectedAttributeChange;
            attributeValueEditableTree.Changed += onAttributeValueChange;
            attributeValueEditableTree.Expanded += onAttributeValueExpend;
            attributeValueEditableTree.ShowMore += onAttributeValueShowMore;

            nameKeyEventHandler = new KeyEventHandler(onNameTextChange);
            nameTextBox.KeyUp += nameKeyEventHandler;
            nameTextBox.LostFocus += onNameTextBoxLostFocus;
        }

        /// <summary>
        ///
        /// </summary>
        protected void onAttributeChange()
        {
            if (ActiveEntity != null) ActiveEntity.UpdateParents();
            this.IsModify = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void onAttributeValueChange()
        {
            Kernel.Domain.Attribute attribute = attributeEditableTree.GetSelectedValue();
            if (ActiveEntity != null && attribute != null)
            {
                attribute.UpdateParents();
                ActiveEntity.UpdateParents();                
            }
            this.IsModify = true;
        }

        protected void OnSelectedAttributeChange(object sender, RoutedPropertyChangedEventArgs<object> arg)
        {     
            Kernel.Domain.Attribute selection = attributeEditableTree.GetSelectedValue(); ;
            if (selection != null && !selection.isCompleted && selection.oid.HasValue)
            {
                BrowserDataFilter filter = new BrowserDataFilter();
                filter.groupOid = selection.oid.Value;
                filter.page = 1;
                filter.pageSize = 10;
                BrowserDataPage<Kernel.Domain.AttributeValue> page = ModelService.getRootAttributeValuesByAttribute(filter);
                selection.valueListChangeHandler.originalList = page.rows;
                selection.isCompleted = true;
                filter.page = page.currentPage;
                filter.totalPages = page.pageCount;
                selection.Filter = filter;
            }
            attributeValueEditableTree.DisplayAttribute(selection);
        }

        private void onAttributeValueExpend(object item)
        {
            if (item != null && item is Kernel.Domain.AttributeValue)
            {
                Kernel.Domain.AttributeValue selection = (Kernel.Domain.AttributeValue)item;
                if (selection != null && !selection.isCompleted && selection.oid.HasValue)
                {
                    if (selection.Filter == null)
                    {
                        selection.Filter = new BrowserDataFilter();
                        selection.Filter.groupOid = selection.oid.Value;
                        selection.Filter.page = 0;
                        selection.Filter.pageSize = 10;
                    }

                    selection.Filter.page++;
                    BrowserDataPage<Kernel.Domain.AttributeValue> page = ModelService.getAttributeValueChildren(selection.Filter);
                    if (!selection.isCompleted)
                    {
                        foreach (Kernel.Domain.AttributeValue value in selection.childrenListChangeHandler.originalList.ToArray())
                        {
                            selection.childrenListChangeHandler.forget(value);
                        }
                    }
                    foreach(Kernel.Domain.AttributeValue value in page.rows){
                        value.parent = selection;
                        selection.childrenListChangeHandler.Items.Add(value);
                    }
                    selection.childrenListChangeHandler.Items.BubbleSort();
                    selection.isCompleted = true;
                    selection.Filter.page = page.currentPage;
                    selection.Filter.totalPages = page.pageCount;
                }
            }
        }

        private void onAttributeValueShowMore(object item)
        {
            if (item != null && item is Kernel.Domain.AttributeValue)
            {
                Kernel.Domain.AttributeValue selection = (Kernel.Domain.AttributeValue)item;
                Kernel.Domain.AttributeValue parent = selection.parent;
                
                parent.Filter.page++;
                BrowserDataPage<Kernel.Domain.AttributeValue> page = parent.parent != null
                    ? ModelService.getAttributeValueChildren(parent.Filter)
                    : ModelService.getRootAttributeValuesByAttribute(parent.Filter);                
                foreach(Kernel.Domain.AttributeValue value in page.rows){
                    value.parent = parent;
                    parent.childrenListChangeHandler.Items.Add(value);
                }
                parent.childrenListChangeHandler.Items.BubbleSort();
                parent.isCompleted = true;
                parent.Filter.page = page.currentPage;
                parent.Filter.totalPages = page.pageCount;
            }
        }

        

        /// <summary>
        /// Cette méthode est exécuté lorqu'un objet ou une Vc est sélectionné dans le diagram.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onSelectBlok(object sender)
        {
            if (sender is DesignerItem)
            {                
                object tag = ((DesignerItem)sender).Tag;
                if (tag != null && tag is Kernel.Domain.Entity)
                {
                    Kernel.Domain.Entity entity = (Kernel.Domain.Entity)tag;
                    if(!entity.isCompleted && entity.oid.HasValue) {

                        entity.attributeListChangeHandler.originalList = ModelService.getRootAttributesByEntity(entity.oid.Value);
                        entity.isCompleted = true;
                    }
                    DisplayEntity(entity);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void onClearSelection()
        {
            DisplayEntity(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onAddBlock(DesignerItem sender)
        {
            if (this.EditedObject != null && sender.Tag != null)
            {
                JavaScriptSerializer serial = new JavaScriptSerializer();
                Kernel.Domain.Entity entity = null;
                if (sender.Tag is Kernel.Domain.Entity) entity = (Kernel.Domain.Entity)sender.Tag;
                else if (sender.Tag is string) entity = serial.Deserialize<Kernel.Domain.Entity>((string)sender.Tag);
                else return;
                this.EditedObject.AddEntity(entity);
                this.IsModify = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onDeleteBlock(DesignerItem sender)
        {
            if (this.EditedObject != null && sender.Tag != null)
            {
                Kernel.Domain.Entity entity = (Kernel.Domain.Entity)sender.Tag;
                if (entity.parent != null)
                {
                    entity.parent.RemoveChild(entity);
                }
                else this.EditedObject.DeleteEntity(entity);                
                this.IsModify = true;
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onModifyBlock(DesignerItem sender)
        {
            if (this.EditedObject != null && sender.Tag != null)
            {
                Kernel.Domain.Entity entity = (Kernel.Domain.Entity)sender.Tag;
                if (entity.parent != null) entity.parent.UpdateChild(entity);
                else this.EditedObject.UpdateEntity(entity);
                this.IsModify = true;
            }
        }
        
        /// <summary>
        /// Cette methode est appelée lorsau'on a ajouté un lien entre deaux blocks
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        protected void onAddLink(DesignerItem parent, DesignerItem child)
        {
            if (parent.Tag == null || child.Tag == null) return;
            Kernel.Domain.Entity parentTag = (Kernel.Domain.Entity)parent.Tag;
            Kernel.Domain.Entity childTag = (Kernel.Domain.Entity)child.Tag;
            if (parentTag.isValueChain || childTag.isValueChain) return;

            this.EditedObject.ForgetEntity(childTag);
            parentTag.AddChild(childTag);
            this.IsModify = true;
        }

        /// <summary>
        /// Cette methode est appelée lorsau'on a supprimé un lien entre deaux blocks
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        protected void onDeleteLink(DesignerItem parent, DesignerItem child)
        {
            if (parent.Tag == null || child.Tag == null) return;
            Kernel.Domain.Entity parentTag = (Kernel.Domain.Entity)parent.Tag;
            Kernel.Domain.Entity childTag = (Kernel.Domain.Entity)child.Tag;

            parentTag.ForgetChild(childTag);
            childTag.parent = null;
            this.EditedObject.UpdateEntity(childTag);
            this.IsModify = true;
        }


        protected void onNameTextChange(object sender, KeyEventArgs args)
        {
            if(diagramEditor.designerCanvas.SelectionService.CurrentSelection.Count > 0)
            {
                var designerItems = diagramEditor.designerCanvas.SelectionService.CurrentSelection.OfType<DesignerItem>();
                DesignerItem item = designerItems.ElementAt(0);
                if (args.Key == Key.Escape)
                {
                    item.CancelEdition();
                }
                else if (args.Key == Key.Enter)
                {
                    item.Editor.Text = nameTextBox.Text;
                    item.ValidateEdition();
                }                
            }
        }


        protected void onNameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (diagramEditor.designerCanvas.SelectionService.CurrentSelection.Count > 0)
            {
                var designerItems = diagramEditor.designerCanvas.SelectionService.CurrentSelection.OfType<DesignerItem>();
                DesignerItem item = designerItems.ElementAt(0);
                item.Editor.Text = nameTextBox.Text;
                item.ValidateEdition();
            }
        }



        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        public Misp.Kernel.Domain.Model EditedObject { get; set; }

        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        public Misp.Kernel.Domain.Model getNewObject() { return new Misp.Kernel.Domain.Model(); }

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        public bool validateEdition() {
            return true; 
        }
        
        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        public void fillObject() 
        {
            if (this.EditedObject != null)
            {
                this.EditedObject.diagramXml = this.diagramEditor.designerCanvas.AsString();
                this.EditedObject.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
            }
        }
        
        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        public void displayObject()
        {
            if (this.EditedObject == null) return;
            this.diagramEditor.designerCanvas.Model = this.EditedObject;
            this.visibleInShortcutCheckBox.IsChecked = this.EditedObject.visibleInShortcut;
            if (this.EditedObject.diagramXml != null && this.EditedObject.diagramXml.Length > 0)
            {
                this.diagramEditor.designerCanvas.Display(this.EditedObject.diagramXml);
            }
            foreach (Kernel.Domain.Entity entity in this.EditedObject.entityListChangeHandler.Items)
            {
                refreshEntity(entity);                
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        private void refreshEntity(Kernel.Domain.Entity entity)
        {
            entity.model = this.EditedObject;
            entity.RefreshAttributeEntity();
            this.diagramEditor.designerCanvas.RefreshEntity(entity);
            foreach (Kernel.Domain.Entity child in entity.childrenListChangeHandler.Items)
            {
                child.parent = entity;
                refreshEntity(child);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);
            controls.Add(diagramEditor.designerCanvas);
            controls.Add(this.visibleInShortcutCheckBox);
            controls.Add(attributeEditableTree);
            controls.Add(attributeValueEditableTree);
            return controls;
        }

        public Kernel.Domain.Entity ActiveEntity { get; set; }

        public void DisplayEntity(Kernel.Domain.Entity entity)
        {
            nameTextBox.KeyUp -= nameKeyEventHandler;
            ActiveEntity = entity;
            nameTextBox.Clear();
            TypeTextBox.Clear();
            //attributeValueTree.DisplayAttribute(null);
            nameTextBox.IsEnabled = false;
            if (entity != null)
            {
                nameTextBox.IsEnabled = true;
                nameTextBox.Text = entity.name;
                TypeTextBox.Text = entity.isObject ? "Object" : "ValueChain";                
            }
            attributeEditableTree.DisplayEntity(entity);
            nameTextBox.KeyUp += nameKeyEventHandler;
        }

    }
}
