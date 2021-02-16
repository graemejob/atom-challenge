namespace ImageServiceCore.Interfaces
{
    public interface IBlobStorage
    {
        public string[] List();
        public bool Exists(string name);
        public byte[] Get(string name);
        public void Set(string name, byte[] bytes);
    }
}
