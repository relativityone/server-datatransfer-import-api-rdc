// ----------------------------------------------------------------------------
// <copyright file="TestFilesRepository.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Concurrent;
	using System.IO;
	using System.Net;
	using System.Threading.Tasks;

	public static class TestFilesRepository
	{
		private static readonly WebClient WebClient = new WebClient();
		private static readonly ConcurrentDictionary<string, byte[]> Cache =
			new ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

		public static async Task<byte[]> GetFileByHashAsync(string hashMd5)
		{
			if (Cache.TryGetValue(hashMd5, out byte[] bytes))
			{
				return bytes;
			}

			string url = $"https://testfilesgbu.blob.core.windows.net/test-files-initial/{hashMd5}";
			try
			{
				bytes = await WebClient.DownloadDataTaskAsync(url).ConfigureAwait(false);
				Cache.TryAdd(hashMd5, bytes);
			}
			catch (Exception)
			{
				throw new IOException($"Could not download file from {url}. Make sure the file is uploaded to blob storage.");
			}

			return bytes;
		}
	}
}