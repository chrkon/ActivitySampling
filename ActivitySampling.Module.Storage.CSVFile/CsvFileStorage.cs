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
            records.Add(record.Start.Ticks, record);
            SaveDataToFile(filename, records);
        }

        public void SaveChangedActivity(DateTime timeStamp, TimeSpan interval, string activityDescription)
        {
            var record = BuildActivityEntry(timeStamp, interval, activityDescription);
            var filename = BuildFileName(DateTime.Now, BaseDataPath, FileExtension);
            var records = ReadDataFromFile(filename);
            records[record.Start.Ticks] = record;
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

        private static SortedList<long, ActivityEntry> ReadDataFromFile(string filename)
        {
            var records = new SortedList<long, ActivityEntry>();

            if (File.Exists(filename))
            {
                Configuration cnf = new Configuration(CultureInfo.InvariantCulture);
                using (var reader = new StreamReader(filename))
                using (var csv = new CsvReader(reader, cnf))
                {
                    IEnumerable<ActivityEntry> recs = csv.GetRecords<ActivityEntry>();
                    foreach (ActivityEntry rec in recs)
                    {
                        long key = rec.Start.Ticks;
                        if (records.ContainsKey(key))
                        {
                            records.Remove(key);
                        }
                        records.Add(key, rec);
                    }
                }
            }

            return records;
        }

        private static void SaveDataToFile(string filename, SortedList<long, ActivityEntry> records)
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
