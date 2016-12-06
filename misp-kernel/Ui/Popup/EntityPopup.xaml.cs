using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Misp.Kernel.Util;
namespace Misp.Kernel.Ui.Popup
{
    /// <summary>
    /// Interaction logic for EntityPopup.xaml
    /// </summary>
    public partial class EntityPopup : UserControl
    {
        public event Base.SelectedItemChangedEventHandler OnValidate;
        private StackPanel stackPanel;
        private CheckBox selectDeselectAllChechBox;
        public bool IsOpen
        {
            get { return myPopup.IsOpen; }
            set { myPopup.IsOpen = value; }
        }

        public List<object> ItemSource { get; set; }

        private List<object> currentList { get; set; }
        
        public List<object> selectedItem;

        public List<string> selectedNames;

        public EntityPopup()
        {
            InitializeComponent();
            this.ascending.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/sort_incr.png", UriKind.Relative)) };
            this.descending.Icon = new Image { Source = new BitmapImage(new Uri("../../Resources/Images/Icons/sort_decrease.png", UriKind.Relative)) };
            
            stackPanel = new StackPanel();
            selectDeselectAllChechBox = new CheckBox();
            selectDeselectAllChechBox.Content = "Select All";
            selectDeselectAllChechBox.Click += OnSelectItem;
            selectDeselectAllChechBox.Margin = new Thickness(0, 5, 5, 5);
         
            this.ItemSource = new List<object>(0);
            selectedItem = new List<object>(0);
            currentList = new List<object>(0);
            selectedNames = new List<string>(0);
        }

        public void FillSelectedNames()
        {
            foreach (object obj in this.selectedItem) 
            {
                this.selectedNames.Add(obj.ToString());
            }

            bool isSelecteAll = selectedNames.Count > 0 && selectedNames.Count == ItemSource.Count;
            selectDeselectAllChechBox.Content = isSelecteAll ? "Deselect All" : "Select All";
            selectDeselectAllChechBox.IsChecked = isSelecteAll;
        }

        public void Display(List<object> liste = null)
        {
            if (liste == null)
            {
                liste = new List<object>(0);
                liste = this.ItemSource;
            }
            if (currentList != null && currentList.Count > 0) liste = currentList;
            stackPanel.Children.Clear();
            stackPanel.Children.Add(selectDeselectAllChechBox);
            
            foreach (object obj in liste)
            {
                CheckBox ch = new CheckBox();
                ch.Click += OnSelectItem;
                ch.Margin = new Thickness(0, 5, 5, 5);
                ch.Tag = obj;
                ch.Content = obj.ToString();
                ch.IsChecked = selectedNames.Contains(obj.ToString());
                stackPanel.Children.Add(ch);
            }
            MainPanel.Content = stackPanel;
        }

        #region Handlers
        private void OnSelectItem(object sender, RoutedEventArgs e)
        {
            selectionAction(sender);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            filterText(sender);
        }

        private void btnClosePopup_Click(object sender, RoutedEventArgs e)
        {
            butonActions(sender);
        }

        private void OnOrdering(object sender, RoutedEventArgs e)
        {
            menuOrdersActions(sender);
        }


        #endregion
       
        /// <summary>
        /// Handler buttons actions 
        /// </summary>
        /// <param name="sender"></param>
        private void butonActions(object sender) 
        {
            //myPopup.IsOpen = false;
            if (sender == okButton)
            {
                if (OnValidate != null) OnValidate(new Object[] { selectedItem,this.Tag });
            }
            else if (sender == cancelButton)
            {
                this.selectedNames.Clear();
                this.selectedItem.Clear();
                this.ItemSource.Clear();
                if (OnValidate != null) OnValidate(null);
            }
            else if (sender == closeButton) 
            {
                //myPopup.IsOpen = false;
            }
            myPopup.Tag = null;
        }

        /// <summary>
        /// Handler Menu actions
        /// </summary>
        /// <param name="sender"></param>
        private void menuOrdersActions(object sender) 
        {
            if (currentList == null || currentList.Count == 0)
                currentList = this.ItemSource;

            if (sender == ascending)
            {
                this.currentList.BubbleSortByName();
            }
            else if (sender == descending)
            {
                this.currentList.BubbleSortByName(false);
            }
            Display(currentList);            
        }

        /// <summary>
        /// Handles Textbox filter actions
        /// </summary>
        /// <param name="sender"></param>
        private void filterText(object sender) 
        {
            currentList = new List<object>(ItemSource);
            string critere = ((TextBox)sender).Text.Trim();
            if (string.IsNullOrEmpty(critere) || critere.Equals("Search...", StringComparison.InvariantCultureIgnoreCase))
            {
                Display();
                return;
            }

            for (int i = currentList.Count - 1; i >= 0; i--)
            {
                object node = currentList[i];
                if (!node.ToString().StartsWith(critere, StringComparison.InvariantCultureIgnoreCase))
                {
                    currentList.Remove(node);
                }
            }
            Display(currentList);
        }

        /// <summary>
        /// Handles Items actions.
        /// </summary>
        /// <param name="sender"></param>
        private void selectionAction(object sender) 
        {
            if (sender is CheckBox)
            {
                if (sender == selectDeselectAllChechBox)
                {
                    selectDeselectAllChechBox.Content = selectDeselectAllChechBox.IsChecked.Value ? "Deselect All" : "Select All";
                    if (selectDeselectAllChechBox.IsChecked.Value)
                    {
                        if (this.currentList != null && this.currentList.Count == 0) this.currentList.AddRange(this.ItemSource);
                        this.selectedItem.AddRange(this.currentList);
                        foreach (CheckBox ch in stackPanel.Children)
                        {
                            ch.IsChecked = true;
                            if (ch.Tag != null)
                            {
                                selectedNames.Add(ch.Tag.ToString());
                            }
                        }
                    }
                    else
                    {
                        this.selectedItem.Clear();
                        this.selectedNames.Clear();
                        foreach (CheckBox ch in stackPanel.Children)
                        {
                            ch.IsChecked = false;

                        }
                    }
                }
                else
                {
                    CheckBox check = (CheckBox)sender;
                    if (check.IsChecked.Value)
                    {
                        this.selectedItem.Add(check.Tag);
                        selectedNames.Add(check.Tag.ToString());
                    }
                    else
                    {
                        if (selectedNames.Contains(check.Tag.ToString()))
                        {
                            for (int i = this.selectedItem.Count - 1; i >= 0; i--)
                            {
                                if (this.selectedItem[i].ToString().Equals(check.Tag.ToString()))
                                {
                                    this.selectedItem.RemoveAt(i);
                                }
                            }
                            selectedNames.Remove(check.Tag.ToString());
                        }
                    }
                }
            }
        }

       

    }
}
