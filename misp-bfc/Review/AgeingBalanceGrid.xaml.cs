using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.ConditionalFormatting;
using Misp.Bfc.Model;
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

namespace Misp.Bfc.Review
{
    /// <summary>
    /// Interaction logic for AgeingBalanceGrid.xaml
    /// </summary>
    public partial class AgeingBalanceGrid : GridControl
    {
                
        #region Properties

        
        bool throwHandlers;

        #endregion


        #region Constructors

        public AgeingBalanceGrid()
        {
            //AgeingBalanceGrid.AllowInfiniteGridSize = true;
            this.MaxHeight = 400;            
            InitializeComponent();
            InitializeHandlers();
            throwHandlers = true;
        }

        #endregion


        #region Operations

        public void Display(List<AgeingBalanceData> datas)
        {
            throwHandlers = false;
            this.ItemsSource = datas;            
            throwHandlers = true;
        }

        public void FillFilter(ReviewFilter filter)
        {
            if (filter == null) filter = new ReviewFilter();
            /*if (this.Scheme != null)
            {
                filter.schemeIdOids.Add(this.Scheme.oid.Value);
            }
            filter.startDateTime = this.StartDatePicker.SelectedDate;
            filter.endDateTime = this.EndDatePicker.SelectedDate;*/
        }

        #endregion


        #region Handlers

        private void InitializeHandlers()
        {
            //this.TableView.FormatConditions.Add(new FormatCondition()
            //{                
            //    ApplyToRow = false,
            //    Expression = "[amountType] == 'Balance'",
            //    FieldName = null,
            //    Format = new Format() { Foreground = Brushes.Red }
            //});
            //this.TableView.FormatConditions.Add(new FormatCondition()
            //{
            //    ApplyToRow = true,
            //    Expression = "[amountType] == 'Balance'",
            //    FieldName = null,
            //    Format = new Format() { Background = Brushes.Blue }
            //});
        }

        private void OnCellMerge(object sender, CellMergeEventArgs e)
        {
            if (e.Column.FieldName == "replenishmentInstruction" || e.Column.FieldName == "sentDate"
                || e.Column.FieldName == "valueDate" || e.Column.FieldName == "memberBankName" || e.Column.FieldName == "pml")
            {
                bool canBemerge = SameCellValues("replenishmentInstruction", e.RowHandle1, e.RowHandle2)
                    && SameCellValues("sentDate", e.RowHandle1, e.RowHandle2)
                    && SameCellValues("valueDate", e.RowHandle1, e.RowHandle2)
                    && SameCellValues("memberBankName", e.RowHandle1, e.RowHandle2)
                    && SameCellValues("pml", e.RowHandle1, e.RowHandle2);
                e.Merge = canBemerge;
            }
            else e.Merge = false;
            e.Handled = true;
        }

        private bool SameCellValues(String field, int row1, int row2)
        {
            try
            {
                object value1 = this.GetCellValue(row1, this.Columns[field]);
                object value2 = this.GetCellValue(row2, this.Columns[field]);
                return (value1 == null && value2 == null) || (value1 != null && value2 != null && value1.ToString() == value2.ToString());
            }
            catch (Exception) { }
            return false;
        }


        private void OnCustomCellAppearance(object sender, CustomCellAppearanceEventArgs e)
        {
            if (e.RowSelectionState == SelectionState.None)
            {
                object result = e.ConditionalValue;
                if (e.Property == TextBlock.BackgroundProperty)
                {
                    int n = e.RowHandle / 3;
                    if (n % 2 == 0)
                    {
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAFCFC"));
                    }
                }
                if (e.Property == TextBlock.ForegroundProperty)
                {
                    if (e.Column != null && e.Column.FieldName != "replenishmentInstruction" && e.Column.FieldName != "sentDate"
                        && e.Column.FieldName != "valueDate" && e.Column.FieldName != "memberBankName" && e.Column.FieldName != "pml")
                    {
                        if ((e.RowHandle + 1) % 3 == 0)
                        {
                            result = Brushes.Red;
                        } 
                    }
                }
                e.Result = result;
                e.Handled = true;
            }
        }

        #endregion

        
        
    }
}
