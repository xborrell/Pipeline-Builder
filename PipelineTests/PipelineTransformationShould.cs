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

    public class PipelineTransformationShould
    {
        private readonly IIoCAbstractFactory factory;
        private readonly IDataflowPipeline<int> pipeline;

        public PipelineTransformationShould()
        {
            factory = Substitute.For<IIoCAbstractFactory>();
            pipeline = Substitute.For<IDataflowPipeline<int>>();
            pipeline.BlockOptions.Returns(new ExecutionDataflowBlockOptions());
        }

        [Fact]
        public void StoreDefaultInputLink()
        {
            // arrange
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var target = Substitute.For<IPipelineTarget>();

            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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

        [Fact]
        public void CallTheFactoryWhenBuildsBlock()
        {
            // arrange
            var item = new PipelineTransformation<IIntTransformation, int, int>();

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            factory.Received(1).Resolve<IIntTransformation>();
        }

        [Fact]
        public void BuildsBlock()
        {
            // arrange
            var item = new PipelineTransformation<IIntTransformation, int, int>();

            //action
            item.BuildBlock(pipeline, factory);

            //assert
            item.Block.Should().NotBeNull();
            item.Block.Should().BeAssignableTo<TransformBlock<int, int>>();
        }

        [Fact]
        public void GetsTheBlockAsSource()
        {
            // arrange
            var item = new PipelineTransformation<IIntTransformation, int, int>();
            item.BuildBlock(pipeline, factory);
            var normalLink = Substitute.For<IPipelineLink>();
            normalLink.IsDefault.Returns(false);
            item.AddOutputLink(normalLink);

            //action
            var source = item.GetAsSource<int>(normalLink);

            //assert
            source.Should().NotBeNull();
            source.Should().BeAssignableTo<ISourceBlock<int>>();
        }

        [Fact]
        public void GetsTheBlockAsTarget()
        {
            // arrange
            var item = new PipelineTransformation<IIntTransformation, int, int>();
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
