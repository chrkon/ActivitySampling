using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ActivitySampling.Module.View.CLI
{
    public class CommandLineInterface : Interfaces.ICommandLineInterface
    {
        public bool KeyAvailable
        {
            get { return Console.KeyAvailable; }
        }

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return Console.ReadKey(intercept);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer)
        {
            string actualActivity = string.Empty;
            DateTime timeOut = DateTime.Now + timeToAnswer;
            DateTime nextBeep = DateTime.Now + TimeSpan.FromSeconds(5);

            var cl = Console.CursorLeft;
            var ct = Console.CursorTop;

            Console.Beep();
            Console.Beep();

            while (this.KeyAvailable == false && DateTime.Now < timeOut)
            {
                if (DateTime.Now > nextBeep)
                {
                    nextBeep = DateTime.Now + TimeSpan.FromSeconds(5);
                    Console.Beep();
                }

                Console.SetCursorPosition(cl, ct);
                Console.Write($"{question}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($" {lastActivity}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" [press 'return' to use last activity]");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($" ({timeOut - DateTime.Now:ss}s)");

                Console.ResetColor();
                Console.SetCursorPosition(cl, ct);
                Console.Write($"{question} ");
                Thread.Sleep(250);
            }

            if (DateTime.Now < timeOut)
            {
                Console.SetCursorPosition(cl, ct);
                Console.Write($"".PadRight(Console.LargestWindowWidth));
                Console.SetCursorPosition(cl, ct);
                Console.Write($"{question} ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string answer = this.ReadLine();
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
            Console.Write($"{question}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($" {actualActivity}");
            Console.ResetColor();

            Console.Beep();

            return actualActivity;
        }

    }
}
