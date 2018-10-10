namespace Pipeline.Unit.Tests
{
    using Autofac;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class PipelineBuilderShould
    {
        private readonly IPipelineFactory<int> factory;
        private readonly PipelineAction intAction;
        private readonly PipelineAction stringAction1;
        private readonly PipelineAction stringAction2;
        private readonly PipelineAction tupla2Action;
        private readonly PipelineTransformation intTransformation;
        private readonly PipelineTransformation intToStringTransformation;
        private readonly PipelineTransformation stringTransformation;

        public PipelineBuilderShould()
        {
            intAction = new PipelineAction(typeof(IIntAction));
            stringAction1= new PipelineAction(typeof(IStringAction));
            stringAction2 = new PipelineAction(typeof(IStringAction));
            intTransformation = new PipelineTransformation(typeof(IIntTransformation), typeof(int), typeof(int));
            intToStringTransformation = new PipelineTransformation(typeof(IIntToStringTransformation), typeof(int), typeof(string));
            stringTransformation = new PipelineTransformation(typeof(IStringTransformation), typeof(string), typeof(string));
            tupla2Action = new PipelineAction(typeof(ITupla2Action));
            
            factory = Substitute.For<IPipelineFactory<int>>();
            factory.CreateAction<IIntAction>().Returns(intAction);
            factory.CreateAction<IStringAction>().Returns(stringAction1, stringAction2);
            factory.CreateTransformation<IIntTransformation>().Returns(intTransformation);
            factory.CreateTransformation<IIntToStringTransformation>().Returns(intToStringTransformation);
            factory.CreateTransformation<IStringTransformation>().Returns(stringTransformation);
            factory.CreateAction<ITupla2Action>().Returns(tupla2Action);

            factory.CreateLink(Arg.Any<bool>(), Arg.Any<IPipelineSource>(), Arg.Any<IPipelineTarget>()).Returns(x =>
            {
                var isDefault = x.Arg<bool>();
                var source = x.Arg<IPipelineSource>();
                var target = x.Arg<IPipelineTarget>();

                var link = new PipelineLink(isDefault, source, target);

                return link;
            });

            factory.CreateJoin().Returns( x => new PipelineJoin());
            factory.CreateFork().Returns(x => new PipelineFork());
            factory.CreatePipeline().Returns(x => new DataflowPipeline<int>());
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
        public void RejectStepWhenBuildAndTheTypeNotMatchThePipelineAndIsTheFirstStep()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                .AddAction<IStringAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenBuildAndTheTypeMatchTheDefaultLinkedTransformation()
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
        public void RejectStepWhenBuildAndTheTypeNotMatchTheDefaultLinkedTransformation()
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
                    .AddAction<IStringAction>()
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenBuildAndTheTypeMatchTheLastTransformation()
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
        public void CountTheLinksBeforeBuild()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IIntAction>()
                    .LinkTo("name")
                ;

            intTransformation.InputLinks.Count().Should().Be(0);
            intTransformation.OutputLinks.Count().Should().Be(2);

            intToStringTransformation.InputLinks.Count().Should().Be(1);
            intToStringTransformation.OutputLinks.Count().Should().Be(1);

            stringAction1.InputLinks.Count().Should().Be(1);

            intAction.InputLinks.Count().Should().Be(1);
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
            try
            {
                pipeline.Build();
            }
            catch (Exception e)
            {
                throw new Exception(pipeline.ToString(), e );
            }

            //Assert
            var joinsEnumeration = from transformation in pipeline.Items.OfType<IPipelineJoin>() select transformation;
            var join = joinsEnumeration.FirstOrDefault();

            join.Should().NotBeNull();
        }

        [Fact]
        public void ResolveTheJoinLinks()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            var linkBetweenTransformations = intTransformation.OutputLinks.First();
            intTransformation.RemoveOutputLink(linkBetweenTransformations);

            //Action
            pipeline.Build();

            //Assert
            var joinsEnumeration = from transformation in pipeline.Items.OfType<IPipelineJoin>() select transformation;
            var join = joinsEnumeration.First();
            
            join.InputLinks.First().Source.Should().BeSameAs(intTransformation);
            join.InputLinks.Last().Source.Should().BeSameAs(stringTransformation);
            join.OutputLinks.First().Target.Should().BeSameAs(tupla2Action);
        }

        [Fact]
        public void InsertForkWhenTwoOutputLinksExists()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IStringAction>()
                ;

            //Action
            pipeline.Build();

            //Assert
            var forkEnumeration = from transformation in pipeline.Items.OfType<IPipelineFork>() select transformation;
            var fork = forkEnumeration.FirstOrDefault();

            fork.Should().NotBeNull();
        }

        [Fact]
        public void RejectStepWhenBuildAndTheTypeNotMatchTheJoinedTypes()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<IIntAction>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;
            var linkBetweenTransformations = intTransformation.OutputLinks.First();
            intTransformation.RemoveOutputLink(linkBetweenTransformations);

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }
        
        [Fact]
        public void ResolveSimpleLink()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddAction<IIntAction>()
                ;

            var expectedTree = new StringBuilder();
            expectedTree.AppendLine("[0] Transformation<IIntTransformation>");
            expectedTree.AppendLine("    --> [1]");
            expectedTree.AppendLine("[1] Action<IIntAction>");


            //Action
            pipeline.Build();

            //Assert
            pipeline.ToString().Should().Be(expectedTree.ToString());
        }
        
        [Fact]
        public void ResolveSimpleChain()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                ;

            var expectedTree = new StringBuilder();
            expectedTree.AppendLine("[0] Transformation<IIntTransformation>");
            expectedTree.AppendLine("    --> [1]");
            expectedTree.AppendLine("[1] Transformation<IIntToStringTransformation>");
            expectedTree.AppendLine("    --> [2]");
            expectedTree.AppendLine("[2] Action<IStringAction>");


            //Action
            pipeline.Build();

            //Assert
            pipeline.ToString().Should().Be(expectedTree.ToString());
        }
        
        [Fact]
        public void ResolveSimpleFork()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IStringAction>()
                ;

            var expectedTree = new StringBuilder();
            expectedTree.AppendLine("[0] Transformation<IIntToStringTransformation>");
            expectedTree.AppendLine("    --> [3]");
            expectedTree.AppendLine("[1] Action<IStringAction>");
            expectedTree.AppendLine("[2] Action<IStringAction>");
            expectedTree.AppendLine("[3] Fork");
            expectedTree.AppendLine("    --> [1]");
            expectedTree.AppendLine("    --> [2]");


            //Action
            pipeline.Build();

            //Assert
            pipeline.ToString().Should().Be(expectedTree.ToString());
        }
        
        [Fact]
        public void ResolveSimpleJoin()
        {
            var pipeline = new PipelineBuilder<int>(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            var linkBetweenTransformations = intTransformation.OutputLinks.First();
            linkBetweenTransformations.Remove();

            var expectedTree = new StringBuilder();
            expectedTree.AppendLine("[0] Transformation<IIntTransformation>");
            expectedTree.AppendLine("    --> [3]");
            expectedTree.AppendLine("[1] Transformation<IStringTransformation>");
            expectedTree.AppendLine("    --> [3]");
            expectedTree.AppendLine("[2] Action<ITupla2Action>");
            expectedTree.AppendLine("[3] Join");
            expectedTree.AppendLine("    --> [2]");

            //Action
            pipeline.Build();

            //Assert
            pipeline.ToString().Should().Be(expectedTree.ToString());
        }

        [Fact]
        public void BuildPipeline()
        {
            var builder = new PipelineBuilder<int>(factory)
                    .AddAction<IIntAction>()
                ;

            //Action
            var pipeline = builder.Build();

            //Assert
            pipeline.Should().NotBeNull();
            pipeline.Should().BeAssignableTo<IDataflowPipeline<int>>();
        }
    }
}
