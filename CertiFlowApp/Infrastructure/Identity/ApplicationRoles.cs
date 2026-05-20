namespace CertiFlow.Web.Infrastructure.Identity;

public static class ApplicationRoles
{
    public const string Operator = "Operator";
    public const string Verifier = "Verifier";
    public const string Approver = "Approver";
    public const string SystemAdmin = "SystemAdmin";
    public const string Client = "Client";

    public static readonly string[] All =
    [
        Operator,
        Verifier,
        Approver,
        SystemAdmin,
        Client
    ];
}