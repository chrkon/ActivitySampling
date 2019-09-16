using System;
using System.Text;
using System.Threading;
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

        public void AskForActivity(DateTime timeStampOfQuestion, TimeSpan interval, string lastActivity)
        {
            var actualActivity = ShowQuestion(lastActivity, timeStampOfQuestion, interval, TimeToAnswer);
            HandleAnswer(actualActivity);
        }

        private const string Question = "Was machst Du gerade?";
        private const string CancelKey = "x";
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
                Console.Write($"  [enter '{CancelKey}' to close, press Return to use last activity]");
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
                if (actualActivity == CancelKey)
                {
                    OnRaiseApplicationCloseEvent(new EventArgs());
                }
                else
                {
                    TimeStampOfAnswer = DateTime.Now;
                    OnRaiseActivityAddedEvent(new ActivityAddedEventArgs(TimeStampOfAnswer, actualActivity));
                }
            }
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
