namespace Auth.Authentication.SessionStorage;

public class ClientInfo
{
    public string? UserAgent { get; set; }
    public string? Ip { get; set; }
    public string? IpCountry { get; set; }
}