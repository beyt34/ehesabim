using System.IO;
using System.Web.Hosting;
using eHesabim.Core.Logging;

namespace eHesabim.Core.Storage
{
    public class StorageService : IStorageService
    {
        private readonly ILogging logging;
        private readonly string rootPath;

        public StorageService(ILogging logging)
        {
            this.logging = logging;
            rootPath = HostingEnvironment.MapPath("~/Content");
        }

        public void Upload(Stream fileStream, string path, string fileName, string contentType)
        {
            var filePath = GetFilePath(path, fileName);
            EnsureDirectory(path);

            using (var output = File.Create(filePath))
            {
                fileStream.CopyTo(output);
            }
        }

        public void Upload(byte[] buffer, string path, string fileName, string contentType)
        {
            var filePath = GetFilePath(path, fileName);
            EnsureDirectory(path);
            File.WriteAllBytes(filePath, buffer);
        }

        public bool Delete(string path, string fileName)
        {
            var filePath = GetFilePath(path, fileName);
            if (!File.Exists(filePath))
            {
                return false;
            }

            File.Delete(filePath);
            return true;
        }

        private void EnsureDirectory(string path)
        {
            var directoryPath = Path.Combine(rootPath ?? string.Empty, path);
            if (Directory.Exists(directoryPath))
            {
                return;
            }

            Directory.CreateDirectory(directoryPath);
            logging.Info("StorageService => Directory Created");
        }

        private string GetFilePath(string path, string fileName)
        {
            return Path.Combine(rootPath ?? string.Empty, path, fileName);
        }
    }
}
