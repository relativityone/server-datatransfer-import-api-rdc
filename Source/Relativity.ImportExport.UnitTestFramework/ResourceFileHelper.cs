// ----------------------------------------------------------------------------
// <copyright file="ResourceFileHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.ImportExport.UnitTestFramework
{
	/// <summary>
	/// Defines static helper methods to manage resource test files.
	/// </summary>
	public static class ResourceFileHelper
	{
        public static string GetBasePath()
        {
            string basePath = System.IO.Path.GetDirectoryName(typeof(FieldHelper).Assembly.Location);
            return basePath;
        }

        public static string GetDocsResourceFilePath(string fileName)
        {
            return GetResourceFilePath("Docs", fileName);
        }

        public static string GetImagesResourceFilePath(string fileName)
        {
            return GetResourceFilePath("Images", fileName);
        }

        public static string GetResourceFilePath(string folder, string fileName)
        {
            string basePath = System.IO.Path.GetDirectoryName(typeof(FieldHelper).Assembly.Location);
            string sourceFile =
                System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.Combine(basePath, "Resources"), folder), fileName);
            return sourceFile;
        }
    }
}