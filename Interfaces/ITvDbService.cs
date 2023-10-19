namespace FileRenamer.Interfaces
{
    public interface ITvDbService
    {
        Task<string?> GetNewNameAsync(string originalName, string fileType);
    }
}
