using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using MyOnlineStore.Messages.Billing;
using MyOnlineStore.Messages.Billing.Commands;

namespace MyOnlineStore.Billing.Actors
{
    public class CartDispatcherActor: ReceiveActor
    {
        private readonly List<IActorRef> _carts;

        public CartDispatcherActor()
        {
            _carts = new List<IActorRef>();
            Receive<AddProductToCart>(HandleAddProductToCart);
            Receive<GetUserCartCommand>(HandleGetUserCartCommand);
        }

        private void HandleGetUserCartCommand(GetUserCartCommand command)
        {
            var cartUser = GetUser(command.User);
            cartUser.Forward(command);
        }

        private void HandleAddProductToCart(AddProductToCart message)
        {
            var cartUser = GetUser(message.User);
            cartUser.Forward(message);
        }

        private IActorRef GetUser(string userName)
        {
            var actorName = "user-" + userName;
            var cartUser = _carts.FirstOrDefault(x => x.Path.Name == actorName);

            if (cartUser is null)
            {
                var cartProps = Props.Create(() => new CartActor(userName));
                cartUser = Context.ActorOf(cartProps, actorName);
                _carts.Add(cartUser);
            }

            return cartUser;
        }
    }
}
