using ActivitySampling.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using CsvHelper;
using CsvHelper.Configuration;

namespace ActivitySampling.Module.Storage.CSVFile
{
    public class CsvFileStorage : IStorage
    {
        public CsvFileStorage(string baseDataPath, string fileExtension)
        {
            BaseDataPath = baseDataPath;
            FileExtension = fileExtension;
        }

        public string BaseDataPath { get; set; }
        public string FileExtension { get; set; }

        public void SaveActivity(DateTime timeStamp, TimeSpan interval, string activityDescription)
        {
            var record = BuildActivityEntry(timeStamp, interval, activityDescription);
            var filename = BuildFileName(DateTime.Now, BaseDataPath, FileExtension);
            var records = ReadDataFromFile(filename);
            records.Add(record.Start, record);
            SaveDataToFile(filename, records);
        }

        public void SaveChangedActivity(DateTime timeStamp, TimeSpan interval, string activityDescription)
        {
            var record = BuildActivityEntry(timeStamp, interval, activityDescription);
            var filename = BuildFileName(DateTime.Now, BaseDataPath, FileExtension);
            var records = ReadDataFromFile(filename);
            records[record.Start] = record;
            SaveDataToFile(filename, records);
        }

        private static ActivityEntry BuildActivityEntry(DateTime timeStamp, TimeSpan interval, string activityDescription)
        {
            var halfInterval = TimeSpan.FromSeconds(interval.TotalSeconds / 2.0);
            var record = new ActivityEntry
            {
                Start = timeStamp - halfInterval,
                Stop = timeStamp + halfInterval,
                Activity = activityDescription
            };
            return record;
        }

        private static string BuildFileName(DateTime actualDateTime, string path, string extension)
        {
            var date = actualDateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
            var filename = Path.Combine($"{path}", $"{date}{extension}");
            return filename;
        }

        private static SortedList<DateTime, object> ReadDataFromFile(string filename)
        {
            var records = new SortedList<DateTime, object>();

            if (File.Exists(filename))
            {
                Configuration cnf = new Configuration(CultureInfo.InvariantCulture);
                using (var reader = new StreamReader(filename))
                using (var csv = new CsvReader(reader, cnf))
                {
                    var typeDef = new
                    {
                        Start = default(DateTime),
                        Stop = default(DateTime),
                        Activity = string.Empty
                    };
                    var recs = csv.GetRecords(typeDef);
                    foreach (var rec in recs)
                    {
                        records.Add(rec.Start, rec);
                    }
                }
            }

            return records;
        }

        private static void SaveDataToFile(string filename, SortedList<DateTime, object> records)
        {
            Configuration cnf = new Configuration(CultureInfo.InvariantCulture);
            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, cnf))
            {
                csv.WriteRecords(records.Values);
            }
        }

    }
}
