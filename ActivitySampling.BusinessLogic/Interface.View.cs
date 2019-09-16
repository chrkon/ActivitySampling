using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Interfaces
{
    public class ActivityAddedEventArgs : EventArgs
    {
        public string Description { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public ActivityAddedEventArgs(DateTime time, string activityDescription)
        {
            TimeStamp = time;
            Description = activityDescription;
        }
    }

    public interface IView
    {
        void AskForActivity(DateTime timeStampOfQuestion, TimeSpan interval, string lastActivity);
        event EventHandler RaiseNoActivityEvent;
        event EventHandler<ActivityAddedEventArgs> RaiseActivityAddedEvent;
        event EventHandler RaiseApplicationCloseEvent;
    }

}
