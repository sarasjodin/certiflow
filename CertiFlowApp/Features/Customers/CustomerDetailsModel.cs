namespace CertiFlowApp.Features.Customers
{
    public sealed class CustomerDetailsModel
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string OrganizationNumber { get; init; } = string.Empty;

        public DateTimeOffset CreatedAtUtc { get; init; }

        public string CreatedByUserId { get; init; } = string.Empty;

        public string CreatedByUserName { get; init; } = string.Empty;

        public DateTimeOffset? UpdatedAtUtc { get; init; }

        public string? UpdatedByUserId { get; init; }

        public string? UpdatedByUserName { get; init; }

        public int JobCount { get; init; }
    }
}
