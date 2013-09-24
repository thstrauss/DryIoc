﻿using System.Runtime.Remoting.Messaging;
using NUnit.Framework;

namespace DryIoc.UnitTests.AttributedRegistration
{
    [TestFixture]
    public class ExportAsDecoratorTests
    {
        [Test]
        public void Decorator_can_be_applied_based_on_Name()
        {
            var container = new Container();
            container.RegisterExported(typeof(LoggingHandlerDecorator), typeof(FastHandler), typeof(SlowHandler));

            Assert.That(container.Resolve<IHandler>("fast"), Is.InstanceOf<FastHandler>());

            var slow = container.Resolve<IHandler>("slow");
            Assert.That(slow, Is.InstanceOf<LoggingHandlerDecorator>());
            Assert.That(((LoggingHandlerDecorator)slow).Handler, Is.InstanceOf<SlowHandler>());
        }

        [Test]
        public void Decorator_can_be_applied_based_on_Metadata()
        {
            var container = new Container();
            container.RegisterExported(typeof(RetryHandlerDecorator), typeof(FastHandler), typeof(SlowHandler));

            Assert.That(container.Resolve<IHandler>("slow"), Is.InstanceOf<SlowHandler>());

            var fast = container.Resolve<IHandler>("fast");
            Assert.That(fast, Is.InstanceOf<RetryHandlerDecorator>());
            Assert.That(((RetryHandlerDecorator)fast).Handler, Is.InstanceOf<FastHandler>());
        }

        [Test]
        public void Decorator_can_be_applied_based_on_both_name_and_Metadata()
        {
            var container = new Container();
            container.RegisterExported(typeof(TransactHandlerDecorator), typeof(FastHandler), typeof(SlowHandler), typeof(TransactHandler));

            Assert.That(container.Resolve<IHandler>("slow"), Is.InstanceOf<SlowHandler>());
            Assert.That(container.Resolve<IHandler>("fast"), Is.InstanceOf<FastHandler>());

            var transact = container.Resolve<IHandler>("transact");
            Assert.That(transact, Is.InstanceOf<TransactHandlerDecorator>());
            Assert.That(((TransactHandlerDecorator)transact).Handler, Is.InstanceOf<TransactHandler>());
        }
    }

    public interface IHandler { }

    [ExportAll(ContractName = "fast"), ExportWithMetadata(2)]
    class FastHandler : IHandler { }

    [ExportAll(ContractName = "slow"), ExportWithMetadata(1)]
    class SlowHandler : IHandler { }

    [ExportAll(ContractName = "transact"), ExportWithMetadata(1)]
    class TransactHandler : IHandler { }

    [ExportAll, ExportAsDecorator(OfName = "slow")]
    class LoggingHandlerDecorator : IHandler
    {
        public IHandler Handler { get; set; }

        public LoggingHandlerDecorator(IHandler handler)
        {
            Handler = handler;
        }
    }

    [ExportAll, ExportAsDecorator(OfMetadata = true), ExportWithMetadata(2)]
    class RetryHandlerDecorator : IHandler
    {
        public IHandler Handler { get; set; }

        public RetryHandlerDecorator(IHandler handler)
        {
            Handler = handler;
        }
    }


    [ExportAll, ExportAsDecorator(OfName = "transact", OfMetadata = true), ExportWithMetadata(1)]
    class TransactHandlerDecorator : IHandler
    {
        public IHandler Handler { get; set; }

        public TransactHandlerDecorator(IHandler handler)
        {
            Handler = handler;
        }
    }
}
