using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Misp.Kernel.Application;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Misp.Kernel.Domain;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Cette classe représente un éditeur pour les objets de type T.
    /// On peut ouvrir plusieurs pages à la fois; une page correspondant à un seul objet.
    /// </summary>
    /// <typeparam name="T">Le type d'objet manager par cet éditeur</typeparam>

    public abstract class Editor<T> : LayoutDocumentPane, IView where T : Domain.Persistent
    {

        public SubjectType SubjectType { get; set; }

        public bool IsReadOnly { get; set; }

        public event NewPageEventHandler NewPageSelected;
        public delegate void NewPageEventHandler();
        public EventHandler newPageEventHandler;

        public EventHandler ActivePageChangedEventHandler { get; set; }

        public EditorItem<T> NewPage { get; set; }

        /// <summary>
        /// Construit une nouvelle instance de Editor
        /// </summary>
        public Editor(SubjectType subjectType)
        {
            this.SubjectType = subjectType;
            ListChangeHandler = new Domain.PersistentListChangeHandler<T>();
            InitializeNewPage();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            this.IsReadOnly = readOnly;
        }

        protected virtual void InitializeNewPage()
        {
            NewPage = getNewPage();
            NewPage.CanClose = false;
            NewPage.CanFloat = false;
            NewPage.Title = "+";

            newPageEventHandler = new EventHandler(this.OnNewPageSelected);
            NewPage.IsActiveChanged += newPageEventHandler;
            
        }

        protected virtual void OnNewPageSelected(object sender, EventArgs args)
        {
            if (NewPageSelected != null) NewPageSelected();
           
        }
        

        /// <summary>
        /// 
        /// </summary>
        public Domain.PersistentListChangeHandler<T> ListChangeHandler { get; set; }

        /// <summary>
        /// Spécifie la méthode à exécuter lorsqu'un changement survient sur la vue.
        /// </summary>
        public ChangeEventHandlerBuilder ChangeEventHandler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeEventHandler"></param>
        public virtual void SetChangeEventHandler(ChangeEventHandlerBuilder ChangeEventHandler)
        {
            this.ChangeEventHandler = ChangeEventHandler;
        }

        
        /// <summary>
        /// Selection la page d'édition d'un objet donné si elle existe.
        /// Si la page n'existe pas, on la crée, on la rajoute aà l'éditeur et on la selectionne.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être créée ou selectionnée</param>
        public EditorItem<T> addOrSelectPage(T anObject, bool readOnly = false) 
        {
            EditorItem<T> page = getPage(anObject);
            if (page == null) page = addPage(anObject, readOnly);
            selectePage(page);
            return page;
        }

        /// <summary>
        /// Selection (active) la page d'édition d'un objet donné.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être selectionnée</param>
        public void selectePage(T anObject) 
        {
            selectePage(getPage(anObject));
        }

        /// <summary>
        /// Selection (active) une page.
        /// </summary>
        /// <param name="page">La page à selectionner</param>
        public void selectePage(EditorItem<T> page)
        {
            if (page == null) return;
            page.IsActive = true;
        }

        /// <summary>
        /// Selection (active) une page sur base de son titre.
        /// </summary>
        /// <param name="title">Le titre de la page à selectionner</param>
        public void selectePage(string title)
        {
            if (string.IsNullOrEmpty(title)) return;
            selectePage(getPage(title));
        }

        /// <summary>
        /// Retourne la page active..
        /// </summary>
        /// <returns>La page active</returns>
        public EditorItem<T> getActivePage()
        {
            foreach (EditorItem<T> page in getPages())
            {
                if (page.IsSelected) return page;
            }
            return null;
        }
         
        
         
      /*  public void changeActivePage( newPage)
        {  
            foreach (EditorItem<T> page in getPages())
            {
                if (page.IsSelected) page.IsSelected = false;
            }
            newPage.IsSelected = true;
        } */

        /// <summary>
        /// Retoune la page de l'éditeur correspondante à un objet.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être retrouvé</param>
        /// <returns>La page d'édition de l'objet ou NULL si une telle page n'existe pas</returns>
        public EditorItem<T> getPage(T anObject) 
        {
            foreach (EditorItem<T> page in getPages())
            {
                if (page.EditedObject != null && page.EditedObject.Equals(anObject)) return page;
            }
            return null;
        }

        /// <summary>
        /// Retoune la page de l'éditeur correspondante à un objet.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être retrouvé</param>
        /// <returns>La page d'édition de l'objet ou NULL si une telle page n'existe pas</returns>
        public EditorItem<T> getPage(int oid)
        {
            foreach (EditorItem<T> page in getPages())
            {
                if (page.EditedObject != null && page.EditedObject.oid != null && page.EditedObject.oid == oid) return page;
            }
            return null;
        }

        /// <summary>
        /// Retoune la page de l'éditeur correspondante à un objet.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être retrouvé</param>
        /// <returns>La page d'édition de l'objet ou NULL si une telle page n'existe pas</returns>
        public EditorItem<T> getPage(string title)
        {
            foreach (EditorItem<T> page in getPages())
            {
                if (page.Title != null && page.Title == title) return page;
            }
            return null;
        }

        /// <summary>
        /// Retoune la page de l'éditeur correspondante à un objet.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être retrouvé</param>
        /// <returns>La page d'édition de l'objet ou NULL si une telle page n'existe pas</returns>
        public EditorItem<T> getPageAndSelect(string title)
        {
            foreach (EditorItem<T> page in getPages())
            {
                if (page.Title != null && page.Title == title)
                {
                    page.IsSelected = true;
                    return page;
                }
            }
            return null;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="anObject"></param>
       /// <returns></returns>
   /*     public EditorItem<T> removePage(EditorItem<T> page)
        {
            page.IsActiveChanged -= ActivePageChangedEventHandler;
            this.Children.Remove(page);
            return page;
        }*/
        /// <summary>
        /// Retourne la liste des pages de l'éditeur.
        /// </summary>
        /// <returns>La liste des pages</returns>
        public List<EditorItem<T>> getPages() 
        {
            List<EditorItem<T>> pages = new List<EditorItem<T>>(0);
            foreach(LayoutContent content in this.Children)
            {
                if (content is EditorItem<T> && NewPage != content) pages.Add((EditorItem<T>)content);
            }
            return pages;
        }


        public List<EditorItem<T>> getAllPages()
        {
            List<EditorItem<T>> pages = new List<EditorItem<T>>(0);
            foreach (LayoutContent content in this.Children)
            {
                pages.Add((EditorItem<T>)content);
            }
            return pages;
        }
        /// <summary>
        /// Crée et rajoute la page d'édition d'un objet donné.
        /// </summary>
        /// <param name="anObject">L'objet dont la page doit être rejoutée</param>
        /// <returns>La page créée</returns>
        protected virtual EditorItem<T> addPage(T anObject, bool readOnly = false)
        {            
           EditorItem<T> page = getNewPage();
           if (readOnly) page.SetReadOnly(readOnly);
           page.ChangeEventHandler = this.ChangeEventHandler;
           page.EditedObject = anObject;
           page.Title = anObject != null ? anObject.ToString() : "";
           page.displayObject();
           page.IsActiveChanged += ActivePageChangedEventHandler;
           bool canAddNewPage = NewPage != null;

           if (canAddNewPage) NewPage.IsActiveChanged -= newPageEventHandler;
               
           try
           {
                if (canAddNewPage) this.Children.Remove(NewPage);
                this.Children.Add(page);
                if (canAddNewPage) this.Children.Add(NewPage);
           }
           catch (Exception)
           {
           }
           page.IsActive = true;
           if (canAddNewPage)  NewPage.IsActiveChanged += newPageEventHandler;
           return page;
        }

        /// <summary>
        /// Retourne une nouvelle page.
        /// </summary>
        /// <returns>Une nouvelle instance de EditorItem</returns>
        protected abstract EditorItem<T> getNewPage();

        /// <summary>
        /// Assigne ou retourne la valeur indiquant
        /// qu'une modification est survenue sur une page de l'éditeur.
        /// </summary>
        public bool IsModify 
        {
            get
            {
                foreach (EditorItem<T> page in getPages())
                {
                    if (page.IsModify) return true;
                }
                return false;
            }

            set 
            {
                if (value)
                {
                    EditorItem<T> page = getActivePage();
                    if (page != null) page.IsModify = value;
                }
                else
                {
                    foreach (EditorItem<T> page in getPages())
                    {
                        page.IsModify = value;
                    }
                }
            }
        }

       /*  public ContextMenu getContextMenu()
           {
            if (ContextMenu != null) return ContextMenu;
            else return new ContextMenu();
           }
        */ 
    }
}
