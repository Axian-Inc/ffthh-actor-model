using Akka.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Axian.ActorModel.Website.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAxSystem(this IServiceCollection services, string name = null, string jsonOrHoconFilepath = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Config config = OptionallyLoadConfigFromDisk(jsonOrHoconFilepath);

            // TODO: Register the ActorSystem as a singleton
            services.AddSingleton(_ => AxSystem.Create(name, config));

            return services;
        }

        private static Config OptionallyLoadConfigFromDisk(string path)
        {
            if (path != null && File.Exists(path))
                return ConfigurationFactory.ParseString(File.ReadAllText(path));
            return null;
        }
    }
}
