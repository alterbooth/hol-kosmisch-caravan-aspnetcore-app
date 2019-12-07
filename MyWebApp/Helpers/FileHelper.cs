using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace MyWebApp.Helpers
{
    public static class FileHelper
    {
        public static async Task Create(IFormFile file)
        {
            string connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount);

            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("mycontainer");
            await cloudBlobContainer.CreateAsync();

            BlobContainerPermissions permissions = await cloudBlobContainer.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            await cloudBlobContainer.SetPermissionsAsync(permissions);

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.FileName);
            await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());
        }
    }
}