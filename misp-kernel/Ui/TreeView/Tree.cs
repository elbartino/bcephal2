using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Misp.Kernel.Domain;
using System.ComponentModel;
using EO.Wpf;


namespace Misp.Kernel.Ui.TreeView
{
    /// <summary>
    /// Cette classe implémente un arbre éditable pemettant d'effectuer des actions telles que:
    ///     - Ajouter un noeud, 
    ///     - Supprimer un noeud, 
    ///     - Déplacer un noeud vers le haut, le bas, la gauche ou la droite.
    ///     - ...
    /// </summary>
    /// 
    public class Tree : EO.Wpf.TreeView 
    {


        public static EditableTextBlock valeurSelection;


        /// <summary>
        /// Crée une nouvelle instance de EditableTree
        /// </summary>
        public Tree()
        {
             this.Width=200; this.Height=200;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void DisplayRoot(IHierarchyObject root)
        {
            if (root == null) this.ItemsSource = null;
            else
            {
                RefreshParent(root);
                //this.ItemsSource = this.Root.GetItems();
                this.ItemsSource = new string[] { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" };
            }
        }

        private void RefreshParent(IHierarchyObject item)
        {
            if (item != null)
            {
                foreach(IHierarchyObject child in item.GetItems())
                {
                    child.SetParent(item);
                    RefreshParent(child);
                }
            }
        }
        
    }
}
