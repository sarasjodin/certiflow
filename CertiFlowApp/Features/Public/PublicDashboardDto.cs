namespace CertiFlowApp.Features.Public
{
    public sealed class PublicDashboardDto
    {
        public int ApprovedJobCount { get; init; }
        public int MeasurementCount { get; init; }
        public int AvailableToolCount { get; init; }
    }
}
