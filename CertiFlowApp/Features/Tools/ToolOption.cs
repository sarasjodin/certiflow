namespace CertiFlowApp.Features.Tools;

// Sealed because it is a read selection model used for UI (not intended to be inherited)
// Used to display tool dropdown - combines Name and SerialNumber
public sealed class ToolOption
{
    // Id used as foreign key - not displayed to the user
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;
}
