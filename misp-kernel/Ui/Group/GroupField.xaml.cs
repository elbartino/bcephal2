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
using Misp.Kernel.Service;
using Misp.Kernel.Ui.Base;

namespace Misp.Kernel.Ui.Group
{
    /// <summary>
    /// Interaction logic for GroupField.xaml
    /// </summary>
    public partial class GroupField : StackPanel
    {

        public event ChangeEventHandler Changed;

        /// <summary>
        /// Le GroupService au controller.
        /// </summary>
        public GroupService GroupService { get; set; }

        public  Kernel.Domain.SubjectType subjectType { get; set; }

        private Kernel.Domain.BGroup goup;

        public Kernel.Domain.BGroup Group 
        {
            get {return this.goup;}
            set{
                this.goup = value;
                if (Group != null)
                {
                    Group.name = Group != null && Group.name != null ? Group.name : Kernel.Domain.SubjectType.DEFAULT.label;
                    this.textBox.Text = Group.name;
                }
            }
        }

        public GroupField()
        {
            InitializeComponent();
        }

        protected void OnButtonClick(object sender, EventArgs args)
        {
            GroupPanel panel = new GroupPanel();
            panel.Tree.GroupService = this.GroupService;
            if (this.GroupService == null) return;
            Kernel.Domain.BGroup root = this.GroupService.getRootGroup(subjectType);
            panel.Tree.subjectType = subjectType;
            panel.Tree.DisplayRoot(root);
            if (this.goup != null) panel.Tree.SetSelectedGroup(this.goup.name);
            Dialog dialog = new Dialog("Groups", panel);
            dialog.Height = 200;
            dialog.Width = 300;
            if (dialog.ShowCenteredToMouse().Value)
            {                
                Kernel.Domain.BGroup g = panel.Tree.GetSelectedGroup();
                if (g != null)
                {
                    g = GroupService.getByOid(g.oid.Value);
                    this.Group = g;
                    if (Changed != null) Changed();
                }                
            }
        }


        public void SetReadOnly(bool readOnly)
        {
            this.button.Visibility = readOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;
            this.textBox.IsEnabled = !readOnly;
        }
    }
}
