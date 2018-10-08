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

    public class PipelineTransformationShould
    {
        [Fact]
        public void StoreStepType()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);

            //action
            var item = new PipelineTransformation(stepType, inputType, outputType);

            //assert
            item.Step.Should().Be(stepType);
        }

        [Fact]
        public void StoreInputType()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);

            //action
            var item = new PipelineTransformation(stepType, inputType, outputType);

            //assert
            item.InputType.Should().Be(inputType);
        }

        [Fact]
        public void StoreOutputType()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);

            //action
            var item = new PipelineTransformation(stepType, inputType, outputType);

            //assert
            item.OutputType.Should().Be(outputType);
        }

        [Fact]
        public void StoreDefaultInputLink()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(true);

            //action
            item.AddInputLink(link);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(link);
        }

        [Fact]
        public void StoreNormalInputLink()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddInputLink(link);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(link);
        }

        [Fact]
        public void ReplaceDefaultInputLink()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var defaultLink = Substitute.For<IPipelineLink>();
            defaultLink.IsDefault.Returns(true);
            item.AddInputLink(defaultLink);

            var normalLink = Substitute.For<IPipelineLink>();
            normalLink.IsDefault.Returns(false);

            //action
            item.AddInputLink(normalLink);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(normalLink);
        }

        [Fact]
        public void AcceptsTwoInputLinks()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var normalLink1 = Substitute.For<IPipelineLink>();
            normalLink1.IsDefault.Returns(false);
            item.AddInputLink(normalLink1);

            var normalLink2 = Substitute.For<IPipelineLink>();
            normalLink2.IsDefault.Returns(false);

            //action
            item.AddInputLink(normalLink2);

            //assert
            item.InputLinks.Count().Should().Be(2);
            item.InputLinks.First().Should().Be(normalLink1);
            item.InputLinks.Last().Should().Be(normalLink2);
        }

        [Fact]
        public void StoreDefaultOutputLink()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(true);

            //action
            item.AddOutputLink(link);

            //assert
            item.OutputLinks.Count().Should().Be(1);
            item.OutputLinks.First().Should().Be(link);
        }

        [Fact]
        public void StoreNormalOutputLink()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddOutputLink(link);

            //assert
            item.OutputLinks.Count().Should().Be(1);
            item.OutputLinks.First().Should().Be(link);
        }

        [Fact]
        public void ReplaceDefaultOutputLink()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var target = Substitute.For<IPipelineTarget>();

            var item = new PipelineTransformation(stepType, inputType, outputType);
            var defaultLink = Substitute.For<IPipelineLink>();
            defaultLink.IsDefault.Returns(true);
            defaultLink.Source.Returns(item);
            defaultLink.Target.Returns(target);
            item.AddOutputLink(defaultLink);

            var normalLink = Substitute.For<IPipelineLink>();
            normalLink.IsDefault.Returns(false);
            normalLink.Source.Returns(item);
            normalLink.Target.Returns(target);

            //action
            item.AddOutputLink(normalLink);

            //assert
            item.OutputLinks.Count().Should().Be(1);
            item.OutputLinks.First().Should().Be(normalLink);
        }

        [Fact]
        public void AcceptsTwoOutputLinks()
        {
            // arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineTransformation(stepType, inputType, outputType);
            var normalLink1 = Substitute.For<IPipelineLink>();
            normalLink1.IsDefault.Returns(false);
            item.AddOutputLink(normalLink1);

            var normalLink2 = Substitute.For<IPipelineLink>();
            normalLink2.IsDefault.Returns(false);

            //action
            item.AddOutputLink(normalLink2);

            //assert
            item.OutputLinks.Count().Should().Be(2);
            item.OutputLinks.First().Should().Be(normalLink1);
            item.OutputLinks.Last().Should().Be(normalLink2);
        }
    }
}
