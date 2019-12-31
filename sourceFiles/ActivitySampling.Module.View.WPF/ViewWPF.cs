using ActivitySampling.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ActivitySampling.Module.View.WPF
{
    public class ViewWpf : IView
    {
        private ViewWpf()
        {
            // Singleton!
            TimeToAnswer = TimeSpan.FromSeconds(20.0);
        }
        private static IView _instance = null;
        public static IView Instance => _instance ?? (_instance = new ViewWpf());

        public TimeSpan TimeToAnswer { get; set; }
        public DateTime TimeStampOfAnswer { get; set; }

        public event EventHandler<ActivityAddedEventArgs> RaiseActivityAddedEvent;
        public event EventHandler RaiseApplicationCloseEvent;
        public event EventHandler RaiseNoActivityEvent;

        public void AskForActivity(DateTime timeStampOfQuestion, TimeSpan interval, string lastActivity)
        {
            var actualActivity = ShowQuestionWindow(lastActivity, timeStampOfQuestion, interval, TimeToAnswer);
            HandleAnswer(actualActivity);
        }

        private string ShowQuestionWindow(string lastActivity, DateTime timeStampOfQuestion, TimeSpan workingInterval, TimeSpan timeToAnswer)
        {
            // Show Dialog for 20 Seconds
            string actualActivity = DateTime.Now.ToLongTimeString(); //string.Empty;
            return actualActivity;
        }

        private void HandleAnswer(string actualActivity)
        {
            if (String.IsNullOrWhiteSpace(actualActivity))
            {
                // Leerer String = keine Tätigkeit = kein Eintrag
                OnRaiseNoActivityEvent(new EventArgs());
            }
            else
            {
                TimeStampOfAnswer = DateTime.Now;
                OnRaiseActivityAddedEvent(new ActivityAddedEventArgs(TimeStampOfAnswer, actualActivity));
            }
        }

        private void OnRaiseActivityAddedEvent(ActivityAddedEventArgs e)
        {
            RaiseActivityAddedEvent?.Invoke(this, e);
        }

        private void OnRaiseNoActivityEvent(EventArgs e)
        {
            RaiseNoActivityEvent?.Invoke(this, e);
        }

    }
}
