using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using System;

namespace MyWebApp.Helpers
{
    public static class FileHelper
    {
        public static void Create(string path, IFormFile file)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECT_STR") ?? "UseDevelopmentStorage=true";
            CloudStorageAccount.TryParse(connectionString, out var storageAccount);

            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var containerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME") ?? "mycontainer";
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            cloudBlobContainer.CreateAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream()).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}