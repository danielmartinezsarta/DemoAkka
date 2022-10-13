using Akka.Actor;
using MyOnlineStore.Billing;
using MyOnlineStore.Billing.Refs;

Console.WriteLine("Starting billing system");

var service = new BillingService();
service.Start();
var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += async (sender, events) =>
{
    await service.StopAsync(cancellationTokenSource.Token);
    events.Cancel = true;
};

ActorDirectory.ActorSystem!.WhenTerminated.Wait();