namespace CertiFlowApp.Features.Public
{
    // Sealed because it is a DTO used for read-only UI data
    public sealed class PublicDashboardDto
    {
        // Counts are initialized to 0 by default
        // Will be set to actual values when creating the DTO
        // Counts are read-only after initialization
        // Counts are used to display statistics on the public dashboard
        // The business logic is implemented in the PublicDashboardService
        public int ApprovedJobCount { get; init; }
        public int MeasurementCount { get; init; }
        public int AvailableToolCount { get; init; }
    }
}
