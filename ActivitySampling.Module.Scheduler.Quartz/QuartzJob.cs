using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;
using Quartz;

namespace ActivitySampling.Module.Scheduler.Quartz
{
    public class QuartzJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        { 
            var @event = (EventHandler<SchedulerEventArgs>)context.JobDetail.JobDataMap["event-handler"];
            await Task.Run( ()=> @event?.Invoke(this, new SchedulerEventArgs(DateTime.Now)));
        }
    }
}
