using System;
using Akka.Actor;

namespace Axian.ActorModel
{
    // TODO: Inherit from Akka.Actor.UntypedActor
    public class UntypedGreeter : UntypedActor
    {
        // TODO: Override OnReceive
        protected override void OnReceive(object message)
        {
            Sender.Tell("hello");
        }
    }
}
