using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActivitySampling.Interfaces
{
    public class SchedulerEventArgs : EventArgs
    {
        public DateTime TimeStamp { get; private set; }
        public SchedulerEventArgs(DateTime time)
        {
            TimeStamp = time;
        }
    }

    public interface IQuestionScheduler
    {
        bool IsRunning { get; }
        Task Start(TimeSpan interval);
        void Stop();
        event EventHandler<SchedulerEventArgs> RaiseSchedulerEvent;
    }

}
