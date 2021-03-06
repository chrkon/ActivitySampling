﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public async void Beep()
        {
            // Console.Beep();

            var cposX = Console.CursorLeft;
            var cposY = Console.CursorTop;
            var previousBackColor = Console.BackgroundColor;
            var previousForegroundColor = Console.ForegroundColor;

            await Task.Run(() => ShowBlinkingBar(5, cposX, cposY+2));

            Console.SetCursorPosition(cposX, cposY);
            Console.BackgroundColor = previousBackColor;
            Console.ForegroundColor = previousForegroundColor;
            Console.Write("                    ");
            Console.SetCursorPosition(cposX, cposY);
        }

        private void ShowBlinkingBar(int secondsToBlink, int x, int y)
        {
            var previousBackColor = Console.BackgroundColor;
            var previousForegroundColor = Console.ForegroundColor;

            var start = DateTime.Now;
            var stop = DateTime.Now + TimeSpan.FromSeconds(secondsToBlink);
            while (DateTime.Now < stop)
            {
                Task.Delay(250).Wait();
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("    ----    ----    ");
                Console.SetCursorPosition(x, y);
                Task.Delay(500).Wait();
                Console.BackgroundColor = previousBackColor;
                Console.ForegroundColor = previousForegroundColor;
                Console.Write("    ----    ----    ");
                Console.SetCursorPosition(x, y);
                Task.Delay(250).Wait();
            }
        }


        public string ShowLastQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer, string hint)
        {
            var cl = 0;
            var ct = Console.CursorTop;
            if (ct > 1) ct--;
            Console.SetCursorPosition(cl,ct);
            return ShowQuestion(question, lastActivity, timeStampOfQuestion, timeToAnswer, hint);
        }

        public string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer, string hint)
        {
            string actualActivity = string.Empty;
            DateTime timeOut = DateTime.Now + timeToAnswer;
            DateTime nextBeep = DateTime.Now + TimeSpan.FromSeconds(5);

            var cl = Console.CursorLeft;
            var ct = Console.CursorTop;

            Beep();
            Beep();

            while (this.KeyAvailable == false && DateTime.Now < timeOut)
            {
                if (DateTime.Now > nextBeep)
                {
                    nextBeep = DateTime.Now + TimeSpan.FromSeconds(5);
                    Beep();
                }

                Console.SetCursorPosition(cl, ct);
                Console.Write($"{question}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($" {lastActivity}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" {hint}");
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

            Beep();

            return actualActivity;
        }

    }
}
