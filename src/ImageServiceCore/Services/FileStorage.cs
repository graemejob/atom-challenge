using ImageServiceCore.Interfaces;
using System.IO;

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
        }

        private string GetFullPath(string name)
        {
            return Path.Combine(options.BasePath);
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
    }
}
