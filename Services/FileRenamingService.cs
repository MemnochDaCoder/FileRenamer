using FileRenamer.Enum;
using FileRenamer.Interfaces;
using FileRenamer.Models;
using Microsoft.OpenApi.Services;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace FileRenamer.Services
{
    public class FileRenamingService : IFileRenamingService
    {
        private readonly ILogger<FileRenamingService> _logger;
        private readonly ITvDbService _tvDbService;

        public FileRenamingService(ILogger<FileRenamingService> logger, ITvDbService tvDbService)
        {
            _logger = logger;
            _tvDbService = tvDbService;
        }

        public async Task<List<ProposedChange>> ProposeChangesAsync(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var (name, seasonAndEpisode) = _fileParser.ParseFileName(fileName);

            var searchResults = await _tvDbService.SearchShowsOrMoviesAsync(name);
            // Further logic to propose changes based on the search results
            return proposedChanges;
        }

        public async Task<List<SearchResult>> SearchShowsOrMoviesAsync(string query)
        {
            // Implement the logic to call the search endpoint of the TVDB API
            // Parse the response and return a list of SearchResult objects
        }

        public async Task<MovieDetailModel> GetMovieDetailAsync(string movieId)
        {
            // Implement the logic to get movie details using the movieId
            // Parse the response and return a MovieDetail object
        }

        public async Task<EpisodeDetailModel> GetEpisodeDetailAsync(string episodeId)
        {
            // Implement the logic to get episode details using the episodeId
            // Parse the response and return an EpisodeDetail object
        }

        public async Task<bool> ExecuteRenamingAsync(List<ConfirmedChangeModel> confirmedChanges)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing renaming.");
                return false;
            }
        }
    }
}
