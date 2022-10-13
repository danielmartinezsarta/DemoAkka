using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using MyOnlineStore.Messages;
using MyOnlineStore.Messages.Catalogs;

namespace MyOnlineStore.Actors
{
    public class CatalogActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IStash Stash { get; set; }
        private IActorRef? _dbActor;
        private IActorRef? _cacheActor;
        private IActorRef? _externalCatalogService;
        private int _callCounter = 0;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        public CatalogActor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            Initializing();
        }

        private void Initializing()
        {
            Receive<StartSystemMessage>(HandleStartSystemMessage);
            Receive<AvailableProductsResponse>(HandleAvailableProductsResponse);
            ReceiveAny(_=>
            {
                Stash.Stash();
                Sender.Tell(new ServiceUnavailableResponse("Starting system"));
            });
        }

        private void StartReceiving()
        {
            ReceiveAsync<QueryStoreCatalog>(HandleQueryStoreCatalog);
            ReceiveAny(_ => Sender.Tell("I don't understand"));
        }

        private void HandleStartSystemMessage(StartSystemMessage message)
        {
            _dbActor.Tell(new LoadAvailableProducts());
        }
        private void HandleAvailableProductsResponse(AvailableProductsResponse message)
        {
            _cacheActor.Tell(message);
            Become(StartReceiving);
            Stash.UnstashAll();
        }


        protected override void PreStart()
        {
            var dbActorProps = Props.Create(() => new DbActor());
            _dbActor = Context.ActorOf(dbActorProps, "db-actor");

            var cacheActorProps = Props.Create(() => new DbActor());
            _cacheActor = Context.ActorOf(cacheActorProps, "cache-actor");

            var externalCatalogServiceProps = Props.Create(() => new ExternalCatalogServiceActor(_httpClientFactory));
            _externalCatalogService = Context.ActorOf(externalCatalogServiceProps, "external-catalog-actor");

            base.PreStart();
        }

        private async Task HandleQueryStoreCatalog(QueryStoreCatalog message)
        {
            _callCounter++;
            _logger.Info("Request No: {0}", _callCounter);
            // Load inventory
            // match with catalog
            //Sender.Tell(await _dbActor.Ask(message));
            Sender.Tell(await _externalCatalogService.Ask(new QueryExternalCatalogMessage()));
        }

        
    }
}
