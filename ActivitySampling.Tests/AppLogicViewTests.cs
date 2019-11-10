
using System;
using System.Threading;
using System.Threading.Tasks;
using ActivitySampling.Module.View.CLI;
using Xunit;

namespace ActivitySampling.Tests
{
    public class AppLogicViewTests
    {
        [Fact]
        public void View_instance_is_created()
        {
            var sut = new ViewCLI();
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task View_CallAskForActivity_noInput()
        {
            var sut = new ViewCLI();
            sut.TimeToAnswer = TimeSpan.FromMilliseconds(250);
            AutoResetEvent _eventIsCalled = new AutoResetEvent(false);
            sut.RaiseNoActivityEvent += (sender,e ) => _eventIsCalled.Set();

            sut.AskForActivity(DateTime.Now, TimeSpan.FromMinutes(20), "");
            Assert.True(_eventIsCalled.WaitOne());
        }

        [Fact]
        public async Task View_CallAskForActivity_WithKeyInput()
        {
            var sut = new ViewCLI();
            sut.TimeToAnswer = TimeSpan.FromMilliseconds(500);
            sut.CLI = new CommandLineInterfaceFake();
            AutoResetEvent _eventIsCalled = new AutoResetEvent(false);
            sut.RaiseNoActivityEvent += (sender,e ) => _eventIsCalled.Set();

            sut.AskForActivity(DateTime.Now, TimeSpan.FromMinutes(20), ""); 
            Assert.True(_eventIsCalled.WaitOne());
        }

    }
}