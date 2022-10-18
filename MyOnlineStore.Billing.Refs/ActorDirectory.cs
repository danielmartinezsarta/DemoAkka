using Akka.Actor;

namespace MyOnlineStore.Billing.Refs
{
    public static class ActorDirectory
    {
        public static ActorSystem? ActorSystem { get; set; }
        public static IActorRef? CartDispatcher { get; set; }
        public static IActorRef? CartShard { get; set; }
    }
}