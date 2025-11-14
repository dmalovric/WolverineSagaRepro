namespace WolverineSagaIssue.StringId;

public class LocalOrderWithStringId
{
    public required string Id { get; set; }
    public required string Description { get; set; } = null!;
    public bool ConfirmationEmailSent { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; } = null;
};
