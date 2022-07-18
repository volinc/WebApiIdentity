namespace Auth.Authentication.SessionStorage;

public static class SessionStorageHelper
{
    public static string Key(string audience, string userId) => $"Token-{audience}-{userId}";
    public static string Key(string audience, long userId) => $"Token-{audience}-{userId}";
    public static string Key(string userId) => $"Token-{userId}";
    public static string Key(long userId) => $"Token-{userId}";
}