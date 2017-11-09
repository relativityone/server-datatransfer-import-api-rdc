
namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represents an abstract file system service.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>Gets the size, in bytes, of the current file. </summary>
        /// <param name="fileName"></param>
        /// <returns>The size of the current file in bytes.</returns>
        long GetFileLength(string fileName);
    }
}
