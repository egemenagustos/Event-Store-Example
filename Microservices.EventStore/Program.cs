using EventStore.Client;
using System.Text.Json;

string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";

#region Bağlantı Kurma
var settings = EventStoreClientSettings
    .Create(connectionString);

var client = new EventStoreClient(settings);
#endregion

#region Event oluşturma ve Stream'e gönderme
OrderPlacedEvent orderPlacedEvent = new()
{
    OrderId = 1,
    TotalAmount = 2000
};

//Gönderecek olduğumuz datayı event store'a yollamak için önce eventdata türüne dönüştürmemiz gerekiyor.
EventData eventData = new(
    eventId: Uuid.NewUuid(),
    type: orderPlacedEvent.GetType().Name,
    data: JsonSerializer.SerializeToUtf8Bytes(orderPlacedEvent)
    );

await client.AppendToStreamAsync(
    streamName : "order-stream",
    expectedState : StreamState.Any,
    eventData : new[] {eventData}
    );
#endregion

Console.ReadLine();


public class OrderPlacedEvent
{
    public int OrderId { get; set; }

    public int TotalAmount { get; set; }
}