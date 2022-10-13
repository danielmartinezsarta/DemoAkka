using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using MyOnlineStore.Messages.Billing;
using MyOnlineStore.Messages.Billing.Commands;

namespace MyOnlineStore.Actors
{
    public class CheckoutActor: ReceiveActor
    {
        private ILoggingAdapter _logger = Context.GetLogger();
        private IActorRef _router;
        public CheckoutActor()
        {
            ReceiveAsync<GetUserCartCommand>(HandleGetUserCartCommand);

            ReceiveAsync<AddProductToCart>(async d =>
            {
                _logger.Warning("Receiving message");
                //var remoteActor = Context.ActorSelection("akka.tcp://online-store@localhost:8088/user/dispatcher-router");

                Sender.Tell(await _router.Ask(new ConsistentHashableEnvelope(d, d.ProductName)
                    ));
            });
        }

        private async Task HandleGetUserCartCommand(GetUserCartCommand command)
        {
            Sender.Tell(await _router.Ask(new ConsistentHashableEnvelope(command, command.User)));
        }


        protected override void PreStart()
        {
            var routerProps = Props.Empty.WithRouter(FromConfig.Instance);
            _router = Context.ActorOf(routerProps, "cart-dispatcher");
            base.PreStart();
        }
    }
}
