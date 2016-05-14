using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misp.Kernel.Ui.Base;
using Misp.Kernel.Domain.Browser;
using System.Windows.Controls;
using Misp.Kernel.Application;
using Xceed.Wpf.AvalonDock.Layout;

namespace Misp.Reconciliation.Posting
{
    public class PostingBrowser : Browser<PostingBrowserData>
    {

        public PostingBrowserForm form;

        protected override void initializeGrid()
        {
            base.initializeGrid();
            this.Children.RemoveAt(this.Children.Count - 1);

            form = new PostingBrowserForm();

            LayoutDocument page = new LayoutDocument();
            page.CanClose = false;
            page.CanFloat = false;
            page.Title = getTitle();
            page.Content = form;
            this.Children.Add(page);
        }

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 7;
        }

        protected override string getTitle() { return "Postings"; }

        /// <summary>
        /// Build and returns the column at index position
        /// </summary>
        /// <param name="index">The position of the column</param>
        /// <returns></returns>
        protected override DataGridColumn getColumnAt(int index)
        {
            switch (index)
            {
                case 0: return new DataGridTextColumn();
                case 1: return new DataGridTextColumn();
                case 2: return new DataGridTextColumn();
                case 3: return new DataGridTextColumn();
                case 4: return new DataGridTextColumn();
                case 5: return new DataGridTextColumn();
                case 6: return new DataGridTextColumn();
                case 7: return new DataGridTextColumn();
                default: return new DataGridTextColumn();
            }
        }


        /// <summary>
        /// Column Label
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getColumnHeaderAt(int index)
        {
            switch (index)
            {
                case 0: return "Posting N°";
                case 1: return "Account";
                case 2: return "Account Name";
                case 3: return "Scheme";
                case 4: return "Date";
                case 5: return "D/C";
                case 6: return "Amount";
                case 7: return "Reco N°";
                default: return "";
            }
        }

        /// <summary>
        /// Column Width
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override DataGridLength getColumnWidthAt(int index)
        {
            switch (index)
            {
                case 0: return 120;
                case 1: return 120;
                case 2: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 3: return 120;
                case 4: return 120;
                case 5: return 50;
                case 6: return 120;
                case 7: return 100;
                default: return 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getBindingNameAt(int index)
        {
            switch (index)
            {
                case 0: return "postingNumber";
                case 1: return "account";
                case 2: return "accountName";
                case 3: return "Scheme";
                case 4: return "date";
                case 5: return "dc";
                case 6: return "amount";
                case 7: return "reconciliationNumber";
                default: return "oid";
            }
        }

        protected override bool isReadOnly(int index)
        {
            switch (index)
            {                
                default: return true;
            }
        }


    }
}
