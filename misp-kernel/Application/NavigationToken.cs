using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Application
{
    public class NavigationToken
    {
        /// <summary>
        /// Gets or sets fonctionality short name. 
        /// </summary>
        public string Functionality { get; set; }

        /// <summary>
        /// Gets or sets FunctionalityType. 
        /// </summary>
        public FunctionalityType FunctionalityType { get; set; }

        /// <summary>
        /// Gets or sets type of view to open. 
        /// </summary>
        public ViewType ViewType { get; set; }

        /// <summary>
        /// Gets or sets the EditionMode. 
        /// </summary>
        public EditionMode EditionMode { get; set; }

        /// <summary>
        /// Gets or sets Id of item to edit in case of edition view in read-only or modify mode
        /// </summary>
        public object ItemId { get; set; }

        public List<object> ItemIds = new List<object>(0);

        public string currentActiveFunctionality { get; set; }
        /// <summary>
        /// Initialize
        /// Use this to create a token to navigate to a search view for example 
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <param name="viewType">Type of view to open</param>
        public NavigationToken(string functionality, ViewType viewType)
        {
            this.Functionality = functionality;
            this.ViewType = viewType;
            this.FunctionalityType = FunctionalityType.MAIN_FONCTIONALITY;
        }

        /// <summary>
        /// Initialize
        /// Use this to create a token to navigate to a create view for example
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <param name="viewType">Type of view to open</param>
        /// <param name="mode">Mode of edition</param>
        public NavigationToken(string functionality, ViewType viewType, EditionMode mode)
        {
            this.Functionality = functionality;
            this.ViewType = viewType;
            this.EditionMode = mode;
            this.FunctionalityType = FunctionalityType.MAIN_FONCTIONALITY;
        }

        /// <summary>
        /// Initialize
        /// Use this to create a token to navigate to a edition view for example
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <param name="viewType">Type of view to open</param>
        /// <param name="mode">Mode of edition</param>
        /// <param name="id">Item id</param>
        public NavigationToken(string functionality, ViewType viewType, EditionMode mode, object id)
        {
            this.Functionality = functionality;
            this.ViewType = viewType;
            this.EditionMode = mode;
            this.ItemId = id;
            this.FunctionalityType = FunctionalityType.MAIN_FONCTIONALITY;
        }

        public NavigationToken(string functionality, ViewType viewType, EditionMode mode, List<object> ids)
        {
            this.Functionality = functionality;
            this.ViewType = viewType;
            this.EditionMode = mode;
            if (ids != null && ids.Count > 0)
            {
                this.ItemId = ids[0]; this.ItemIds = ids;
            }
            this.FunctionalityType = FunctionalityType.MAIN_FONCTIONALITY;
        }


        /// <summary>
        /// History tag
        /// </summary>
        /// <returns>String history tag with format <activity>;<FunctionalityType>;<viewType>;[<editionMode>;[<itemId>]]</returns>
        public string GetTag()
        {
            string tag = this.Functionality + ";" + this.FunctionalityType + ";" + this.ViewType;
            tag = tag + ";" + (this.EditionMode == EditionMode.CREATE ? EditionMode.CREATE : this.EditionMode == EditionMode.MODIFY ? EditionMode.MODIFY : EditionMode.READ_ONLY);
            if (this.ItemId != null)
            {
                tag = tag + ";" + this.ItemId;
            }
            return tag;
        }



        /// <summary>
        /// Navigation token for the search view of given activity
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <returns>Navigation token for the search view of given activity</returns>
        public static NavigationToken GetSearchViewToken(string functionality)
        {
            return new NavigationToken(functionality, ViewType.SEARCH);
        }

        /// <summary>
        /// Navigation token for the edition view of given activity in modify mode
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <param name="id">Item id</param>
        /// <returns>Navigation token for the edition view of given activity in modify mode</returns>
        public static NavigationToken GetModifyViewToken(string functionality, object itemId)
        {
            NavigationToken token = new NavigationToken(functionality, ViewType.EDITION, EditionMode.MODIFY, itemId);
            return token;
        }

        public static NavigationToken GetModifyViewToken(string functionality, List<object> itemIds)
        {
            NavigationToken token = new NavigationToken(functionality, ViewType.EDITION, EditionMode.MODIFY, itemIds);
            return token;
        }

        /// <summary>
        /// Navigation token for the Edition view of given activity in read-only mode
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <param name="id">Item id</param>
        /// <returns>Navigation token for the Edition view of given activity in read-only mode</returns>
        public static NavigationToken GetReadOnlyToken(string functionality, object itemId)
        {
            NavigationToken token = new NavigationToken(functionality, ViewType.EDITION, EditionMode.READ_ONLY, itemId);
            return token;
        }

        /// <summary>
        /// Navigation token for the Edition view of given activity in modify mode
        /// </summary>
        /// <param name="functionality">Fonctionality short name</param>
        /// <returns>Navigation token for the Edition view of given activity in modify mode</returns>
        public static NavigationToken GetCreateViewToken(string functionality)
        {
            NavigationToken token = new NavigationToken(functionality, ViewType.EDITION, EditionMode.CREATE);
            return token;
        }


    }
}
