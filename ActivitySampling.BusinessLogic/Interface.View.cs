using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Interfaces
{
    public class ActivityAddedEventArgs : EventArgs
    {
        public string Description { get; private set; }
        public DateTime TimeOfEntry { get; private set; }
        public DateTime IntervalCenterTime { get; private set; }
        public TimeSpan Interval { get; private set; }
        public ActivityAddedEventArgs(DateTime entryTime, DateTime centerTimeOfInterval, TimeSpan interval, string activityDescription)
        {
            TimeOfEntry = entryTime;
            IntervalCenterTime = centerTimeOfInterval;
            Interval = interval;
            Description = activityDescription;
        }
    }

    public interface IView
    {
        void AskForActivity(DateTime timeStampOfQuestion, DateTime centerTimeOfInterval, TimeSpan interval, string lastActivity);
        void ActivateMenu();
        void DeactivateMenu();
        event EventHandler RaiseNoActivityEvent;
        event EventHandler<ActivityAddedEventArgs> RaiseActivityAddedEvent;
        event EventHandler RaiseApplicationCloseEvent;
    }


    public interface ICommandLineInterface
    {
        bool KeyAvailable { get; }
        ConsoleKeyInfo ReadKey(bool intercept = false);
        string ReadLine();
        string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer);
    }
}
