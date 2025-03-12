using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SaaS_Storage_Manager.Orchestrator;
using SaaS_Storage_Manager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaaS_Storage_Manger.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<StorageOrchestrator>();

var app = builder.Build();

app.MapPost("/upload", async (HttpContext context, StorageOrchestrator orchestrator) =>
{
    try
    {
        var request = context.Request;

        if (!request.HasFormContentType || request.Form.Files.Count == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }

        var file = request.Form.Files[0];
        var filePath = Path.GetTempFileName(); 

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            await orchestrator.UploadFileAsync(filePath);
        }

        return Results.Ok($"File '{file.FileName}' uploaded successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error uploading file: {ex.Message}");
    }
});

app.MapDelete("/delete/{blobName}", async (string blobName, StorageOrchestrator orchestrator) =>
{
    try
    {
        await orchestrator.DeleteFileAsync(blobName);
        return Results.Ok($"File '{blobName}' deleted successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error deleting file '{blobName}': {ex.Message}");
    }
});

app.Run();