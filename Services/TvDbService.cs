using FileRenamer.Enum;
using FileRenamer.Interfaces;
using FileRenamer.Models;
using Microsoft.VisualBasic.FileIO;
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

        public async Task<string> GetNewNameAsync(string originalName, FileType fileType, string? seasonAndEpisode = null)
        {
            try
            {
                string apiUrl = $"https://api.thetvdb.com/search?name={originalName}";
                if (fileType == FileType.TVShow && !string.IsNullOrEmpty(seasonAndEpisode))
                {
                    apiUrl += $"&seasonAndEpisode={seasonAndEpisode}";
                }

                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Root>(content);
                    // Logic to extract and construct the new name from the API response
                    return fileType == FileType.Movie ?
                        $"{data.Data[0].Name} ({data.Data[0].Year})" :
                        $"{data.Data[0].Name} {seasonAndEpisode} {data.Data[0].Overview}";
                }
                else
                {
                    _logger.LogError($"Failed to fetch data from the API. Status code: {response.StatusCode}");
                    return null!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data from the API.");
                return null!;
            }
        }
    }
}
