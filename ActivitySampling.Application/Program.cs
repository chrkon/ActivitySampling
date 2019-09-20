using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Quartz;
using ActivitySampling.Module.Storage.CSVFile;

namespace ActivitySampling.Application
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Activity Sampling V1");

            _ = RunLogic(args);

            Console.WriteLine();
            Console.WriteLine("Application halted.");
        }

        static async Task RunLogic(string[] args)
        {
            TimeSpan Interval = TimeSpan.FromSeconds(30);

            if (args.GetUpperBound(0) >= 0)
            {
                var ok = int.TryParse(args[0], out int min);
                if (ok)
                {
                    Interval = TimeSpan.FromMinutes(min);
                }
            }

            var bl = new BusinessLogic();
            await bl.Run(Interval);

            while (bl.IsRunning)
            {
                Thread.Sleep(1000);
            }

        }
    }
}
