using System.Configuration;
using System.IO;
using eHesabim.Core.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace eHesabim.Core.Storage {
    public class StorageService : IStorageService {
        private readonly ILogging logging;

        public StorageService(ILogging logging) {
            this.logging = logging;
        }

        public void Upload(Stream fileStream, string path, string fileName, string contentType) {
            var blockBlobReference = GetContainer(path).GetBlockBlobReference(fileName);
            blockBlobReference.Properties.ContentType = contentType;
            blockBlobReference.UploadFromStream(fileStream);
        }

        public void Upload(byte[] buffer, string path, string fileName, string contentType) {
            var blockBlobReference = GetContainer(path).GetBlockBlobReference(fileName);
            blockBlobReference.Properties.ContentType = contentType;
            blockBlobReference.Properties.CacheControl = "public, max-age=2629000";
            blockBlobReference.UploadFromByteArray(buffer, 0, buffer.Length);
        }
        
        public bool Delete(string path, string fileName) {
            return GetContainer(path).GetBlockBlobReference(fileName).DeleteIfExists();
        }

        protected CloudBlobClient ConnectStorage() {
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            if (string.IsNullOrEmpty(connectionString)) {
                logging.Error("StorageService => ConnectionString not found");
            }

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            return storageAccount.CreateCloudBlobClient();
        }

        protected CloudBlobContainer GetContainer(string containerName) {
            var client = ConnectStorage();
            var container = client.GetContainerReference(containerName);
            var created = container.CreateIfNotExists();

            if (created) {
                logging.Info("StorageService => Blob Created");
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return container;
        }
    }
}
