using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class Target : Persistent, INotifyPropertyChanged
    {

        private string _name;
        private int _position;

        public Target()
        {
            targetItemListChangeHandler = new PersistentListChangeHandler<TargetItem>();
            this.cardinality = -1;
            this.visibleInShortcut = true;
            this.defaultData = false;
        }

        public Target(Type type, TargetType targetType) : this()
        {
            this.type = type.ToString();
            this.targetType = targetType.ToString();
        }

        public Target(Type type, TargetType targetType, string name)
            : this(type, targetType)
        {
            this.name = name;
        }

        /** Type definition */
        public enum Type { OBJECT, VC, OBJECT_VC }

        public enum TargetType
        {

            CUSTOMIZED,

            STANDARD_OBJECT,

            STANDARD_VC,

            ALL_UNIVERS,

            EMPTY,

            NULL,

            COMBINED,

            CELL_TARGET,

            FOLDER_TARGET

        }

        public bool defaultData { get; set; }

        public int position
        {
            get { return _position; }

            set
            {
                _position = value;
                this.OnPropertyChanged("position");
            }
        }

        public string name
        {
            get { return _name; }

            set
            {
                _name = value;
                //this.OnPropertyChanged("name");
            }
        }

        public BGroup group { get; set; }

        public string targetType { get; set; }

        public string type { get; set; }

        public bool visibleInShortcut { get; set; }

        public PersistentListChangeHandler<TargetItem> targetItemListChangeHandler { get; set; }

        [ScriptIgnore]
        public long cardinality { get; set; }

        public void buildName()
        {
            string text = "";
            foreach(TargetItem item in targetItemListChangeHandler.Items)
            {
                text += string.IsNullOrEmpty(text.Trim()) ? item.name : " " + item.name;
            }
            this.name = text;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddTargetItem(TargetItem item, bool sort = true)
        {
            item.isModified = true;
            item.position = targetItemListChangeHandler.Items.Count;
            item.parent = this;
            targetItemListChangeHandler.AddNew(item, sort);
            this.cardinality = -1;
            OnPropertyChanged("targetItemListChangeHandler.Items");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void UpdateTargetItem(TargetItem item, bool sort = true)
        {
            item.isModified = true;
            targetItemListChangeHandler.AddUpdated(item, sort);
            this.cardinality = -1;
            OnPropertyChanged("targetItemListChangeHandler.Items");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void RemoveTargetItem(TargetItem item, bool sort = true)
        {
            item.isModified = true;
            targetItemListChangeHandler.AddDeleted(item, sort);
            foreach (TargetItem child in targetItemListChangeHandler.Items)
            {
                if (child.position > item.position) child.position = child.position - 1;
            }
            this.cardinality = -1;
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetTargetItem(TargetItem item, bool sort = true)
        {            
            targetItemListChangeHandler.forget(item, sort);
            foreach (TargetItem child in targetItemListChangeHandler.Items)
            {
                if (child.position > item.position) child.position = child.position - 1;
            }
            item.position = -1;
            this.cardinality = -1;
        }

        /// <summary>
        /// Retourne l'item à la position spécifiée.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public TargetItem GetTargetItem(int position)
        {
            foreach (TargetItem item in targetItemListChangeHandler.Items)
            {
                if (item.position == position) return item;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public void SynchronizeTargetItems(Target scope)
        {
            foreach (TargetItem item in targetItemListChangeHandler.Items)
            {
                TargetItem foundItem = scope.GetTargetItem(item.position);
                if (foundItem == null) { RemoveTargetItem(item); return; }
                item.value = foundItem.value;
                if (item.position != 0) item.operatorType = foundItem.operatorType;
                UpdateTargetItem(item);
            }
            foreach (TargetItem item in scope.targetItemListChangeHandler.Items)
            {
                TargetItem foundItem = this.GetTargetItem(item.position);
                if (foundItem == null) 
                {
                    foundItem = new TargetItem();
                    foundItem.position = item.position;
                    foundItem.value = item.value;
                    if (item.position != 0) foundItem.operatorType = item.operatorType;
                    AddTargetItem(foundItem);
                }
            }
            this.isModified = true;
            buildName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public void SynchronizeTargetItems(TargetItem targetItem)
        {
            TargetItem foundItem = this.GetTargetItem(targetItem.position);
            if (foundItem == null) {
                foundItem = new TargetItem();
                foundItem.value = targetItem.value;
                foundItem.loop = targetItem.loop;
                foundItem.refValueName = targetItem.refValueName;
                foundItem.attribute = targetItem.attribute;
                foundItem.formula = targetItem.formula;
                foundItem.nameSheet = foundItem.nameSheet;
                if (targetItem.position != 0) foundItem.operatorType = targetItem.operatorType;
                AddTargetItem(foundItem); 
            }
            else
            {
                foundItem.value = targetItem.value;
                foundItem.loop = targetItem.loop;
                foundItem.refValueName = targetItem.refValueName;
                foundItem.attribute = targetItem.attribute;
                foundItem.formula = targetItem.formula;
                foundItem.nameSheet = foundItem.nameSheet;
                if (foundItem.position != 0) foundItem.operatorType = targetItem.operatorType;
                UpdateTargetItem(foundItem);
            }
            this.isModified = true;
            buildName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public void SynchronizeDeleteTargetItem(TargetItem targetItem)
        {
            TargetItem foundItem = this.GetTargetItem(targetItem.position);
            if (foundItem == null) return;
            RemoveTargetItem(foundItem);
            this.isModified = true;
            buildName();
        }


        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Target)) return 1;
            return this.position.CompareTo(((Target)obj).position);
        }

        public  bool CopyToClipboard()
        {
            try
            {
                DataFormats.Format format =
                     DataFormats.GetFormat(Kernel.Util.ClipbordUtil.TARGET_VALUE_CLIPBOARD_FORMAT);
                IDataObject dataObj = new DataObject();
                dataObj.SetData(format.Name, false, this);
                Clipboard.SetDataObject(dataObj, false);
            }
            catch (Exception) 
            {
                return false;
            }
            return true;
        }

        public Target GetCopy()
        {
            Target target = new Target();
            target.name = this.name;
            target.type = this.type;
            target.targetType = this.targetType;
            target.position = this.position;
            foreach (TargetItem item in this.targetItemListChangeHandler.Items)
            {
                target.AddTargetItem(item.GetCopy());
            }
            return target;
        }


    }
}
