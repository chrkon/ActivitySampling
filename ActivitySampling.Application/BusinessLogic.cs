using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Quartz;
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
            schedule = new QuartzScheduler();
            view = new ViewCLI(); 

            schedule.RaiseSchedulerEvent += Schedule_RaiseSchedulerEvent;

            view.RaiseActivityAddedEvent += View_RaiseActivityAddedEvent;
            view.RaiseNoActivityEvent += View_RaiseNoActivityEvent;
            view.RaiseApplicationCloseEvent += View_RaiseApplicationCloseEvent;
        }

        internal async Task Run(TimeSpan timeSpan)
        {
            Interval = timeSpan;
            await schedule.Start(Interval);
        }

        private void Schedule_RaiseSchedulerEvent(object sender, SchedulerEventArgs e)
        {
            view.AskForActivity(ActualTime, Interval, LastActivity);
        }

        private void View_RaiseNoActivityEvent(object sender, EventArgs e)
        {
            Console.WriteLine("no Activity");
            LastActivity = string.Empty;
        }

        private void View_RaiseActivityAddedEvent(object sender, ActivityAddedEventArgs e)
        {
            LastActivity = e.Description;
            IStorage storage = new CsvFileStorage();
            storage.SaveActivity(e.TimeStamp, Interval, e.Description);
            Console.WriteLine($@" ... saved ({DateTime.Now:t}) [press 'h' for help]");
        }
        private void View_RaiseApplicationCloseEvent(object sender, EventArgs e)
        {
            Console.WriteLine(@" ... closing ");
            IStorage storage = new CsvFileStorage();
            string ApplicationStoppedMessage = "Activity Sampling halted";
            storage.SaveActivity(DateTime.Now,TimeSpan.Zero, ApplicationStoppedMessage);
            schedule.Stop();
        }

    }
}
