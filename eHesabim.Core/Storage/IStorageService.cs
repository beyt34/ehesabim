using System.IO;

namespace eHesabim.Core.Storage {
    public interface IStorageService {
        void Upload(Stream fileStream, string path, string fileName, string contentType);

        bool Delete(string path, string fileName);
    }
}