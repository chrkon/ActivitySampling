using ActivitySampling.Interfaces;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace ActivitySampling.Module.Scheduler.Quartz
{
    public class QuartzScheduler : IQuestionScheduler
    {
        public bool IsRunning { get; set; }
        public event EventHandler<SchedulerEventArgs> RaiseSchedulerEvent;

        public async Task Start(TimeSpan interval)
        {
            IsRunning = true;
            await RunScheduler(interval);
        }
        public void Stop()
        {
            IsRunning = false;
        }

        private async Task RunScheduler(TimeSpan interval)
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = await schedFact.GetScheduler();
            await sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<QuartzJob>()
                .WithIdentity("Job", "ActivitySampling")
                .SetJobData(new JobDataMap{{"event-handler", RaiseSchedulerEvent}})
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("Trigger", "ActivitySampling")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithInterval(interval)
                  .RepeatForever())
              .Build();

            await sched.ScheduleJob(job, trigger);
        }

    }
}
