using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class WriteOffConfiguration : Persistent
    {
        public PersistentListChangeHandler<WriteOffField> fieldListChangeHandler;

   
        public WriteOffConfiguration()
        {
            fieldListChangeHandler = new PersistentListChangeHandler<WriteOffField>();
        }

        public void AddFieldValue(WriteOffField field)
        {
            field.position = fieldListChangeHandler.Items.Count;
            fieldListChangeHandler.AddNew(field);
        }

        public void UpdateField(WriteOffField fieldValue)
        {
            fieldListChangeHandler.AddUpdated(fieldValue);
        }

        public void DeleteFieldValue(WriteOffField field)
        {
            field.position = -1;
            fieldListChangeHandler.AddDeleted(field);
            foreach (WriteOffField fvalue in fieldListChangeHandler.Items)
            {
                if (fvalue.position > field.position) fvalue.position = fvalue.position - 1;
            }
        }


        public void ForgetFieldValue(WriteOffField field)
        {
            fieldListChangeHandler.forget(field);
            foreach (WriteOffField fvalue in fieldListChangeHandler.Items)
            {
                if (fvalue.position > field.position) fvalue.position = fvalue.position - 1;
            }
            field.position = -1;
        }

    }
}
