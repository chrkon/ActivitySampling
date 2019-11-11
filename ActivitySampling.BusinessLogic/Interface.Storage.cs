using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Interfaces
{
    public interface IStorage
    {
        void SaveActivity(DateTime entryTime, DateTime intervalCenterTime, TimeSpan interval, string activityDescription);
    }

}
