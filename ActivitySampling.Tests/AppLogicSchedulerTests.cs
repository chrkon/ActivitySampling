using System;
using ActivitySampling.Interfaces;
using ActivitySampling.Module.Scheduler.Question;
using Xunit;

namespace ActivitySampling.Tests
{
    public class AppLogicSchedulerTests
    {
        [Fact]
        public void Scheduler_GetSchedulerSingletonInstance()
        {
            IQuestionScheduler sut = new QuestionScheduler();
            Assert.NotNull(sut);
        }

        [Fact]
        public void Scheduler_Call_Start()
        {
            IQuestionScheduler sut = new QuestionScheduler();
            sut.Start(TimeSpan.FromMinutes(20));
            Assert.True(sut.IsRunning);
        }

        [Fact]
        public void Scheduler_Call_Stop()
        {
            IQuestionScheduler sut = new QuestionScheduler();
            sut.Stop();
            Assert.False(sut.IsRunning);
        }

    }
}