using Newtonsoft.Json;
using System;
using System.IO;

namespace Toponym.Core.Services {
    public class FileService {

        public static T Read<T>(string path) {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path)) {
                Utils.LogError($"File \"{Utils.GetFileName(path)}\" not found");
                throw new InvalidOperationException();
            }

            using (var fileStream = new FileStream(path, FileMode.Open)) {
                var streamReader = new StreamReader(fileStream);
                var jsonTextReader = new JsonTextReader(streamReader);
                var serializer = new JsonSerializer();
                var obj = serializer.Deserialize<T>(jsonTextReader);

                if (obj.Equals(null)) {
                    Utils.LogError($"File \"{Utils.GetFileName(path)}\" is wrong");
                    throw new InvalidOperationException();
                }

                Utils.LogRead($"\"{Utils.GetFileName(path)}\" read");
                return obj;
            }
        }

        public static void Save(string path, object obj) {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (var fileStream = new FileStream(path, FileMode.Create)) {
                var streamWriter = new StreamWriter(fileStream);
                var jsonTextWriter = new JsonTextWriter(streamWriter);
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonTextWriter, obj);
                jsonTextWriter.Close();
                Utils.LogSaved($"\"{Utils.GetFileName(path)}\" saved");
            }
        }
    }
}
