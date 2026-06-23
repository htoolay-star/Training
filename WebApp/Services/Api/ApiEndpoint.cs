namespace WebApp.Services.Api;

public sealed class ApiEndpoint
{
    public static string Login { get; } = "/api/Auth/Login";

    public static string Refresh { get; } = "api/Auth/Refresh";
    
    public static string Logout { get; } = "api/Auth/Logout";

}