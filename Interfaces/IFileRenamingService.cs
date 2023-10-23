using FileRenamer.Models;

namespace FileRenamer.Interfaces
{
    public interface IFileRenamingService
    {
        Task<List<ProposedChangeModel>> ProposeChangesAsync(RenamingTask task);
        bool ExecuteRenamingAsync(List<ConfirmedChangeModel> confirmedChanges);
    }
}
