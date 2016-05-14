using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public static class ListUtil
    {

        public static void BubbleSort(this IList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    object o1 = list[j - 1];
                    object o2 = list[j];
                    if (((IComparable)o1).CompareTo(o2) > 0)
                    {
                        list.Remove(o1);
                        list.Insert(j, o1);
                    }
                }
            }
        }

        public static void BubbleSortByName(this IList list,bool asc=true)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    Kernel.Domain.Target o1 = list[j - 1] as Kernel.Domain.Target;
                    Kernel.Domain.Target o2 = list[j] as Kernel.Domain.Target;
                    if (asc)
                    {
                        if (o1.name.CompareTo(o2.name) > 0)
                        {
                            list.Remove(o1);
                            list.Insert(j, o1);
                        }
                    }
                    else
                    {
                        if (o1.name.CompareTo(o2.name) < 0)
                        {
                            list.Remove(o1);
                            list.Insert(j, o1);
                        }
                    }
                }
            }
        }

        public static void BubbleSortDesc(this IList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    object o1 = list[j - 1];
                    object o2 = list[j];
                    if (((IComparable)o1).CompareTo(o2) < 0)
                    {
                        list.Remove(o1);
                        list.Insert(j, o1);
                    }
                }
            }
        }

        /// <summary>
        /// Cette méthode classe par ordre croissant de date de modification 
        /// une liste contenant des objet de types Kernel.Domain.Persistent;
        /// </summary>
        /// <param name="list"></param>
        public static void BubbleSortByDate(this IList list) 
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    Kernel.Domain.Persistent o1 =  list[j - 1] as Kernel.Domain.Persistent;
                    Kernel.Domain.Persistent o2 = list[j] as Kernel.Domain.Persistent;
                    if (o1.modificationDateTime < o2.modificationDateTime)
                    {
                        list.Remove(o1);
                        list.Insert(j, o1);
                    }
                }
            }
        }

        /// <summary>
        /// Cette méthode limite le nombre d'éléments d'une liste
        /// </summary>
        /// <param name="list"></param>
        /// <param name="nombreMax">le nombre maximum de la liste</param>
        public static void Limit(this IList list, int nombreMax=10)
        {

            if (list.Count > nombreMax)
            {
                for (int i = list.Count - 1; i >= nombreMax; i--) 
                {
                    list.Remove(list[i]);
                }
            }
        }

        /// <summary>
        /// Convert a List to an observableCollection. Used to refresh a listBox.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> myList)
        {
            var oc = new ObservableCollection<T>();
            foreach (var item in myList)
                oc.Add(item);
            return oc;
        }

        /// <summary>
        /// Cette méthode classe par ordre croissant de date de modification 
        /// une liste contenant des objet de types Kernel.Domain.AutomaticSourcingSheet;
        /// </summary>
        /// <param name="list"></param>
        public static void BubbleSortSheetList(this IList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    Kernel.Domain.AutomaticSourcingSheet o1 = list[j - 1] as Kernel.Domain.AutomaticSourcingSheet;
                    Kernel.Domain.AutomaticSourcingSheet o2 = list[j] as Kernel.Domain.AutomaticSourcingSheet;
                    if ((o1.position - o2.position) > 0)
                    {
                        list.Remove(o1);
                        list.Insert(j, o1);
                    }
                }
            }
        }

        /// <summary>
        /// Cette méthode classe par ordre croissant de date de modification 
        /// une liste contenant des objet de types Kernel.Domain.AutomaticSourcingColumn;
        /// </summary>
        /// <param name="list"></param>
        public static void BubbleSortColumnAndColumnTargetItem(this IList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    object o1 = list[j - 1];
                    object o2 = list[j];

                    if (o1 is Kernel.Domain.AutomaticSourcingColumn && o2 is Kernel.Domain.AutomaticSourcingColumn)
                    {
                        var oColumn1 = o1 as Kernel.Domain.AutomaticSourcingColumn;
                        var oColumn2 = o2 as Kernel.Domain.AutomaticSourcingColumn;
                        if ((oColumn1.columnIndex - oColumn2.columnIndex) > 0)
                        {
                            list.Remove(oColumn1);
                            list.Insert(j, oColumn1);
                        }

                    }
                    else if (o1 is Kernel.Domain.ColumnTargetItem && o2 is Kernel.Domain.ColumnTargetItem)
                    {
                        var oColumnTarget1 = o1 as Kernel.Domain.ColumnTargetItem;
                        var oColumnTarget2 = o2 as Kernel.Domain.ColumnTargetItem;
                        if ((oColumnTarget1.columnIndex - oColumnTarget2.columnIndex) > 0)
                        {
                            list.Remove(oColumnTarget1);
                            list.Insert(j, oColumnTarget1);
                        }
                    }
                }
            }
        }

    }
}
