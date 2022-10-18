using Akka.Actor;

namespace MyOnlineStoreAPI.Refs
{
    public static class ActorDirectory
    {
        public static ActorSystem? ActorSystem { get; set; }

        public static IActorRef? CatalogActorRouter { get; set; }
        public static IActorRef? CheckOutActor { get; set; }
        public static IActorRef? CheckoutShardingActor { get; set; }
    }
}
