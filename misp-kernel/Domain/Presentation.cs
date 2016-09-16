using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Misp.Kernel.Domain
{
    public class Presentation : Persistent, IComparable
    {
        public string name { get; set; }

        public string slideFileName { get; set; }

        public String slideFileExtension { get; set; }

        public BGroup group { get; set; }

        public bool openPresentationAfterRun { get; set; }

        public PersistentListChangeHandler<PresentationSlide> slideListChangeHandler;

        public string userSavingDir { get; set; }

        [ScriptIgnore]
        public static string defaultSavingFolder 
        {
            get{
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + System.IO.Path.DirectorySeparatorChar;
            }
        }

        public Presentation()
        {
            slideListChangeHandler = new PersistentListChangeHandler<PresentationSlide>();
        }

        public Presentation(string _name):this()
        {
            this.name = _name;
        }

        public bool containsSlide(PresentationSlide slide) 
        {
            foreach (PresentationSlide item in slideListChangeHandler.getItems())
            {
                if (slide.position == item.position) return true;
            }
            return false;
        }

        public PresentationSlide containsSlide(int position)
        {
            if (slideListChangeHandler == null) slideListChangeHandler = new PersistentListChangeHandler<PresentationSlide>();
            
            foreach (PresentationSlide item in slideListChangeHandler.Items)
            {
                if (position == item.position) return item;
            }
            return null;
        }
              

        public void AddSlide(PresentationSlide slide) 
        {
            slide.position = slideListChangeHandler.Items.Count + 1;
            slideListChangeHandler.AddNew((PresentationSlide)slide);
            OnPropertyChanged("slideListChangeHandler.Items");
        }

        /// <summary>
        /// Met à jour un fils
        /// </summary>
        /// <param name="child"></param>
        public void UpdateSlide(PresentationSlide child)
        {
            slideListChangeHandler.AddUpdated((PresentationSlide)child);
            OnPropertyChanged("childrenListChangeHandler.Items");
        }

        /// <summary>
        /// Retire un fils
        /// </summary>
        /// <param name="child"></param>
        public void RemoveSlide(PresentationSlide slide)
        {
            foreach (PresentationSlide item in slideListChangeHandler.Items)
            {
                if (item.position > slide.position)
                {
                    item.position = item.position - 1;
                    slideListChangeHandler.AddUpdated((PresentationSlide)item);
                }
            }
            slide.position = -1;
            slideListChangeHandler.AddDeleted((PresentationSlide)slide);
        }

        public void ForgetSlide(PresentationSlide slide)
        {
            foreach (PresentationSlide item in slideListChangeHandler.Items)
            {
                if (item.position > slide.position) item.position = item.position - 1;
            }
            slide.position = -1;
            slideListChangeHandler.forget((PresentationSlide)slide);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure1"></param>
        /// <param name="measure2"></param>
        public void SwichtPosition(PresentationSlide slide1, PresentationSlide slide2)
        {
            int position = slide1.position;
            slide1.position = slide2.position;
            slide2.position =  position;
            slideListChangeHandler.AddUpdated(slide1);
            slideListChangeHandler.AddUpdated(slide2);
            OnPropertyChanged("slideListChangeHandler.Items");
        }

        public PresentationSlide getSlide(int position) 
        {
            foreach(PresentationSlide slide in slideListChangeHandler.Items)
            {
                if(slide.position == position) return slide;
            }
            return null;
        }

        [ScriptIgnore]
        public bool HasSlides
        {
            get
            {
                return this.slideListChangeHandler.Items.Count == 0;
            }
        }

       

        public override string ToString()
        {
            return this.name != null ? this.name : base.ToString();
        }

        public override int CompareTo(object obj)
        {
            if (obj == null || !(obj is Presentation)) return 1;
            return this.name.CompareTo(((Presentation)obj).name);
        }
    }
}
