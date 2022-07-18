using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Auth.Authentication.TokenStorage;

public class Utf8JsonBinarySerializer
{
    public static readonly JsonSerializerSettings DefaultSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        Culture = CultureInfo.InvariantCulture
    };

    private readonly JsonSerializerSettings _settings;

    public Utf8JsonBinarySerializer() : this(DefaultSettings)
    {
    }

    public Utf8JsonBinarySerializer(JsonSerializerSettings settings)
    {
        _settings = settings;
    }

    public byte[] Serialize<T>(T value)
    {
        var json = JsonConvert.SerializeObject(value, _settings);
        return Encoding.UTF8.GetBytes(json);
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json, _settings);
    }

    public Task<T?> DeserializeAsync<T>(Stream stream) => throw new NotSupportedException();
}