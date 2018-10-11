namespace Pipeline.Unit.Tests
{
    using Autofac;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Xunit;

    public class PipelineJoinShould
    {
        private readonly IPipelineFactory<int> factory;
        private readonly IDataflowPipeline<int> pipeline;

        public PipelineJoinShould()
        {
            factory = Substitute.For<IPipelineFactory<int>>();
            pipeline = Substitute.For<IDataflowPipeline<int>>();
            pipeline.BlockOptions.Returns(new ExecutionDataflowBlockOptions());
        }
        [Fact]
        public void RejectsDefaultInputLink()
        {
            // arrange
            var item = new PipelineJoin();
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
            var item = new PipelineJoin();
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
            var item = new PipelineJoin();
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
            var item = new PipelineJoin();
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
            var item = new PipelineJoin();
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
            var item = new PipelineJoin();
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

        [Fact]
        public void BuildsBlock()
        {
            // arrange
            var item = new PipelineJoin();
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(false);
            item.AddInputLink(link1);
            link1.Type.Returns(typeof(int));

            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(false);
            item.AddInputLink(link2);
            link2.Type.Returns(typeof(string));

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            item.Block.Should().NotBeNull();
            item.Block.Should().BeAssignableTo<JoinBlock<int,string>>();
        }

        [Fact]
        public void GetsTheBlockAsSource()
        {
            // arrange
            var item = new PipelineJoin();
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(false);
            item.AddInputLink(link1);
            link1.Type.Returns(typeof(int));

            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(false);
            item.AddInputLink(link2);
            link2.Type.Returns(typeof(string));

            item.BuildBlock(pipeline, factory);

            //action
            var source = item.GetAsSource<Tuple<int, string>>(link1);

            //assert
            source.Should().NotBeNull();
            source.Should().BeAssignableTo<ISourceBlock<Tuple<int, string>>>();
        }

        [Fact]
        public void GetsTheBlockAsFirstTarget()
        {
            // arrange
            var item = new PipelineJoin();
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(false);
            item.AddInputLink(link1);
            link1.Type.Returns(typeof(int));

            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(false);
            item.AddInputLink(link2);
            link2.Type.Returns(typeof(string));

            item.BuildBlock(pipeline, factory);

            //action
            var target = item.GetAsTarget<int>(link1);

            //assert
            target.Should().NotBeNull();
            target.Should().BeAssignableTo<ITargetBlock<int>>();
        }

        [Fact]
        public void GetsTheBlockAsSecondTarget()
        {
            // arrange
            var item = new PipelineJoin();
            var link1 = Substitute.For<IPipelineLink>();
            link1.IsDefault.Returns(false);
            item.AddInputLink(link1);
            link1.Type.Returns(typeof(int));

            var link2 = Substitute.For<IPipelineLink>();
            link2.IsDefault.Returns(false);
            item.AddInputLink(link2);
            link2.Type.Returns(typeof(string));

            item.BuildBlock(pipeline, factory);

            //action
            var target = item.GetAsTarget<string>(link2);

            //assert
            target.Should().NotBeNull();
            target.Should().BeAssignableTo<ITargetBlock<string>>();
        }
    }
}
