using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    [Serializable]
    public class PeriodName : Persistent, IHierarchyObject, INotifyPropertyChanged
    {

        private string _name;
        
        public PeriodName()
        {
            intervalListChangeHandler = new PersistentListChangeHandler<PeriodInterval>();
            childrenListChangeHandler = new PersistentListChangeHandler<PeriodName>();
            IsDefault = false;
            this.defaultData = false;
        }

        public PeriodName(string name):this() { this.name = name; } 

        public PeriodName(string name, bool isDefault) : this(name)
        {
            this.iDateDefault = isDefault;
        }

        public bool defaultData { get; set; }

        public string name
        {
            get { return _name; }

            set
            {
                _name = value;
                this.OnPropertyChanged("name");
            }
        }
        
        public int position { get; set; }

        public bool iDateDefault { get; set; }

        public bool showYear { get; set; }

        public bool showMonth { get; set; }

        public bool showWeek { get; set; }

        public bool showDay { get; set; }

        [ScriptIgnore]
        public string curentIntervalGroupName { get; set; }

        /// <summary>
        /// La date de début
        /// </summary>
        public string periodFrom { get; set; }
        /// <summary>
        /// La date de fin
        /// </summary>
        public string periodTo { get; set; }

        /// <summary>
        /// La date de début
        /// </summary>
        [ScriptIgnore]
        public DateTime periodFromTime
        {
            get
            {
                if (periodFrom != null) return DateTime.Parse(periodFrom);
                else return new DateTime();
            }
            set { periodFrom = value.ToShortDateString(); }
        }

        /// <summary>
        /// La date de fin
        /// </summary>
        [ScriptIgnore]
        public DateTime periodToTime
        {
            get
            {
                if (periodTo != null) return DateTime.Parse(periodTo);
                else return new DateTime();
            }
            set { periodTo = value.ToShortDateString(); }
        }

        public int incrementationCount { get; set; }

        public string granularity { get; set; }

        
        [ScriptIgnore]
        public string show { get; set; }

        [ScriptIgnore]
        public PeriodName parent { get; set; }

        

        public PersistentListChangeHandler<PeriodInterval> intervalListChangeHandler;
        public PersistentListChangeHandler<PeriodName> childrenListChangeHandler;

        public override string ToString()
        {
            return this.name;
        }
        
        [ScriptIgnore]
        public PersistentListChangeHandler<Kernel.Domain.PeriodName> listePeriodNames 
        {
            get
            {
                PersistentListChangeHandler<Kernel.Domain.PeriodName> list = new PersistentListChangeHandler<Kernel.Domain.PeriodName>();
                if (childrenListChangeHandler.Items.Count == 0) return new PersistentListChangeHandler<PeriodName>();
                if (childrenListChangeHandler.updatedItems.Count == 0) return childrenListChangeHandler;
                foreach (Kernel.Domain.PeriodName pname in childrenListChangeHandler.Items)
                {
                    list.AddNew(pname);
                    foreach (Kernel.Domain.PeriodName updates in childrenListChangeHandler.updatedItems)
                    {
                        if (pname.position == updates.position)
                        {
                            list.forget(pname);
                            list.AddNew(updates);
                        }
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Indique si cette périodicité est de type YEAR
        /// </summary>
        /// <returns>True si la périodicité est de type YEAR et False sinon</returns>
        public bool isYear() { return !string.IsNullOrWhiteSpace(granularity) && granularity.Equals(Granularity.YEAR.name); }

        /// <summary>
        /// Indique si cette périodicité est de type Month
        /// </summary>
        /// <returns>True si la périodicité est de type Month et False sinon</returns>
        public bool isMonth() { return !string.IsNullOrWhiteSpace(granularity) && granularity.Equals(Granularity.MONTH.name); }

        /// <summary>
        /// Indique si cette périodicité est de type Week
        /// </summary>
        /// <returns>True si la périodicité est de type Week et False sinon</returns>
        public bool isWeek() { return !string.IsNullOrWhiteSpace(granularity) && granularity.Equals(Granularity.WEEK.name); }

        /// <summary>
        /// Indique si cette périodicité est de type Day
        /// </summary>
        /// <returns>True si la périodicité est de type Day et False sinon</returns>
        public bool isDay() { return !string.IsNullOrWhiteSpace(granularity) && granularity.Equals(Granularity.DAY.name); }

        /// <summary>
        /// Liste les périodes liés à une périodicité donnée.
        /// </summary>
        /// <param name="periodName">La périodicité</param>
        /// <returns>Les périodes de cette périodicité</returns>
        public List<PeriodInterval> getPeriod(PeriodName periodName)
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            if (periodName.granularity == Granularity.YEAR.name) return buildYearPeriods();
            if (periodName.granularity == Granularity.MONTH.name) return buildMonthPeriods();
            if (periodName.granularity == Granularity.WEEK.name) return buildWeekPeriods();
            if (periodName.granularity == Granularity.DAY.name) return buildDayPeriods();

            return periods;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodFrom"></param>
        /// <param name="periodTo"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public static int GetPeriodCount(DateTime periodFrom, DateTime periodTo, string granularity)
        {
            if (granularity == Granularity.YEAR.name) return GetYearCount(periodFrom, periodTo);
            if (granularity == Granularity.MONTH.name) return GetMonthCount(periodFrom, periodTo);
            if (granularity == Granularity.WEEK.name) return GetWeekCount(periodFrom, periodTo);
            if (granularity == Granularity.DAY.name) return GetDayCount(periodFrom, periodTo);
            return 0;
        }

        private static int GetWeekCount(DateTime periodFrom, DateTime periodTo)
        {
            return  GetWeekNumber(periodTo) - GetWeekNumber(periodFrom);
        }

        /// <summary>
        /// Calcule et retourne le nombre d'années comprises entre les deux dates.
        /// </summary>
        /// <param name="periodFrom"></param>
        /// <param name="periodTo"></param>
        /// <returns></returns>
        public static int GetYearCount(DateTime periodFrom, DateTime periodTo)
        {
            int fromYear = periodFrom.Year;
            int toYear = periodTo.Year;
            return toYear - fromYear + 1;
        }


        /// <summary>
        /// Calcule et retourne le nombre de mois compris entre les deux dates.
        /// </summary>
        /// <param name="periodFrom"></param>
        /// <param name="periodTo"></param>
        /// <returns></returns>
        public static int GetMonthCount(DateTime periodFrom, DateTime periodTo)
        {
            int fromYear = periodFrom.Year;
            int toYear = periodTo.Year;
            int yearCount = toYear - fromYear + 1;

            int fromMonth = 12 - periodFrom.Month + 1;
            int toMonth = periodTo.Month;

            int semesterCount = yearCount >= 2 ? ((yearCount - 2) * 12) + fromMonth + toMonth
                : yearCount == 1 ? toMonth - periodFrom.Month + 1
                : 0;

            return semesterCount;
        }

        /// <summary>
        /// Calcule et retourne le nombre de jours entre les deux dates.
        /// </summary>
        /// <param name="periodFrom"></param>
        /// <param name="periodTo"></param>
        /// <returns></returns>
        public static int GetDayCount(DateTime periodFrom, DateTime periodTo)
        {
            TimeSpan days = periodTo - periodFrom;
            return (int)days.TotalDays;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        public List<PeriodInterval> buildPeriods()
        {
            if (this.isYear()) return buildYearPeriods();
            if (this.isMonth()) return buildMonthPeriods();
            if (this.isWeek()) return buildWeekPeriods();
            if (this.isDay()) return buildDayPeriods();
            return new List<PeriodInterval>(0);
        }
        /// <summary>
        /// Construit et retourne la liste des périodes de type MONTH.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        public List<PeriodInterval> buildMonthPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            while (periodFromTime <= periodToTime)
            {
                position++;
                periodFromTime = new DateTime(periodFromTime.Year, periodFromTime.Month, 1);
                string name = (periodFromTime.Month < 10 ? "0" : "") + periodFromTime.Month + "/" + periodFromTime.Year;
                periods.Add(new PeriodInterval(position, name, periodFromTime, periodFromTime.AddMonths(1).AddDays(-1)));
                this.intervalListChangeHandler.AddNew(new PeriodInterval(position, name, periodFromTime, periodFromTime.AddMonths(1).AddDays(-1)));
                periodFromTime = periodFromTime.AddMonths(1);
            }
            return periods;
        }
        /// <summary>
        /// return the week number of a date time
        /// </summary>
        /// <param name="dtPassed"></param>
        /// <returns></returns>
        public static int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
       
        /* private List<PeriodInterval> buildWeekPeriods()
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
        }*/


        /// <summary>
        /// Construit et retourne la liste des périodes de type YEAR.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        public List<PeriodInterval> buildYearPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int fromYear = periodFromTime.Year;
            int toYear = periodToTime.Year;
            int position = 0;
            while (fromYear <= toYear)
            {
                position++;
                string name = fromYear.ToString();
                DateTime from = new DateTime(fromYear, 1, 1);
                DateTime to = new DateTime(fromYear, 12, 31);
                periods.Add(new PeriodInterval(position, name, from, to));
                this.intervalListChangeHandler.AddNew(new PeriodInterval(position, name, from, to));
                fromYear++;
            }
            return periods;
        }

        public List<PeriodInterval> buildWeekPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int fromYear = periodFromTime.Year;
            int toYear = periodToTime.Year;
            int position = 0;
            long dateFrom = periodFromTime.Ticks;
            long dateTo = periodToTime.Ticks;
            TimeSpan difference = periodToTime - periodFromTime;
            double day = difference.TotalDays;
            DateTime fromDate = periodFromTime;
            double d = 0;
            if (this.intervalListChangeHandler == null) this.intervalListChangeHandler = new PersistentListChangeHandler<PeriodInterval>();
            while(d < day)
            {
                fromDate = periodFromTime.AddDays(d);
                string name = getWeekName(fromDate,Granularity.WEEK.name);
                DateTime from = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
                int result = (int)fromDate.DayOfWeek;
                double diffToEndWeek = (7 - result);
                double dayToAdd = day <= diffToEndWeek ? day : diffToEndWeek ; 
                DateTime to = fromDate.AddDays(dayToAdd);
                periods.Add(new PeriodInterval(position, name, from, to));
                this.intervalListChangeHandler.AddNew(new PeriodInterval(position, name, from, to));
                d += (dayToAdd+1);
            }
            return periods;
        }

        /// <summary>
        /// Retourne la position de la semaine dans l'année position sur 52 semaines.
        /// Date Test 15/06/1999
        /// J =numéro du premier jour de l'année
        /// N = position du jour dans l'année
        /// W = position de la semaine dans l'année
        /// J = 5, N = 166 , W = 24
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public int getWeekNumberInYear(DateTime period) 
        {
            int annee = period.Year;
            bool isBissextile = annee % 4 == 0 ? annee % 100 == 0 ? annee % 400 == 0 ? true : false : true : false;
            int J = getFirstYearDayNumber(period);

            

            int month = period.Month;

            //numéro du jour dans l'année
            //année non bissextile N = numeroJour + PartieEntière( 30,6 numeroMois - 32,3 ) 
            //année bissextile N = numeroJour + PartieEntière( 30,6 numeroMois - 32,3 ) + 1 
            int N;
            int day = period.Day;
            switch (month) 
            {
                case 1 :
                {
                    N = day;
                    break;
                }
                case 2:
                {
                    N = 31 + day;
                    break;
                }
                default:
                {
                    N = day + ((int)((30.6 * month) - 32.3)) + (isBissextile ? 1 : 0);
                    break;
                }
            }

            //Numéro de semaine dans l'année 
            int W = ((J + N + 5) / 7) -(J / 5);

            return W ;
        }

        public int getFirstYearDayNumber(DateTime period) 
        {
            int annee = period.Year;
            int S = annee / 100;
            int A = annee - (100 * S);
            bool isBissextile = annee % 4 == 0 ? annee % 100 == 0 ? annee % 400 == 0 ? true : false : true : false;
     
            int S5 = 5 * S, S_4 = S / 4, A_4 = A / 4;
            int J = (S5 + S_4 + A + A_4) % 7;
            return J;
        }

        /// <summary>
        /// Ajuste le nom de la semaine en fonction de la position de la semaine
        /// dans l'année.
        /// La première semaine de l'année peut être à la position 0
        /// Dans ce cas la première semaine correspond à la dernière semaine
        ///  de l'année précédente.
        /// La dernière semaine peut être à la position 53.
        /// Dans ce cas la dernière semaine correspond à la première semaine 
        /// de l'année suivante.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="WeekPrefrix">préfixe de semaine ici = "WEEK"</param>
        /// <returns>prefix => "WeekPrefix" + Postion (position de la semaine) + Year (année contenant la semaine) </returns>
        public string getWeekName(DateTime date, string WeekPrefrix) 
        {
            int weekPosition = getWeekNumberInYear(date);
            string year =  ""+date.Year;
           /* if (weekPosition == 0)
            {
                weekPosition = 52;
                year =""+(date.Year - 1);
            }
            if (weekPosition == 53) 
            {
                weekPosition = 1;
                year = "" + (date.Year + 1);
            }*/
            return WeekPrefrix + weekPosition + " " + year;
        }

        /// <summary>
        /// Construit et retourne la liste des périodes de type DAY.
        /// </summary>
        /// <returns>La liste des périodes</returns>
        public List<PeriodInterval> buildDayPeriods()
        {
            List<PeriodInterval> periods = new List<PeriodInterval>(0);
            int position = 0;
            while (periodFromTime <= periodToTime)
            {
                position++;
                string name = periodFromTime.ToShortDateString();
                periods.Add(new PeriodInterval(position, name, periodFromTime, periodFromTime));
                this.intervalListChangeHandler.AddNew(new PeriodInterval(position, name, periodFromTime, periodFromTime));
                periodFromTime = periodFromTime.AddDays(1);
            }
            return periods;
        }
        /// <summary>
        /// Liste les périodes liés à une périodicité donnée.
        /// </summary>
        /// <param name="periodName">La périodicité</param>
        /// <returns>Les périodes de cette périodicité</returns>
        public ObservableCollection<PeriodInterval> getHierarchicalPeriod(int show)
        {
            ObservableCollection<PeriodInterval> periods = new ObservableCollection<PeriodInterval>();
            foreach (PeriodInterval year in buildYearPeriods())
            {
                periods.Add(year);
                PeriodName yearPeriodName = new PeriodName();
                yearPeriodName.periodFromTime = periodFromTime > year.periodFromDateTime ? periodFromTime : year.periodFromDateTime;
                yearPeriodName.periodToTime = periodToTime < year.periodToDateTime ? periodToTime : year.periodToDateTime;

                foreach (PeriodInterval month in yearPeriodName.buildMonthPeriods())
                {
                    year.childrens.Add(month);
                    PeriodName monthPeriodName = new PeriodName();
                    monthPeriodName.periodFromTime = periodFromTime > month.periodFromDateTime ? periodFromTime : month.periodFromDateTime;
                    monthPeriodName.periodToTime = periodToTime < month.periodToDateTime ? periodToTime : month.periodToDateTime;
                    foreach (PeriodInterval week in monthPeriodName.buildWeekPeriods())
                    {
                        month.childrens.Add(week);
                        PeriodName weekPeriodName = new PeriodName();
                        weekPeriodName.periodFromTime = periodFromTime > month.periodFromDateTime ? periodFromTime : month.periodFromDateTime;
                        weekPeriodName.periodToTime = periodToTime < month.periodToDateTime ? periodToTime : month.periodToDateTime;

                    }
                }


            }

            return periods;
        }

        [ScriptIgnore]
        public static string DEFAULT_DATE_NAME = "Date";


        [ScriptIgnore]
        public List<Kernel.Domain.PeriodInterval> listePeriodInterVals
        {
            get
            {
                if (this.intervalListChangeHandler == null) this.intervalListChangeHandler = new PersistentListChangeHandler<PeriodInterval>();
                RefreshPeriodName();
                return this.intervalListChangeHandler.getItems().ToList();
            }
        }

        [ScriptIgnore]
        public ObservableCollection<Kernel.Domain.PeriodInterval> Items
        {
            get
            {
                return this.intervalListChangeHandler.Items;
            }
        }


        [ScriptIgnore]
        public List<PeriodInterval> Leafs
        {
            get
            {
                List<PeriodInterval> values = new List<PeriodInterval>(0);
                foreach (PeriodInterval value in intervalListChangeHandler.Items)
                {
                    if (value.IsLeaf) values.Add(value);
                    else values.AddRange(value.Leafs);
                }
                return values;
            }
        }
        
        public void RefreshPeriodName() 
        {
            foreach (PeriodInterval interval in this.intervalListChangeHandler.Items)
            {
                interval.periodName = this;
                interval.refreshPeriodInterval(this);
            }
        }

        

        private PeriodInterval root;
        private string DEFAULT_DATE_NAME1;
        private bool p;
        public PeriodInterval GetRootPeriodInterval()
        {
            if (this.intervalListChangeHandler.Items.Count == 0)
                this.root = null;

            if (this.root == null)
            {
                root = new PeriodInterval();
                root.name = "Root PeriodInterval";
                root.periodFrom = this.periodFrom;
                root.periodTo = this.periodTo;
                root.periodName = this;
            }
            root.childrenListChangeHandler = intervalListChangeHandler;
            return root;
        }

        public PeriodName getDefaultPeriodName()
        {
            foreach (PeriodName periodname in childrenListChangeHandler.Items) 
            {
                if (periodname.iDateDefault) return periodname;
            }
            return null;
        }

       /// <summary>
        /// Rajoute un fils
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IHierarchyObject child) {
            child.SetPosition(childrenListChangeHandler.Items.Count);
            child.SetParent(this);
            childrenListChangeHandler.AddNew((PeriodName)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }


        public void AddChild(List<Kernel.Domain.PeriodName> listeChild, bool sort = true) 
        {
            childrenListChangeHandler.AddNew(listeChild.Cast<PeriodName>().ToList(), sort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(PeriodName periodName1, PeriodName periodName2)
        {
            int position = periodName1.position;
            periodName1.SetPosition(periodName2.position);
            periodName2.SetPosition(position);
            childrenListChangeHandler.AddUpdated(periodName1);
            childrenListChangeHandler.AddUpdated(periodName2);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateChild(IHierarchyObject child)
        {
            childrenListChangeHandler.AddUpdated((PeriodName)child);
            UpdateParents();
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IHierarchyObject child) 
        {
            foreach (IHierarchyObject item in childrenListChangeHandler.Items)
            {
                if (item.GetPosition() > child.GetPosition()) 
                { 
                   item.SetPosition(item.GetPosition() - 1);
                   childrenListChangeHandler.AddUpdated((PeriodName)item); 
                
                }
            }
            child.SetPosition(-1);
            PeriodName Child = (PeriodName)child;
            childrenListChangeHandler.AddDeleted(Child);
            UpdateParents();
        }

        public void AddPeriodInterval(PeriodInterval interval)
        {
            interval.SetPosition(intervalListChangeHandler.Items.Count);
            intervalListChangeHandler.AddNew(interval);
            UpdateParents();
        }

        public void RemovePeriodInterval(PeriodInterval interval) 
        {
            foreach (PeriodInterval item in intervalListChangeHandler.Items)
            {
                if (item.GetPosition() > interval.GetPosition())
                {
                    item.SetPosition(item.GetPosition() - 1);
                    intervalListChangeHandler.AddUpdated(item);

                }
            }
            interval.SetPosition(-1);
            intervalListChangeHandler.AddDeleted(interval);
            UpdateParents();
        }

        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetPeriodInterval(PeriodInterval Root)
        {
            foreach (PeriodInterval item in intervalListChangeHandler.Items)
            {
                if (item.GetPosition() > Root.GetPosition()) item.SetPosition(item.GetPosition() - 1);
            }
            Root.SetPosition(-1);
            intervalListChangeHandler.forget(Root);
        }


        /// <summary>
        /// Oublier un fils
        /// </summary>
        /// <param name="child"></param>
        public void ForgetChild(IHierarchyObject child)
        {
            foreach (IHierarchyObject item in childrenListChangeHandler.Items)
            {
                if (item.GetPosition() > child.GetPosition()) item.SetPosition(item.GetPosition() - 1);
            }
            child.SetPosition(-1);
            childrenListChangeHandler.forget((PeriodName)child);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetChildByPosition(int position)
        {
            foreach (IHierarchyObject item in childrenListChangeHandler.Items)
            {
                if (item.GetPosition() == position) return item;
            }
            return null;
        }

        /// <summary>
        /// Définit le parent
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(IHierarchyObject parent) { this.parent = (PeriodName)parent; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetParent() { return this.parent; }

        /// <summary>
        /// Définit la position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(int position) { this.position = position; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPosition() { return this.position; }

        /// <summary>
        ///
        /// </summary>
        public void UpdateParents() 
        {
            if (this.parent != null)
            {
                this.parent.childrenListChangeHandler.AddUpdated(this);
                this.parent.UpdateParents();
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Collections.IList GetItems() { return childrenListChangeHandler.Items; }//return new ObservableCollection<Measure>(from measure in childrenListChangeHandler.Items orderby measure.position select measure); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetChildByName(string name)
        {

            foreach (PeriodName periodName in childrenListChangeHandler.Items)
            {
                if (periodName.name == null) continue;
                if (periodName.name.ToUpper().Equals(name.ToUpper())) return periodName;
                IHierarchyObject ob = periodName.GetChildByName(name);
                if (ob != null) return ob;
            }
            return null;
        }


        public bool hasPeriodName(String name) 
        {
            return GetChildByName(name) != null;
        }


        /// <summary>
        /// return existing child with name: name and in not edition mode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHierarchyObject GetNotEditedChildByName(PeriodName periodName, string name)
        {
            if (periodName.name.ToUpper().Equals(name.ToUpper()))
            {

                foreach (PeriodName periodname in childrenListChangeHandler.Items)
                {
                    if (periodName.name == null) continue;
                    if (periodname.name.ToUpper().Equals(name.ToUpper()) && !periodname.Equals(periodName)) return periodname;
                    IHierarchyObject ob = periodname.GetNotEditedChildByName(periodName, name);
                    if (ob != null) return ob;
                }
            
            }
            return null;
        }

        public PeriodName GetChildByName(PeriodName periodName, string name)
        {
            foreach (PeriodName period in childrenListChangeHandler.Items)
            {
                if (period.name.ToUpper().Equals(name.ToUpper()) && !period.Equals(periodName)) return periodName;                
            }
            return null;
        }


       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHierarchyObject GetCopy()
        {
            PeriodName periodName = new PeriodName();
            periodName.name = "Copy Of " + this.name;
            periodName.position = -1;
            periodName.parent = null;
            foreach (PeriodName child in this.childrenListChangeHandler.Items)
            {
                IHierarchyObject copy = child.GetCopy();
                periodName.AddChild(copy);
            }
            return periodName;
        }
        public IHierarchyObject CloneObject() 
        {
            PeriodName periodName = new PeriodName();
            string stringValue = this.name;
            periodName.name =  stringValue;
            int intValue = this.position;
            periodName.position = intValue;
            //periodName.parent = this.parent != null ? (PeriodName)this.parent.CloneObject() : null;
            stringValue = this.show;
            periodName.show = stringValue;
            bool boolValue = this.showDay;
            periodName.showDay = boolValue;
            boolValue = this.showMonth;
            periodName.showMonth = boolValue;
            boolValue = this.showWeek;
            periodName.showWeek = boolValue;
            boolValue = this.showYear;
            periodName.showYear = boolValue;
            stringValue = this.granularity;
            periodName.granularity = stringValue;
            boolValue = this.iDateDefault;
            periodName.iDateDefault = boolValue;
            intValue = this.incrementationCount;
            periodName.incrementationCount = intValue;
            boolValue = this.isModified;
            periodName.isModified = boolValue;
            stringValue = this.modificationDate;
            periodName.modificationDate = stringValue;
            stringValue = this.creationDate;
            periodName.creationDate = stringValue;
            intValue = this.oid.HasValue ? this.oid.Value : 0;
            if(intValue!=0)
            periodName.oid = intValue;
            stringValue = this.typeName;
            periodName.typeName = stringValue;
            stringValue = this.periodFrom;
            periodName.periodFrom = stringValue;
            stringValue = this.periodTo;
            periodName.periodTo = stringValue;
            periodName.root = null;
            periodName.intervalListChangeHandler = new PersistentListChangeHandler<PeriodInterval>();
            foreach (PeriodInterval intervalle in this.intervalListChangeHandler.Items) 
            {
                periodName.intervalListChangeHandler.AddNew((PeriodInterval)intervalle.CloneObject());
            }
            periodName.root = periodName.GetRootPeriodInterval();
            boolValue = this.isModified;
            periodName.isModified = boolValue;
            boolValue = this.isCompleted;
            periodName.isCompleted = boolValue;
            stringValue = this.DEFAULT_DATE_NAME1;
            periodName.DEFAULT_DATE_NAME1 = stringValue;
            periodName.childrenListChangeHandler = new PersistentListChangeHandler<PeriodName>();
            foreach (PeriodName periodname in this.childrenListChangeHandler.Items) 
            {
                periodName.AddChild((PeriodName)periodname.CloneObject());
            }
            
            return periodName;          
        }
      

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is PeriodName)) return 1;
            return this.position.CompareTo(((PeriodName)obj).position);
        }

        
        public void Update(PeriodName p)
        {
            this.curentIntervalGroupName = p.curentIntervalGroupName;
            this.periodFrom = p.periodFrom;
            this.periodTo = p.periodTo;
            //this.position = p.position;
            this.showDay = p.showDay;
            this.showMonth = p.showMonth;
            this.showWeek = p.showWeek;
            this.showYear = p.showYear;
            this.typeName = p.typeName;
            this.name = p.name;
            this.incrementationCount = p.incrementationCount;
            this.granularity = p.granularity;

            PeriodInterval group = new PeriodInterval(0, p.curentIntervalGroupName, p.periodFromTime, p.periodToTime);
            group.childrenListChangeHandler.AddNew(p.intervalListChangeHandler.Items);
            this.AddPeriodInterval(group);
        }

        public string getNewGroupName()
        {
            int i = 1;
            String newName = "Standard Period" + i++;
            while (getGroupByName(newName) != null) newName = "Standard Period" + i++;
            return newName;
        }

        private PeriodInterval getGroupByName(String name)
        {
            foreach(PeriodInterval interval in this.intervalListChangeHandler.getItems())
            {
                if (interval.name.Equals(name, StringComparison.OrdinalIgnoreCase)) return interval;
            }
            return null;
        }

        public PeriodItem getFromPeriodItem(string sign)
        {
            PeriodItem item = new PeriodItem();
            item.name = this.name;
            item.value = this.periodFrom;
            if (!String.IsNullOrWhiteSpace(sign)) item.operatorSign = sign;
            return item;
        }

        public PeriodItem getToPeriodItem(string sign)
        {
            PeriodItem item = new PeriodItem();
            item.name = this.name;
            item.value = this.periodTo;
            if (!String.IsNullOrWhiteSpace(sign)) item.operatorSign = sign;
            return item;
        }


        public PeriodName getPeriodNameByPosition(int position){
            foreach(PeriodName child in childrenListChangeHandler.Items){
                if (child.position == position) return child;
            }
            return null;
        }

    }
 }

