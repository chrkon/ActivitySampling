using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Question;
using ActivitySampling.Module.Storage.CSVFile;

namespace ActivitySampling.Application
{
    class Program
    {
        private static readonly TimeSpan IntervalWithoutCommandLineArg = TimeSpan.FromSeconds(35);

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
            TimeSpan Interval = GetIntervalFromCommandLineArgs(args);

            var bl = new BusinessLogic();
            await bl.Run(Interval);

            while (bl.IsRunning)
            {
                Thread.Sleep(1000);
            }
        }

        private static TimeSpan GetIntervalFromCommandLineArgs(string[] args)
        {
            TimeSpan result = IntervalWithoutCommandLineArg;

            if (args.GetUpperBound(0) >= 0)
            {
                var ok = int.TryParse(args[0], out int minutes);
                if (ok)
                {
                    result = TimeSpan.FromMinutes(minutes);
                }
            } 
            return result;
        }
    }
}
