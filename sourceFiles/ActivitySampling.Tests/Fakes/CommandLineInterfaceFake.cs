﻿using System;

namespace ActivitySampling.Tests
{
    public class CommandLineInterfaceFake : Interfaces.ICommandLineInterface
    {
        public ConsoleKey? NextInputKey {get; set;}
        public string NextInputString {get; set;}

        public bool KeyAvailable => NextInputKey.HasValue;

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            if (NextInputKey.HasValue)
            {
                cki = new ConsoleKeyInfo(char.MinValue, NextInputKey.Value, false, false, false);
            }
            return cki;
        }

        public string ReadLine()
        {
            return NextInputString;
        }

        public void WriteLine(string text)
        {
            // no action
        }

        public string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer, string hint)
        {
            return lastActivity;
        }

        public string ShowLastQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer, string hint)
        {
            return lastActivity + " (edited)";
        }
    }
}
