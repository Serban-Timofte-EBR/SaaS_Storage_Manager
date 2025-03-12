using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaaS_Storage_Manger.Services;

namespace SaaS_Storage_Manager.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            var storageAccountName = configuration["AzureStorage:AccountName"];
            var containerName = configuration["AzureStorage:ContainerName"];
            
            if (string.IsNullOrWhiteSpace(storageAccountName) || string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException("Storage account name or container name is missing in configuration.");
            }

            var storageUri = new Uri($"https://{storageAccountName}.blob.core.windows.net");
            var blobServiceClient = new BlobServiceClient(storageUri, new Azure.Identity.DefaultAzureCredential());

            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _logger = logger;
        }

        public async Task UploadFileAsync(Stream fileStream, string blobName)
        {
            _logger.LogInformation($"Uploading file: {blobName}");

            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            _logger.LogInformation($"File '{blobName}' uploaded successfully.");
        }

        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            _logger.LogInformation($"Downloading file: {blobName}");

            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            var response = await blobClient.DownloadAsync();

            _logger.LogInformation($"File '{blobName}' downloaded successfully.");
            return response.Value.Content;
        }

        public async Task<List<string>> ListFilesAsync()
        {
            _logger.LogInformation("Listing all files in the storage container.");

            var blobItems = _blobContainerClient.GetBlobsAsync();
            var files = new List<string>();

            await foreach (var blobItem in blobItems)
            {
                files.Add(blobItem.Name);
            }

            _logger.LogInformation($"Total {files.Count} files found.");
            return files;
        }
        
        public async Task DeleteFileAsync(string blobName)
        {
            _logger.LogInformation($"Deleting file: {blobName}");

            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation($"File '{blobName}' deleted successfully.");
        }
    }
}