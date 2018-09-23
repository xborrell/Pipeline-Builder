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

        [Fact]
        public void ThrowExceptionWhenTheFirstLinkIsNotDefault()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new TestPipelineItem(stepType, inputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            Action acc = () => item.AddLink(link);

            //assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void ThrowExceptionWhenAddAnotherDefaultLink()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new TestPipelineItem(stepType, inputType);
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(true);
            item.AddLink(link1);

            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(true);

            //action
            Action acc = () => item.AddLink(link2);

            //assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void ReplaceTheDefaultLinkWhenAnotherLinkIsProvided()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new TestPipelineItem(stepType, inputType);
            var defaultLink = Substitute.For<IPipelineLink>();
            defaultLink.IsDefault.Returns(true);
            item.AddLink(defaultLink);

            var explicitLink = Substitute.For<IPipelineLink>();
            explicitLink.IsDefault.Returns(false);

            //action
            item.AddLink(explicitLink);

            //assert
            item.Links.Count().Should().Be(1);
            item.Links.First().Should().Be(explicitLink);
        }

        private class TestPipelineItem : PipelineItem
        {
            public TestPipelineItem(Type step, Type input) : base(step, input) { }
        }
    }
}
