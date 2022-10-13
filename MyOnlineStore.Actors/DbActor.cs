using System.Collections;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using Akka.Actor;
using MyOnlineStore.Entities;
using MyOnlineStore.Messages.Catalogs;

namespace MyOnlineStore.Actors
{
    public class DbActor: ReceiveActor
    {
        public DbActor()
        {
            Receive<LoadAvailableProducts>(HandleLoadAvailableProducts);
            Receive<QueryStoreCatalog>(HandleQueryStoreCatalog);
        }

        private void HandleQueryStoreCatalog(QueryStoreCatalog obj)
        {
            Sender.Tell(new AvailableProductsResponse(
                Enumerable.Range(0, 3).Select(x => new Product() { Id = x, Name = $"Product Available {x}" }).ToImmutableArray()));
        }

        private void HandleLoadAvailableProducts(LoadAvailableProducts message)
        {
            //closures
            var sender = Sender;

            Task.Run(() =>
            {
                Task.Delay(10000).Wait();
                return Enumerable.Range(0, 10).Select(x => new Product() { Id = x, Name = $"Product {x}" }).ToImmutableArray();
            }).ContinueWith(r=> new AvailableProductsResponse(r.Result), 
                TaskContinuationOptions.ExecuteSynchronously).PipeTo(sender);
        }
    }
}