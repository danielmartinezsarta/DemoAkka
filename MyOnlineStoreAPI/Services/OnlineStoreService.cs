using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.Routing;
using MyOnlineStore.Actors;
using MyOnlineStore.Messages;
using MyOnlineStore.Messages.Sharding;
using MyOnlineStoreAPI.Refs;
using OnlineStore.Extensions;

namespace MyOnlineStoreAPI.Services
{
    public class OnlineStoreService: IHostedService
    {
        private readonly IServiceScope _diScope;
        private readonly IHttpClientFactory _httpClientFactory;

        public OnlineStoreService(IServiceProvider  serviceProvider)
        {
            _diScope = serviceProvider.CreateScope();
            _httpClientFactory = _diScope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var configuration = ConfigurationFactory.ParseString(File.ReadAllText("StoreApi.conf"));
            ActorDirectory.ActorSystem = Akka.Actor.ActorSystem.Create("online-store", configuration).StartPbm();

            var catalogActorProps = Props.Create(() => new CatalogActor(_httpClientFactory))
                .WithRouter(FromConfig.Instance);

            ActorDirectory.CatalogActorRouter =
                ActorDirectory.ActorSystem.ActorOf(catalogActorProps, "catalog-actor-router");

            ActorDirectory.CatalogActorRouter.Tell(new StartSystemMessage());


            Cluster.Get(ActorDirectory.ActorSystem).RegisterOnMemberUp(() =>
            {
                var checkoutActorProps = Props.Create(() => new CheckoutActor());
                ActorDirectory.CheckOutActor =
                    ActorDirectory.ActorSystem.ActorOf(checkoutActorProps, "checkout");


                var sharding = ClusterSharding.Get(ActorDirectory.ActorSystem);
                
                ActorDirectory.CheckoutShardingActor = sharding.StartProxy("cartactor", "users", new ShardMessageRouter());
            });


           

            

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await ActorDirectory.ActorSystem.WhenTerminated.WaitAsync(TimeSpan.FromSeconds(10));
        }
    }
}
