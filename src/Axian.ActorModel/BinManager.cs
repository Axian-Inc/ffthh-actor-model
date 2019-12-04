using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axian.ActorModel
{
    public class BinManager : ReceiveActor
    {
        #region Protocol

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

        #endregion

        public BinManager()
        { 
        }
    }
}
