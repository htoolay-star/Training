namespace Shared.Base
{
    public interface IBaseService
    {
        string? AccessToken { get; }
        long? UserId { get; }
        string? UserName { get; }
        string? RoleCode { get; }
        DateTime CurrentMyanmarDateTime { get; }
    }
}
