using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class Granularity
    {

        public static Granularity HOURS = new Granularity("HOURS");

        public static Granularity DAY = new Granularity("DAY");

        public static Granularity WEEK = new Granularity("WEEK");

        public static Granularity MONTH = new Granularity("MONTH");

        public static Granularity YEAR = new Granularity("YEAR");

        public static Granularity SEMESTER = new Granularity("SEMESTER");

        public static Granularity QUATER = new Granularity("QUATER");

        public String name { get; set; }

        private Granularity(String name)
        {
            this.name = name;
	    }
        
        public static Granularity getByName(String name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (HOURS.name.Equals(name)) return HOURS;
            if (DAY.name.Equals(name)) return DAY;
            if (WEEK.name.Equals(name)) return WEEK;
            if (MONTH.name.Equals(name)) return MONTH;
            if (QUATER.name.Equals(name)) return QUATER;
            if (SEMESTER.name.Equals(name)) return SEMESTER;
            if (YEAR.name.Equals(name)) return YEAR;
            return null;
        }

    }
}
