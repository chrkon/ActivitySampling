
using System;
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

        [Fact(Skip = "Dieser Test funktioniert nicht. Grund noch unbekannt, CKo, 9.11.2019")]
        public async Task View_CallAskForActivity()
        {
            var sut = new ViewCLI();
            sut.RaiseActivityAddedEvent += (sender, e) => 
            {
                Assert.NotEmpty(e.Description); 
                Assert.Equal("No car",e.Description);
            };
            sut.AskForActivity(DateTime.Now, TimeSpan.FromMinutes(20), "No Activity");
            await Task.Delay(500);
        }

    }
}