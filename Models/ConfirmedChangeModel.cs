namespace FileRenamer.Models
{
    public class ConfirmedChangeModel
    {
        public required string DirectoryPath { get; set; }
        public required string OriginalName { get; set; }
        public required string NewName { get; set; }
    }
}
