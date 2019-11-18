using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axian.ActorModel
{
    public class BinManager : ReceiveActor
    {
        public class CreateBin
        {

        }

        public class CreateBinResponse
        {
            public CreateBinResponse(string bin)
            {
                Bin = bin;
            }

            public string Bin { get; }
        }

        public class ListBins
        {

        }

        public class ListBinsResponse
        {
            public ListBinsResponse(IEnumerable<string> bins)
            {
                Bins = bins.ToList().AsReadOnly();
            }

            public IReadOnlyList<string> Bins { get; }
        }

        public class CaptureRequest
        {
            public CaptureRequest(string bin, object data)
            {
                Bin = bin;
                Data = data;
            }

            public string Bin { get; }
            public object Data { get; }
        }

        public class ListRequests
        {
            public ListRequests(string bin)
            {
                Bin = bin;
            }

            public string Bin { get; }
        }

        private readonly Dictionary<string, IActorRef> _binsByName = new Dictionary<string, IActorRef>();
        private readonly Dictionary<IActorRef, string> _namesByBin = new Dictionary<IActorRef, string>();

        public BinManager()
            : this(Context.System.Settings.Config)
        { }

        public BinManager(Config config)
        {
            config = config.SafeWithFallback(DefaultConfig.Instance);

            Receive<CreateBin>(command =>
            {
                string name = Guid.NewGuid().ToString();
                IActorRef bin = Context.ActorOf(Props.Create(() => new Bin(config)), name);
                _binsByName.Add(name, bin);
                _namesByBin.Add(bin, name);

                Context.Watch(bin);

                var response = new CreateBinResponse(name);
                Sender.Tell(response);
            });

            Receive<ListBins>(query =>
            {
                var response = new ListBinsResponse(_binsByName.Keys);
                Sender.Tell(response);
            });

            Receive<CaptureRequest>(command =>
            {
                IActorRef bin = GetBin(command.Bin);
                bin?.Tell(new Bin.Put(command.Data), Sender);
            });

            Receive<ListRequests>(query =>
            {
                IActorRef bin = GetBin(query.Bin);
                bin?.Tell(new Bin.List(), Sender);
            });

            Receive<Terminated>(msg =>
            {
                string name = GetName(msg.ActorRef);
                _binsByName.Remove(name);
                _namesByBin.Remove(msg.ActorRef);
            });
        }

        
        private IActorRef GetBin(string name)
        {
            _binsByName.TryGetValue(name, out IActorRef bin);
            return bin;
        }

        private string GetName(IActorRef bin)
        {
            _namesByBin.TryGetValue(bin, out string name);
            return name;
        }
    }
}
