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

        public void UpdateFieldValue(WriteOffField fieldValue)
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

        public void SynchronizeDeleteWriteOffField(WriteOffField writeOffField)
        {
            WriteOffField foundItem = this.GetWriteOffField(writeOffField.position);
            if (foundItem == null) return;
            DeleteFieldValue(foundItem);
            this.isModified = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public void SynchronizeDeleteWriteOffConfiguration(WriteOffConfiguration writeOffConfig)
        {
            foreach (WriteOffField item in fieldListChangeHandler.Items)
            {
                WriteOffField foundItem = writeOffConfig.GetWriteOffField(item.position);
                if (foundItem == null) { DeleteFieldValue(item); return; }
                AddFieldValue(item);
            }
            foreach (WriteOffField item in writeOffConfig.fieldListChangeHandler.Items)
            {
                WriteOffField foundItem = this.GetWriteOffField(item.position);
                if (foundItem == null)
                {
                    foundItem = new WriteOffField();
                    foundItem.position = item.position;
                    AddFieldValue(foundItem);
                }
            }
            this.isModified = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        public WriteOffField  SynchronizeWriteOffField(WriteOffField writeOffField)
        {
            WriteOffField foundItem = this.GetWriteOffField(writeOffField.position);
            if (foundItem == null)
            {
                foundItem = new WriteOffField();
                foundItem.mandatory = writeOffField.mandatory;
                foundItem.attributeField = writeOffField.attributeField;
                foundItem.periodField = writeOffField.periodField;
                foundItem.measureField = writeOffField.measureField;
                foundItem.writeOffFieldType = writeOffField.writeOffFieldType;
                if (writeOffField.defaultValueTypeEnum == null && foundItem.isPeriod()) foundItem.defaultValueTypeEnum = WriteOffFieldValueType.TODAY;
                else foundItem.defaultValueTypeEnum = writeOffField.defaultValueTypeEnum;
                foundItem.writeOffFieldValueListChangeHandler = writeOffField.writeOffFieldValueListChangeHandler;
                AddFieldValue(foundItem);
            }
            else
            {
                foundItem.position = writeOffField.position;
                foundItem.mandatory = writeOffField.mandatory;
                foundItem.attributeField = writeOffField.attributeField;
                foundItem.periodField = writeOffField.periodField;
                foundItem.measureField = writeOffField.measureField;
                foundItem.writeOffFieldType = writeOffField.writeOffFieldType;
                foundItem.defaultValueTypeEnum = writeOffField.defaultValueTypeEnum;
                foundItem.writeOffFieldValueListChangeHandler = writeOffField.writeOffFieldValueListChangeHandler;
                UpdateFieldValue(foundItem);
            }
            this.isModified = true;
            return foundItem;
        }

        /// <summary>
        /// Retourne l'item à la position spécifiée.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public WriteOffField GetWriteOffField(int position)
        {
            foreach (WriteOffField item in fieldListChangeHandler.Items)
            {
                if (item.position == position) return item;
            }
            return null;
        }



    }
}
