using WolverineSagaIssue.GuidId;
using WolverineSagaIssue.StringId;

namespace WolverineSagaIssue;

public interface IRepository
{
    Task<LocalOrderWithGuidId> Create(LocalOrderWithGuidId order, CancellationToken cancellationToken = default);
    Task MarkConfirmationEmailSent(Guid orderId, CancellationToken cancellationToken = default);
    Task MarkAsProcessed(Guid orderId, CancellationToken cancellationToken = default);
    Task<LocalOrderWithStringId> Create(LocalOrderWithStringId order, CancellationToken cancellationToken = default);
    Task MarkConfirmationEmailSent(string orderId, CancellationToken cancellationToken = default);
    Task MarkAsProcessed(string orderId, CancellationToken cancellationToken = default);
}

public class Repository(LocalOrdersEfContext localOrdersEfContext) : IRepository
{
    public async Task<LocalOrderWithGuidId> Create(LocalOrderWithGuidId order, CancellationToken cancellationToken = default)
    {
        localOrdersEfContext.GuidIdOrders.Add(order);
        await localOrdersEfContext.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task MarkConfirmationEmailSent(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await localOrdersEfContext.GuidIdOrders.FindAsync([orderId], cancellationToken) 
            ?? throw new ArgumentNullException(nameof(orderId));

        order.ConfirmationEmailSent = true;
        await localOrdersEfContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsProcessed(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await localOrdersEfContext.GuidIdOrders.FindAsync([orderId], cancellationToken)
            ?? throw new ArgumentNullException(nameof(orderId));

        order.Processed = true;
        order.ProcessedAt = DateTime.UtcNow;
        await localOrdersEfContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<LocalOrderWithStringId> Create(LocalOrderWithStringId order, CancellationToken cancellationToken = default)
    {
        localOrdersEfContext.StringIdOrders.Add(order);
        await localOrdersEfContext.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task MarkConfirmationEmailSent(string orderId, CancellationToken cancellationToken = default)
    {
        var order = await localOrdersEfContext.StringIdOrders.FindAsync([orderId], cancellationToken)
            ?? throw new ArgumentNullException(nameof(orderId));

        order.ConfirmationEmailSent = true;
        await localOrdersEfContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsProcessed(string orderId, CancellationToken cancellationToken = default)
    {
        var order = await localOrdersEfContext.StringIdOrders.FindAsync([orderId], cancellationToken)
            ?? throw new ArgumentNullException(nameof(orderId));

        order.Processed = true;
        order.ProcessedAt = DateTime.UtcNow;
        await localOrdersEfContext.SaveChangesAsync(cancellationToken);
    }
}
