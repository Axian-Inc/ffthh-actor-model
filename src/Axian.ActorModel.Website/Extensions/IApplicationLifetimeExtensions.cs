using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Axian.ActorModel.Website.Extensions
{
    public static class IApplicationLifetimeExtensions
    {
        public static void ConfigureAxSystem(this IApplicationLifetime lifetime, IServiceProvider container)
        {
            // TODO: Create an ActorSystem on start up

            // TODO: Stop the ActorSystem when stopping
        }
    }
}
