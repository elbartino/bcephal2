using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Domain
{
    public class WriteOffFieldValueType
    {
        public static String LEFT_SIDE_NAME = "LEFT_SIDE";
        public static String RIGHT_SIDE_NAME = "RIGHT_SIDE";
        public static String CUSTOM_NAME = "CUSTOM";
        public static String CUSTOM_DATE_NAME = "CUSTOM_DATE";
        public static String TODAY_NAME = "TODAY";
        
        public static WriteOffFieldValueType LEFT_SIDE = new WriteOffFieldValueType(LEFT_SIDE_NAME, "Left Side Value");
        public static WriteOffFieldValueType RIGHT_SIDE = new WriteOffFieldValueType(RIGHT_SIDE_NAME, "Right Side Value");
        public static WriteOffFieldValueType CUSTOM = new WriteOffFieldValueType(CUSTOM_NAME, "Custom Value");
        public static WriteOffFieldValueType TODAY = new WriteOffFieldValueType(TODAY_NAME, "Today");
        public static WriteOffFieldValueType CUSTOM_DATE = new WriteOffFieldValueType(CUSTOM_DATE_NAME, "Custom Date");
        
        public String label { get; set; }

        public String name { get; set; }

        private WriteOffFieldValueType(String name, String label)
        {
            this.name = name;
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static WriteOffFieldValueType getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (LEFT_SIDE.name.Equals(name)) return LEFT_SIDE;
            if (RIGHT_SIDE.name.Equals(name)) return RIGHT_SIDE;
            if (CUSTOM.name.Equals(name)) return CUSTOM;
            if (CUSTOM_DATE.name.Equals(name)) return CUSTOM_DATE;
            if (TODAY.name.Equals(name)) return TODAY;
            return null;
        }

        public static WriteOffFieldValueType getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (LEFT_SIDE.label.Equals(label)) return LEFT_SIDE;
            if (RIGHT_SIDE.label.Equals(label)) return RIGHT_SIDE;
            if (CUSTOM.label.Equals(label)) return CUSTOM;
            if (CUSTOM_DATE.label.Equals(label)) return CUSTOM_DATE;
            if (TODAY.label.Equals(label)) return TODAY;
            return null;
        }
    }
}
