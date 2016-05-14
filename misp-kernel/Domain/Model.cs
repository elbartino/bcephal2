using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class Model : Target
    {
        public enum UniverseGenereStatus { NOT_GENERATED, GENERATED, ONGOING, FAILED }
        
        public Model()
        {
            this.entityListChangeHandler = new PersistentListChangeHandler<Entity>();
            this.type = Target.Type.OBJECT_VC.ToString();
            this.targetType = Target.TargetType.FOLDER_TARGET.ToString();
            this.universeGenerated = UniverseGenereStatus.NOT_GENERATED.ToString();
        }

        public string universeGenerated { get; set; }
        public bool refreshUnivWhenRun { get; set; }

        public string diagramXml { get; set; }

        public PersistentListChangeHandler<Entity> entityListChangeHandler { get; set; }
        
        public void AddEntity(Entity entity)
        {
            entityListChangeHandler.AddNew(entity);
            entity.model = this;
        }
        [ScriptIgnore]
        public string modelFilename { get; set; }


        public void DeleteEntity(Entity entity)
        {
            entityListChangeHandler.AddDeleted(entity);
        }

        public void UpdateEntity(Entity entity)
        {
            entityListChangeHandler.AddUpdated(entity);
        }

        public void ForgetEntity(Entity entity)
        {
            entityListChangeHandler.forget(entity);
        }

        public List<Entity> GetAllEntities()
        {
            List<Entity> entities = new List<Entity>(0);
            foreach(Entity entity in entityListChangeHandler.Items)
            {
                entities.Add(entity);
                if(entity.isObject) entities.AddRange(entity.GetDescendentsTree());
            }
            return entities;
        }


        [ScriptIgnore]
        public System.Windows.Data.CollectionViewSource EntityCollectionViewSource
        {
            get
            {
                System.Windows.Data.CollectionViewSource source = new System.Windows.Data.CollectionViewSource();
                source.Source = entityListChangeHandler.Items;
                source.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("entityType"));
                return source;
            }
        }

        public bool IsUniverseGenerated()
        {
            return universeGenerated != null && universeGenerated == UniverseGenereStatus.GENERATED.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Model GetCopy()
        {
            Model model = new Model();
            model.name = "Copy Of " + this.name;
            model.position = -1;
            foreach (Entity entity in this.entityListChangeHandler.Items)
            {
                Entity copy = entity.GetCopy();
                model.AddEntity(copy);
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Model)) return false;
            return this.name.Equals(((Model)obj).name);
        }
        
    }
}
