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
        public DateTime TimeStampOfAnswer { get; set; }

        public ICommandLineInterface CLI {get; set;}

        public event EventHandler RaiseNoActivityEvent;
        public event EventHandler<ActivityAddedEventArgs> RaiseActivityAddedEvent;
        public event EventHandler RaiseApplicationCloseEvent;

        private Task MenueTask = null;
        private CancellationTokenSource cts;

        private const string Question = "Was machst Du gerade?";

        public void AskForActivity(DateTime timeStampOfQuestion, TimeSpan interval, string lastActivity)
        {
            DeactivateMenu();
            var actualActivity = CLI.ShowQuestion(Question, lastActivity, timeStampOfQuestion, interval, TimeToAnswer);
            HandleAnswer(actualActivity);
            ActivateMenu();
        }

        private void HandleAnswer(string actualActivity)
        {
            if (String.IsNullOrWhiteSpace(actualActivity))
            {
                // Leerer String = keine Tätigkeit = kein Eintrag
                OnRaiseNoActivityEvent(new EventArgs());
            }
            else
            {
                TimeStampOfAnswer = DateTime.Now;
                OnRaiseActivityAddedEvent(new ActivityAddedEventArgs(TimeStampOfAnswer, actualActivity));
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

        private void MenueHandler(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (Console.KeyAvailable == false)
                {
                    Thread.Sleep(250);
                }
                else
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    var isExitKey = key.Key == CancelKey;
                    var isHelpKey = key.Key == HelpKey;

                    if (isExitKey)
                    {
                        OnRaiseApplicationCloseEvent(new EventArgs());
                        break;
                    }
                    else if (isHelpKey)
                    {
                        ShowHelpText();
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
