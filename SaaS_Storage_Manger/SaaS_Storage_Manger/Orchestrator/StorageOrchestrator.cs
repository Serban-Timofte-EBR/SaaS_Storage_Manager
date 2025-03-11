using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SaaS_Storage_Manger.Orchestrator;

public class StorageOrchestrator
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<StorageOrchestrator> _logger;

    public StorageOrchestrator(IConfiguration configuration, ILogger<StorageOrchestrator> logger)
    {
        var storageAccountName = configuration["AzureStorage:AccountName"];
        _containerName = configuration["AzureStorage:ContainerName"];
        bool usedManagedIdentity = bool.Parse(configuration["AzureStorage:UseManagedIdentity"]);

        if (string.IsNullOrWhiteSpace(storageAccountName))
        {
            throw new ArgumentNullException(nameof(storageAccountName), "Storage account name is not configured.");
        }
        
        var storageUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
        var credential = new DefaultAzureCredential();
        
        _blobServiceClient = new BlobServiceClient(storageUri, credential);
        _logger = logger;
    }

    public async Task CreateStorageContainerAsync()
    {
        _logger.LogInformation($"Creating storage container: {_containerName}");
        await _blobServiceClient.GetBlobContainerClient(_containerName).CreateIfNotExistsAsync();
        _logger.LogInformation("Storage container created successfully");
    }
}