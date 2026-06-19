namespace Shared.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public int MaxFailedAttempts { get; init; } = 5;
    public int LockoutMinutes { get; init; } = 15;
}
