using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using MyOnlineStore.Billing.Actors;
using MyOnlineStore.Billing.Refs;
using OnlineStore.Extensions;

namespace MyOnlineStore.Billing
{
    internal class BillingService
    {

        public void Start()
        {
            var configuration = ConfigurationFactory.ParseString(File.ReadAllText("Billing.conf"));
            ActorDirectory.ActorSystem = Akka.Actor.ActorSystem.Create("online-store", configuration).StartPbm();

            var cartDispatcherProps = Props.Create(() => new CartDispatcherActor());
            ActorDirectory.CartDispatcher = ActorDirectory.ActorSystem.ActorOf(cartDispatcherProps, "dispatcher-router");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return CoordinatedShutdown.Get(ActorDirectory.ActorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
