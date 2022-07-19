namespace Auth.Authentication;

public class HttpHeaderSettings
{
    public string IPHeaderName { get; init; } = null!;
    public string IPCountryHeaderName { get; init; } = null!;
}