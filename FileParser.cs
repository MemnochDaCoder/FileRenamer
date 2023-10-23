namespace FileRenamer
{
    public static class FileParser
    {
        public static (string name, string? seasonAndEpisode) ParseFileName(string fileName)
        {
            // Logic to parse file names and extract the necessary information.
            var parts = fileName.Split('.');
            var name = parts[0];
            var seasonAndEpisode = parts.Length > 1 ? parts[1] : null;
            return (name, seasonAndEpisode);
        }
    }
}
