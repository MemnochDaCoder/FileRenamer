namespace FileRenamer.Models
{
    public class ConfirmedChange
    {
        public required string DirectoryPath { get; set; }
        public required string OriginalName { get; set; }
        public required string NewName { get; set; }
    }
}
