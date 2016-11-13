using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Ui.TreeView;
using Misp.Kernel.Domain;
using Misp.Kernel.Util;
using System.Collections;
using Misp.Kernel.Ui.Sidebar;

namespace Misp.Kernel.Ui.Base
{
    public class EntityGroup : SidebarGroup
    {

        #region Events
        public  delegate void SelectTarget(Domain.Target target);
        public event SelectTarget OnSelectTarget;

        public delegate void SelectAttributeValue(Domain.AttributeValue value);
        public event SelectAttributeValue OnSelectAttributeValue;

        #endregion

        #region Properties

        public EntityTreeview EntityTreeview { get; set; }

        public Service.ModelService ModelService { get; set; }

        #endregion


        #region Contructors

        /// <summary>
        /// 
        /// </summary>
        public EntityGroup() : base() { }

        public EntityGroup(string header) : base(header) { }
        
        public EntityGroup(string header, bool expanded):base(header,expanded) 
        {
            InitializeHandlers();
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            this.EntityTreeview = new EntityTreeview();
            this.ContentPanel.Children.Add(this.EntityTreeview);
        }

        private void InitializeHandlers()
        {
            this.EntityTreeview.ExpandAttribute += OnExpandeAttribute;
            this.EntityTreeview.SelectionChanged += OnSelectTargetFromSideBar;
            this.EntityTreeview.OnRightClick += onRightClickFromSidebar;
        }

        public void InitializeTreeViewDatas() 
        {
            List<Model> models = ModelService.getModelsForSideBar();
            this.EntityTreeview.DisplayModels(models);
        }

        private void onRightClickFromSidebar(object sender)
        {
            if (sender != null && sender is Kernel.Ui.Popup.EntityPopup)
            {
                Kernel.Ui.Popup.EntityPopup popup = (Kernel.Ui.Popup.EntityPopup)sender;
                popup.OnValidate += OnValidate;
                Kernel.Domain.Attribute attribute = null;

                if (popup.Tag is Kernel.Domain.Attribute)
                {
                    attribute = (Kernel.Domain.Attribute)popup.Tag;
                    popup.selectedItem.Clear();
                    popup.selectedNames.Clear();


                    popup.ItemSource.Clear();
                    List<Kernel.Domain.AttributeValue> values = ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    if (values.Count == 0) return;
                    values.BubbleSortByName();
                    popup.ItemSource.AddRange(values);
                    popup.selectedItem.AddRange(attribute.FilterAttributeValues);
                    popup.FillSelectedNames();
                    popup.Tag = attribute;
                }
                //else if (popup.Tag is Kernel.Domain.AttributeValue) 
                //{
                //    popup.IsChildren = true;
                //    Kernel.Domain.AttributeValue value = (Kernel.Domain.AttributeValue)popup.Tag;
                //    popup.ItemSource.AddRange(value.childrenListChangeHandler.Items);
                //    popup.Tag = value;
                //}
                popup.IsOpen = true;
                popup.Display();
            }
        }

        private void OnValidate(object sender)
        {
            if (sender == null) return;
            if (!(sender is Array)) return;
            object[] senderArray = (object[])sender;
            bool isAttribute;
            Kernel.Domain.Attribute attribute = null;
            Kernel.Domain.AttributeValue value = null;
            List<Kernel.Domain.AttributeValue> listValues = new List<AttributeValue>(0);
            
            isAttribute = senderArray[1] is Kernel.Domain.Attribute;
            if (senderArray[0] is IList && senderArray[1] is Kernel.Domain.Target)
            {
                List<object> liste = (List<object>)senderArray[0];
                listValues.AddRange(liste.Cast<Kernel.Domain.AttributeValue>().ToList());
                attribute = isAttribute ? (Kernel.Domain.Attribute)senderArray[1] : null;
                value = !isAttribute ? (Kernel.Domain.AttributeValue)senderArray[1] : null;
            }

            if (isAttribute) 
            {
                attribute.valueListChangeHandler.Items.Clear();
                attribute.FilterAttributeValues.Clear();
                attribute.FilterAttributeValues.AddRange(listValues);
            }
            else
            {
                attribute.FilterAttributeValues.Clear();
                attribute.FilterAttributeValues.AddRange(listValues);
            }

            foreach (Kernel.Domain.AttributeValue avalue in listValues)
            {
                attribute.valueListChangeHandler.Items.Add(avalue);
            }
        }

        private void OnSelectTargetFromSideBar(object sender)
        {
            if (sender == null) return;
            if (!(sender is Domain.Target)) return; 
            Domain.Target target = (Domain.Target)sender;
            if (OnSelectTarget != null)  OnSelectTarget(target);
            if (OnSelectAttributeValue != null && sender is Domain.AttributeValue) OnSelectAttributeValue((Domain.AttributeValue)target);
        }

        private void OnExpandeAttribute(object sender)
        {
            if (sender != null && sender is Kernel.Domain.Attribute)
            {
                Kernel.Domain.Attribute attribute = (Kernel.Domain.Attribute)sender;
                if (attribute.FilterAttributeValues.Count > 0) return;
                if (!attribute.LoadValues)
                {
                    List<Kernel.Domain.AttributeValue> values = ModelService.getAttributeValuesByAttribute(attribute.oid.Value);
                    attribute.valueListChangeHandler.Items.Clear();
                    foreach (Kernel.Domain.AttributeValue value in values)
                    {
                        attribute.valueListChangeHandler.Items.Add(value);
                    }
                    attribute.LoadValues = true;
                }
            }
        }

        #endregion

    }
}
