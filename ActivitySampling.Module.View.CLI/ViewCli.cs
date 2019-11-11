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
            TimeToAnswer = TimeSpan.FromSeconds(10.0);
            TimeStampOfLastAnswer = DateTime.Now;
            CLI = new CommandLineInterface();
        }

        public TimeSpan TimeToAnswer { get; set; }
        public DateTime TimeStampOfLastAnswer { get; set; }

        public ICommandLineInterface CLI {get; set;}

        public event EventHandler RaiseNoActivityEvent;
        public event EventHandler<ActivityAddedEventArgs> RaiseActivityAddedEvent;
        public event EventHandler RaiseApplicationCloseEvent;

        private Task MenueTask = null;
        private CancellationTokenSource cts;

        private const string Question = "Was machst Du gerade?";

        public void AskForActivity(DateTime timeStampOfQuestion, DateTime centerTimeOfInterval, TimeSpan interval, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowQuestion(Question, lastActivity, timeStampOfQuestion, TimeToAnswer);
            HandleAnswer(actualActivity, timeStampOfQuestion, centerTimeOfInterval, interval);
            ActivateMenu();
        }
        private void HandleAnswer(string actualActivity, DateTime timeStampOfQuestion, DateTime centerTimeOfInterval, TimeSpan interval)
        {
            if (String.IsNullOrWhiteSpace(actualActivity))
            {
                // Leerer String = keine Tätigkeit = kein Eintrag
                OnRaiseNoActivityEvent(new EventArgs());
            }
            else
            {
                TimeStampOfLastAnswer = timeStampOfQuestion;
                OnRaiseActivityAddedEvent(new ActivityAddedEventArgs(timeStampOfQuestion, centerTimeOfInterval, interval, actualActivity));
            }
        }

        public void AskForBelatedActivity(DateTime timeStampOfQuestion, DateTime centerTimeOfInterval, TimeSpan interval, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowQuestion(Question, lastActivity, timeStampOfQuestion, TimeToAnswer);
            HandleBelatedAnswer(actualActivity, timeStampOfQuestion, centerTimeOfInterval, interval);
            ActivateMenu();
        }

        private void HandleBelatedAnswer(string belatedActivity, DateTime timeStampOfQuestion, DateTime centerTimeOfInterval, TimeSpan interval)
        {
            if (String.IsNullOrWhiteSpace(belatedActivity))
            {
                // Leerer String = keine Tätigkeit = kein Eintrag
                OnRaiseNoActivityEvent(new EventArgs());
            }
            else
            {
                TimeStampOfLastAnswer = timeStampOfQuestion;
                OnRaiseActivityAddedEvent(new ActivityAddedEventArgs(timeStampOfQuestion, centerTimeOfInterval, interval, belatedActivity));
            }
        }

        public void ActivateMenu()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            MenueTask = Task.Factory.StartNew(() => MenueHandler(cts.Token), cts.Token);
        }

        public void DeactivateMenu()
        {
            cts?.Cancel();
        }

        private const ConsoleKey CancelKey = ConsoleKey.X;
        private const ConsoleKey HelpKey = ConsoleKey.H;
        private const ConsoleKey AddKey = ConsoleKey.A;

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
                            AskForBelatedActivity(DateTime.Now, "");
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
            Console.WriteLine("'x' = exit");
            Console.ResetColor();
        }

        private void OnRaiseActivityAddedEvent(ActivityAddedEventArgs e)
        {
            RaiseActivityAddedEvent?.Invoke(this, e);
        }

        private void OnRaiseNoActivityEvent(EventArgs e)
        {
            RaiseNoActivityEvent?.Invoke(this, e);
        }

        private void OnRaiseApplicationCloseEvent(EventArgs e)
        {
            RaiseApplicationCloseEvent?.Invoke(this, e);
        }
    }
}
