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

            factory.CreateStep<IRejectableTransformation>().Returns(x => Substitute.For<IPipelineItem>());
            factory.CreateStep<IIntAction>().Returns(x => new PipelineAction(typeof(IIntAction), typeof(int)));
            factory.CreateStep<IStringAction>().Returns(x => new PipelineAction(typeof(IStringAction), typeof(string)));
            factory.CreateStep<IIntTransformation>().Returns(x => new PipelineTransformation(typeof(IIntTransformation), typeof(int), typeof(int)));
            factory.CreateStep<IIntToStringTransformation>().Returns(x => new PipelineTransformation(typeof(IIntToStringTransformation), typeof(int), typeof(string)));

            factory.CreateLink(Arg.Any<bool>(), Arg.Any<IPipelineTransformation>(), Arg.Any<IPipelineItem>()).Returns(x =>
            {
                var isDefault = x.Arg<bool>();
                var source = x.Arg<IPipelineTransformation>();
                var target = x.Arg<IPipelineItem>();

                return new PipelineLink(isDefault, source, target);
            });
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
            var pipeline = new PipelineBuilder<int>(factory)
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void RejectStepWhenbuildAndTheTypeNotMatchThePipelineAndIsTheFirstStep()
        {
            var pipeline = new PipelineBuilder<string>(factory)
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenbuildAndTheTypeMatchTheDefaultLinkedTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                .AddTransformation<IIntTransformation>()
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void RejectStepWhenbuildAndTheTypeNotMatchTheDefaultLinkedTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                .AddTransformation<IIntToStringTransformation>()
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AddTransformationWithName()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                .AddTransformation<IIntToStringTransformation>("name")
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenbuildAndTheTypeMatchTheLastTransformation()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                .AddTransformation<IIntToStringTransformation>()
                .AddAction<IStringAction>()
                .AddAction<IStringAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void ExplicitLinkSteps()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                .AddTransformation<IIntTransformation>("name")
                .AddTransformation<IIntToStringTransformation>()
                .AddAction<IStringAction>()
                .AddAction<IIntAction>()
                    .LinkTo("name")
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }
    }
}
