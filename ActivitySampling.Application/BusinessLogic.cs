using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Question;
using ActivitySampling.Module.Storage.CSVFile;
using ActivitySampling.Module.View.CLI;

namespace ActivitySampling.Application
{
    public class BusinessLogic
    {
        public TimeSpan Interval { get; set; }
        public DateTime ActualTime { get => DateTime.Now; }
        public bool IsRunning => schedule.IsRunning;
        public string LastActivity { get; set; }

        private IView view;
        private IQuestionScheduler schedule;

        public BusinessLogic()
        {
            schedule = new QuestionScheduler();
            view = new ViewCLI(); 

            schedule.RaiseSchedulerEvent += Schedule_RaiseSchedulerEvent;

            view.RaiseActivityAddedEvent += View_RaiseActivityAddedEvent;
            view.RaiseActivityChangedEvent += View_RaiseActivityChangedEvent;
            view.RaiseNoActivityEvent += View_RaiseNoActivityEvent;
            view.RaiseApplicationCloseEvent += View_RaiseApplicationCloseEvent;
            view.ActivateMenu();
        }

        internal async Task Run(TimeSpan timeSpan)
        {
            view.Output($@"Starting ... ({DateTime.Now:t}) [press 'h' for help]");
            Interval = timeSpan;
            IStorage storage = new CsvFileStorage();
            string ApplicationStartMessage = $@"Activity Sampling started ({DateTime.Now:t})";
            storage.SaveActivity(DateTime.Now, TimeSpan.Zero, ApplicationStartMessage);

            await schedule.Start(Interval);
        }

        private void Schedule_RaiseSchedulerEvent(object sender, SchedulerEventArgs e)
        {
            view.AskForActivity(ActualTime, LastActivity);
        }

        private void View_RaiseNoActivityEvent(object sender, EventArgs e)
        {
            view.Output("no Activity");
            LastActivity = string.Empty;
        }

        private void View_RaiseActivityAddedEvent(object sender, ActivityEventArgs e)
        {
            LastActivity = e.Description;
            IStorage storage = new CsvFileStorage();
            storage.SaveActivity(e.TimeStamp, Interval, e.Description);
            view.Output($@" ... saved ({DateTime.Now:t})");
        }
        private void View_RaiseActivityChangedEvent(object sender, ActivityEventArgs e)
        {
            LastActivity = e.Description;
            IStorage storage = new CsvFileStorage();
            storage.SaveChangedActivity(e.TimeStamp, Interval, e.Description);
            view.Output($@" ... changed ({e.TimeStamp:t})");
        }

        private void View_RaiseApplicationCloseEvent(object sender, EventArgs e)
        {
            view.DeactivateMenu();
            view.Output(@" ... closing ");
            IStorage storage = new CsvFileStorage();
            string ApplicationStoppedMessage = "Activity Sampling halted";
            storage.SaveActivity(DateTime.Now, TimeSpan.Zero, ApplicationStoppedMessage);
            schedule.Stop();
        }

    }
}
