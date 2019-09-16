using System;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Quartz;
using NUnit.Framework;

namespace ActivitySampling.Tests
{
    public class AppLogicSchedulerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Scheduler_GetSchedulerSingletonInstance()
        {
            IQuestionScheduler sut = new QuartzScheduler();
            Assert.That(sut != null);
        }

        [Test]
        public void Scheduler_Call_Start()
        {
            IQuestionScheduler sut = new QuartzScheduler();
            sut.Start(TimeSpan.FromMinutes(20));
            Assert.That(sut.IsRunning == true);
        }

        [Test]
        public void Scheduler_Call_Stop()
        {
            IQuestionScheduler sut = new QuartzScheduler();
            sut.Stop();
            Assert.That(sut.IsRunning == false);
        }

    }
}