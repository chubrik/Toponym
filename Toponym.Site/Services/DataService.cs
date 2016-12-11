using Microsoft.AspNetCore.Hosting;
using System.IO;
using Toponym.Core.Services;

namespace Toponym.Site.Services {
    public class DataService {

        public readonly string DataDir;

        public DataService(IHostingEnvironment environment) {
            DataDir = Path.Combine(environment.ContentRootPath, "App_Data");
        }

        public T ReadData<T>(string fileName) => FileService.Read<T>(Path.Combine(DataDir, fileName));

        public void SaveData(string fileName, object obj) => FileService.Save(Path.Combine(DataDir, fileName), obj);
    }
}
