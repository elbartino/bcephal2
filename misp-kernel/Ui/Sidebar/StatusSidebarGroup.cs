using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Misp.Kernel.Ui.Sidebar
{
    public class StatusSidebarGroup : SidebarGroup
    {

        #region Properties

        public static List<string> StatusNames { get; set; }
        public List<StatusSidebarGroupItem> StatusButtons { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusSidebarGroup() : this("Status") { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        public StatusSidebarGroup(string header) : this(header, true) { }

        /// <summary>
        /// Create a new instance of StatusSidebarGroup with title.
        /// </summary>
        /// <param name="header">The title of the group</param>
        /// <param name="expanded"></param>
        public StatusSidebarGroup(string header, bool expanded) : base(header, expanded) { }

        #endregion


        #region Operations

        /// <summary>
        /// Select status
        /// </summary>
        /// <param name="name">Status to select</param>
        public void SelectStatus(String name)
        {
            if (name == null) return;
            foreach (StatusSidebarGroupItem item in this.StatusButtons)
            {
                item.SetSelectedIfEquals(name);
            }
        }

        #endregion


        #region Initializations

        /// <summary>
        /// Perform somes user initializations.
        /// </summary>
        protected override void UserInitializations()
        {
            this.StatusButtons = new List<StatusSidebarGroupItem>(0);
            Brush color = (Brush)new BrushConverter().ConvertFrom("#FFFAC090");
            this.BorderBrush = color;
            this.Background = color;

            foreach (string name in StatusNames)
            {
                StatusSidebarGroupItem item = new StatusSidebarGroupItem();
                item.Label.Content = name;
                this.ContentPanel.Children.Add(item);
                this.StatusButtons.Add(item);
            }
        }

        #endregion


    }
}
