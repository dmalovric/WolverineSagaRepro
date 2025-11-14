using Wolverine;

namespace WolverineSagaIssue.StringId;

public record PersistOrder(string Id, string Description);
public record OrderPersisted(string Id);
public record OrderEmailSent(string Id);

// Somewhat contrived example of a saga that processes an order, but also needs to persist locally to keep track of processing state

public class StringIdOrderSaga : Saga
{
    public string? Id { get; set; }

    public static (StringIdOrderSaga, PersistOrder) Start(StringIdOrderPlacedMessage message)
    {
        return
        (
            new StringIdOrderSaga
            {
                Id = message.Id
            }, 
            new PersistOrder
            (
                message.Id, 
                message.Description
            )
        );
    }

    public async Task<OrderPersisted> Handles(PersistOrder order, IRepository repository)
    {
        // Persist the order to the local database
        LocalOrderWithStringId toPersist = new()
        {
            Id = order.Id,
            Description = order.Description
        };
        var persistedOrder = await repository.Create(toPersist);

        return new OrderPersisted(Id: persistedOrder.Id);
    }

    public async Task<OrderEmailSent> Handles(OrderPersisted order, IRepository repository)
    {
        // Pretend we send a confirmation email here
        await Task.Delay(1000);

        await repository.MarkConfirmationEmailSent(order.Id);

        return new OrderEmailSent(order.Id);
    }

    public async Task<StringIdOrderProcessedMessage> Handles(OrderEmailSent orderEmailSent, IRepository repository)
    {
        await repository.MarkAsProcessed(orderEmailSent.Id);

        return new StringIdOrderProcessedMessage(orderEmailSent.Id);
    }

    public void Handles(StringIdOrderProcessedMessage message)
    {
        MarkCompleted();
    }
}
