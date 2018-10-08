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

    public class PipelineJoinShould
    {
        [Fact]
        public void StoreInputTypes()
        {
            // arrange
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);

            //action
            var item = new PipelineJoin(inputType1, inputType2);

            //assert
            item.InputType.Should().Be(inputType1);
            item.InputType2.Should().Be(inputType2);
        }

        [Fact]
        public void StoreOutputType()
        {
            // arrange
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);

            //action
            var item = new PipelineJoin(inputType1, inputType2);

            //assert
            item.OutputType.Should().Be(typeof(Tuple<int,int>));
        }

        [Fact]
        public void RejectsDefaultInputLink()
        {
            // arrange
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);
            var item = new PipelineJoin(inputType1, inputType2);
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
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);
            var item = new PipelineJoin(inputType1, inputType2);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddInputLink(link);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(link);
        }

        [Fact]
        public void StoreTwoNormalInputLink()
        {
            // arrange
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);
            var item = new PipelineJoin(inputType1, inputType2);
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(false);
            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(false);
            item.AddInputLink(link1);

            //action
            item.AddInputLink(link2);

            //assert
            item.InputLinks.Count().Should().Be(2);
            item.InputLinks.First().Should().Be(link1);
            item.InputLinks.Last().Should().Be(link2);
        }

        [Fact]
        public void RejectsDefaultOutputLink()
        {
            // arrange
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);
            var item = new PipelineJoin(inputType1, inputType2);
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
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);
            var item = new PipelineJoin(inputType1, inputType2);
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddOutputLink(link);

            //assert
            item.OutputLinks.Count().Should().Be(1);
            item.OutputLinks.First().Should().Be(link);
        }

        [Fact]
        public void RejectsNormalOutputLinkOverAnotherLink()
        {
            // arrange
            var inputType1 = typeof(int);
            var inputType2 = typeof(int);
            var item = new PipelineJoin(inputType1, inputType2);
            var normalLink1 = Substitute.For<IPipelineLink>();
            normalLink1.IsDefault.Returns(false);
            item.AddOutputLink(normalLink1);

            var normalLink2 = Substitute.For<IPipelineLink>();
            normalLink2.IsDefault.Returns(false);

            //action
            Action acc = () => item.AddOutputLink(normalLink2);

            //assert
            acc.Should().Throw<Exception>();
        }
    }
}
