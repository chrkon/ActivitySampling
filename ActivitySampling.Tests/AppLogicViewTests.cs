
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using ActivitySampling.Module.View.CLI;

namespace ActivitySampling.Tests
{
    [SingleThreaded]
    public class AppLogicViewTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void View_instance_is_created()
        {
            var sut = new ViewCLI();
            Assert.That(sut != null);
        }

        [Test]
        public async Task View_CallAskForActivity()
        {
            var sut = new ViewCLI();
            sut.RaiseActivityAddedEvent += Sut_RaiseActivityAddedEvent;
            sut.AskForActivity(DateTime.Now, TimeSpan.FromMinutes(20), "No Activity");
            await Task.Delay(500);
        }

        private void Sut_RaiseActivityAddedEvent(object sender, Interfaces.ActivityAddedEventArgs e)
        {
            Assert.That(e.Description, Is.Not.Empty);
        }
    }
}