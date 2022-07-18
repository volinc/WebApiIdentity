using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Auth.Authentication;

[DataContract]
public class AccessTokenResponse
{
    [DataMember(Name = "access_token")]
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [DataMember(Name = "token_type")]
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;

    [DataMember(Name = "expires_in")]
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [DataMember(Name = "refresh_token")]
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}