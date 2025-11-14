namespace WolverineSagaIssue.StringId;

// This message would be consumed from an external source to start the order processing saga

public record StringIdOrderPlacedMessage(
    string Id,
    string Description
);
