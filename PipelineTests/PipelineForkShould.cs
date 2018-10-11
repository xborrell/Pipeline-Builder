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

    public class PipelineForkShould
    {
        private readonly IPipelineFactory<int> factory;
        private readonly IDataflowPipeline<int> pipeline;

        public PipelineForkShould()
        {
            factory = Substitute.For<IPipelineFactory<int>>();
            pipeline = Substitute.For<IDataflowPipeline<int>>();
            pipeline.BlockOptions.Returns(new ExecutionDataflowBlockOptions());
        }

        [Fact]
        public void RejectsDefaultInputLink()
        {
            // arrange
            var item = new PipelineFork();
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
            var item = new PipelineFork();
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
            var item = new PipelineFork();
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
        public void StoreNormalOutputLink()
        {
            // arrange
            var item = new PipelineFork();
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
            var item = new PipelineFork();
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

        [Fact]
        public void BuildsBlock()
        {
            // arrange
            var item = new PipelineFork();
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);
            item.AddInputLink(link);
            link.Type.Returns(typeof(int));

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            item.Block.Should().NotBeNull();
            item.Block.Should().BeAssignableTo<BroadcastBlock<int>>();
        }

        [Fact]
        public void GetsTheBlockAsSource()
        {
            // arrange
            var item = new PipelineFork();
            var inputLink = Substitute.For<IPipelineLink>();
            inputLink.IsDefault.Returns(false);
            item.AddInputLink(inputLink);
            inputLink.Type.Returns(typeof(int));

            var outputLink = Substitute.For<IPipelineLink>();
            outputLink.IsDefault.Returns(false);
            item.AddOutputLink(outputLink);
            outputLink.Type.Returns(typeof(int));

            item.BuildBlock(pipeline, factory);

            //action
            var source = item.GetAsSource<int>(inputLink);

            //assert
            source.Should().NotBeNull();
            source.Should().BeAssignableTo<ISourceBlock<int>>();
        }

        [Fact]
        public void GetsTheBlockAsTarget()
        {
            // arrange
            var item = new PipelineFork();
            var inputLink = Substitute.For<IPipelineLink>();
            inputLink.IsDefault.Returns(false);
            item.AddInputLink(inputLink);
            inputLink.Type.Returns(typeof(int));

            item.BuildBlock(pipeline, factory);

            //action
            var target = item.GetAsTarget<int>(inputLink);

            //assert
            target.Should().NotBeNull();
            target.Should().BeAssignableTo<ITargetBlock<int>>();
        }
    }
}
