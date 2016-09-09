using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataGridFilterLibrary.Support;
using System.Collections;

namespace DataGridFilterLibrary.Querying
{
    public class QueryControllerFactory
    {
        public static QueryController 
            GetQueryController(
            System.Windows.Controls.DataGrid dataGrid,
            FilterData filterData, IEnumerable itemsSource)
        {
            QueryController query;

            query = DataGridExtensions.GetDataGridFilterQueryController(dataGrid);

            if (query == null)
            {
                query = new QueryController();
                DataGridExtensions.SetDataGridFilterQueryController(dataGrid, query);
            }

            query.ColumnFilterData        = filterData;
            query.ItemsSource             = itemsSource;
            query.CallingThreadDispatcher = dataGrid.Dispatcher;
            query.UseBackgroundWorker     = DataGridExtensions.GetUseBackgroundWorkerForFiltering(dataGrid);

            return query;
        }
    }
}
