using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ActivitySampling.Interfaces
{
    public interface IStorage
    {
        string BaseDataPath { get; set; }
        string FileExtension { get; set; }

        void SaveActivity(DateTime timeStamp, TimeSpan interval, string activityDescription);
        void SaveChangedActivity(DateTime timeStamp, TimeSpan interval, string activityDescription);
    }

}
