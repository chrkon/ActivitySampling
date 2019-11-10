using System;
using System.Collections.Generic;
using System.Text;

namespace ActivitySampling.Tests
{
    public class CommandLineInterfaceFake : Interfaces.ICommandLineInterface
    {
        public string ShowQuestion(string question, string lastActivity, DateTime timeStampOfQuestion, TimeSpan workingInterval, TimeSpan timeToAnswer)
        {
            throw new NotImplementedException();
        }
    }
}
