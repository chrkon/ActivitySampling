using System;
using Xunit;

namespace ActivitySampling.Tests
{
    public class CommandLineInterfaceFakeTests
    {
        [Fact()]
        public void ReadKey_W_sucessful_Test()
        {
            var expected = ConsoleKey.W;
            var sut = new CommandLineInterfaceFake();
            sut.NextInputKey = expected;
            bool isKeyAvailable = sut.KeyAvailable;
            Assert.True(isKeyAvailable);
            ConsoleKeyInfo result = sut.ReadKey(false);
            Assert.True(result.Key == expected, "unexpected result");
        }

        [Fact()]
        public void ReadKey_NoKey_Test()
        {
            var sut = new CommandLineInterfaceFake();
            bool isKeyAvailable = sut.KeyAvailable;
            Assert.False(isKeyAvailable);
            ConsoleKeyInfo result = sut.ReadKey(false);
            Assert.True(result.Key == 0);
        }

    }
}