using Misp.Kernel.Domain;
using Misp.Kernel.Service;
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

        public ChangeEventHandler ChangeEventHandler;
       
        public List<Domain.Profil> allProfils;

        public List<Domain.Profil> useProfils;

        public int objectOid;

        public ProfilService profilService;

        public UserService userService;


        #endregion

        #region Constructors
        public UserRightPanel()
        {
            InitializeComponent();
            initHandlers();
        }
        #endregion

        #region Handlers
        protected void initHandlers()
        {
            this.CommentPopup.Opened += OnCommentPopupOpened;
            this.NoCommentButton.Checked += OnComment;
        }
        #endregion

        
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
        public void Display(List<Domain.Profil> profilList)
        {
            useProfils = profilList;
            if (this.IsReadOnly) SetReadOnly(this.IsReadOnly);
            this.panel.Children.Clear();
            int index = 1;
            if (useProfils.Count == 0)
            {
                this.ActiveItemPanel = new UserRightItemPanel(index);
                AddItemPanel(this.ActiveItemPanel);
                return;
            }
            foreach (Domain.Profil item in useProfils)
            {
                UserRightItemPanel itemPanel = new UserRightItemPanel();
                itemPanel.Index = index;
                AddItemPanel(itemPanel);
                itemPanel.objectOid = objectOid;
                itemPanel.Display(item);
                index++;
            }
            
            //this.ActiveItemPanel = new UserRightItemPanel(index);
            //AddItemPanel(this.ActiveItemPanel);
        }

        public void InitService(ProfilService profilServic, int? objetOid)
        {
            profilService = profilServic;
            if (objetOid != null) objectOid = (int)objetOid;
            allProfils = profilService.getAll();
            Display(new List<Domain.Profil>());
        }

        private List<Domain.Profil> unUseProfilList()
        {
            List<Domain.Profil> items = new List<Domain.Profil>();
            foreach (Domain.Profil item in allProfils)
            {
                if (!useProfils.Contains(item)) items.Add(item);
            }
            return items;
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
            itemPanel.objectOid = objectOid;
            itemPanel.Added += OnAdded;
            itemPanel.Updated += OnUpdated;
            itemPanel.Deleted += OnDeleted;
            itemPanel.Activated += OnActivated;
            itemPanel.FillProfil(unUseProfilList());
            //itemPanel.FillProfil(allProfils);
            this.ActiveItemPanel = itemPanel;
            itemPanel.RightSelected += OnRightSelected;
            itemPanel.ChangeEventHandler += onUserRightValueChange;
            this.panel.Children.Add(itemPanel);
        }

        private void OnActivated(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            if (this.ActiveItemPanel != panel)
            {
                this.ActiveItemPanel = panel;
                this.ActiveItemPanel.objectOid = objectOid;
                List<Domain.Profil> items = unUseProfilList();
                if (panel.profil != null && panel.profil.oid != null) items.Add(panel.profil);
                if (ItemChanged != null && panel.profil != null) ItemChanged(panel.profil);
                this.ActiveItemPanel.FillProfil(items);
            }
        }

        private void OnRightSelected(Right right, bool selected)
        {
            if (this.ActiveItemPanel.profil != null)
            {
                if (selected) this.ActiveItemPanel.profil.AddRight(right);
                else this.ActiveItemPanel.profil.RemoveRight(right);
            }
            if (Changed != null) Changed();
        }

        private void OnDeleted(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            this.panel.Children.Remove(panel);
            if (panel.profil != null)
            {
                List<Domain.Right> deletedRight = panel.deleteRightValue(panel.profil);
                foreach (Domain.Right r in deletedRight)
                {
                    panel.profil.RemoveRight(r);
                }
                useProfils.Remove(panel.profil);
            }

            if (this.panel.Children.Count == 0)
            {
                OnAdded(null);
                return;
            }

            if (useProfils.Count >= 1)
            {
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
                OnChanged(panel.profil);
            }
        }

        bool updated = false;
        private void OnUpdated(object item)
        {
            UserRightItemPanel panel = (UserRightItemPanel)item;
            Domain.Profil pf = null;
            if(panel.ProfilComboBox.SelectionBoxItem is Domain.Profil) pf = (Domain.Profil)panel.ProfilComboBox.SelectionBoxItem;
            if (panel.profil.oid != null && !useProfils.Contains(panel.profil))
            {
                useProfils.Remove(pf);
                useProfils.Add(panel.profil);
            }
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
            panel.objectOid = objectOid;
            if (panel.profil.oid != null && !useProfils.Contains(panel.profil)) useProfils.Add(panel.profil);
            updated = false;
            OnChanged(panel.profil);
        }


        public void customiseViewForGrid()
        {
            
        }

        public void customiseViewForTable()
        {
            
        }

        public void customiseViewForTree()
        {

        }

        #endregion

        #region Control
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

        private void onUserRightValueChange()
        {
            if (ChangeEventHandler != null) ChangeEventHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>La liste des controls éditables</returns>
        public List<object> getEditableControls()
        {
            List<object> controls = new List<object>(0);

            return controls;

        }

        #endregion

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
