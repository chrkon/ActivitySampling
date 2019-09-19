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
            TimeToAnswer = TimeSpan.FromSeconds(20.0);
        }

        public TimeSpan TimeToAnswer { get; set; }
        public DateTime TimeStampOfAnswer { get; set; }

        public event EventHandler RaiseNoActivityEvent;
        public event EventHandler<ActivityAddedEventArgs> RaiseActivityAddedEvent;
        public event EventHandler RaiseApplicationCloseEvent;

        private Task MenueTask = null;
        private CancellationTokenSource cts;

        public void AskForActivity(DateTime timeStampOfQuestion, TimeSpan interval, string lastActivity)
        {
            StopMenue();
            var actualActivity = ShowQuestion(lastActivity, timeStampOfQuestion, interval, TimeToAnswer);
            HandleAnswer(actualActivity);
            StartMenue();
        }

        private const string Question = "Was machst Du gerade?";
        private string ShowQuestion(string lastActivity, DateTime timeStampOfQuestion, TimeSpan workingInterval, TimeSpan timeToAnswer)
        {
            string actualActivity = string.Empty;
            DateTime timeOut = DateTime.Now + timeToAnswer;

            var cl = Console.CursorLeft;
            var ct = Console.CursorTop;

            Console.Beep();
            Console.Beep();

            while (Console.KeyAvailable == false && DateTime.Now < timeOut)
            {
                Console.SetCursorPosition(cl, ct);
                Console.Write($"{Question}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($" {lastActivity}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" [press 'return' to use last activity]");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" ({timeOut - DateTime.Now:ss}s)");

                Console.ResetColor();
                Console.SetCursorPosition(cl, ct);
                Console.Write($"{Question} ");
                Thread.Sleep(250);
            }

            if (DateTime.Now < timeOut)
            {
                Console.SetCursorPosition(cl, ct);
                Console.Write($"".PadRight(Console.LargestWindowWidth));
                Console.SetCursorPosition(cl, ct);
                Console.Write($"{Question} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string answer = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(answer))
                {
                    actualActivity = lastActivity; // nur Enter gedrückt
                }
                else
                {
                    actualActivity = answer;
                } 
            }

            Console.SetCursorPosition(cl, ct);
            Console.Write($"".PadRight(Console.LargestWindowWidth));
            Console.SetCursorPosition(cl, ct);
            Console.ResetColor();
            Console.Write($"{Question}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($" {actualActivity}");
            Console.ResetColor();

            Console.Beep();

            return actualActivity;
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

        private void StartMenue()
        {
            cts = new CancellationTokenSource();
            MenueTask = Task.Factory.StartNew(() => MenueHandler(cts.Token), cts.Token);
        }

        private void StopMenue()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
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
