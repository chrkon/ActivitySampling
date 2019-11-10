using System;
using System.Collections.Generic;
using System.Text;
using ActivitySampling.Interfaces;

namespace ActivitySampling.Tests
{
    public class CommandLineInterfaceFake : Interfaces.ICommandLineInterface
    {
        public char? NextInputKey {get; set;}
        public string NextInputString {get; set;}

        public bool KeyAvailable => NextInputKey.HasValue; 

        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo(NextInputKey.Value,ConsoleKey.NoName, false, false, false);
            return cki;
        }

        public string ReadLine()
        {
            return NextInputString;
        }

        public string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan timeToAnswer)
        {
            return lastActivity;
        }
    }
}
