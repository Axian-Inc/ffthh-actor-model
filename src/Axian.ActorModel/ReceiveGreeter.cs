using Akka.Actor;

namespace Axian.ActorModel
{
    // TODO: Inherit from Akka.Actor.ReceiveActor
    public class ReceiveGreeter : ReceiveActor
    {
        public ReceiveGreeter()
        {
            // TODO: Call ReceiveAny
            ReceiveAny(msg =>
            {
                Sender.Tell("hello");
            });
        }

        
    }
}
