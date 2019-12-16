
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
            AutoResetEvent _eventIsCalled = new AutoResetEvent(false);
            var sut = new ViewCLI();
            sut.CLI = new CommandLineInterfaceFake();
            sut.TimeToAnswer = TimeSpan.FromMilliseconds(250);
            sut.RaiseNoActivityEvent += (sender,e ) => _eventIsCalled.Set();
            sut.AskForActivity(DateTime.Now, "");
            var wasEventCalled = _eventIsCalled.WaitOne(300);
            Assert.True(wasEventCalled);
        }

        [Fact]
        public async Task View_CallAskForActivity_WithInput()
        {
            string receivedActivity = string.Empty;
            var sut = new ViewCLI();
            sut.TimeToAnswer = TimeSpan.FromMilliseconds(500);
            sut.CLI = new CommandLineInterfaceFake();
            AutoResetEvent _eventIsCalled = new AutoResetEvent(false);
            sut.RaiseActivityAddedEvent += (sender,e ) => {
                receivedActivity = e.Description;
                _eventIsCalled.Set();
            };

            string expectedActivity = "last Activity";
            sut.AskForActivity(DateTime.Now, expectedActivity); 
            Assert.Equal(expectedActivity, receivedActivity);
        }

        [Fact]
        public async Task View_pressKeyX_AppEnds()
        {
            AutoResetEvent _eventIsCalled = new AutoResetEvent(false);
            var sut = new ViewCLI();
            sut.CLI = new CommandLineInterfaceFake();
            sut.RaiseApplicationCloseEvent += (sender, e) => _eventIsCalled.Set();
            sut.ActivateMenu();
            ((CommandLineInterfaceFake)sut.CLI).NextInputKey = ConsoleKey.X;
            var wasEventCalled = _eventIsCalled.WaitOne(1000);
            Assert.True(wasEventCalled);
        }

    }
}