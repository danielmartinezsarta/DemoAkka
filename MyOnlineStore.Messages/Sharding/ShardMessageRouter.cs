using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Cluster.Sharding;
using MyOnlineStore.Messages.Billing;

namespace MyOnlineStore.Messages.Sharding
{
    public class ShardMessageRouter: IMessageExtractor
    {
        public string EntityId(object message) => (message as ShardEnvelope)?.EntityId.ToString()!;

        public string ShardId(object message) => (message as ShardEnvelope)?.ShardId.ToString()!;

        public object EntityMessage(object message) => (message as ShardEnvelope)?.Message!;
    }
}
