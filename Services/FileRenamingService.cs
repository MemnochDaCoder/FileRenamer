using FileRenamer.Interfaces;
using System.Text.RegularExpressions;

namespace FileRenamer.Services
{
    public class FileRenamingService : IFileRenamingService
    {
        private readonly ITvDbService _tvDbService;
        private readonly ILogger<FileRenamingService> _logger;

        public FileRenamingService(ITvDbService tvDbService, ILogger<FileRenamingService> logger)
        {
            _tvDbService = tvDbService;
            _logger = logger;
        }

        public async Task<List<ProposedChange>> ProposeChangesAsync(RenamingTask task)
        {
            var proposedChanges = new List<ProposedChange>();
            var videoFiles = Directory.GetFiles(task.SourceDirectory).Where(f => Regex.IsMatch(f, @"\.(mp4|mkv|avi)$")).ToList();

            foreach (var filePath in videoFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileType = videoFiles.Count > 1 ? "show" : "movie";
                var proposedName = await _tvDbService.GetNewNameAsync(fileName, fileType);

                if (!string.IsNullOrEmpty(proposedName))
                {
                    proposedChanges.Add(new ProposedChange { OriginalName = fileName, NewName = proposedName });
                }
            }

            return proposedChanges;
        }

        public async Task<bool> ExecuteRenamingAsync(List<ConfirmedChange> confirmedChanges)
        {
            // Implementation for renaming the files
        }
    }
}
