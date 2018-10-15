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
    using TASuite.Commons.Crosscutting;
    using Xunit;

    public class PipelineActionShould
    {
        private readonly IDataflowPipeline<int> pipeline;
        private readonly IIoCAbstractFactory factory;

        public PipelineActionShould()
        {
            factory = Substitute.For<IIoCAbstractFactory>();
            pipeline = Substitute.For<IDataflowPipeline<int>>();
            pipeline.BlockOptions.Returns(new ExecutionDataflowBlockOptions());
        }

        [Fact]
        public void StoreDefaultLink()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(true);

            //action
            item.AddInputLink(link);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(link);
        }

        [Fact]
        public void StoreNormalLink()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();
            var link = Substitute.For<IPipelineLink>();
            link.IsDefault.Returns(false);

            //action
            item.AddInputLink(link);

            //assert
            item.InputLinks.Count().Should().Be(1);
            item.InputLinks.First().Should().Be(link);
        }

        [Fact]
        public void ReplaceDefaultLink()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();
            var defaultLink = Substitute.For<IPipelineLink>();
            defaultLink.IsDefault.Returns(true);
            item.AddInputLink(defaultLink);

            defaultLink.When(x => x.Remove()).Do(x => item.RemoveInputLink(defaultLink));

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
            var item = new PipelineAction<IIntAction, int>();
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
        public void CallTheFactoryWhenBuildsBlock()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            factory.Received(1).Resolve<IIntAction>();
        }

        [Fact]
        public void BuildsBlock()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            var block = item.Blocks.FirstOrDefault();

            block.Should().NotBeNull();
            block.Should().BeAssignableTo<ActionBlock<int>>();
        }

        [Fact]
        public void AddsTheBlockAsEndStep()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            var block = item.Blocks.FirstOrDefault();
            pipeline.Received(1).AddEndStep(block);

        }

        [Fact]
        public void GetsTheBlockAsTarget()
        {
            // arrange
            var item = new PipelineAction<IIntAction, int>();
            item.BuildBlock(pipeline, factory);
            var normalLink = Substitute.For<IPipelineLink>();
            normalLink.IsDefault.Returns(false);
            item.AddInputLink(normalLink);

            //action
            var target = item.GetAsTarget<int>(normalLink);

            //assert
            target.Should().NotBeNull();
            target.Should().BeAssignableTo<ITargetBlock<int>>();
        }
    }
}
