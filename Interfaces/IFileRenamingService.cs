namespace FileRenamer.Interfaces
{
    public interface IFileRenamingService
    {
        Task<List<ProposedChange>> ProposeChangesAsync(RenamingTask task);
        Task<bool> ExecuteRenamingAsync(List<ConfirmedChange> confirmedChanges);
    }
}
