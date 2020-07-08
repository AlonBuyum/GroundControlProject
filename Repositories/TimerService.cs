using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;

namespace Repositories
{
    public interface ITimerService
    {
       Timer GetTimer { get; set; }
        void NewTimer();
    }
    public class TimerService: ITimerService
    {
        public Timer GetTimer { get; set; }

        public TimerService()
        {
            GetTimer = new Timer();
        }

        public void NewTimer()
        {
            GetTimer = new Timer();
        }
    }
}
