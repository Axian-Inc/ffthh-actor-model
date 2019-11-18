using Akka.Configuration;

namespace Axian.ActorModel
{
    static class DefaultConfig
    {
        // Load the default config stored as an embedded resource
        public static readonly Config Instance = ConfigurationFactory.FromResource<AxSystem>("Axian.ActorModel.default.conf");
    }
}
