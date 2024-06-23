using EventStore.Client;
using System.Text.Json;

public class EventStoreService
{
    EventStoreClientSettings GetEventStoreClientSettings(string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false") =>
        EventStoreClientSettings.Create(connectionString);

    EventStoreClient Client { get => new EventStoreClient(GetEventStoreClientSettings()); }

    public async Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventDatas)
    => await Client.AppendToStreamAsync(
        streamName: streamName,
        eventData: eventDatas,
        expectedState: StreamState.Any
        );

    public EventData GenerateEventData(object @event)
        => new(
            eventId : Uuid.NewUuid(),
            type : @event.GetType().Name,
            data : JsonSerializer.SerializeToUtf8Bytes(@event)
            );

    public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared)
        => await Client.SubscribeToStreamAsync(
            streamName : streamName,
            start : FromStream.Start,
            eventAppeared : eventAppeared,
            subscriptionDropped: (streamSubscription, subscriptionDroppedReason, exception) => Console.WriteLine("Disconnected..!")
            );
}