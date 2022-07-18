namespace Auth.Authentication.SessionStorage;

public class UserSignIn
{
    public Dictionary<string, SecurityTokens> Items { get; } = new();
}