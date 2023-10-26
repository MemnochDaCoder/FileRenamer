using FileRenamer.Interfaces;
using FileRenamer.Models;
using FileRenamer.Services;

class Program
{
    static async Task Main(string[] args)
    {
        // Logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TvDbService>();

        // HttpClientFactory
        var httpClientFactory = new SimpleHttpClientFactory(); // You might need to implement a simple factory

        // IConfiguration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Creating TvDbService
        var tvDbService = new TvDbService(httpClientFactory, logger, configuration);

        // Creating FileRenamingService
        var fileRenamerService = new FileRenamingService(tvDbService, loggerFactory.CreateLogger<FileRenamingService>());

        string sourceFolderPath = GetFolderPath("Enter the source folder path:");
        string destinationFolderPath = GetFolderPath("Enter the destination folder path:");

        // Assuming ProposeChanges is a method that communicates with the service and gets the proposed changes
        var proposedChanges = await fileRenamerService.ProposeChangesAsync(new RenamingTask() { SourceDirectory = sourceFolderPath, DestinationDirectory = destinationFolderPath });

        Console.WriteLine("Proposed Changes:");
        for (int i = 0; i < proposedChanges.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {proposedChanges[i]}");
        }

        while (true)
        {
            Console.WriteLine("Enter the number of the change you want to modify or press Enter to proceed:");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                break;

            int index;
            if (int.TryParse(input, out index) && index >= 1 && index <= proposedChanges.Count)
            {
                Console.WriteLine($"Current: {proposedChanges[index - 1]}");
                Console.WriteLine("Enter new value:");
                var proposedChange = Console.ReadLine();
                proposedChanges[index - 1] = new ProposedChangeModel()
                {
                    OriginalFilePath = sourceFolderPath,
                    ProposedFileName = proposedChange ?? throw new ArgumentNullException("The proposed change was null."),
                    FileType = proposedChanges[index - 1].FileType,
                    OriginalFileName = proposedChanges[index - 1].OriginalFileName
                };
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number or press Enter to proceed.");
            }
        }

        // Assuming ExecuteRenaming is a method that executes the renaming process
        var confirmedList = ConvertProposedToConfirmed(proposedChanges, destinationFolderPath) ?? throw new ArgumentException($"The confirmed list returned null.");

        var success = fileRenamerService.ExecuteRenamingAsync(confirmedList);

        if (success)
        {
            Console.WriteLine("Renaming process completed successfully.");
        }
        else
        {
            Console.WriteLine("Renaming failed.");
        }
    }

    static string GetFolderPath(string prompt)
    {
        string? folderPath;
        do
        {
            Console.WriteLine(prompt);
            folderPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("Error: The folder path is either empty or does not exist. Please enter a valid folder path.");
            }
        } while (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath));

        return folderPath;
    }

    public class SimpleHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }

    static List<ConfirmedChangeModel> ConvertProposedToConfirmed(List<ProposedChangeModel> proposedChanges, string destinationPath)
    {
        var confirmedChanges = new List<ConfirmedChangeModel>();

        foreach (var prop in proposedChanges)
        {
            confirmedChanges.Add(new ConfirmedChangeModel()
            {
                NewFileName = prop.ProposedFileName,
                OriginalFileName = prop.OriginalFileName,
                NewFilePath = destinationPath,
                OriginalFilePath = prop.OriginalFilePath
            });
        }

        return confirmedChanges;
    }
}
