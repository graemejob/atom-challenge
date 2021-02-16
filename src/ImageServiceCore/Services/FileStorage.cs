using ImageServiceCore.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger logger;

        public FileStorage(Options options, ILogger logger)
        {
            this.options = options;
            this.logger = logger;
            if (!Directory.Exists(this.options.BasePath)) Directory.CreateDirectory(this.options.BasePath);
        }

        private string GetFullPath(string name)
        {
            return Path.Combine(options.BasePath, name);
        }

        public byte[] Get(string name)
        {
            logger.LogInformation($"Reading file {name}");
            string fullPath = GetFullPath(name);
            return File.ReadAllBytes(fullPath);
        }

        public void Set(string name, byte[] bytes)
        {
            logger.LogInformation($"Writing file {name}");
            string fullPath = GetFullPath(name);
            File.WriteAllBytes(fullPath, bytes);
        }

        public bool Exists(string name)
        {
            logger.LogInformation($"Checking file {name}");
            string fullPath = GetFullPath(name);
            return File.Exists(fullPath);
        }

        public string[] List()
        {
            logger.LogInformation($"Listing files");
            return Directory.GetFiles(options.BasePath).Select(s => Path.GetFileName(s)).ToArray();
        }
    }
}
