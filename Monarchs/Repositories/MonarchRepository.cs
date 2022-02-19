using Monarchs.Dtos;
using Newtonsoft.Json;

namespace Monarchs.Repositories;

// Should probably interface this to make it easier to stub, but seeing as this is on the API level DTO
// (and no Tests in sight) it may be a bit overkill for now.
// Would make more sense to create an extra abstraction for getting the Monarch Entity instead of the DTO.
public class MonarchRepository
{
    private readonly HttpClient _client;

    public MonarchRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<MonarchDto>> GetMonarchsAsync()
    {
        // Would add this in AppSettings.json.
        var response = await _client.GetAsync(
            "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        var monarchs = JsonConvert.DeserializeObject<List<MonarchDto>>(responseBody);
        if (monarchs == null || !monarchs.Any())
        {
            throw new Exception("No Monarchs found.");
        }

        return monarchs;
    }
}