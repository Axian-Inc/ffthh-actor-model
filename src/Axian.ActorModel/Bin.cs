using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axian.ActorModel
{

    public class Bin : ReceiveActor
    {
        #region Protocol

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

        #endregion

        public Bin()
        {

        }
    }
}
