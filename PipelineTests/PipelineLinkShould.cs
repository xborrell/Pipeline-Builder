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

    public class PipelineLinkShould
    {
        [Fact]
        public void LinkSourceWithLinkWhenCreateLinks()
        {
            //arrange
            var source = Substitute.For<IPipelineSource>();
            var target = Substitute.For<IPipelineTarget>();

            //Action
            new PipelineLink(true, source, target);

            //Assert
            source.Received(1).AddOutputLink(Arg.Any<IPipelineLink>());
        }

        [Fact]
        public void LinkTargetWithLinkWhenCreateLinks()
        {
            //arrange
            var source = Substitute.For<IPipelineSource>();
            var target = Substitute.For<IPipelineTarget>();

            //Action
            new PipelineLink(true, source, target);

            //Assert
            target.Received(1).AddInputLink(Arg.Any<IPipelineLink>());
        }
    }
}
