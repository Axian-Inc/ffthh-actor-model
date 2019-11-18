using Akka.TestKit;
using Akka.TestKit.Xunit;
using Xunit;

namespace Axian.ActorModel
{
    [Collection(nameof(AxCollection))]
    public abstract class AxTestKit : TestKit
    {
        public AxTestKit(AxFixture fixture)
            : base(fixture.Configuration)
        {
        }

        public TestScheduler Scheduler => (TestScheduler)Sys.Scheduler;
    }
}
