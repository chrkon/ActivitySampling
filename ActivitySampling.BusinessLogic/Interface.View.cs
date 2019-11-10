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
        void AskForActivity(DateTime timeStampOfQuestion, string lastActivity);
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
