using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace Misp.Kernel.Domain
{
    /// <summary>
    /// Une périodicité est un ensemble de périodes.
    /// 
    /// La périodicité est définie par une date de début, une date de fin et un type qui peut être:
    ///  - YEAR : les périodes sont des années
    ///  - SEMESTER : les périodes sont des semestre
    ///  - QUARTER : les périodes sont des trimestres
    ///  - MONTH : les périodes sont des mois
    ///  - DAY : les périodes sont des jours.
    /// </summary>
    public class Periodicity : Persistent
    {
        public static string PERIOD_TYPE_YEAR = "YEAR";
        public static string PERIOD_TYPE_SEMESTER = "SEMESTER";
        public static string PERIOD_TYPE_QUARTER = "QUARTER";
        public static string PERIOD_TYPE_MONTH = "MONTH";
        public static string PERIOD_TYPE_DAY = "DAY";

        public Periodicity() { }


        /// <summary>
        /// La date de début
        /// </summary>
        public string fromDate { get; set; }

        /// <summary>
        /// La date de fin
        /// </summary>
        public string toDate { get; set; }

        /// <summary>
        /// Le type de période:
        /// YEAR, QUARTER, MONTH ou DAY
        /// </summary>
        public string periodType { get; set; }

        /// <summary>
        /// La date de début
        /// </summary>
        [ScriptIgnore]
        public DateTime fromDateTime
        {
            get { return DateTime.Parse(fromDate); }
            set { fromDate = value.ToShortDateString(); }
        }

        /// <summary>
        /// La date de fin
        /// </summary>
        [ScriptIgnore]
        public DateTime toDateTime
        {
            get { return DateTime.Parse(toDate); }
            set { toDate = value.ToShortDateString(); }
        }

        /// <summary>
        /// Indique si cette périodicité est de type YEAR
        /// </summary>
        /// <returns>True si la périodicité est de type YEAR et False sinon</returns>
        public bool isYear() { return !string.IsNullOrWhiteSpace(periodType) && periodType.Equals(PERIOD_TYPE_YEAR); }

        /// <summary>
        /// Indique si cette périodicité est de type Semester
        /// </summary>
        /// <returns>True si la périodicité est de type Semester et False sinon</returns>
        public bool isSemester() { return !string.IsNullOrWhiteSpace(periodType) && periodType.Equals(PERIOD_TYPE_SEMESTER); }

        /// <summary>
        /// Indique si cette périodicité est de type Quarter
        /// </summary>
        /// <returns>True si la périodicité est de type Quarter et False sinon</returns>
        public bool isQuarter() { return !string.IsNullOrWhiteSpace(periodType) && periodType.Equals(PERIOD_TYPE_QUARTER); }

        /// <summary>
        /// Indique si cette périodicité est de type Month
        /// </summary>
        /// <returns>True si la périodicité est de type Month et False sinon</returns>
        public bool isMonth() { return !string.IsNullOrWhiteSpace(periodType) && periodType.Equals(PERIOD_TYPE_MONTH); }

        /// <summary>
        /// Indique si cette périodicité est de type Day
        /// </summary>
        /// <returns>True si la périodicité est de type Day et False sinon</returns>
        public bool isDay() { return !string.IsNullOrWhiteSpace(periodType) && periodType.Equals(PERIOD_TYPE_DAY); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static int GetPeriodCount(DateTime fromDate, DateTime toDate, string periodType)
        {
            if (periodType == Periodicity.PERIOD_TYPE_YEAR) return GetYearCount(fromDate, toDate);
            if (periodType == Periodicity.PERIOD_TYPE_SEMESTER) return GetSemesterCount(fromDate, toDate);
            if (periodType == Periodicity.PERIOD_TYPE_QUARTER) return GetQuarterCount(fromDate, toDate);
            if (periodType == Periodicity.PERIOD_TYPE_MONTH) return GetMonthCount(fromDate, toDate);
            if (periodType == Periodicity.PERIOD_TYPE_DAY) return GetDayCount(fromDate, toDate);
            return 0;
        }

        /// <summary>
        /// Calcule et retourne le nombre d'années comprises entre les deux dates.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetYearCount(DateTime fromDate, DateTime toDate)
        {
            int fromYear = fromDate.Year;
            int toYear = toDate.Year;
            return toYear - fromYear + 1;
        }

        /// <summary>
        /// Calcule et retourne le nombre de semestres compris entre les deux dates.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetSemesterCount(DateTime fromDate, DateTime toDate)
        {
            int fromYear = fromDate.Year;
            int toYear = toDate.Year;
            int yearCount = toYear - fromYear + 1;

            int fromSemester = fromDate.Month <= 6 ? 2 : 1;
            int toSemester = toDate.Month <= 6 ? 1 : 2;

            int semesterCount = yearCount >= 2 ? ((yearCount - 2) * 2) + fromSemester + toSemester
                : yearCount == 1 ? toSemester
                : 0;

            return semesterCount;
        }

        /// <summary>
        /// Calcule et retourne le nombre de trimestres compris entre les deux dates.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetQuarterCount(DateTime fromDate, DateTime toDate)
        {
            int fromYear = fromDate.Year;
            int toYear = toDate.Year;
            int yearCount = toYear - fromYear + 1;

            int fromQuarter = fromDate.Month >= 10 ? 1 : fromDate.Month >= 7 ? 2 : fromDate.Month >= 4 ? 3 : 4;
            int fromQuart = fromDate.Month <= 3 ? 1 : fromDate.Month <= 6 ? 2 : fromDate.Month <= 9 ? 3 : 4;
            int toQuarter = toDate.Month <= 3 ? 1 : toDate.Month <= 6 ? 2 : toDate.Month <= 9 ? 3 : 4;

            int semesterCount = yearCount >= 2 ? ((yearCount - 2) * 4) + fromQuarter + toQuarter
                : yearCount == 1 ? toQuarter - fromQuart + 1
                : 0;

            return semesterCount;
        }

        /// <summary>
        /// Calcule et retourne le nombre de mois compris entre les deux dates.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetMonthCount(DateTime fromDate, DateTime toDate)
        {
            int fromYear = fromDate.Year;
            int toYear = toDate.Year;
            int yearCount = toYear - fromYear + 1;

            int fromMonth = 12 - fromDate.Month + 1;
            int toMonth = toDate.Month;

            int semesterCount = yearCount >= 2 ? ((yearCount - 2) * 12) + fromMonth + toMonth
                : yearCount == 1 ? toMonth - fromDate.Month + 1
                : 0;

            return semesterCount;
        }

        /// <summary>
        /// Calcule et retourne le nombre de jours entre les deux dates.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetDayCount(DateTime fromDate, DateTime toDate)
        {
            int count = 0;
            while (fromDate <= toDate)
            {
                count++;
                fromDate = fromDate.AddDays(1);
            }
            return count;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        public List<PeriodInterval> buildPeriods()
        {
            if (this.isYear()) return buildYearPeriods();
            if (this.isSemester()) return buildSemesterPeriods();
            if (this.isQuarter()) return buildQuarterPeriods();
            if (this.isMonth()) return buildMonthPeriods();
            if (this.isDay()) return buildDayPeriods();
            return new List<PeriodInterval>(0);
        }

        /// <summary>
        /// Construit et retourne la liste des périodes de type YEAR.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        private List<PeriodInterval> buildYearPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int fromYear = fromDateTime.Year;
            int toYear = toDateTime.Year;
            int position = 0;
            while (fromYear <= toYear)
            {
                position++;
                string name = fromYear.ToString();
                DateTime from = new DateTime(fromYear, 1, 1);
                DateTime to = new DateTime(fromYear, 12, 31);
                periods.Add(new PeriodInterval(position, name, from, to));
                fromYear++;
            }
            return periods;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes de type SEMESTER.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        private List<PeriodInterval> buildSemesterPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);

            int fromYear = fromDateTime.Year;
            int toYear = toDateTime.Year;
            int fromSemester = fromDateTime.Month <= 6 ? 1 : 2;
            int toSemester = toDateTime.Month <= 6 ? 1 : 2;

            int position = 0;
            int semesterCount = 2;

            while (fromYear <= toYear)
            {
                if (fromYear == toYear) semesterCount = toSemester;
                while (fromSemester <= semesterCount)
                {
                    position++;
                    string name = "S" + fromSemester + "/" + fromYear.ToString();
                    int fromMonth = fromSemester == 1 ? 1 : 7;
                    int toMonth = fromSemester == 1 ? 6 : 12;
                    DateTime from = new DateTime(fromYear, fromMonth, 1);
                    DateTime to = new DateTime(fromYear, toMonth, toMonth == 6 ? 30 : 31);
                    periods.Add(new PeriodInterval(position, name, from, to));
                    fromSemester++;
                }
                fromSemester = 1;
                fromYear++;
            }
            return periods;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes de type QUARTER.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        private List<PeriodInterval> buildQuarterPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);

            int fromYear = fromDateTime.Year;
            int toYear = toDateTime.Year;
            int fromQuarter = fromDateTime.Month <= 3 ? 1 : fromDateTime.Month <= 6 ? 2 : fromDateTime.Month <= 9 ? 3 : 4;
            int toQuarter = toDateTime.Month <= 3 ? 1 : toDateTime.Month <= 6 ? 2 : toDateTime.Month <= 9 ? 3 : 4;

            int position = 0;
            int quarterCount = 4;

            while (fromYear <= toYear)
            {
                if (fromYear == toYear) quarterCount = toQuarter;
                while (fromQuarter <= quarterCount)
                {
                    position++;
                    string name = "Q" + fromQuarter + "/" + fromYear.ToString();
                    int fromMonth = fromQuarter == 1 ? 1 : fromQuarter == 2 ? 4 : fromQuarter == 3 ? 7 : 10;
                    int toMonth = fromQuarter == 1 ? 3 : fromQuarter == 2 ? 6 : fromQuarter == 3 ? 9 : 12;
                    DateTime from = new DateTime(fromYear, fromMonth, 1);
                    DateTime to = new DateTime(fromYear, toMonth, toMonth == 6 || toMonth == 9 ? 30 : 31);
                    periods.Add(new PeriodInterval(position, name, from, to));
                    fromQuarter++;
                }
                fromQuarter = 1;
                fromYear++;
            }
            return periods;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes de type MONTH.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        private List<PeriodInterval> buildMonthPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            while (fromDateTime <= toDateTime)
            {
                position++;
                fromDateTime = new DateTime(fromDateTime.Year, fromDateTime.Month, 1);
                string name = (fromDateTime.Month < 10 ? "0" : "") + fromDateTime.Month + "/" + fromDateTime.Year;
                periods.Add(new PeriodInterval(position, name, fromDateTime, fromDateTime.AddMonths(1).AddDays(-1)));
                fromDateTime = fromDateTime.AddMonths(1);
            }
            return periods;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes de type DAY.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        private List<PeriodInterval> buildDayPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            while (fromDateTime <= toDateTime)
            {
                position++;
                string name = fromDateTime.ToShortDateString();
                periods.Add(new PeriodInterval(position, name, fromDateTime, fromDateTime));
                fromDateTime = fromDateTime.AddDays(1);
            }
            return periods;
        }

        /// <summary>
        /// Liste les périodes liés à une périodicité donnée.
        /// </summary>
        /// <param name="periodicity">La périodicité</param>
        /// <returns>Les périodes de cette périodicité</returns>
        public List<PeriodInterval> getPeriod(Periodicity periodicity)
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            if (periodicity.periodType == Periodicity.PERIOD_TYPE_YEAR) return buildYearPeriods();
            if (periodicity.periodType == Periodicity.PERIOD_TYPE_SEMESTER) return buildSemesterPeriods();
            if (periodicity.periodType == Periodicity.PERIOD_TYPE_QUARTER) return buildQuarterPeriods();
            if (periodicity.periodType == Periodicity.PERIOD_TYPE_MONTH) return buildMonthPeriods();
            if (periodicity.periodType == Periodicity.PERIOD_TYPE_DAY) return buildDayPeriods();

            return periods;
        }

        /// <summary>
        /// Liste les périodes liés à une périodicité donnée.
        /// </summary>
        /// <param name="periodicity">La périodicité</param>
        /// <returns>Les périodes de cette périodicité</returns>
        public ObservableCollection<PeriodInterval> getHierarchicalPeriod()
        {
            ObservableCollection<PeriodInterval> periods = new ObservableCollection<PeriodInterval>();
            foreach (PeriodInterval year in buildYearPeriods())
            {
                periods.Add(year);
                Periodicity yearPeriodicity = new Periodicity();
                yearPeriodicity.fromDateTime = fromDateTime > year.periodFromDateTime ? fromDateTime : year.periodFromDateTime;
                yearPeriodicity.toDateTime = toDateTime < year.periodToDateTime ? toDateTime : year.periodToDateTime;
                foreach (PeriodInterval semester in yearPeriodicity.buildSemesterPeriods())
                {
                    year.childrens.Add(semester);
                    Periodicity semesterPeriodicity = new Periodicity();
                    semesterPeriodicity.fromDateTime = fromDateTime > semester.periodFromDateTime ? fromDateTime : semester.periodFromDateTime;
                    semesterPeriodicity.toDateTime = toDateTime < semester.periodToDateTime ? toDateTime : semester.periodToDateTime;
                    foreach (PeriodInterval quater in semesterPeriodicity.buildQuarterPeriods())
                    {
                        semester.childrens.Add(quater);
                        Periodicity quaterPeriodicity = new Periodicity();
                        quaterPeriodicity.fromDateTime = fromDateTime > quater.periodFromDateTime ? fromDateTime : quater.periodFromDateTime;
                        quaterPeriodicity.toDateTime = toDateTime < quater.periodToDateTime ? toDateTime : quater.periodToDateTime;
                        foreach (PeriodInterval month in quaterPeriodicity.buildMonthPeriods())
                        {
                            quater.childrens.Add(month);
                            Periodicity monthPeriodicity = new Periodicity();
                            monthPeriodicity.fromDateTime = fromDateTime > month.periodFromDateTime ? fromDateTime : month.periodFromDateTime;
                            monthPeriodicity.toDateTime = toDateTime < month.periodToDateTime ? toDateTime : month.periodToDateTime;

                        }
                    }
                }
            }

            return periods;
        }

    }
}
