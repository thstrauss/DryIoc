using System;
using NUnit.Framework;

namespace DryIoc.IssuesTests
{
    [TestFixture]
    public class SO_IsInstantiatedViaDecorator
    {
        [Test]
        public void Test()
        {
            var c = new Container();
            c.Register<IService, X>(Reuse.Singleton);

            c.Register<XProvider>(Reuse.Singleton);

            c.Register<IService>(
                Made.Of(_ => ServiceInfo.Of<XProvider>(), p => p.Create(Arg.Of<Func<IService>>())),
                Reuse.Singleton,
                Setup.Decorator);

            c.Register<A>();

            var x = c.Resolve<XProvider>();
            Assert.IsFalse(x.IsCreated);

            c.Resolve<A>();

            Assert.IsTrue(x.IsCreated);
        }

        public interface IService { }
        public class X : IService { }

        public class A
        {
            public A(IService service) { }
        }

        public class XProvider
        {
            public bool IsCreated { get; private set; }
            public IService Create(Func<IService> factory)
            {
                IsCreated = true;
                return factory();
            }
        }
    }
}
