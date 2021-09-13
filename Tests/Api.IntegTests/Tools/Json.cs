using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.IntegTests.Tools {
  public class Json {
    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj);

    public static StringContent SerializeRequest<T>(T obj) => new StringContent(Serialize(obj), Encoding.Default, "application/json");

    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    public async static Task<T> DeserializeResponse<T>(HttpResponseMessage response) => Deserialize<T>(await response.Content.ReadAsStringAsync());
  }
}