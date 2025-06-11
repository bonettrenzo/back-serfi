using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace backend_serfi.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _apiUrl = "https://restcountries.com/v3.1/all?fields=name";

        public CountryService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<CountryDto>> GetCountryNamesAsync()
        {
            if (_cache.TryGetValue("countryList", out List<CountryDto> cachedCountries))
            {
                return cachedCountries;
            }

            var response = await _httpClient.GetAsync(_apiUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);

            var countryList = new List<CountryDto>();

            foreach (var country in jsonDoc.RootElement.EnumerateArray())
            {
                var name = country.GetProperty("name");
                var common = name.GetProperty("common").GetString();
                var official = name.GetProperty("official").GetString();

                var nativeNames = new Dictionary<string, string>();

                if (name.TryGetProperty("nativeName", out var nativeNameProp))
                {
                    foreach (var native in nativeNameProp.EnumerateObject())
                    {
                        if (native.Value.TryGetProperty("common", out var nativeCommon))
                        {
                            nativeNames[native.Name] = nativeCommon.GetString();
                        }
                    }
                }

                countryList.Add(new CountryDto
                {
                    CommonName = common,
                    OfficialName = official,
                    NativeNames = nativeNames
                });
            }

            _cache.Set("countryList", countryList, TimeSpan.FromHours(12));
            return countryList;
        }
    }
}
public interface ICountryService
{
    Task<List<CountryDto>> GetCountryNamesAsync();
}
public class CountryDto
{
    public string CommonName { get; set; }
    public string OfficialName { get; set; }
    public Dictionary<string, string> NativeNames { get; set; }
}

public class CountryRaw
{
    public Name Name { get; set; }
}

public class Name
{
    public string Common { get; set; }
    public string Official { get; set; }
    public Dictionary<string, NativeName> NativeName { get; set; }
}

public class NativeName
{
    public string Common { get; set; }
    public string Official { get; set; }
}