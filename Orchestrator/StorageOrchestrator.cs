using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SaaS_Storage_Manager.Services;
using SaaS_Storage_Manger.Services;

namespace SaaS_Storage_Manager.Orchestrator
{
    public class StorageOrchestrator
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<StorageOrchestrator> _logger;

        public StorageOrchestrator(IBlobStorageService blobStorageService, ILogger<StorageOrchestrator> logger)
        {
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        public async Task UploadFileAsync(string filePath)
        {
            _logger.LogInformation($"Uploading file: {filePath}");

            using var fileStream = File.OpenRead(filePath);
            var blobName = Path.GetFileName(filePath);

            await _blobStorageService.UploadFileAsync(fileStream, blobName);

            _logger.LogInformation($"File '{blobName}' uploaded.");
        }

        public async Task DownloadFileAsync(string blobName, string downloadPath)
        {
            _logger.LogInformation($"Downloading file: {blobName}");

            using var stream = await _blobStorageService.DownloadFileAsync(blobName);
            using var fileStream = File.Create(downloadPath);
            await stream.CopyToAsync(fileStream);

            _logger.LogInformation($"File '{blobName}' downloaded to {downloadPath}.");
        }

        public async Task ListFilesAsync()
        {
            var files = await _blobStorageService.ListFilesAsync();

            _logger.LogInformation("Files in container:");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
        }

        public async Task DeleteFileAsync(string blobName)
        {
            _logger.LogInformation($"Deleting file: {blobName}");
            await _blobStorageService.DeleteFileAsync(blobName);
            _logger.LogInformation($"File '{blobName}' deleted.");
        }
    }
}