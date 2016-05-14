using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Ui.Base
{
    /// <summary>
    /// Interface des vues éditables
    /// </summary>
    /// <typeparam name="T">Le type des objets éditables par la vue</typeparam>
    public interface IEditableView<T> : IView
    {

        /// <summary>
        /// Indique si la vue a été modifiée.
        /// </summary>
        bool IsModify { get; set; }

        /// <summary>
        /// L'objet en édition
        /// </summary>
        T EditedObject { get; set; }
        
        /// <summary>
        /// Une nouvelle instance de l'objet éditable.
        /// Cette méthode est appelée par fillObject() si l'objet en édition est null;
        /// </summary>
        /// <returns>Une nouvelle instance de l'objet éditable</returns>
        T getNewObject();

        /// <summary>
        /// Cette méthode permet valider les données éditée.
        /// </summary>
        /// <returns>true si les données sont valides</returns>
        bool validateEdition();

        /// <summary> 
        /// Cette méthode permet de prendre les données éditées à l'écran 
        /// pour les charger dans l'objet en édition.
        /// </summary>
        void fillObject();

        /// <summary>
        /// Cette méthode permet d'afficher les données de l'objet à éditer 
        /// pour les afficher dans la vue.
        /// </summary>
        void displayObject();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        List<object> getEditableControls();



    }
}
