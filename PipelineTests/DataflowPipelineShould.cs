namespace Pipeline.Unit.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using Xunit;
    using System.Threading.Tasks.Dataflow;

    public class DataflowPipelineShould
    {
        private readonly Dictionary<string, int> actionResults = new Dictionary<string, int>();

        [Fact]
        public async Task AddAction()
        {
            //Arrange
            var value = 0;
            var pipeline = new DataflowPipeline<int>();
            var block = new ActionBlock<int>(receivedValue => value = receivedValue, pipeline.BlockOptions);

            //Action
            pipeline.MarkAsFirstStep(block);
            pipeline.AddBlock(block);
            pipeline.AddEndStep(block);

            //Assert
            pipeline.Post(1);
            pipeline.Complete();

            await pipeline.Completion.ConfigureAwait(false);

            value.Should().Be(1);
        }

        [Fact]
        public async Task LinkTwoSteps()
        {
            //Arrange
            var value = 0;
            var pipeline = new DataflowPipeline<int>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);
            var block2 = new ActionBlock<int>(receivedValue => value = receivedValue, pipeline.BlockOptions);

            //Action
            pipeline.MarkAsFirstStep(block1);
            pipeline.AddBlock(block1);
            pipeline.AddBlock(block2);
            pipeline.AddEndStep(block2);

            block1.LinkTo(block2, pipeline.LinkOptions);

            //Assert
            pipeline.Post(1);
            pipeline.Complete();

            await pipeline.Completion.ConfigureAwait(false);

            value.Should().Be(2);
        }

        [Fact]
        public async Task PutAFork()
        {
            //Arrange
            var value1 = 0;
            var value2 = 0;
            var pipeline = new DataflowPipeline<int>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);
            var fork = new BroadcastBlock<int>(receivedValue => receivedValue);
            var block2 = new ActionBlock<int>(receivedValue => value1 = receivedValue * 2, pipeline.BlockOptions);
            var block3 = new ActionBlock<int>(receivedValue => value2 = receivedValue * 3, pipeline.BlockOptions);

            //Action
            pipeline.MarkAsFirstStep(block1);
            pipeline.AddBlock(block1);
            pipeline.AddBlock(block2);
            pipeline.AddBlock(block3);
            pipeline.AddBlock(fork);

            pipeline.AddEndStep(block2);
            pipeline.AddEndStep(block3);

            block1.LinkTo(fork, pipeline.LinkOptions);
            fork.LinkTo(block2, pipeline.LinkOptions);
            fork.LinkTo(block3, pipeline.LinkOptions);

            //Assert
            pipeline.Post(1);
            pipeline.Complete();

            await pipeline.Completion.ConfigureAwait(false);

            value1.Should().Be(4);
            value2.Should().Be(6);
        }
    }
}