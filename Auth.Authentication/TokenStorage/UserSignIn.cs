namespace Auth.Authentication.TokenStorage;

public class UserSignIn
{
    public Dictionary<string, SecurityTokens> Items { get; } = new();
}