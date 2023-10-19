using Microsoft.VisualBasic.FileIO;

namespace FileRenamer.Interfaces
{
    public interface ITvDbService
    {
        Task<string> GetNewNameAsync(string originalName, FileType fileType, string seasonAndEpisode = null);
    }
}
