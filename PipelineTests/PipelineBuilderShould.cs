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

                var link = new PipelineLink(isDefault, source, target);

                source.AddOutputLink(link);
                target.AddInputLink(link);

                return link;
            });

            factory.CreateJoin(Arg.Any<Type[]>()).Returns(x =>
                {
                    var sources = (Type[])x[0];

                    var source1 = sources[0];
                    var source2 = sources[1];

                    return new PipelineJoin(source1, source2);
                }
            );
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
            intAction.InputLinks.Count().Should().Be(1);
            var link = intAction.InputLinks.First();
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
            intAction.InputLinks.Count().Should().Be(1);
            var link = intAction.InputLinks.First();
            link.IsDefault.Should().BeFalse();
            link.Source.Should().Be(intTransformation);
        }

        [Fact]
        public void InsertJoinWhenTwoInputLinksExists()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            //Action
            pipeline.Build();

            //Assert
            var joinsEnumeration = from transformation in pipeline.Transformations.OfType<IPipelineJoin>() select transformation;
            var join = joinsEnumeration.First();

            join.Should().NotBeNull();
        }

        [Fact]
        public void ResolveTheJoinTypes()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            //Action
            pipeline.Build();

            //Assert
            var joinsEnumeration = from transformation in pipeline.Transformations.OfType<IPipelineJoin>() select transformation;
            var join = joinsEnumeration.First();

            join.InputType.Should().Be(intTransformation.OutputType);
            join.InputType2.Should().Be(stringTransformation.OutputType);
            join.OutputType.Should().Be(tupla2Action.InputType);
        }

        [Fact]
        public void ResolveTheJoinLinks()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            //Action
            pipeline.Build();

            //Assert
            var joinsEnumeration = from transformation in pipeline.Transformations.OfType<IPipelineJoin>() select transformation;
            var join = joinsEnumeration.First();



            join.InputLinks.First().Source.Should().Be(intTransformation);
            join.InputLinks.Last().Source.Should().Be(stringTransformation);
            join.OutputLinks.First().Target.Should().Be(tupla2Action);
        }

        //[Fact]
        //public void InsertJoinWhenTwoInputLinksExists()
        //{
        //    var pipeline = new PipelineBuilder<int>(factory)
        //            .AddTransformation<IIntTransformation>("name1")
        //            .AddTransformation<IIntToStringTransformation>()
        //            .AddTransformation<IStringTransformation>("name2")
        //            .AddAction<ITupla2Action>()
        //            .LinkTo("name1")
        //            .LinkTo("name2")
        //        ;

        //    //Action
        //    pipeline.Build();

        //    //Assert
        //    var joinsEnumeration = from transformation in pipeline.Transformations.OfType<IPipelineJoin>() select transformation;
        //    var join = joinsEnumeration.First();

        //    join.Should().NotBeNull();
        //    join.InputType.Should().Be(intTransformation.OutputType);
        //    join.InputType2.Should().Be(intToStringTransformation.OutputType);
        //    join.OutputType.Should().Be(tupla2Action.InputType);



        //    tupla2Action.InputLinks.Count().Should().Be(1);
        //    var link = tupla2Action.InputLinks.First();
        //    link.Source.Should().BeAssignableTo<IPipelineJoin>();
        //}

        //[Fact]
        //public void ResolveStepsWithTwoLinks()
        //{
        //    var pipeline = new PipelineBuilder<int>(factory)
        //            .AddTransformation<IIntTransformation>("name1")
        //            .AddTransformation<IIntToStringTransformation>()
        //            .AddTransformation<IStringTransformation>("name2")
        //            .AddAction<IStringAction>()
        //            .AddAction<ITupla2Action>()
        //            .LinkTo("name1")
        //            .LinkTo("name2")
        //        ;

        //    //Action
        //    pipeline.Build();

        //    //Assert
        //    tupla2Action.InputLinks.Count().Should().Be(1);
        //    var link = tupla2Action.InputLinks.First();
        //    link.Source.Should().BeAssignableTo<IPipelineJoin>();
        //}

        //[Fact]
        //public void ResolveStepsWithTwoOutputLinks()
        //{
        //    var pipeline = new PipelineBuilder<int>(factory)
        //            .AddTransformation<IIntTransformation>("name1")
        //            .AddAction<IIntAction>()
        //            .AddTransformation<IIntToStringTransformation>()
        //            .LinkTo("name1")
        //        ;

        //    //Action
        //    pipeline.Build();

        //    //Assert
        //    tupla2Action.InputLinks.Count().Should().Be(1);
        //    var link = tupla2Action.InputLinks.First();
        //    link.Source.Should().BeAssignableTo<IPipelineJoin>();
        //}
    }
}
