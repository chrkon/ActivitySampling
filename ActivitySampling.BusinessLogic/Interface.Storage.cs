using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Interfaces
{
    public interface IStorage
    {
        void SaveActivity(DateTime timeStamp, TimeSpan interval, string activityDescription);
        void SaveChangedActivity(DateTime timeStamp, TimeSpan interval, string activityDescription);
    }

}
