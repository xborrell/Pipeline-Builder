namespace Pipeline.Unit.Tests
{
    using Autofac;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class PipelineBuilderShould
    {
        private readonly IPipelineFactory factory;

        public PipelineBuilderShould()
        {
            factory = Substitute.For<IPipelineFactory>();

            factory.Create<IRejectableTransformation>().Returns(Substitute.For<IPipelineItem>());
            factory.Create<IIntAction>().Returns(new PipelineAction(typeof(IIntAction), typeof(int)));
            factory.Create<IIntTransformation>().Returns(new PipelineTransformation(typeof(IIntTransformation), typeof(int), typeof(int)));
            factory.Create<IIntToStringTransformation>().Returns(new PipelineTransformation(typeof(IIntToStringTransformation), typeof(int), typeof(string)));
        }

        [Fact]
        public void CreateWithDependencies()
        {
            //Arrange

            //Action
            var pipeline = new PipelineBuilder<int>(factory);

            //Assert
            pipeline.Should().NotBeNull();
        }

        [Fact]
        public void AcceptStepAsActionWhenImplementsICompilerAction()
        {
            var pipeline = new PipelineBuilder<int>(factory);

            //Action
            Action acc = () => pipeline.AddAction<IIntAction>();

            //Assert
            acc.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RejectStepAsActionWhenNotImplementsICompilerAction()
        {
            var pipeline = new PipelineBuilder<int>(factory);

            //Action
            Action acc = () => pipeline.AddAction<IRejectableTransformation>();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptStepAsTransformationWhenImplementsICompilerTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory);

            //Action
            Action acc = () => pipeline.AddTransformation<IIntTransformation>();

            //Assert
            acc.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RejectStepAsTransformationWhenNotImplementsICompilerTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory);

            //Action
            Action acc = () => pipeline.AddTransformation<IRejectableTransformation>();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenBuildAndTheTypeMatchThePipelineAndIsTheFirstStep()
        {
            var pipeline = new PipelineBuilder<int>(factory);
            pipeline.AddAction<IIntAction>();

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void RejectStepWhenbuildAndTheTypeNotMatchThePipelineAndIsTheFirstStep()
        {
            var pipeline = new PipelineBuilder<string>(factory);
            pipeline.AddAction<IIntAction>();

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenbuildAndTheTypeMatchTheDefaultLinkedTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory);
            pipeline.AddTransformation<IIntTransformation>();
            pipeline.AddAction<IIntAction>();

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void RejectStepWhenbuildAndTheTypeNotMatchTheDefaultLinkedTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory);
            pipeline.AddTransformation<IIntToStringTransformation>();
            pipeline.AddAction<IIntAction>();

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AddTransformationWithName()
        {
            var pipeline = new PipelineBuilder<int>(factory);
            pipeline.AddTransformation<IIntToStringTransformation>("name");

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        private interface IRejectableTransformation : ICompilerTransformation { }
        private interface IIntAction : ICompilerAction<int> { }
        private interface IIntTransformation : ICompilerTransformation<int, int> { }
        private interface IIntToStringTransformation : ICompilerTransformation<int, string> { }
    }
}
