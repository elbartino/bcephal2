using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Misp.Kernel.Domain
{
    public class WriteOffFieldValueType
    {
        public static WriteOffFieldValueType LEFT_SIDE = new WriteOffFieldValueType("Left Side Value");
        public static WriteOffFieldValueType RIGHT_SIDE = new WriteOffFieldValueType("Right Side Value");
        public static WriteOffFieldValueType CUSTOM = new WriteOffFieldValueType("Custom Value");
        
        public String label { get; set; }

        private WriteOffFieldValueType(String label)
        {
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static WriteOffFieldValueType getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (LEFT_SIDE.label.Equals(label)) return LEFT_SIDE;
            if (RIGHT_SIDE.label.Equals(label)) return RIGHT_SIDE;
            if (CUSTOM.label.Equals(label)) return CUSTOM;
            return null;
        }
    }
}
