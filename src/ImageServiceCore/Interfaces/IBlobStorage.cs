using System.IO;
using System.Threading.Tasks;

namespace ImageServiceCore.Interfaces
{
    public interface IBlobStorage
    {
        public bool Exists(string name);
        public byte[] Get(string name);
        public void Set(string name, byte[] bytes);
    }
}
