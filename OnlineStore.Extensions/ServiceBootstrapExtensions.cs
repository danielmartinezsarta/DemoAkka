using Akka.Actor;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Cluster.Sharding;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;

namespace OnlineStore.Extensions
{
    public static class ServiceBootstrapExtensions
    {
        public static ActorSystem StartPbm(this ActorSystem system)
        {
            var pbm = PetabridgeCmd.Get(system);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            pbm.RegisterCommandPalette(ClusterShardingCommands.Instance);
            pbm.Start();
            return system;
        }
    }
}