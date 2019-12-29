using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Module.Storage.CSVFile
{
    public class ActivityEntry
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public string Activity { get; set; }
    }
}
