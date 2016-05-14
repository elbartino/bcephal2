using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Misp.Kernel.Ui.Base
{
    public class SideBar : ScrollViewer
    {

        public static Brush ActiveStatusColor = Brushes.White;
        public static Brush UnActiveStatusColor = Brushes.White;
        public static BitmapImage ActiveStatusImage = new BitmapImage(new Uri("../../Resources/Images/Icons/Check.png", UriKind.Relative));
        public static List<string> StatusNames { get; set; }
        public List<StatusItem> StatusButtons { get; set; }

        public StackPanel MainPanel { get; protected set; }
        public SideBarGroup StatusGroup { get; set; }

        public SideBar()
        {
            this.MainPanel = new StackPanel();
            this.MainPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.Content = this.MainPanel;
            this.StatusButtons = new List<StatusItem>(0);
            InitializeGroups();
        }

        public void AddGroup(SideBarGroup group)
        {
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Add(group);
        }

        public void AddGroup(SideBarExpander group, int position)
        {
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Insert(position, group);
        }

        public void RemoveGroup(int position)
        {
            this.MainPanel.Children.RemoveAt(position);
        }

        public void AddGroup(SideBarExpander group)
        {
            group.Margin = new Thickness(5, 10, 10, 20);
            this.MainPanel.Children.Add(group);
        }

        public void RemoveGroup(SideBarExpander group)
        {
            this.MainPanel.Children.Remove(group);
        }

        public void RemoveGroup(SideBarGroup group) {
            this.MainPanel.Children.Remove(group);
        }

        protected virtual void InitializeGroups()
        {
            InitializeStatusGroup();
        }

        protected void InitializeStatusGroup()
        {
            if (StatusGroup == null)
            {
                StatusGroup = new SideBarGroup();
                StatusGroup.Header = "Status";
                StatusGroup.IsExpanded = true;
                StatusGroup.ContentPanel.Children.Clear();
                this.StatusButtons.Clear();
                foreach (string name in StatusNames)
                {
                    StatusItem item = new StatusItem();
                    item.Label.Content = name;
                    //item.Image;
                    item.Background = UnActiveStatusColor;
                    //item.Height = 25;
                    StatusGroup.ContentPanel.Children.Add(item);
                    this.StatusButtons.Add(item);
                }
                this.AddGroup(StatusGroup);
            }
        }

        public void SelectStatus(String name)
        {
            if (name == null) return;
            foreach (StatusItem item in this.StatusButtons)
            {
                item.Background = UnActiveStatusColor;
                item.Image.Source = null;
                if (name != null && item.Label.Content.Equals(name))
                {                    
                    item.Background = ActiveStatusColor;
                    item.Image.Source = ActiveStatusImage;
                }
            }
        }

        public void Clear() {
            this.MainPanel.Children.Clear();
        }

    }
}
