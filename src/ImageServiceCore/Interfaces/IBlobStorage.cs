using System.IO;

namespace ImageServiceCore.Interfaces
{
    public interface IBlobStorage
    {
        Stream Get(string name);
        void Set(Stream stream, string name);
    }
}
