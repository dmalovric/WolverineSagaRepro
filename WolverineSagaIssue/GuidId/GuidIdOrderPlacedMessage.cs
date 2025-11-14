namespace WolverineSagaIssue.GuidId;

// This message would be consumed from an external source to start the order processing saga

public record GuidIdOrderPlacedMessage(
    Guid Id,
    string Description
);
