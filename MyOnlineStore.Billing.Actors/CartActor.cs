using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MyOnlineStore.Billing.Actors.State;
using MyOnlineStore.Billing.Entities;
using MyOnlineStore.Messages;
using MyOnlineStore.Messages.Billing;
using MyOnlineStore.Messages.Billing.Commands;
using MyOnlineStore.Messages.Billing.Responses;
using MyOnlineStore.Messages.Events;

namespace MyOnlineStore.Billing.Actors
{
    public class CartActor : ReceivePersistentActor
    {
        private string? _user;
        private CartActorState? _state;
        private ILoggingAdapter _logger = Context.GetLogger();
        private int _snapshotMaxNumber = 2;
        public override string PersistenceId { get; }

        protected override void PreStart()
        {
            _state = new CartActorState();
            base.PreStart();
        }

        public CartActor(string? user)
        {
            PersistenceId = user;
            _user = user;

            Recover<SnapshotOffer>(offer =>
            {
                _state = offer.Snapshot as CartActorState;


            });
            Recover<AddedNewCartItem>(item =>
            {
                _state.Items.Add(new CartItem()
                {
                    Product = item.Message.ProductName,
                    Quantity = item.Message.Quantity,
                    Value = item.Message.Value
                });
            });
            Recover<RecoveryCompleted>(_ =>
            {
                Become(Ready);
                Stash.UnstashAll();
            });

            Initializing();
        }

        #region Behaviours

        private void Initializing()
        {
            
            CommandAny(_ =>
            {
                Stash.Stash();
                Sender.Tell(new ServiceUnavailableResponse("Recovering Cart State"));
            });
        }

        private void Ready()
        {
            Command<AddProductToCart>(HandleAddProductToCart);
            Command<GetUserCartCommand>(HandleGetUserCartCommand);
            Command<SaveSnapshotSuccess>(HandleSuccessSnapshot);
            Command<SaveSnapshotFailure>(failure => _logger.Error(failure.Cause.ToString()));
        }

        #endregion

        #region Command Handlers
        private void HandleSuccessSnapshot(SaveSnapshotSuccess ack)
        {
            _logger.Info($"Snapshot OK: {ack.Metadata.SequenceNr}");
            DeleteMessages(ack.Metadata.SequenceNr);
            DeleteSnapshots(new SnapshotSelectionCriteria(ack.Metadata.SequenceNr - _snapshotMaxNumber));
        }

        private void HandleGetUserCartCommand(GetUserCartCommand command)
        {
            Sender.Tell(new GetUserCartResponse(_state.Items));
        }

        private void HandleAddProductToCart(AddProductToCart message)
        {
            var sender = Sender;
            Persist(new AddedNewCartItem() { Message = message }, item =>
            {
                _state.Items.Add(new CartItem() { Product = item.Message.ProductName, Quantity = item.Message.Quantity, Value = item.Message.Value });
                sender.Tell(new ProductAddedToCartResponse(DateTime.UtcNow));

                if (LastSequenceNr % _snapshotMaxNumber == 0)
                    SaveSnapshot(_state);

            });


        }

        #endregion

    }
}