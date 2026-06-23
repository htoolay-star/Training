namespace WebApp.Services.Api;

public sealed class ApiEndpoint
{
    public static string Login { get; } = "/api/Auth/Login";

    public static string Refresh { get; } = "api/Auth/Refresh";

    public static string Logout { get; } = "api/Auth/Logout";

    // Role
    public static string Role { get; } = "api/Role";
    public static string RoleById(long id) => $"api/Role/{id}";
    
    //Admin
    public static string Admin { get; } = "api/admin";
}