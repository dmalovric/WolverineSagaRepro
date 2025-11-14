namespace WolverineSagaIssue.GuidId;

// This message would be published externally when an order has been fully processed
public record GuidIdOrderProcessedMessage(Guid Id);
