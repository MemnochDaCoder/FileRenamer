using FileRenamer.Interfaces;
using FileRenamer.Models;
using System.Text.RegularExpressions;

namespace FileRenamer.Services
{
    public class FileRenamingService : IFileRenamingService
    {
        private readonly ITvDbService _tvDbService;
        private readonly ILogger _logger;

        public FileRenamingService(ITvDbService tvDbService, ILogger<FileRenamingService> logger)
        {
            _tvDbService = tvDbService;
            _logger = logger;
        }

        public async Task<List<ProposedChangeModel>> ProposeChangesAsync(RenamingTask task)
        {
            var proposedChanges = new List<ProposedChangeModel>();
            var allowedExtensions = new[] { ".mp4", ".mkv", ".avi" };

            try
            {
                var files = Directory.GetFiles(task.SourceDirectory)
                    .Where(file => allowedExtensions.Contains(Path.GetExtension(file)))
                    .ToList();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var deconstructedFileName = fileName.Split('.');
                    var apiData = await _tvDbService.SearchShowsOrMoviesAsync(deconstructedFileName[0]);

                    if (apiData != null)
                    {
                        if (files.Count == 0)
                        {
                            _logger.LogError("No files were found in the source directory.");
                            throw new ArgumentException("No files were found in the source directory.");
                        }
                        else if (files.Count == 1)
                        {
                            //Movies
                            _logger.LogInformation("Started renaming a movie.");

                            var movieDetail = await _tvDbService.GetMovieDetailsAsync(int.Parse(apiData.Data[0].Id));

                            proposedChanges.Add(new ProposedChangeModel
                            {
                                OriginalFilePath = file,
                                OriginalFileName = fileName,
                                ProposedFileName = $"{movieDetail.Name} ({movieDetail.Year})",
                                FileType = deconstructedFileName[deconstructedFileName.Length - 1]
                            });
                        }
                        else
                        {
                            //Shows
                            _logger.LogInformation("Started renaming episode.");

                            var pattern = @"S(\d{2})E(\d{2})";
                            Match match = Regex.Match(deconstructedFileName[1], pattern);

                            if (match.Success)
                            {
                                var season = int.Parse(match.Groups[1].Value);
                                var episode = int.Parse(match.Groups[2].Value);
                                var episodeDetail = await _tvDbService.GetEpisodeDetailsAsync(int.Parse(apiData.Data[0].Id), season.ToString(), episode.ToString());
                                var ss = episodeDetail.Data.Episodes[0].SeasonNumber.ToString().Length == 1 ? "S0" : "S";
                                var ee = episodeDetail.Data.Episodes[0].Number.ToString().Length == 1 ? "E0" : "E";
                                var test = $"{episodeDetail.Data.Series.Name} {ss}{episodeDetail.Data.Episodes[0].SeasonNumber}{ee}{episodeDetail.Data.Episodes[0].Number} {episodeDetail.Data.Episodes[0].Name}";

                                proposedChanges.Add(new ProposedChangeModel
                                {
                                    OriginalFilePath = file,
                                    OriginalFileName = fileName,
                                    ProposedFileName = $"{episodeDetail.Data.Series.Name} {ss + episodeDetail.Data.Episodes[0].SeasonNumber}{ee + episodeDetail.Data.Episodes[0].Number} {episodeDetail.Data.Episodes[0].Name}",
                                    FileType = deconstructedFileName[deconstructedFileName.Length - 1],
                                    Season = season.ToString(),
                                    Episode = episode.ToString()
                                });
                            }
                        }
                    }
                }
                return proposedChanges;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proposing changes.");
                return proposedChanges;
            }
        }

        public bool ExecuteRenamingAsync(List<ConfirmedChangeModel> confirmedChanges)
        {
            var allowedExtensions = new[] { ".mp4", ".mkv", ".avi" }; // Defining allowed extensions

            try
            {
                foreach (var change in confirmedChanges)
                {
                    if (allowedExtensions.Contains(Path.GetExtension(change.OriginalFileName))) // Checking file extension before renaming
                    {
                        var oldPath = Path.Combine(change.OriginalFilePath, change.OriginalFileName);
                        var newPath = Path.Combine(change.NewFilePath, change.NewFileName);

                        if (File.Exists(newPath))
                        {
                            _logger.LogWarning($"File with the name {change.NewFileName} already exists. Skipping renaming of {change.OriginalFileName}.");
                            continue;
                        }

                        File.Move(oldPath, newPath);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing renaming.");
                return false;
            }
        }
    }
}
