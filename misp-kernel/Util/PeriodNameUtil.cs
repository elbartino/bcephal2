using Misp.Kernel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class PeriodNameUtil
    {
      
        public static List<PeriodInterval> buildIntervals(PeriodName periodName)
        {
            if (periodName == null) return new List<PeriodInterval>(0);
            if (periodName.isYear()) return buildYearPeriods(periodName);
            if (periodName.isMonth()) return buildMonthPeriods(periodName);
            if (periodName.isWeek()) return buildWeekPeriods(periodName);
            if (periodName.isDay()) return buildDayPeriods(periodName);
            return new List<PeriodInterval>(0);
        }

        public static List<PeriodInterval> buildYearPeriods(PeriodName periodName)
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int fromYear = periodName.periodFromTime.Year;
            int toYear = periodName.periodToTime.Year;
            int position = 0;
            DateTime to = periodName.periodToTime;
            DateTime from = periodName.periodFromTime;
            while (from <= to)
            {
                position++;
                string name = "YEAR " + from.Year.ToString();
                int months = from.Month;
                int monthToAdd = 12 * periodName.incrementationCount;
                DateTime localto = from.AddMonths(monthToAdd).AddDays(-1);
                if (localto > to) localto = to;
                periods.Add(new PeriodInterval(position, name, from, localto));
                if (fromYear == toYear) break;
                from = localto.AddDays(1);
            }
            return periods;
        }

        public static List<PeriodInterval> buildMonthPeriods(PeriodName periodName)
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            DateTime to = periodName.periodToTime;
            DateTime from = periodName.periodFromTime;
            PeriodInterval parentPeriod = null;
            List<PeriodInterval> parents = new List<PeriodInterval>(0);
            if (periodName.showYear)
            {
                periods = buildYearPeriods(periodName);
                parents = getLeafs(periods);
            }
            while (from <= to)
            {
                if (periodName.showYear) parentPeriod = getLeafPeriodContaining(parents, from);
                
                position++;                
                string name = "MONTH " + from.Year.ToString() 
                            + " " + ((from.Month < 10 ? "0" : " ") + from.Month)
                            + " " + ((from.Day < 10 ? "0" : "") + from.Day);

                int monthToAdd = periodName.incrementationCount;
                DateTime localto = from.AddMonths(monthToAdd).AddDays(-1);
                if (localto > to) localto = to;
                if (parentPeriod == null) periods.Add(new PeriodInterval(position, name, from, localto));
                else parentPeriod.AddChild(new PeriodInterval(position, name, from, localto));
                from = localto.AddDays(1);
            }
            return periods;
        }

        public static List<PeriodInterval> buildWeekPeriods(PeriodName periodName)
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            DateTime to = periodName.periodToTime;
            DateTime from = periodName.periodFromTime;
            PeriodInterval parentPeriod = null;
            List<PeriodInterval> parents = new List<PeriodInterval>(0);
            if (periodName.showMonth)
            {
                periods = buildMonthPeriods(periodName);
                parents = getLeafs(periods);
            }
            else if (periodName.showYear)
            {
                periods = buildYearPeriods(periodName);
                parents = getLeafs(periods);
            }
            while (from <= to)
            {
                if (periodName.showMonth) parentPeriod = getLeafPeriodContaining(parents, from);
                else if (periodName.showYear) parentPeriod = getLeafPeriodContaining(parents, from);

                position++;
                string name = "WEEK " + from.Year.ToString()
                            + " " + ((from.Month < 10 ? "0" : " ") + from.Month)
                            + " " + ((from.Day < 10 ? "0" : "") + from.Day);

                int dayToAdd = 7 * periodName.incrementationCount;
                DateTime localto = from.AddDays(dayToAdd).AddDays(-1);
                if (localto > to) localto = to;
                if (parentPeriod == null) periods.Add(new PeriodInterval(position, name, from, localto));
                else parentPeriod.AddChild(new PeriodInterval(position, name, from, localto));
                from = localto.AddDays(1);                
            }
            return periods;
        }

        public static List<PeriodInterval> buildDayPeriods(PeriodName periodName)
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            DateTime to = periodName.periodToTime;
            DateTime from = periodName.periodFromTime;
            PeriodInterval parentPeriod = null;
            List<PeriodInterval> parents = new List<PeriodInterval>(0);
            if (periodName.showWeek)
            {
                periods = buildWeekPeriods(periodName);
                parents = getLeafs(periods);
            }
            else if (periodName.showMonth)
            {
                periods = buildMonthPeriods(periodName);
                parents = getLeafs(periods);
            }
            else if (periodName.showYear)
            {
                periods = buildYearPeriods(periodName);
                parents = getLeafs(periods);
            }
            while (from <= to)
            {
                if (periodName.showWeek) parentPeriod = getLeafPeriodContaining(parents, from);
                else if (periodName.showMonth) parentPeriod = getLeafPeriodContaining(parents, from);
                else if (periodName.showYear) parentPeriod = getLeafPeriodContaining(parents, from);

                position++;
                string name = "DAY " + from.Year.ToString()
                            + " " + ((from.Month < 10 ? "0" : " ") + from.Month)
                            + " " + ((from.Day < 10 ? "0" : " ") + from.Day);

                int dayToAdd = periodName.incrementationCount;
                DateTime localto = from.AddDays(dayToAdd).AddDays(-1);
                if (localto > to) localto = to;
                if (parentPeriod == null) periods.Add(new PeriodInterval(position, name, from, localto));
                else parentPeriod.AddChild(new PeriodInterval(position, name, from, localto));
                from = localto.AddDays(1);
            }
            return periods;
        }


        private static PeriodInterval getPeriodByName(List<PeriodInterval> periods, string yearName)
        {
            foreach (PeriodInterval period in periods)
            {
                if (period.name.Equals(yearName)) return period;
                PeriodInterval p = getPeriodByName(period, yearName);
                if (p != null) return p;
            }
            return null;
        }

        private static PeriodInterval getPeriodByName(PeriodInterval interval, string yearName)
        {
            if (interval.name.Equals(yearName)) return interval;
            foreach (PeriodInterval period in interval.childrenListChangeHandler.Items)
            {
                if (period.name.Equals(yearName)) return period;
                PeriodInterval p = getPeriodByName(period, yearName);
                if (p != null) return p;
            }
            return null;
        }

        private static List<PeriodInterval> getLeafs(List<PeriodInterval> periods)
        {
            List<PeriodInterval> leafs = new List<PeriodInterval>(0);
            foreach (PeriodInterval period in periods)
            {
                if (period.IsLeaf) leafs.Add(period);
                else leafs.AddRange(period.Leafs);
            }
            return leafs;
        }

        private static PeriodInterval getLeafPeriodContaining(List<PeriodInterval> leafs, DateTime date)
        {
            PeriodInterval period = null;
            foreach (PeriodInterval leaf in leafs)
            {
                if (leaf.Containts(date))
                {
                    if(period == null) period = leaf;
                    else if (period.Containts(leaf.periodFromDateTime)) period = leaf;
                }
            }
            return period;
        }



    }
}
