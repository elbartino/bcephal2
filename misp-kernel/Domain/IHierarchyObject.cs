using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Misp.Kernel.Domain
{
    public interface IHierarchyObject : INotifyPropertyChanged, IComparable
    {

        /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        void AddChild(IHierarchyObject child);

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        void UpdateChild(IHierarchyObject child);

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        void RemoveChild(IHierarchyObject child);

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        void ForgetChild(IHierarchyObject child);

        /// <summary>
        /// Définit le parent
        /// </summary>
        /// <param name="parent"></param>
        void SetParent(IHierarchyObject parent);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IHierarchyObject GetParent();

        /// <summary>
        /// Définit la position
        /// </summary>
        /// <param name="position"></param>
        void SetPosition(int position);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetPosition();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IHierarchyObject GetChildByPosition(int position);

        /// <summary>
        /// 
        /// </summary>
        void UpdateParents();

        /// <summary>
        /// 
        /// </summary>
        System.Collections.IList GetItems();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IHierarchyObject GetChildByName(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IHierarchyObject GetCopy();
                
    }
}
