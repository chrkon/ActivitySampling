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
            var bl = new BusinessLogic();
            await bl.Run(TimeSpan.FromMinutes(20));

            while (bl.IsRunning)
            {
                Thread.Sleep(1000);
            }

        }
    }
}
