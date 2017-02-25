using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Profil : Persistent
    {
        public string name { get; set; }

        public bool active { get; set; }

        public bool visibleInShortcut { get; set; }
        
        public PersistentListChangeHandler<Right> rightsListChangeHandler { get; set; }
        

        public Profil()
        {
            this.rightsListChangeHandler = new PersistentListChangeHandler<Right>();            
        }

        #region Build Rights

        public void AddRight(Right newRight)
        {
            Right right = GetRight(newRight);
            if (right == null)
            {
                rightsListChangeHandler.AddNew(newRight);
            }
        }

        public void RemoveRight(Right oldRight)
        {
            Right right = GetRight(oldRight);
            if (right != null) rightsListChangeHandler.AddDeleted(right);
        }

        public Right GetRight(Right aRight)
        {
            foreach (Right right in rightsListChangeHandler.Items)
            {
                if (right.functionnality != null && right.functionnality.Equals(aRight.functionnality) && right.rightType == aRight.rightType) return right;
            }
            return null;
        }

        #endregion
        
        /// <summary>
        /// toString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        /// <summary>
        /// compare
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Profil)) return 1;
            return this.name.CompareTo(((Profil)obj).name);
        }


        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;

            if (obj is Kernel.Domain.Profil)
            {
                Kernel.Domain.Profil objm = (Kernel.Domain.Profil)obj;
                if (objm.oid.HasValue && this.oid.HasValue)
                {
                    if (objm.oid == this.oid) return true;
                }
                if (objm.name != null && objm.name.Equals(this.name)) return true;
            }
            return false;
        }

        
    }
}
