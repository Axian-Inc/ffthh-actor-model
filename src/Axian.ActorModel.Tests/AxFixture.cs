using Akka.Configuration;

namespace Axian.ActorModel
{
    public class AxFixture
    {
        public AxFixture()
        {
            Configuration = ConfigurationFactory.FromResource<AxFixture>("Axian.ActorModel.akka.test.conf");
        }

        public Config Configuration { get; }
    }
}
