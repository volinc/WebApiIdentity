using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Auth.Authentication.SessionStorage;

public class Utf8JsonBinarySerializer
{
    private readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        Culture = CultureInfo.InvariantCulture
    };
    
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
}