using Wolverine;

namespace WolverineSagaIssue.GuidId;

public record PersistOrder(Guid Id, string Description);
public record OrderPersisted(Guid Id);
public record OrderEmailSent(Guid Id);

// Somewhat contrived example of a saga that processes an order, but also needs to persist locally to keep track of processing state

public record OrderCompleted(Guid Id);

public class GuidIdOrderSaga : Saga
{
    public Guid? Id { get; set; }

    public static (GuidIdOrderSaga, PersistOrder) Start(GuidIdOrderPlacedMessage message)
    {
        return
        (
            new GuidIdOrderSaga
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

    public void Handles(OrderCompleted message)
    {
        MarkCompleted();
    }

    public async Task<OrderPersisted> Handles(PersistOrder order, IRepository repository)
    {
        // Persist the order to the local database
        var toPersist = new LocalOrderWithGuidId
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

    public async Task<GuidIdOrderProcessedMessage> Handles(OrderEmailSent orderEmailSent, IRepository repository)
    {
        await repository.MarkAsProcessed(orderEmailSent.Id);

        return new GuidIdOrderProcessedMessage(orderEmailSent.Id);
    }

    public void Handles(GuidIdOrderProcessedMessage message)
    {
        MarkCompleted();
    }
}
