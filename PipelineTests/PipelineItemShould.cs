namespace Pipeline.Unit.Tests
{
    using Autofac;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PipelineItemShould
    {
        [Fact]
        public void StoreStepType()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);

            //action
            var item = new TestPipelineItem(stepType, inputType);

            //assert
            item.Step.Should().Be(stepType);
        }

        [Fact]
        public void StoreInputType()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);

            //action
            var item = new TestPipelineItem(stepType, inputType);

            //assert
            item.Step.Should().Be(stepType);
        }

        [Fact]
        public void StoreLink()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new TestPipelineItem(stepType, inputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(true);

            //action
            item.AddLink(link);

            //assert
            item.Links.Count().Should().Be(1);
            item.Links.First().Should().Be(link);
        }

        private class TestPipelineItem : PipelineItem
        {
            public TestPipelineItem(Type step, Type input) : base(step, input) { }
        }
    }
}
