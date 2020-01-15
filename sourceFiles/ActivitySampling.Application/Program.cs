using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ActivitySampling.Application.Properties;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Question;
using ActivitySampling.Module.Storage.CSVFile;
using ActivitySampling.Module.View.CLI;
using Microsoft.Extensions.Configuration;

namespace ActivitySampling.Application
{
    class Program
    {
        private static readonly TimeSpan IntervalWithoutCommandLineArg = TimeSpan.FromSeconds(35);
        private static IView _view = null;
        private const string windowPosFile = "ActivitySampling.WindowPos.json";

        [STAThread]
        static void Main(string[] args)
         {
            _view = new ViewCLI();

            Rectangle windowRectangle;
            windowRectangle = LoadWindowPosition(windowPosFile);
            _view.WindowRectangle = windowRectangle;

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            string StartMessage = $"{Resources.Program_Main_Activity_Sampling_V1} ({ver.Major}.{ver.Minor}.{ver.Revision})";
            Console.WriteLine(StartMessage);

            _ = RunLogic(args, _view);

            windowRectangle = _view.WindowRectangle;
            SaveWindowPosition(windowRectangle, windowPosFile);

            Console.WriteLine();
            Console.WriteLine(Resources.Program_Main_Application_halted_);
        }

        static async Task RunLogic(string[] args, IView _view)
        {
            TimeSpan Interval = GetIntervalFromCommandLineArgs(args);

            var bl = new BusinessLogic(_view);
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

        private static void SaveWindowPosition(Rectangle data, string file)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var text = JsonSerializer.Serialize<Rectangle>(data, jsonOptions);
                var filename = Path.Combine(Directory.GetCurrentDirectory(), file);
                File.WriteAllText(filename, text);
            }
            catch (Exception)
            {
                // do nothing                
            }
        }

        private static Rectangle LoadWindowPosition(string file)
        {
            var rect = new Rectangle(50, 50, 640, 500);
            try
            {
                var filename = Path.Combine(Directory.GetCurrentDirectory(), file);
                var text = File.ReadAllText(filename);
                rect = JsonSerializer.Deserialize<Rectangle>(text);
            }
            catch (Exception)
            {
                // do nothing                
            }
            return rect;
        }

    }
}
