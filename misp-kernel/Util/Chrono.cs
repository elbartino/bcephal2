using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Util
{
    public class Chrono
    {

        public static Stopwatch StartChrono(Stopwatch stopWatch = null)
        {
            if (stopWatch == null) stopWatch = new Stopwatch();
            stopWatch.Start();
            return stopWatch;
        }

        public static String StopChrono(Stopwatch stopWatch)
        {
            if (stopWatch == null) return "";
            stopWatch.Stop();
            return GetDuration(stopWatch);
        }

        public static String GetDuration(Stopwatch stopWatch)
        {
            if (stopWatch == null) return "";
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            return elapsedTime;
        }

    }
}
