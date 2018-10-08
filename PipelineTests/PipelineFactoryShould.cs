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

    public class PipelineFactoryShould
    {
        private readonly Func<Type, Type, IPipelineAction> actionFactory;
        private readonly Func<Type, Type, Type, IPipelineTransformation> transformationFactory;
        private readonly Func<bool, IPipelineSource, IPipelineTarget, IPipelineLink> linkFactory;
        private readonly Func<Type, Type, IPipelineJoin> joinFactory;
        private readonly PipelineFactory factory;
        private Func<Type, IPipelineFork> forkFactory;

        public PipelineFactoryShould()
        {
            actionFactory = Substitute.For<Func<Type, Type, IPipelineAction>>();
            transformationFactory = Substitute.For<Func<Type, Type, Type, IPipelineTransformation>>();
            linkFactory = Substitute.For<Func<bool, IPipelineSource, IPipelineTarget, IPipelineLink>>();
            joinFactory = Substitute.For<Func<Type, Type, IPipelineJoin>>();
            forkFactory = Substitute.For<Func<Type, IPipelineFork>>();
            factory = new PipelineFactory(actionFactory, transformationFactory, linkFactory, joinFactory, forkFactory);
        }

        [Fact]
        public void CallTheActionFactoryToResolveActions()
        {
            //arrange
            var stepType = typeof(IIntAction);
            var inputType = typeof(int);

            //Action
            factory.CreateStep<IIntAction>();

            //Assert
            actionFactory(stepType, inputType).Received(1);
        }

        [Fact]
        public void CallTheTransformationFactoryToResolveTransformations()
        {
            //arrange
            var stepType = typeof(IIntTransformation);
            var inputType = typeof(int);
            var outputType = typeof(int);

            //Action
            factory.CreateStep<IIntTransformation>();

            //Assert
            transformationFactory(stepType, inputType, outputType).Received(1);
        }

        [Fact]
        public void ThrowExceptionWhenTheTypeIsNotActionNorTransformation()
        {
            //arrange

            //Action
            Action acc = () => factory.CreateStep<IRejectableTransformation>();

            //Assert
            acc.Should().Throw<PipelineBuilderException>();
        }

        [Fact]
        public void CallTheLinkFactoryToResolveLinks()
        {
            //arrange
            var source = Substitute.For<IPipelineSource>();
            var target = Substitute.For<IPipelineTarget>();

            //Action
            factory.CreateLink(true, source, target);

            //Assert
            linkFactory(true, source, target).Received(1);
        }

        [Fact]
        public void CallTheJoinFactoryToResolveJoins()
        {
            //arrange
            var source1 = Substitute.For<Type>();
            var source2 = Substitute.For<Type>();

            //Action
            factory.CreateJoin(source1, source2);

            //Assert
            joinFactory(source1, source2).Received(1);
        }

        [Fact]
        public void CallTheForkFactoryToResolveForks()
        {
            //arrange
            var source = Substitute.For<Type>();

            //Action
            factory.CreateFork(source);

            //Assert
            forkFactory(source).Received(1);
        }
    }
}
