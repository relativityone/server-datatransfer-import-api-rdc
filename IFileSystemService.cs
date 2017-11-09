
namespace kCura.WinEDDS.TApi
{
    internal interface IFileSystemService
    {
        /// <summary>Gets the size, in bytes, of the current file. </summary>
        /// <param name="filename"></param>
        /// <returns>The size of the current file in bytes.</returns>
        long GetFileLength(string filename);
    }
}
