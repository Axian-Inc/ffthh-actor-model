using Akka.Configuration;

namespace Axian.ActorModel
{
    static class DefaultConfig
    {
        public static readonly Config Instance = ConfigurationFactory.FromResource<AxSystem>("Axian.ActorModel.default.conf");
    }
}
