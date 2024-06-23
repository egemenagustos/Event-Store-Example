using EventStore.Client;
using System.Text.Json;

string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";

#region Bağlantı Kurma
var settings = EventStoreClientSettings
    .Create(connectionString);

var client = new EventStoreClient(settings);
#endregion

#region Event oluşturma ve Stream'e gönderme
//OrderPlacedEvent orderPlacedEvent = new()
//{
//    OrderId = 1,
//    TotalAmount = 2000
//};

//Gönderecek olduğumuz datayı event store'a yollamak için önce eventdata türüne dönüştürmemiz gerekiyor.
//EventData eventData = new(
//    eventId: Uuid.NewUuid(),
//    type: orderPlacedEvent.GetType().Name,
//    data: JsonSerializer.SerializeToUtf8Bytes(orderPlacedEvent)
//    );

//await client.AppendToStreamAsync(
//    streamName : "order-stream",
//    expectedState : StreamState.Any,
//    eventData : new[] {eventData}
//    );
#endregion

#region Stream Okuma
//Bütün streamleri getirir.
//await client.ReadAllAsync();

//Adını vermiş olduğumuz stream'i getirir.
//var events =  client.ReadStreamAsync(
//    streamName : "order-stream",
//    direction : Direction.Forwards,
//    revision : StreamPosition.Start
//    );

//var datas = await events.ToListAsync();
#endregion

#region Stream Subscription
await client.SubscribeToStreamAsync(
    streamName : "order-stream",
    start :  FromStream.Start,
    eventAppeared : async(streamSubscription, resolvedEvent, CancellationToken) =>
    {
        OrderPlacedEvent @event = JsonSerializer
        .Deserialize<OrderPlacedEvent>(resolvedEvent.Event.Data.ToArray());

        Console.WriteLine(JsonSerializer.Serialize(@event));
    },
    subscriptionDropped: (streamSubscription, subscriptionDroppedReason, exception) => Console.WriteLine("Disconnected..!")
    );
#endregion

Console.ReadLine();


public class OrderPlacedEvent
{
    public int OrderId { get; set; }

    public int TotalAmount { get; set; }
}