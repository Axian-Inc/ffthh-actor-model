using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axian.ActorModel
{

    public class Bin : ReceiveActor
    {
        public class Put
        {
            public Put(object data)
            {
                Data = data;
            }

            public object Data { get; }
        }

        public class List
        {

        }

        public class ListResponse
        {
            public ListResponse(IEnumerable<object> items)
            {
                Items = items.ToList().AsReadOnly();
            }

            public IReadOnlyList<object> Items { get; }
        }

        private readonly Queue<object> items = new Queue<object>();

        public Bin() 
            : this(Context.System.Settings.Config)
        {
        }

        public Bin(Config config)
        {
            config = config.SafeWithFallback(DefaultConfig.Instance);

            int maxItems = config.GetInt("axian.bin.max-items");
            TimeSpan ttl = config.GetTimeSpan("axian.bin.ttl");

            Context.System.Scheduler.ScheduleTellOnce(ttl, Self, PoisonPill.Instance, Self);

            Receive<Put>(cmd =>
            {
                if (items.Count == maxItems)
                    items.Dequeue();

                items.Enqueue(cmd.Data);

                Sender.Tell(new Status.Success("ok"));
            });

            Receive<List>(cmd =>
            {
                Sender.Tell(new ListResponse(items.Reverse()));
            });
        }
    }
}
