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
    /// Interaction logic for RightsPabel.xaml
    /// </summary>
    public partial class RightsPabel : ScrollViewer
    {

        public event ChangeEventHandler Changed;
        public List<RightGroup> Groups { get; set; }

        public Domain.Profil Profil { get; set; }

        public RightsPabel()
        {
            this.Groups = new List<RightGroup>(0);
            InitializeComponent();
        }

        public RightsPabel(List<Functionality> functionalities) : this()
        {
            SetFunctionalities(functionalities);
        }

        public void SetFunctionalities(List<Functionality> functionalities)
        {
            foreach (Functionality functionality in functionalities)
            {
                RightGroup group = new RightGroup(functionality);
                group.RightSelected += OnRightSelected;
                Groups.Add(group);
            }
            DisplayGroups();
        }

        public void Display(Domain.Profil profil)
        {
            this.Profil = profil;
            foreach (Right right in this.Profil.rightsListChangeHandler.Items)
            {
                foreach (RightGroup group in Groups)
                {
                    group.Select(right.functionnality);
                }
            }
        }

        protected void DisplayGroups()
        {
            int fieldCount = Groups.Count;
            buildGrid(fieldCount);
            if (fieldCount <= 0) return;
            int n = 0;
            foreach (RightGroup groupField in Groups)
            {
                n++;
                int row = (n <= 2 || (n == 3 && fieldCount > 4)) ? 0 : 1;
                int col = 0;
                if (n == 1 || (n == 3 && fieldCount <= 4) || (n == 4 && fieldCount > 4)) col = 0;
                else if (n == 2 || n == 5) col = 1;
                else if (n == 4 && fieldCount == 4) col = 1;
                else if (n == 3 && fieldCount > 4) col = 3;
                else if (n > 5) col = 3;

                if (n > 6)
                {
                    int rest = n % 3;
                    int dev = (n - rest) / 3;
                    row = rest > 0 ? dev : dev - 1;
                    col = rest > 0 ? rest - 1 : 2;
                }

                int rowSpan = 1;
                int colSpan = 1;
                //if (n == 3 && blockCount == 3) colSpan = 2;
                //if (n == 3 && blockCount > 4) rowSpan = 2;
                AddGroupField(groupField, row, col, rowSpan, colSpan);
            }
        }

        private void AddGroupField(RightGroup block, int row, int col, int rowSpan, int colSpan)
        {
            Grid.SetRow(block, row);
            Grid.SetColumn(block, col);
            Grid.SetRowSpan(block, rowSpan);
            Grid.SetColumnSpan(block, colSpan);
            this.GroupFieldGrid.Children.Add(block);
        }

        private void buildGrid(int groupFieldCount)
        {
            this.GroupFieldGrid.Children.Clear();
            this.GroupFieldGrid.RowDefinitions.Clear();
            this.GroupFieldGrid.ColumnDefinitions.Clear();

            if (groupFieldCount <= 0) return;
            int row = 1;
            int col = 1;
            if (groupFieldCount == 1) { row = 1; col = 1; }
            else if (groupFieldCount == 2) { row = 1; col = 2; }
            else if (groupFieldCount == 3) { row = 2; col = 2; }
            else if (groupFieldCount == 4) { row = 2; col = 2; }
            else if (groupFieldCount == 5) { row = 2; col = 3; }
            else if (groupFieldCount == 6) { row = 2; col = 3; }

            else if (groupFieldCount == 7) { row = 3; col = 3; }
            else if (groupFieldCount == 8) { row = 3; col = 3; }
            else if (groupFieldCount == 9) { row = 3; col = 3; }
            else if (groupFieldCount == 10) { row = 4; col = 3; }
            else if (groupFieldCount == 11) { row = 4; col = 3; }
            else if (groupFieldCount == 12) { row = 4; col = 3; }

            else { row = 5; col = 3; }

            for (int i = 1; i <= row; i++)
            {
                RowDefinition def = new RowDefinition();
                def.Height = new GridLength(200 + 20);
                this.GroupFieldGrid.RowDefinitions.Add(def);
            }

            for (int i = 1; i <= col; i++)
            {
                ColumnDefinition def = new ColumnDefinition();
                def.Width = new GridLength(1, GridUnitType.Star);
                this.GroupFieldGrid.ColumnDefinitions.Add(def);
            }
        }


        private void OnRightSelected(string functionality, bool selected)
        {
            if (this.Profil != null)
            {
                if (selected) this.Profil.AddRight(functionality);
                else this.Profil.RemoveRight(functionality);
            }
            if (Changed != null) Changed();            
        }

    }
}
