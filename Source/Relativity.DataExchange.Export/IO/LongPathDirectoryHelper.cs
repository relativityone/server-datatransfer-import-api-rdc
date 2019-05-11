namespace Relativity.DataExchange.Io
{
	using ZetaLongPaths;

	internal class LongPathDirectoryHelper : IDirectory
    {
        private object _lockObject = new object();

        public string Combine(string path1, string path2)
        {
            return ZlpPathHelper.Combine(path1, path2);
        }

        public void CreateDirectory(string path)
        {
            lock (this._lockObject)
            {
                if (!this.Exists(path))
                {
                    ZlpIOHelper.CreateDirectory(path);
                }
            }
        }

        public void Delete(string path, bool recursive)
        {
            lock (this._lockObject)
            {
                if (this.Exists(path))
                {
                    ZlpIOHelper.DeleteDirectory(path, recursive);
                }
            }
        }

		public void Delete(string path)
		{
			// Required by interface but not used by this API.
			throw new System.NotImplementedException();
		}

		public bool Exists(string path)
        {
            return ZlpIOHelper.DirectoryExists(path);
        }

		public IDirectoryInfo GetParent(string path)
		{
			// Required by interface but not used by this API.
			throw new System.NotImplementedException();
		}

		public string GetTempPath()
        {
            return ZlpPathHelper.GetTempDirectoryPath();
        }
    }
}