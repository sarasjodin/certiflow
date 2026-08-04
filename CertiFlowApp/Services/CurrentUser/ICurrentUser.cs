namespace CertiFlowApp.Services.CurrentUser
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
    }
}
