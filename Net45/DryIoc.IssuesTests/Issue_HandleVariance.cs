using System.Linq;
using NUnit.Framework;

namespace DryIoc.IssuesTests
{
    [TestFixture]
    public class Issue_HandleVariance
    {
        [Test, Ignore("todo: fix later")]
        public void CommandHandlers_CanBeResolved_From_IoC()
        {
            var container = new Container();
            container.Register(typeof(IBird<>), typeof(BirdBaseImpl<>));
            container.Register(typeof(IBird<Bird>), typeof(BirdImpl));

            var services = container.ResolveMany<IBird<Bird>>();

            Assert.AreEqual(2, services.Count());
        }

        public interface IBird<in T> { }
        public class BirdBase<T> { }
        public class Bird : BirdBase<string> { }
        public class BirdImpl : IBird<Bird> { }
        public class BirdBaseImpl<T> : IBird<BirdBase<T>> { }

        [Test]
        public void Should_discover_variant_interface()
        {
            var c = new Container();

            c.RegisterMany(new [] { typeof(FakeRepo).Assembly },
                t => t.GetGenericDefinitionOrNull() == typeof(IQuery<,>));

            c.Resolve<IQuery<string, object>>();
        }

        public interface IQuery<in T, out R> { }
        public class FakeRepo : IQuery<string, object> { }
    }
}
