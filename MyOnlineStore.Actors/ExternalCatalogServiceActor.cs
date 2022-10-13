using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using MyOnlineStore.Entities;
using MyOnlineStore.Messages.Catalogs;

namespace MyOnlineStore.Actors
{
    public class ExternalCatalogServiceActor: ReceiveActor
    {
        private readonly IHttpClientFactory _clientFactory;

        public ExternalCatalogServiceActor(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            Receive<QueryExternalCatalogMessage>(HandleQueryExternalCatalogMessage);

        }

        private void HandleQueryExternalCatalogMessage(QueryExternalCatalogMessage message)
        {
            var sender = Sender;
            var client = _clientFactory.CreateClient("external-catalog");

            client.GetAsync("https://633c7ed874afaef1640a3b30.mockapi.io/products")
                .ContinueWith(async httpRequest =>
                {
                    var response = httpRequest.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var final = await response.Content.ReadFromJsonAsync<List<Product>>();
                        return new AvailableProductsResponse(final.ToImmutableArray());
                    }

                    return default(AvailableProductsResponse);
                }).PipeTo(sender);
        }
    }
}
