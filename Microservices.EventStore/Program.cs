using EventStore.Client;
using System.Text.Json;

//string connectionString = "";

#region Bağlantı Kurma
//var settings = EventStoreClientSettings
//    .Create(connectionString);

//var client = new EventStoreClient(settings);
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
//await client.SubscribeToStreamAsync(
//    streamName : "order-stream",
//    start :  FromStream.Start,
//    eventAppeared : async(streamSubscription, resolvedEvent, CancellationToken) =>
//    {
//        OrderPlacedEvent @event = JsonSerializer
//        .Deserialize<OrderPlacedEvent>(resolvedEvent.Event.Data.ToArray());

//        Console.WriteLine(JsonSerializer.Serialize(@event));
//    },
//    subscriptionDropped: (streamSubscription, subscriptionDroppedReason, exception) => Console.WriteLine("Disconnected..!")
//    );
#endregion

#region Bakiye Örneği
EventStoreService eventStoreService = new();
//AccountCreatedEvent accountCreatedEvent = new()
//{
//    AccountId = "12345",
//    CustomerId = "98765",
//    StartBalance = 0,
//    Date = DateTime.UtcNow.Date
//};

//MoneyDepositedEvent moneyDepositedEvent1 = new()
//{
//    AccountId = "12345",
//    Amount = 1000,
//    Date = DateTime.UtcNow.Date
//};

//MoneyDepositedEvent moneyDepositedEvent2 = new()
//{
//    AccountId = "12345",
//    Amount = 500,
//    Date = DateTime.UtcNow.Date
//};

//MoneyWithdrawnEvent moneyWithdrawnEvent = new()
//{
//    AccountId = "12345",
//    Amount = 200,
//    Date = DateTime.UtcNow.Date
//};

//MoneyDepositedEvent moneyDepositedEvent3 = new()
//{
//    AccountId = "12345",
//    Amount = 50,
//    Date = DateTime.UtcNow.Date
//};

//MoneyTransferredEvent moneyTransferredEvent1 = new()
//{
//    AccountId = "12345",
//    Amount = 250,
//    Date = DateTime.UtcNow.Date
//};

//MoneyTransferredEvent moneyTransferredEvent2 = new()
//{
//    AccountId = "12345",
//    Amount = 150,
//    Date = DateTime.UtcNow.Date
//};

//MoneyDepositedEvent moneyDepositedEvent4 = new()
//{
//    AccountId = "12345",
//    Amount = 2000,
//    Date = DateTime.UtcNow.Date
//};

//await eventStoreService.AppendToStreamAsync(
//    streamName: $"customer-{accountCreatedEvent.CustomerId}-stream",
//    eventDatas: new[]
//    {
//    eventStoreService.GenerateEventData(accountCreatedEvent),
//    eventStoreService.GenerateEventData(moneyDepositedEvent1),
//    eventStoreService.GenerateEventData(moneyDepositedEvent2),
//    eventStoreService.GenerateEventData(moneyWithdrawnEvent),
//    eventStoreService.GenerateEventData(moneyDepositedEvent3),
//    eventStoreService.GenerateEventData(moneyTransferredEvent1),
//    eventStoreService.GenerateEventData(moneyTransferredEvent2),
//    eventStoreService.GenerateEventData(moneyDepositedEvent4),
//    });
BalanceInfo balanceInfo = new();


await eventStoreService.SubscribeToStreamAsync(
    streamName: "customer-98765-stream",
    eventAppeared: async (streamSubscription, resolvedEvent, cancellationToken) =>
    {
        string eventType = resolvedEvent.Event.EventType;
        object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(), Type.GetType(eventType));

        switch (@event)
        {
            case AccountCreatedEvent e:
                balanceInfo.AccountId = e.AccountId;
                balanceInfo.Balance = e.StartBalance;
                break;

            case MoneyDepositedEvent e:
                balanceInfo.Balance += e.Amount;
                break;

            case MoneyWithdrawnEvent e:
                balanceInfo.Balance -= e.Amount;
                break;

            case MoneyTransferredEvent e:
                balanceInfo.Balance -= e.Amount;
                break;

            default:
                break;
        }
        Console.WriteLine(JsonSerializer.Serialize(balanceInfo));
    }
    );


#endregion

Console.ReadLine();
