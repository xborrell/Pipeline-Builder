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

    public class PipelineBuilderShould
    {
        private readonly IPipelineFactory factory;
        private readonly PipelineAction intAction;
        private readonly PipelineAction stringAction1;
        private readonly PipelineAction stringAction2;
        private readonly PipelineAction tupla2Action;
        private readonly PipelineTransformation intTransformation;
        private readonly PipelineTransformation intToStringTransformation;
        private readonly PipelineTransformation stringTransformation;

        public PipelineBuilderShould()
        {
            intAction = new PipelineAction(typeof(IIntAction), typeof(int));
            stringAction1= new PipelineAction(typeof(IStringAction), typeof(string));
            stringAction2 = new PipelineAction(typeof(IStringAction), typeof(string));
            intTransformation = new PipelineTransformation(typeof(IIntTransformation), typeof(int), typeof(int));
            intToStringTransformation = new PipelineTransformation(typeof(IIntToStringTransformation), typeof(int), typeof(string));
            stringTransformation = new PipelineTransformation(typeof(IStringTransformation), typeof(string), typeof(string));
            tupla2Action = new PipelineAction(typeof(ITupla2Action), typeof(Tuple<int, string>));
            
            factory = Substitute.For<IPipelineFactory>();
            factory.CreateStep<IRejectableTransformation>().Returns(x => Substitute.For<IPipelineTarget>());
            factory.CreateStep<IIntAction>().Returns(intAction);
            factory.CreateStep<IStringAction>().Returns(stringAction1, stringAction2);
            factory.CreateStep<IIntTransformation>().Returns(intTransformation);
            factory.CreateStep<IIntToStringTransformation>().Returns(intToStringTransformation);
            factory.CreateStep<IStringTransformation>().Returns(stringTransformation);
            factory.CreateStep<ITupla2Action>().Returns(tupla2Action);

            factory.CreateLink(Arg.Any<bool>(), Arg.Any<IPipelineSource>(), Arg.Any<IPipelineTarget>()).Returns(x =>
            {
                var isDefault = x.Arg<bool>();
                var source = x.Arg<IPipelineSource>();
                var target = x.Arg<IPipelineTarget>();

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
        public void CreateDefaultLink()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddAction<IIntAction>()
                ;

            //Action
            pipeline.Build();

            //Assert
            intAction.Links.Count().Should().Be(1);
            var link = intAction.Links.First();
            link.IsDefault.Should().BeTrue();
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
            pipeline.Build();

            //Assert
            intAction.Links.Count().Should().Be(1);
            var link = intAction.Links.First();
            link.IsDefault.Should().BeFalse();
            link.Source.Should().Be(intTransformation);
        }

        [Fact]
        public void ResolveStepsWithTwoLinks()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<IStringAction>()
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<Exception>();
        }
    }
}
