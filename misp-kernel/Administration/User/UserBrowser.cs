using Misp.Kernel.Domain.Browser;
using Misp.Kernel.Ui.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Misp.Kernel.Administration.User
{
    public class UserBrowser : Browser<UserBrowserData>
    {

        /// <summary>
        /// Column count
        /// </summary>
        /// <returns></returns>
        protected override int getColumnCount()
        {
            return 5;
        }

        protected override string getTitle() { return "Users"; }

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
                case 4: return new DataGridCheckBoxColumn();
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
                case 0: return "Name";
                case 1: return "First Name";
                case 2: return "Profil";
                case 3: return "Creation Date";
                case 4: return "Active";
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
                case 0: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 1: return new DataGridLength(1, DataGridLengthUnitType.Star);
                case 2: return 120;
                case 3: return 120;
                case 4: return 100;
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
                case 0: return "name";
                case 1: return "firstName";
                case 2: return "profil";
                case 3: return "creationDateTime";
                case 4: return "active";
                default: return "oid";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override string getBindingStringFormatAt(int index)
        {
            switch (index)
            {
                case 3: return "{0:dd/MM/yyyy HH:mm:ss}";
                default: return null;
            }
        }

        protected override bool isReadOnly(int index)
        {
            switch (index)
            {
                case 0: return false;
                case 1: return true;
                case 2: return true;
                case 3: return true;
                case 4: return false;
                default: return true;
            }
        }
    }
}
