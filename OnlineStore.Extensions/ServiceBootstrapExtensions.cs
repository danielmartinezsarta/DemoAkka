using Akka.Actor;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;

namespace OnlineStore.Extensions
{
    public static class ServiceBootstrapExtensions
    {
        public static ActorSystem StartPbm(this ActorSystem system)
        {
            var pbm = PetabridgeCmd.Get(system);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            pbm.Start();
            return system;
        }
    }
}