using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoBikeShopWebApp.Services
{
    public static class ApiHelper
    {
        public static string SerializeObj<T>(T modelObject)
        {
            return JsonSerializer.Serialize(modelObject, JsonOptions());
        }

        public static T DeserializeJsonString<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;
        }

        public static StringContent GenerateStringContent(string serialiazedObj)
        {
            return new StringContent(serialiazedObj, Encoding.UTF8, "application/json");
        }

        public static IList<T> DeserializeJsonStringList<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;
        }

        public static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
            };
        }

        public static string GetDescription(string description)
        {
            var appendDots = "...";
            var maxLenth = 100;
            var descriptionLength = description.Length;
            return descriptionLength > maxLenth ? $"{description.Substring(0, 100)}{appendDots}" : description;
        }

        public static async Task<string> ReadContent(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        public static JsonContent GetJsonContent<T>(T modelObject)
        {
            return JsonContent.Create(modelObject);
        }
    }
}