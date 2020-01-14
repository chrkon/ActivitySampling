using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Question;
using ActivitySampling.Module.Storage.CSVFile;
using ActivitySampling.Module.View.CLI;
using Microsoft.Extensions.Configuration;

namespace ActivitySampling.Application
{
    public class BusinessLogic
    {
        public TimeSpan Interval { get; set; }
        public DateTime ActualTime => DateTime.Now;
        public bool IsRunning => _schedule.IsRunning;
        public string LastActivity { get; set; }

        private readonly IView _view;
        private readonly IQuestionScheduler _schedule;

        private readonly string _baseDataPath;
        private readonly string _fileExtension;
        private const string settingsFile = "ActivitySampling.json";

        public IConfiguration Configuration = null;


        public BusinessLogic(IView view)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsFile, optional: false, reloadOnChange: true)
                .Build();

            _baseDataPath = Path.GetFullPath(Configuration["DataPath"]);
            _fileExtension = Configuration["FileExtension"];

            string culture = Configuration["CultureInfo"];
            CultureInfo.CurrentCulture = new CultureInfo(culture, true);
            CultureInfo.CurrentUICulture = new CultureInfo(culture, true);

            _schedule = new QuestionScheduler();
            _view = view;

            _view.Question = Properties.Resources.QuestionText;
            _view.InputHint = Properties.Resources.InputHint;
            _view.HelpText = Properties.Resources.HelpText;
            _view.TimeToAnswer = GetTimeToAnswerFromString(Configuration["TimeToAnswer"]);

            _schedule.RaiseSchedulerEvent += Schedule_RaiseSchedulerEvent;

            _view.RaiseActivityAddedEvent += View_RaiseActivityAddedEvent;
            _view.RaiseActivityChangedEvent += View_RaiseActivityChangedEvent;
            _view.RaiseNoActivityEvent += View_RaiseNoActivityEvent;
            _view.RaiseApplicationCloseEvent += View_RaiseApplicationCloseEvent;
            _view.ActivateMenu();
        }

        internal async Task Run(TimeSpan timeSpan)
        {
            _view.Output($@"{Properties.Resources.BL_ScreenStartingInfo} ({DateTime.Now:t}) {Properties.Resources.BL_ScreenMenuInfo}");
            Interval = timeSpan;
            IStorage storage = new CsvFileStorage(_baseDataPath, _fileExtension);
            storage.SaveActivity(DateTime.Now, TimeSpan.Zero, $@"{Properties.Resources.BL_StartMessage} ({DateTime.Now:t})");

            await _schedule.Start(Interval);
        }

        private TimeSpan GetTimeToAnswerFromString(string resource)
        {
            bool ok = double.TryParse(resource, out var secondsToAnswer);
            return ok? TimeSpan.FromSeconds(secondsToAnswer) : TimeSpan.FromSeconds(25);
        }

        private void Schedule_RaiseSchedulerEvent(object sender, SchedulerEventArgs e)
        {
            _view.AskForActivity(ActualTime, LastActivity);
        }

        private void View_RaiseNoActivityEvent(object sender, EventArgs e)
        {
            _view.Output($@"{Properties.Resources.NoActionText}");
            LastActivity = string.Empty;
        }

        private void View_RaiseActivityAddedEvent(object sender, ActivityEventArgs e)
        {
            LastActivity = e.Description;
            IStorage storage = new CsvFileStorage(_baseDataPath, _fileExtension);
            storage.SaveActivity(e.TimeStamp, Interval, e.Description);
            _view.Output($@"{Properties.Resources.BL_ScreenSavedInfo} ({DateTime.Now:t})");
        }
        private void View_RaiseActivityChangedEvent(object sender, ActivityEventArgs e)
        {
            LastActivity = e.Description;
            IStorage storage = new CsvFileStorage(_baseDataPath, _fileExtension);
            storage.SaveChangedActivity(e.TimeStamp, Interval, e.Description);
            _view.Output($@"{Properties.Resources.BL_ScreenChangedInfo} ({e.TimeStamp:t})");
        }

        private void View_RaiseApplicationCloseEvent(object sender, EventArgs e)
        {
            _view.DeactivateMenu();
            _view.Output($@"{Properties.Resources.BL_ScreenEndingInfo}");
            
            IStorage storage = new CsvFileStorage(_baseDataPath, _fileExtension);
            storage.SaveActivity(DateTime.Now, TimeSpan.Zero, $@"{Properties.Resources.BL_EndMessage}");

            _schedule.Stop();            
        }
    }
}
