﻿namespace Pipeline.Unit.Tests
{
    using Autofac;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PipelineActionShould
    {
        [Fact]
        public void StoreStepType()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);

            //action
            var item = new PipelineAction(stepType, inputType);

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
            var item = new PipelineAction(stepType, inputType);

            //assert
            item.InputType.Should().Be(inputType);
        }

        [Fact]
        public void StoreDefaultLink()
        {
            // arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new PipelineAction(stepType, inputType);
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
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new PipelineAction(stepType, inputType);
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
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var item = new PipelineAction(stepType, inputType);
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
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);
            var outputType = typeof(int);
            var item = new PipelineAction(stepType, inputType);
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
    }
}
