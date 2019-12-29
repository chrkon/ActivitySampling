using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActivitySampling.Interfaces;

namespace ActivitySampling.Module.View.CLI
{
    public class ViewCLI : IView
    {
        public ViewCLI()
        {
            TimeToAnswer = TimeSpan.FromSeconds(30.0);
            CLI = new CommandLineInterface();
        }

        public TimeSpan TimeToAnswer { get; set; }
        public DateTime TimeStampOfLastAnswer { get; set; }
        public string LastAnswer { get; set; }

        public ICommandLineInterface CLI {get; set;}

        public event EventHandler RaiseNoActivityEvent;
        public event EventHandler<ActivityEventArgs> RaiseActivityAddedEvent;
        public event EventHandler<ActivityEventArgs> RaiseActivityChangedEvent;
        public event EventHandler RaiseApplicationCloseEvent;

        private Task MenueTask = null;
        private CancellationTokenSource _cts;

        private const string Question = "Was machst Du gerade?";

        public void AskForActivity(DateTime timeStampOfQuestion, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowQuestion(Question, lastActivity, timeStampOfQuestion, TimeToAnswer);
            HandleAddedAnswer(actualActivity);
            ActivateMenu();
        }

        public void EditLastActivity(DateTime timeStampOfQuestion, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowQuestion(Question, lastActivity, timeStampOfQuestion, TimeToAnswer);
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
            CLI.WriteLine("'x' = exit");
            CLI.WriteLine("'a' = add action");
            CLI.WriteLine("'e' = exit last action");
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
    }
}
