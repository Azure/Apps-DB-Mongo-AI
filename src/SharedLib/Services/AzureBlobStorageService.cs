using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace SharedLib.Services
{
    public class AzureBlobStorageService
    {
        private readonly string containerName;
        private readonly string connectionString;

        public AzureBlobStorageService()
        {
            connectionString =
                "DefaultEndpointsProtocol=https;AccountName=qfrgtap3gjjlyfnstorage;AccountKey=lOYipSJx4R/9Ql9WWs+GuwqVvWfAB1ZPFFNLdbvTjGYWOnOrn0F2u+IE/1kLczyie9bguqHJqIJh+AStZnYv1w==;EndpointSuffix=core.windows.net";
            containerName = "product-photos";
        }

        public async Task<bool> UploadFileAsync(string filePath, string blobName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                await blobContainerClient.CreateIfNotExistsAsync();

                var blobClient = blobContainerClient.GetBlobClient(blobName);

                using (var stream = File.OpenRead(filePath))
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file to Azure Blob Storage: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DeleteFileAsync(string blobName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var blobClient = blobContainerClient.GetBlobClient(blobName);

                var response = await blobClient.DeleteIfExistsAsync();

                return response.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file from Azure Blob Storage: {ex.Message}");
                return false;
            }
        }

        private string GetBlobUrl(string blobName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            return blobClient.Uri.ToString();
        }

        public string GenerateSasUrl(string blobName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var blobSasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // "b" for blobs, "c" for containers
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // Set the expiration time for the SAS token
            };

            blobSasBuilder.SetPermissions(BlobSasPermissions.Read); // Adjust permissions as needed

            var sasUrl = blobContainerClient.GenerateSasUri(blobSasBuilder);

            return sasUrl.ToString();
        }
    }
}