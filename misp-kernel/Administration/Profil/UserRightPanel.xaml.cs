using Misp.Kernel.Domain;
using Misp.Kernel.Ui.Base;
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

namespace Misp.Kernel.Administration.Profil
{
    /// <summary>
    /// Interaction logic for UserRightPanel.xaml
    /// </summary>
    public partial class UserRightPanel : Grid
    {
        
        public bool IsReadOnly { get; set; }

        #region Events
        public ChangeEventHandler Changed;

        public event ChangeItemEventHandler ItemChanged;

        public event DeleteEventHandler ItemDeleted;

        #endregion
        #region Properties
        public UserRightItemPanel ActiveItemPanel { get; set; }

        public PersistentListChangeHandler<Domain.Profil> profilRightsListChangeHandler { get; set; }

        public List<Domain.Profil> allProfils;

        #endregion

        #region Constructors
        public UserRightPanel()
        {
            InitializeComponent();
            initHandlers();
            profilRightsListChangeHandler = new PersistentListChangeHandler<Domain.Profil>();
        }
        #endregion


        #region Handlers
        protected void initHandlers()
        {
            this.CommentPopup.Opened += OnCommentPopupOpened;
            this.NoCommentButton.Checked += OnComment;

        }
        #endregion

        private void OnComment(object sender, RoutedEventArgs e)
        {
            this.CommentPopup.IsOpen = true;
        }

        private void OnCommentPopupOpened(object sender, EventArgs e)
        {            
            string text = "Right\n"
                          + "V : View\n"
                          + "ET : Edit Table\n"
                          + "EC : Edit Cell\n"
                          + "EA : Edit Allocation\n"
                          + "D  : Delete\n"
                          + "L  : Load\n"
                          + "C  : Clear\n"
                          + "S  : Save As\n";
            this.CommentTextBlock.Text = text;
            this.CommentTextBlock.IsEnabled = false;
        }

        private void refreshCommentIcon()
        {
            bool hasComment = !string.IsNullOrWhiteSpace(this.CommentTextBlock.Text);
            this.NoCommentButton.Visibility = hasComment ? Visibility.Hidden : Visibility.Visible;
        }

        #region Operations
        /// <summary>
        /// affiche le UserRightItemPanel en edition
        /// </summary>
        /// <param name="table"></param>
        public void Display(PersistentListChangeHandler<Domain.Profil> profilList)
        {
            profilRightsListChangeHandler = profilList;
            if (this.IsReadOnly) SetReadOnly(this.IsReadOnly);
            this.panel.Children.Clear();
            int index = 1;
            if (profilRightsListChangeHandler.Items.Count == 0)
            {
                this.ActiveItemPanel = new UserRightItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                return;
            }
            foreach (Domain.Profil item in profilRightsListChangeHandler.Items)
            {
                UserRightItemPanel itemPanel = new UserRightItemPanel();
                itemPanel.Index = index;
                AddItemPanel(itemPanel);
                itemPanel.Display(item);
                index++;
            }

            //this.ActiveItemPanel = new UserRightItemPanel(index);
            //AddItemPanel(this.ActiveItemPanel);
        }

        /// <summary>
        /// Définit la valeur du profil en cour d'édition
        /// et affiche cette valeur dans le TextBox
        /// </summary>
        /// <param name="value">La valeur du profil en cour d'édition</param>
        public bool SetProfilRightItemValue(Domain.Profil value)
        {
            if (this.ActiveItemPanel == null) this.ActiveItemPanel = (UserRightItemPanel)this.panel.Children[this.panel.Children.Count - 1];
            return true;
        }


        protected void AddItemPanel(UserRightItemPanel itemPanel)
        {
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.FillProfil(allProfils);
            this.panel.Children.Add(itemPanel);
        }

        private void OnActivated(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            if (this.ActiveItemPanel != panel)
            {
                this.ActiveItemPanel = panel;
                if (ItemChanged != null && panel.profil != null) ItemChanged(panel.profil);
            }
        }

        private void OnDeleted(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            this.panel.Children.Remove(panel);
            if (this.panel.Children.Count == 0)
            {
                OnAdded(null);
                return;
            }
            if (panel.profil != null)
            {
                if (profilRightsListChangeHandler.Items.Count > 1)
                {
                    
                    if (ItemDeleted != null && panel.profil != null) ItemDeleted(panel.profil);


                    if (this.ActiveItemPanel != null && this.ActiveItemPanel == panel)
                        this.ActiveItemPanel = (UserRightItemPanel)this.panel.Children[this.panel.Children.Count - 1];
                    int index = 1;
                    int j = 0;
                    for (int i = this.panel.Children.Count - 1; i >= 0; i--)
                    {
                        UserRightItemPanel pan = this.panel.Children[j] as UserRightItemPanel;
                        pan.Index = index++;
                        j++;
                    }
                    if (Changed != null) Changed();
                }
            }
        }

        bool updated = false;
        private void OnUpdated(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            profilRightsListChangeHandler.AddUpdated(panel.profil);
            updated = true;
            OnChanged(panel.profil); 
        }

        private void OnAdded(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            if (panel == null) panel = new UserRightItemPanel(1);
            if (panel.profil == null)
            {
                panel.profil = new Domain.Profil();
                panel.profil.name = "1";
            }
            this.profilRightsListChangeHandler.AddNew(panel.profil);
            updated = false;
            OnChanged(panel.profil);
        }
        #endregion

        private void OnChanged(object item)
        {
            int count = this.panel.Children.Count;
            if (!updated)
            {
                this.ActiveItemPanel = new UserRightItemPanel(count + 1);
                AddItemPanel(this.ActiveItemPanel);
            }
            if (Changed != null) Changed();
            if (ItemChanged != null && item != null) ItemChanged(item);
        }

        
        public void SetReadOnly(bool readOnly) 
        {
            
        }

        public void Fill()
        {
            
            
        }

        public void setValue(object value)
        {
           
        }

        public void resetComponent()
        {
            
        }
    }
}
