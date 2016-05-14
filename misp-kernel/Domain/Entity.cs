using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class Entity : Target, IComparable
    {

        public Entity()
        {
            this.childrenListChangeHandler = new PersistentListChangeHandler<Entity>();
            this.attributeListChangeHandler = new PersistentListChangeHandler<Attribute>();
        }

        [ScriptIgnore]
        public List<AttributeValue> LeafAttributeValues
        {
            get
            {
                List<AttributeValue> values = new List<AttributeValue>(0);
                foreach (Attribute attribute in attributeListChangeHandler.Items)
                {
                    values.AddRange(attribute.LeafAttributeValues);
                }
                return values;
            }
        }

        [ScriptIgnore]
        public static int ENTITY_NAME_MAX_LENGTH = 40;

        [ScriptIgnore]
        public Model model { get; set; }

        [ScriptIgnore]
        public Entity parent { get; set; }

        [ScriptIgnore]
        public string entityType 
        {
            get {return !this.isObject ? "Value Chains":"Objects";}
        }

        public bool usedToGenerateUniverse { get; set; }

        public PersistentListChangeHandler<Attribute> attributeListChangeHandler { get; set; }

        public PersistentListChangeHandler<Entity> childrenListChangeHandler { get; set; }

        [ScriptIgnore]
        public bool isObject 
        {
            get
            {
                return type != null && type.Equals(Target.Type.OBJECT.ToString());
            }
            set
            { 
                targetType = value ? Target.TargetType.STANDARD_OBJECT.ToString() : Target.TargetType.STANDARD_VC.ToString();
                type = value ? Target.Type.OBJECT.ToString() : Target.Type.VC.ToString();
            }
        }

        [ScriptIgnore]
        public bool isValueChain
        {
            get
            {
                return type != null && type.Equals(Target.Type.VC.ToString());
            }
            set
            {
                targetType = value ? Target.TargetType.STANDARD_VC.ToString() : Target.TargetType.STANDARD_OBJECT.ToString();
                type = value ? Target.Type.VC.ToString() : Target.Type.OBJECT.ToString();
            }
        }

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(Entity child)
        {
            child.parent = this;
            childrenListChangeHandler.AddNew(child);
            UpdateParents();
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(Entity child)
        {
            childrenListChangeHandler.AddUpdated(child);
            UpdateParents();
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(Entity child)
        {
            child.parent = null;
            childrenListChangeHandler.AddDeleted(child);
            UpdateParents();
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetChild(Entity child)
        {            
            childrenListChangeHandler.forget(child);
        }

        public void RefreshAttributeEntity()
        {
            foreach (Attribute attribute in attributeListChangeHandler.Items)
            {
                attribute.entity = this;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void UpdateParents()
        {
            if (this.parent != null)
            {
                this.parent.childrenListChangeHandler.AddUpdated(this);
                this.parent.UpdateParents();
            }
            else if (this.model != null)
            {
                this.model.UpdateEntity(this);
            }
        }
        
        public List<Entity> GetDescendentsTree()
        {
            List<Entity> entities = new List<Entity>(0);            
            foreach(Entity entity in childrenListChangeHandler.Items)
            {
                entities.Add(entity);
                entities.AddRange(entity.GetDescendentsTree());
            }
            return entities;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Entity GetCopy()
        {
            Entity entity = new Entity();
            entity.name = "Copy Of " + this.name;
            entity.isObject = this.isObject;
            entity.position = -1;
            entity.parent = null;
            entity.model = this.model;
            foreach (Attribute attribute in attributeListChangeHandler.Items)
            {
                Attribute copy = (Attribute)attribute.GetCopy();
                if (!copy.IsDefault)
                {
                    copy.position = attribute.position;
                    entity.attributeListChangeHandler.AddNew(copy);
                    copy.entity = entity;
                }
            }            
            return entity;
        }



    }
}
