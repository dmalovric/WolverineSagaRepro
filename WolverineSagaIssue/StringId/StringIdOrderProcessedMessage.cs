namespace WolverineSagaIssue.StringId;

// This message would be published externally when an order has been fully processed
public record StringIdOrderProcessedMessage(string Id);
