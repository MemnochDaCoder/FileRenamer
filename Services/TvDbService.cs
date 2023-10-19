using FileRenamer.Interfaces;
using Newtonsoft.Json;

namespace FileRenamer.Services
{
    public class TvDbService : ITvDbService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TvDbService> _logger;

        public TvDbService(HttpClient httpClient, ILogger<TvDbService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string?> GetNewNameAsync(string originalName, string fileType)
        {
            try
            {
                var apiUrl = $"https://api.thetvdb.com/search?name={originalName}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Root>(content);

                    if (fileType == "movie")
                    {
                        return $"{data.Data[0].Name} ({data.Data[0].Year})";
                    }
                    else
                    {
                        // Additional logic to fetch episode names and construct the new name
                    }
                }
                else
                {
                    _logger.LogError($"Failed to fetch data from the API. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data from the API.");
            }

            return null;
        }
    }
}
