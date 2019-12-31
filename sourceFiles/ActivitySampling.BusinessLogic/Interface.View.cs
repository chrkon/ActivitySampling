using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Interfaces
{
    public class ActivityEventArgs : EventArgs
    {
        public string Description { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public ActivityEventArgs(DateTime time, string activityDescription)
        {
            TimeStamp = time;
            Description = activityDescription;
        }
    }

    public interface IView
    {
        void Output(string message);
        void AskForActivity(DateTime timeStampOfQuestion, string lastActivity);

        string Question { get; set; }
        string InputHint { get; set; }
        TimeSpan TimeToAnswer { get; set; }
        string HelpText { get; set; }

        void EditLastActivity(DateTime timeStampOfQuestion, string lastActivity);
        void ActivateMenu();
        void DeactivateMenu();
        event EventHandler RaiseNoActivityEvent;
        event EventHandler<ActivityEventArgs> RaiseActivityAddedEvent;
        event EventHandler<ActivityEventArgs> RaiseActivityChangedEvent;
        event EventHandler RaiseApplicationCloseEvent;
    }


    public interface ICommandLineInterface
    {
        bool KeyAvailable { get; }
        ConsoleKeyInfo ReadKey(bool intercept = false);
        string ReadLine();
        string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer, string hint);
        string ShowLastQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer, string hint);
        void WriteLine(string text);
    }
}
