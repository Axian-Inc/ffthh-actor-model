﻿using Akka.Actor;
using Akka.Configuration;
using System;

namespace Axian.ActorModel
{
    public class AxSystem
    {
        public static AxSystem Create(string name = null, Config config = null)
        {
            // If a config isn't provide, use the default config otherwise set it as a fallback
            config = config.SafeWithFallback(DefaultConfig.Instance);

            // If the name isn't provided, check the config for a name.
            if (name == null)
                name = config.GetString("name");

            // TODO: Create an ActorSystem
            ActorSystem sys = ActorSystem.Create(name, config);

            return new AxSystem(sys);
        }

        private AxSystem(ActorSystem sys)
        {
            Sys = sys ?? throw new ArgumentNullException(nameof(sys));

            BinManager = Sys.ActorOf<BinManager>("bin");
        }

        public ActorSystem Sys { get; }

        public IActorRef BinManager { get; }

        public void Terminate()
        {
            // TODO: Terminate the ActorSystem and wait for it to stop.
            Sys.Terminate().Wait();
        }
    }
}
