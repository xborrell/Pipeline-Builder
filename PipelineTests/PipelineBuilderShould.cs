namespace Pipeline.Unit.Tests
{
    using Autofac;
    using FluentAssertions;
    using NSubstitute;
    using Pipeline;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using TASuite.Commons.Crosscutting;
    using Xunit;

    public class PipelineBuilderShould
    {
        private readonly IIoCAbstractFactory factory;

        public PipelineBuilderShould()
        {
            factory = Substitute.For<IIoCAbstractFactory>();

            factory.ResolveAll<IIntTransformation>().Returns(new List<IIntTransformation>{Substitute.For<IIntTransformation>()});
            factory.ResolveAll<IStringTransformation>().Returns(new List<IStringTransformation>{Substitute.For<IStringTransformation>()});
        }

        [Fact]
        public void CreateWithDependencies()
        {
            var builder = PipelineBuilder<int, string>.Create(factory);

            builder.Should().NotBeNull();
        }

        [Fact]
        public void AcceptStepAsActionWhenImplementsICompilerAction()
        {
            var builder = PipelineBuilder<int, string>.Create(factory);

            //Action
            Action acc = () => builder.AddAction<IIntAction>();

            //Assert
            acc.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RejectStepAsActionWhenNotImplementsICompilerAction()
        {
            var builder = PipelineBuilder<int, string>.Create(factory);

            //Action
            Action acc = () => builder.AddAction<IRejectableTransformation>();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptStepAsTransformationWhenImplementsICompilerTransformation()
        {
            var builder = PipelineBuilder<int, string>.Create(factory);

            //Action
            Action acc = () => builder.AddTransformation<IIntTransformation>();

            //Assert
            acc.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RejectStepAsTransformationWhenNotImplementsICompilerTransformation()
        {
            var builder = PipelineBuilder<int, string>.Create(factory);

            //Action
            Action acc = () => builder.AddTransformation<IRejectableTransformation>();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenBuildAndTheTypeMatchThePipelineAndIsTheFirstStep()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => builder.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void RejectStepWhenBuildAndTheTypeNotMatchThePipelineAndIsTheFirstStep()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                .AddAction<IStringAction>()
                ;

            //Action
            Action acc = () => builder.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenBuildAndTheTypeMatchTheDefaultLinkedTransformation()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                .AddTransformation<IIntTransformation>()
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => builder.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void RejectStepWhenBuildAndTheTypeNotMatchTheDefaultLinkedTransformation()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                .AddTransformation<IIntToStringTransformation>()
                .AddAction<IIntAction>()
                ;

            //Action
            Action acc = () => builder.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void AddTransformationWithName()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntToStringTransformation>("name")
                    .AddAction<IStringAction>()
                ;

            //Action
            Action acc = () => builder.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void AcceptsStepWhenBuildAndTheTypeMatchTheLastTransformation()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IStringAction>()
                ;

            //Action
            Action acc = () => builder.Build();

            //Assert
            acc.Should().NotThrow<PipelineBuilderException>();
        }

        [Fact]
        public void CreateDefaultLink()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddAction<IIntAction>()
                ;

            builder.Build();

            var intAction = builder.Items.OfType<IPipelineAction<IIntAction, int>>().First();

            intAction.InputLinks.Count().Should().Be(1);
            var link = intAction.InputLinks.First();
            link.IsDefault.Should().BeTrue();
        }

        [Fact]
        public void CountTheLinksBeforeBuild()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>("name")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IIntAction>()
                    .LinkTo("name")
                ;

            var intTransformation = builder.Items.OfType<IPipelineTransformation<IIntTransformation, int, int>>().First();
            intTransformation.InputLinks.Count().Should().Be(0);
            intTransformation.OutputLinks.Count().Should().Be(2);

            var intToStringTransformation = builder.Items.OfType<IPipelineTransformation<IIntToStringTransformation, int, string>>().First();
            intToStringTransformation.InputLinks.Count().Should().Be(1);
            intToStringTransformation.OutputLinks.Count().Should().Be(1);

            var stringAction = builder.Items.OfType<IPipelineAction<IStringAction, string>>().First();
            stringAction.InputLinks.Count().Should().Be(1);

            var intAction = builder.Items.OfType<IPipelineAction<IIntAction, int>>().First();
            intAction.InputLinks.Count().Should().Be(1);
        }

        [Fact]
        public void ExplicitLinkSteps()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                .AddTransformation<IIntTransformation>("name")
                .AddTransformation<IIntToStringTransformation>()
                .AddAction<IStringAction>()
                .AddAction<IIntAction>()
                    .LinkTo("name")
                ;

            //Assert
            var intAction = builder.Items.OfType<IPipelineAction<IIntAction, int>>().First();

            intAction.InputLinks.Count().Should().Be(1);
            var link = intAction.InputLinks.First();
            link.IsDefault.Should().BeFalse();

            var intTransformation = builder.Items.OfType<IPipelineTransformation<IIntTransformation, int, int>>().First();
            link.Source.Should().Be(intTransformation);
        }

        [Fact]
        public void InsertJoinWhenTwoInputLinksExists()
        {
            var pipeline = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>()
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                    .Build()
                ;

            var join = pipeline.Blocks.OfType<JoinBlock<int, string>>().FirstOrDefault();

            join.Should().NotBeNull();
        }

        [Fact]
        public void InsertForkWhenTwoOutputLinksExists()
        {
            var pipeline = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IStringAction>()
                    .Build()
                ;

            var fork = pipeline.Blocks.OfType<BroadcastBlock<string>>().FirstOrDefault();

            fork.Should().NotBeNull();
        }

        [Fact]
        public void RejectStepWhenBuildAndTheTypeNotMatchTheJoinedTypes()
        {
            var pipeline = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IStringTransformation>("name2")
                    .AddAction<IIntAction>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            //Action
            Action acc = () => pipeline.Build();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void ResolveSimpleLink()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddAction<IIntAction>()
                ;

            var expectedTree = new StringBuilder();
            expectedTree.AppendLine("[0] Transformation<IIntTransformation>");
            expectedTree.AppendLine("    --> [1]");
            expectedTree.AppendLine("[1] Action<IIntAction>");


            //Action
            builder.Build();

            //Assert
            builder.ToString().Should().Be(expectedTree.ToString());
        }

        [Fact]
        public void ResolveSimpleChain()
        {
            var pipeline = PipelineBuilder<int, string>.Create(factory)
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
            var builder = PipelineBuilder<int, string>.Create(factory)
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
            builder.Build();

            //Assert
            builder.ToString().Should().Be(expectedTree.ToString());
        }

        [Fact]
        public void ResolveSimpleJoin()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                ;

            var expectedTree = new StringBuilder();
            expectedTree.AppendLine("[0] Transformation<IIntTransformation>");
            expectedTree.AppendLine("    --> [3]");
            expectedTree.AppendLine("[1] Transformation<IIntToStringTransformation>");
            expectedTree.AppendLine("    --> [4]");
            expectedTree.AppendLine("[2] Action<ITupla2Action>");
            expectedTree.AppendLine("[3] Fork");
            expectedTree.AppendLine("    --> [1]");
            expectedTree.AppendLine("    --> [4]");
            expectedTree.AppendLine("[4] Join");
            expectedTree.AppendLine("    --> [2]");

            //Action
            builder.Build();

            //Assert
            builder.ToString().Should().Be(expectedTree.ToString());
        }

        [Fact]
        public void BuildPipeline()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddAction<IIntAction>()
                ;

            //Action
            var pipeline = builder.Build();

            //Assert
            pipeline.Should().NotBeNull();
            pipeline.Should().BeAssignableTo<IDataflowPipeline<int>>();
        }

        [Fact]
        public void BuildActionBlock()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddAction<IIntAction>()
                ;
            var pipeline = builder.Build();

            var intAction = builder.Items.OfType<IPipelineAction<IIntAction, int>>().First();
            intAction.Should().NotBeNull();

            var intBlock = pipeline.Blocks.OfType<ActionBlock<int>>().First();
            intBlock.Should().NotBeNull();

            intAction.Blocks.First().Should().BeSameAs(intBlock);
        }

        [Fact]
        public void BuildTransformationBlock()
        {
            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddAction<IIntAction>()
                ;

            var pipeline = builder.Build();

            var intTransformation = builder.Items.OfType<IPipelineTransformation<IIntTransformation, int, int>>().First();
            intTransformation.Should().NotBeNull();

            var intBlock = pipeline.Blocks.OfType<TransformBlock<int, int>>().First();
            intBlock.Should().NotBeNull();

            intTransformation.Blocks.First().Should().BeSameAs(intBlock);

        }

        [Fact]
        public void BuildForkBlock()
        {
            var pipeline = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntToStringTransformation>()
                    .AddAction<IStringAction>()
                    .AddAction<IStringAction>()
                    .Build()
                ;

            var fork = pipeline.Blocks.OfType<BroadcastBlock<string>>().FirstOrDefault();
            fork.Should().NotBeNull();
            fork.Should().BeOfType<BroadcastBlock<string>>();
        }

        [Fact]
        public void BuildJoinBlock()
        {
            var pipeline = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>("name1")
                    .AddTransformation<IIntToStringTransformation>("name2")
                    .AddAction<ITupla2Action>()
                    .LinkTo("name1")
                    .LinkTo("name2")
                    .Build()
                ;

            var join = pipeline.Blocks.OfType<JoinBlock<int, string>>().FirstOrDefault();
            join.Should().NotBeNull();
            join.Should().BeOfType<JoinBlock<int, string>>();
        }

        [Fact]
        public void BuildManyTransformationBlocks()
        {
            factory.ResolveAll<IIntTransformation>().Returns(new List<IIntTransformation>{Substitute.For<IIntTransformation>(), Substitute.For<IIntTransformation>()});

            var builder = PipelineBuilder<int, string>.Create(factory)
                    .AddTransformation<IIntTransformation>()
                    .AddAction<IIntAction>()
                ;

            var pipeline = builder.Build();

            var intBlocks = pipeline.Blocks.OfType<TransformBlock<int, int>>();
            intBlocks.Should().NotBeNull();
            intBlocks.Count().Should().Be(2);
        }
    }
}
