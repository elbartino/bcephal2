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

            attributeTree.Changed += onAttributeChange;
            attributeValueTree.Changed += onAttributeValueChange;

            nameKeyEventHandler = new KeyEventHandler(onNameTextChange);
            nameTextBox.KeyUp += nameKeyEventHandler;
            nameTextBox.LostFocus += onNameTextBoxLostFocus;
            
            attributeTree.tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(OnSelectedAttributeChange);
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
            Kernel.Domain.Attribute attribute = attributeTree.GetSelectedAttribute();
            if (ActiveEntity != null && attribute != null)
            {
                attribute.UpdateParents();
                ActiveEntity.UpdateParents();                
            }
            this.IsModify = true;
        }

        protected void OnSelectedAttributeChange(object sender, RoutedPropertyChangedEventArgs<object> arg)
        {            
           // Kernel.Domain.Attribute attribute = attributeTree.GetSelectedAttribute() != null ? attributeTree.GetSelectedMultiAttribute():attributeTree.GetSelectedAttribute();       
            if(attributeTree.selectedAttributes.Keys.Count ==1)
            {
                Kernel.Domain.Attribute attribute = attributeTree.selectedAttributes.Keys.Last();
                attributeValueTree.DisplayAttribute(attribute);
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
                    DisplayEntity((Kernel.Domain.Entity)tag);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void onClearSelection()
        {
            DisplayEntity(null);
            attributeTree.selectedAttributes.Clear();
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
            parentTag.attributeListChangeHandler.forget(attributeTree.defaultValue);
            Kernel.Domain.Entity childTag = (Kernel.Domain.Entity)child.Tag;
            childTag.attributeListChangeHandler.forget(attributeTree.defaultValue);
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
                //this.diagramEditor.designerCanvas.Save_Executed(null, null);

                RemoveDefaultInModel(this.EditedObject);
                this.EditedObject.diagramXml = this.diagramEditor.designerCanvas.AsString();
                this.EditedObject.visibleInShortcut = visibleInShortcutCheckBox.IsChecked.Value;
            }
        }
        /// <summary>
        /// Retire les valeurs par défaut présent dans le modèle
        /// </summary>
        /// <param name="model"></param>
        public void RemoveDefaultInModel(Kernel.Domain.Model model)
        {
            foreach (Misp.Kernel.Domain.Entity entity in model.entityListChangeHandler.Items)
            {
                RemoveDefaultAttribute(entity);
            }
        }
        /// <summary>
        /// Retire l'attribut par défaut
        /// </summary>
        /// <param name="Entite"></param>
        public void RemoveDefaultAttribute(Misp.Kernel.Domain.Entity Entite) 
        {

            Entite.attributeListChangeHandler.forget(attributeTree.defaultValue);
            foreach (Misp.Kernel.Domain.Attribute attribute in Entite.attributeListChangeHandler.Items)
            {
                RemoveDefaultAttributeValue(attribute);
            }
            foreach (Misp.Kernel.Domain.Entity entity in Entite.childrenListChangeHandler.Items)
            {
                RemoveDefaultAttribute(entity);
            }
        }
        /// <summary>
        /// Retire l'attribut value par défaut
        /// </summary>
        /// <param name="attribute"></param>
        public void RemoveDefaultAttributeValue(Kernel.Domain.Attribute attribute)
        {
            attribute.valueListChangeHandler.forget(attributeValueTree.defaultValue);
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
            if (!string.IsNullOrEmpty(this.EditedObject.diagramXml)) this.diagramEditor.designerCanvas.Display(this.EditedObject.diagramXml);
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
            controls.Add(attributeTree);
            controls.Add(attributeValueTree);
            return controls;
        }

        public Kernel.Domain.Entity ActiveEntity { get; set; }

        public void DisplayEntity(Kernel.Domain.Entity entity)
        {
            nameTextBox.KeyUp -= nameKeyEventHandler;
            ActiveEntity = entity;
            nameTextBox.Clear();
            TypeTextBox.Clear();
            attributeTree.DisplayEntity(null);
            attributeValueTree.DisplayAttribute(null);
            nameTextBox.IsEnabled = false;
            if (entity != null)
            {
                nameTextBox.IsEnabled = true;
                nameTextBox.Text = entity.name;
                TypeTextBox.Text = entity.isObject ? "Object" : "ValueChain";                
                attributeTree.DisplayEntity(entity);
            }
            nameTextBox.KeyUp += nameKeyEventHandler;
        }

    }
}
