using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Client;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Util;
using MyOnlineStore.Billing.Actors;
using MyOnlineStore.Billing.Refs;
using MyOnlineStore.Messages.Billing;
using MyOnlineStore.Messages.Sharding;
using OnlineStore.Extensions;
using Cluster = Akka.Cluster.Cluster;
using CoordinatedShutdown = Akka.Actor.CoordinatedShutdown;

namespace MyOnlineStore.Billing
{
    internal class BillingService
    {

        public void Start()
        {
            var configuration = ConfigurationFactory.ParseString(File.ReadAllText("app.conf"));
                //.WithFallback(ClusterSharding.DefaultConfig()
                //    .WithFallback(ClusterSingletonManager.DefaultConfig())
                //    .WithFallback(DistributedPubSub.DefaultConfig())
                //    .WithFallback(ClusterClientReceptionist.DefaultConfig()));;
            ActorDirectory.ActorSystem = Akka.Actor.ActorSystem.Create("online-store", configuration).StartPbm();

            var cartDispatcherProps = Props.Create(() => new CartDispatcherActor());
            ActorDirectory.CartDispatcher = ActorDirectory.ActorSystem.ActorOf(cartDispatcherProps, "dispatcher-router");
            

           

            Cluster.Get(ActorDirectory.ActorSystem).RegisterOnMemberUp(async () =>
            {

// start if needed and provide a proxy to a named singleton
                //var proxy = singleton.Init(SingletonActor.Create(Counter<>.Props, "GlobalCounter"));
                
                var sharding = ClusterSharding.Get(ActorDirectory.ActorSystem);
                var cluster = Cluster.Get(ActorDirectory.ActorSystem);


                var config = ActorDirectory.ActorSystem.Settings.Config.GetConfig("akka.cluster.sharding");
                if (config.IsNullOrEmpty())
                    throw ConfigurationException.NullOrEmptyConfig<ClusterShardingSettings>("akka.cluster.sharding");

                var coordinatorSingletonPath = config.GetString("coordinator-singleton");

                var coordinatorConfig = ActorDirectory.ActorSystem.Settings.Config.GetConfig(coordinatorSingletonPath);

                var settings = ClusterShardingSettings.Create(ActorDirectory.ActorSystem);

                var messageExtractor = new ShardMessageRouter();

                ExtractEntityId extractEntityId = messageExtractor.ToExtractEntityId();
                ExtractShardId extractShardId = messageExtractor.ShardId;

                

                var clustCouldStart = cluster.SelfRoles.Contains("users");

                var cartRegion = sharding.Start(
                    "cartactor", 
                    name => Props.Create(() => new CartActor(name)),
                    settings,
                    extractEntityId,
                    extractShardId,
                    DefaultShardAllocationStrategy(settings),
                    PoisonPill.Instance);


                ActorDirectory.CartShard = cartRegion;

                

                //await Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith((s) =>
                //{
                //    var proxy = sharding.StartProxy("cartactor", "users", new ShardMessageRouter());

                //    proxy.Tell(new AddProductToCart("Demo", Guid.NewGuid().ToString(), 1, 0M));
                //});

            });
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return CoordinatedShutdown.Get(ActorDirectory.ActorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }


        public IShardAllocationStrategy DefaultShardAllocationStrategy(ClusterShardingSettings settings)
        {

            if (settings.TuningParameters.LeastShardAllocationAbsoluteLimit > 0)
            {
                // new algorithm
                var absoluteLimit = settings.TuningParameters.LeastShardAllocationAbsoluteLimit;
                var relativeLimit = settings.TuningParameters.LeastShardAllocationRelativeLimit;
                return ShardAllocationStrategy.LeastShardAllocationStrategy(absoluteLimit, relativeLimit);
            }
            else
            {
                // old algorithm
                var threshold = settings.TuningParameters.LeastShardAllocationRebalanceThreshold;
                var maxSimultaneousRebalance = settings.TuningParameters.LeastShardAllocationMaxSimultaneousRebalance;
                return new LeastShardAllocationStrategy(threshold, maxSimultaneousRebalance);
            }
        }
    }



    internal static class Extensions
    {
        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="self">TBD</param>
        /// <returns>TBD</returns>
        public static ExtractEntityId ToExtractEntityId(this IMessageExtractor self)
        {
            ExtractEntityId extractEntityId = msg =>
            {
                if (self.EntityId(msg) != null)
                    return (self.EntityId(msg), self.EntityMessage(msg));

                return Option<(string, object)>.None;
            };

            return extractEntityId;
        }
    }
}
