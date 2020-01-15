using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;

namespace ActivitySampling.Module.View.CLI
{
    public class ViewCLI : IView
    {
        private readonly TimeSpan defaultTimeToAnswer = new TimeSpan(0,0,25);
        public ViewCLI()
        {
            Question = "Was machst Du gerade?";
            TimeToAnswer = TimeSpan.FromSeconds(25);
            InputHint = "mit [return] die vorherige Antwort übernehmen.";
            HelpText = "'x' = exit\n'a' = add action\n'e' = edit last activity";
            CLI = new CommandLineInterface();
        }

        public TimeSpan TimeToAnswer { get; set; }
        public DateTime TimeStampOfLastAnswer { get; set; }
        public string Question { get; set; }
        public string InputHint { get; set; }
        public string LastAnswer { get; set; }
        public string HelpText { get; set; }

        public ICommandLineInterface CLI {get; set;}
        public Rectangle WindowRectangle
        {
            get => GetWindowRectOSDependent(Environment.OSVersion.Platform);
            set => SetWindowRectOSDependent(Environment.OSVersion.Platform, value);
        }

        public event EventHandler RaiseNoActivityEvent;
        public event EventHandler<ActivityEventArgs> RaiseActivityAddedEvent;
        public event EventHandler<ActivityEventArgs> RaiseActivityChangedEvent;
        public event EventHandler RaiseApplicationCloseEvent;

        private Task MenueTask = null;
        private CancellationTokenSource _cts;

        public void AskForActivity(DateTime timeStampOfQuestion, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowQuestion(Question, lastActivity, timeStampOfQuestion, TimeToAnswer, InputHint);
            HandleAddedAnswer(actualActivity);
            ActivateMenu();
        }

        public void EditLastActivity(DateTime timeStampOfQuestion, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowLastQuestion(Question, lastActivity, timeStampOfQuestion, TimeToAnswer, InputHint);
            HandleChangedAnswer(actualActivity);
            ActivateMenu();
        }

        private void HandleAddedAnswer(string actualActivity)
        {
            LastAnswer = actualActivity;
            if (String.IsNullOrWhiteSpace(actualActivity))
            {
                // Leerer String = keine Tätigkeit = kein Eintrag
                OnRaiseNoActivityEvent(new EventArgs());
            }
            else
            {
                TimeStampOfLastAnswer = DateTime.Now;
                OnRaiseActivityAddedEvent(new ActivityEventArgs(TimeStampOfLastAnswer, actualActivity));
            }
        }

        private void HandleChangedAnswer(string actualActivity)
        {
           OnRaiseActivityChangedEvent(new ActivityEventArgs(TimeStampOfLastAnswer, actualActivity));
        }

        public void ActivateMenu()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            MenueTask = Task.Factory.StartNew(() => MenueHandler(_cts.Token), _cts.Token);
        }

        public void DeactivateMenu()
        {
            _cts?.Cancel();
        }

        private const ConsoleKey CancelKey = ConsoleKey.X;
        private const ConsoleKey HelpKey = ConsoleKey.H;
        private const ConsoleKey AddKey = ConsoleKey.A;
        private const ConsoleKey EditKey = ConsoleKey.E;

        private void MenueHandler(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (!CLI.KeyAvailable)
                {
                    Thread.Sleep(250);
                }
                else
                {
                    ConsoleKeyInfo cki = CLI.ReadKey(true);
                    ConsoleKey key = cki.Key;
                    switch (key)
                    {
                        case AddKey:
                            var lastInterval = DateTime.Now - TimeStampOfLastAnswer;
                            AskForActivity(DateTime.Now, "");
                            break;
                        case EditKey:
                            EditLastActivity(TimeStampOfLastAnswer, LastAnswer);
                            break;
                        case CancelKey:
                            OnRaiseApplicationCloseEvent(new EventArgs());
                            break;
                        case HelpKey:
                            ShowHelpText();
                            break;
                        default:
                            Console.Beep(500,600);
                            break;
                    } 
                }
            }
         }

        private void ShowHelpText()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            CLI.WriteLine(HelpText);
            Console.ResetColor();
        }

        private void OnRaiseActivityAddedEvent(ActivityEventArgs e)
        {
            RaiseActivityAddedEvent?.Invoke(this, e);
        }

        private void OnRaiseActivityChangedEvent(ActivityEventArgs e)
        {
            RaiseActivityChangedEvent?.Invoke(this, e);
        }

        private void OnRaiseNoActivityEvent(EventArgs e)
        {
            RaiseNoActivityEvent?.Invoke(this, e);
        }

        private void OnRaiseApplicationCloseEvent(EventArgs e)
        {
            RaiseApplicationCloseEvent?.Invoke(this, e);
        }

        public void Output(string message)
        {
            CLI.WriteLine(message);
        }

        private Rectangle GetWindowRectOSDependent(PlatformID operatingSystemPlatform)
        {
            var rectangle = new Rectangle(50, 50, 590, 400);
            switch (operatingSystemPlatform)
            {
                case PlatformID.Win32S:
                    break;
                case PlatformID.Win32Windows:
                    break;
                case PlatformID.Win32NT:
                    rectangle = Windows.CliPositioning.GetRectangle();
                    break;
                case PlatformID.WinCE:
                    break;
                case PlatformID.Unix:
                    break;
                case PlatformID.Xbox:
                    break;
                case PlatformID.MacOSX:
                    break;
                default:
                    break;
            }
            return rectangle;
        }

        private void SetWindowRectOSDependent(PlatformID operatingSystemPlatform, Rectangle value)
        {
            switch (operatingSystemPlatform)
            {
                case PlatformID.Win32S:
                    break;
                case PlatformID.Win32Windows:
                    break;
                case PlatformID.Win32NT:
                    Windows.CliPositioning.SetRectangle(value);
                    break;
                case PlatformID.WinCE:
                    break;
                case PlatformID.Unix:
                    break;
                case PlatformID.Xbox:
                    break;
                case PlatformID.MacOSX:
                    break;
                default:
                    break;
            }
        }

    }
}
