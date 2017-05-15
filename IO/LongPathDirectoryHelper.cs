using ZetaLongPaths;

namespace kCura.WinEDDS.Core.IO
{
    public class LongPathDirectoryHelper : IDirectoryHelper
    {
        private object _lockObject = new object();

        public string Combine(string path1, string path2)
        {
            return ZlpPathHelper.Combine(path1, path2);
        }

        public void Create(string path)
        {
            lock (_lockObject)
            {
                if (!Exists(path))
                {
                    ZlpIOHelper.CreateDirectory(path);
                }
            }
        }

        public void Delete(string path, bool recursive)
        {
            lock (_lockObject)
            {
                if (Exists(path))
                {
                    ZlpIOHelper.DeleteDirectory(path, recursive);
                }
            }
        }

        public bool Exists(string path)
        {
            return ZlpIOHelper.DirectoryExists(path);
        }

        public string GetTempPath()
        {
            return ZlpPathHelper.GetTempDirectoryPath();
        }
    }
}