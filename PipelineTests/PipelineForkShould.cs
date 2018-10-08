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

    public class PipelineForkShould
    {
        [Fact]
        public void StoreInputType()
        {
            // arrange
            var inputType = typeof(int);

            //action
            var item = new PipelineFork(inputType);

            //assert
            item.InputType.Should().Be(inputType);
        }

        [Fact]
        public void StoreOutputType()
        {
            // arrange
            var inputType = typeof(int);

            //action
            var item = new PipelineFork(inputType);

            //assert
            item.OutputType.Should().Be(inputType);
        }

        [Fact]
        public void RejectsDefaultInputLink()
        {
            // arrange
            var inputType = typeof(int);
            var item = new PipelineFork(inputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(true);

            //action
            Action acc = () => item.AddInputLink(link);

            //assert
            acc.Should().Throw<Exception>();
        }

        [Fact]
        public void StoreNormalInputLink()
        {
            // arrange
            var inputType = typeof(int);
            var item = new PipelineFork(inputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddInputLink(link);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(link);
        }

        [Fact]
        public void RejectsNormalInputLinkOverAnotherLink()
        {
            // arrange
            var inputType = typeof(int);
            var item = new PipelineFork(inputType);
            var normalLink1 = Substitute.For<IPipelineLink>();
            normalLink1.IsDefault.Returns(false);
            item.AddInputLink(normalLink1);

            var normalLink2 = Substitute.For<IPipelineLink>();
            normalLink2.IsDefault.Returns(false);

            //action
            Action acc = () => item.AddInputLink(normalLink2);

            //assert
            acc.Should().Throw<Exception>();
        }

        [Fact]
        public void RejectsDefaultOutputLink()
        {
            // arrange
            var inputType = typeof(int);
            var item = new PipelineFork(inputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(true);

            //action
            Action acc = () => item.AddOutputLink(link);

            //assert
            acc.Should().Throw<Exception>();
        }

        [Fact]
        public void StoreNormalOutputLink()
        {
            // arrange
            var inputType = typeof(int);
            var item = new PipelineFork(inputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddOutputLink(link);

            //assert
            item.OutputLinks.Count().Should().Be(1);
            item.OutputLinks.First().Should().Be(link);
        }

        [Fact]
        public void StoreTwoNormalOutputLinks()
        {
            // arrange
            var inputType = typeof(int);
            var item = new PipelineFork(inputType);
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(false);
            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(false);
            item.AddOutputLink(link1);

            //action
            item.AddOutputLink(link2);

            //assert
            item.OutputLinks.Count().Should().Be(2);
            item.OutputLinks.First().Should().Be(link1);
            item.OutputLinks.Last().Should().Be(link2);
        }
    }
}
