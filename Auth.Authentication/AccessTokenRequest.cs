using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Auth.Authentication;

[DataContract]
public class AccessTokenRequest
{
    [Required]
    [DataMember(Name = "grant_type")]
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = null!;

    [DataMember(Name = "username")]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [DataMember(Name = "password")]
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [DataMember(Name = "refresh_token")]
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}