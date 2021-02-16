using ImageServiceCore.Interfaces;
using System.IO;
using System.Linq;

namespace ImageServiceCore.Services
{
    public class FileStorage : IBlobStorage
    {
        public class Options
        {
            public string BasePath { get; set; }
        }

        public Options options;

        public FileStorage(Options options)
        {
            this.options = options;
            if (!Directory.Exists(this.options.BasePath)) Directory.CreateDirectory(this.options.BasePath);
        }

        private string GetFullPath(string name)
        {
            return Path.Combine(options.BasePath, name);
        }

        public byte[] Get(string name)
        {
            string fullPath = GetFullPath(name);
            return File.ReadAllBytes(fullPath);
        }

        public void Set(string name, byte[] bytes)
        {
            string fullPath = GetFullPath(name);
            File.WriteAllBytes(fullPath, bytes);
        }

        public bool Exists(string name)
        {
            string fullPath = GetFullPath(name);
            return File.Exists(fullPath);
        }

        public string[] List()
        {
            return Directory.GetFiles(options.BasePath).Select(s => Path.GetFileName(s)).ToArray();
        }
    }
}
