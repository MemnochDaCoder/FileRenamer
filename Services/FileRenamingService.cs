using FileRenamer.Interfaces;
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

        public async Task<List<ProposedChange>> ProposeChangesAsync(RenamingTask task)
        {
            var proposedChanges = new List<ProposedChange>();
            var videoFiles = Directory.GetFiles(task.SourceDirectory)
                                     .Where(f => Regex.IsMatch(f, @"\.(mp4|mkv|avi)$"))
                                     .ToList();

            var isMovie = videoFiles.Count == 1;

            foreach (var filePath in videoFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileType = isMovie ? FileType.Movie : FileType.TvShow;
                var newName = await _tvDbService.GetNewNameAsync(fileName, fileType);

                if (!string.IsNullOrEmpty(newName))
                {
                    proposedChanges.Add(new ProposedChange { OriginalName = fileName, NewName = newName });
                }
            }

            return proposedChanges;
        }

        public async Task<bool> ExecuteRenamingAsync(List<ConfirmedChange> confirmedChanges)
        {
            // ... (existing logic for renaming files)
        }
    }
}
