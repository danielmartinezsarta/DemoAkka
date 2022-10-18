using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOnlineStore.Messages.Sharding
{
    public class ShardEnvelope
    {
        public readonly string ShardId;
        public readonly string EntityId;
        public readonly object Message;

        public ShardEnvelope(string shardId, string entityId, object message)
        {
            ShardId = shardId;
            EntityId = entityId;
            Message = message;
        }
    }
}
