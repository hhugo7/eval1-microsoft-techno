using System.Net.Http.Json;

namespace VotePreferentiel;

public class VoteProvider
{
    private readonly HttpClient _httpClient;

    public VoteProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VoteModel?> GetVotesAsync(string url)
    {
        return await _httpClient.GetFromJsonAsync<VoteModel>(url);
    }
}