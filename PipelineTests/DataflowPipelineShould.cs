﻿namespace Pipeline.Unit.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using Xunit;
    using System.Threading.Tasks.Dataflow;
    using System;

    public class DataflowPipelineShould
    {
        private readonly Dictionary<string, int> actionResults = new Dictionary<string, int>();

        [Fact]
        public async Task AddAction()
        {
            //Arrange
            var value = 0;
            var pipeline = new DataflowPipeline<int, string>();
            var block = new ActionBlock<int>(receivedValue => value = receivedValue, pipeline.BlockOptions);

            //Action
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
            var pipeline = new DataflowPipeline<int, string>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);
            var block2 = new ActionBlock<int>(receivedValue => value = receivedValue, pipeline.BlockOptions);

            //Action
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
            var pipeline = new DataflowPipeline<int, string>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);
            var fork = new BroadcastBlock<int>(receivedValue => receivedValue);
            var block2 = new ActionBlock<int>(receivedValue => value1 = receivedValue * 2, pipeline.BlockOptions);
            var block3 = new ActionBlock<int>(receivedValue => value2 = receivedValue * 3, pipeline.BlockOptions);

            //Action
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

        [Fact]
        public async Task PutAJoin()
        {
            //Arrange
            var result = "";
            var pipeline = new DataflowPipeline<int, string>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);
            var fork = new BroadcastBlock<int>(receivedValue => receivedValue);
            var block1a = new TransformBlock<int, string>(receivedValue => receivedValue.ToString(), pipeline.BlockOptions);
            var block2a = new TransformBlock<int, int>(receivedValue => receivedValue *2, pipeline.BlockOptions);
            var join = new JoinBlock<int, string>();
            var block3 = new ActionBlock<Tuple<int, string>>(receivedValue => result = $"{receivedValue.Item2} => {receivedValue.Item1}", pipeline.BlockOptions);

            //Action
            pipeline.AddBlock(block1);
            pipeline.AddBlock(fork);
            pipeline.AddBlock(block1a);
            pipeline.AddBlock(block2a);
            pipeline.AddBlock(join);
            pipeline.AddBlock(block3);

            pipeline.AddEndStep(block3);

            block1.LinkTo(fork, pipeline.LinkOptions);
            fork.LinkTo(block1a, pipeline.LinkOptions);
            fork.LinkTo(block2a, pipeline.LinkOptions);
            block1a.LinkTo(join.Target2, pipeline.LinkOptions);
            block2a.LinkTo(join.Target1, pipeline.LinkOptions);
            join.LinkTo(block3, pipeline.LinkOptions);

            //Assert
            pipeline.Post(1);
            pipeline.Complete();

            await pipeline.Completion.ConfigureAwait(false);

            result.Should().Be("2 => 4");
        }

        [Fact]
        public async Task GetReturnSingleData()
        {
            //Arrange
            var value = 0;
            var pipeline = new DataflowPipeline<int, int>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);

            pipeline.AddBlock(block1);
            pipeline.SetOutput(x => value = x );

            //action
            pipeline.Post(1);
            pipeline.Complete();

            await pipeline.Completion.ConfigureAwait(false);

            //Assert
            value.Should().Be(2);
        }

        [Fact]
        public async Task GetReturnMultipleData()
        {
            //Arrange
            var values = new List<int>();
            var pipeline = new DataflowPipeline<int, int>();
            var block1 = new TransformBlock<int, int>(receivedValue => receivedValue + 1, pipeline.BlockOptions);

            pipeline.AddBlock(block1);
            pipeline.SetOutput(x => values.Add(x) );

            //action
            pipeline.Post(1);
            pipeline.Post(2);
            pipeline.Complete();

            await pipeline.Completion.ConfigureAwait(false);

            //Assert
            values.Count.Should().Be(2);
            values[0].Should().Be(2);
            values[1].Should().Be(3);
        }
    }
}