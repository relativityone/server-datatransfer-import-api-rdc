using System;
using System.Collections.Generic;
using Relativity.Transfer;

namespace kCura.WinEDDS.TApi
{
	/// <summary>
	///     Class used to resolve file path when downloading in Web Mode
	/// </summary>
	public sealed class HttpClientDownloadPathResolver : RemotePathResolverBase
	{
		private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

		/// <summary>
		///     Adds path translation (image's GUID can be different from file name)
		/// </summary>
		/// <param name="path"></param>
		/// <param name="guid"></param>
		public static void AddPathTranslation(string path, string guid)
		{
			_dictionary[path] = guid;
		}

		/// <summary>
		///     Clears all path translations
		/// </summary>
		public static void ClearPathTranslations()
		{
			_dictionary = new Dictionary<string, string>();
		}

		/// <summary>
		///     Resolved Web path for given location
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		protected override string OnResolvePath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (_dictionary.ContainsKey(path))
			{
				return _dictionary[path];
			}

			return FileSystemService.GetFileName(path);
		}
	}
}