namespace CertiFlowApp.Features.Jobs;

// Sealed because it is a read selection model used for UI (not intended to be inherited)
// Used to display job dropdown - combines JobNumber and Title
public sealed class JobOption
{
    // Id used as foreign key - not displayed to the user
    public Guid Id { get; init; }

    public string JobNumber { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
}

