using Akka.Configuration;
using Xunit;

namespace Axian.ActorModel
{
    public class AxSystemSpec
    {
        [Fact]
        public void Should_create_a_new_actor_system()
        {
            const string SYSTEM_NAME = "test-ax";

            AxSystem ax = AxSystem.Create(SYSTEM_NAME, ConfigurationFactory.Default());

            Assert.NotNull(ax.Sys);
            Assert.Equal(SYSTEM_NAME, ax.Sys.Name);
        }

        [Fact]
        public void Should_use_defaults_when_no_name_or_config_is_supplied()
        {
            AxSystem ax = AxSystem.Create();
            Assert.Equal("axian", ax.Sys.Name);
        }

        [Fact]
        public void Should_get_actor_system_name_from_config()
        {
            string hocon = @"name = test-name-from-config";
            Config config = ConfigurationFactory.ParseString(hocon);
            AxSystem ax = AxSystem.Create(null, config);

            Assert.Equal("test-name-from-config", ax.Sys.Name);
        }

        [Fact]
        public void Should_fallback_to_default_name_when_no_name_is_supplied_or_in_config()
        {
            string hocon = @"akka { }";
            Config config = ConfigurationFactory.ParseString(hocon);

            AxSystem ax = AxSystem.Create(null, config);

            Assert.Equal("axian", ax.Sys.Name);
        }
    }
}
