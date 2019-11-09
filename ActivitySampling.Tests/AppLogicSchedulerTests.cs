using System;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Question;
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
            IQuestionScheduler sut = new QuestionScheduler();
            Assert.That(sut != null);
        }

        [Test]
        public void Scheduler_Call_Start()
        {
            IQuestionScheduler sut = new QuestionScheduler();
            sut.Start(TimeSpan.FromMinutes(20));
            Assert.That(sut.IsRunning == true);
        }

        [Test]
        public void Scheduler_Call_Stop()
        {
            IQuestionScheduler sut = new QuestionScheduler();
            sut.Stop();
            Assert.That(sut.IsRunning == false);
        }

    }
}