namespace WolverineSagaIssue.GuidId;

public class LocalOrderWithGuidId
{
    public required Guid Id { get; set; }
    public required string Description { get; set; } = null!;
    public bool ConfirmationEmailSent { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; } = null;
};
